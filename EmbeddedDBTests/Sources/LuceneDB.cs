using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Droog.EmbeddedDB.Tests.Entities;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using ProtoBuf;

namespace Droog.EmbeddedDB.Tests.Sources {
    public class LuceneDB<T> : IEnumerable<T>, IDisposable where T : IEntity {

        public readonly string Default;
        private readonly IndexWriter _writer;
        private readonly QueryParser _parser;
        private readonly FSDirectory _rd;
        private IndexReader _reader;
        private IndexSearcher _searcher;

        public LuceneDB() {
            var path = LuceneDataSource.GetStorageDir<T>();
            Default = "content";
            var analyzer = new StandardAnalyzer();
            _rd = FSDirectory.GetDirectory(path);

            _writer = new IndexWriter(_rd, analyzer, true, IndexWriter.MaxFieldLength.LIMITED);
            _reader = IndexReader.Open(_rd);
            _searcher = new IndexSearcher(_reader);
            _parser = new QueryParser(Default, analyzer);
        }

        public int Count { get {
            GetSearcher();
            return _reader.NumDocs();
        } }
        public bool AutoCommit { get; set; }
        public void Commit() {
            _writer.Commit();
        }
        public T this[int id] {
            set {
                _writer.DeleteDocuments(new Term("id", id.ToString()));
                var d = new Document();
                d.Add(new Field("id", id.ToString(), Field.Store.YES, Field.Index.UN_TOKENIZED));
                using(var ms = new MemoryStream()) {
                    Serializer.SerializeWithLengthPrefix(ms, value, PrefixStyle.Base128, 1);
                    ms.Position = 0;
                    d.Add(new Field(Default, ms.ToArray(), 0, (int)ms.Length, Field.Store.YES));
                }
                _writer.AddDocument(d);
                if(AutoCommit) {
                    _writer.Commit();
                }
            }
            get {
                var searcher = GetSearcher();
                var r = searcher.Search(_parser.Parse("id: " + id));
                if(r.Length() == 0) {
                    return default(T);
                }
                using(var ms = new MemoryStream(r.Doc(0).GetField(Default).GetBinaryValue())) {
                    return Serializer.DeserializeWithLengthPrefix<T>(ms, PrefixStyle.Base128, 1);
                }
            }
        }


        private IndexSearcher GetSearcher() {
            var reader = _reader.Reopen();
            if(reader != _reader) {
                _reader.Close();     
                _searcher.Close();
                _reader = reader;
                _searcher = new IndexSearcher(_reader);
            }
            return _searcher;
        }

        public IEnumerator<T> GetEnumerator() {
            GetSearcher();
            var count = _reader.NumDocs();
            for(var i = 0; i < count; i++) {
                if(!_reader.IsDeleted(i)) {
                    var d = _reader.Document(i);
                    using(var ms = new MemoryStream(d.GetField(Default).GetBinaryValue())) {
                        yield return Serializer.DeserializeWithLengthPrefix<T>(ms, PrefixStyle.Base128, 1);
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public void Dispose() {
            _writer.Close();
            if(_searcher != null) {
                _searcher.Close();
            }
            _reader.Close();
        }
    }
}