using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using BerkeleyDB;
using Droog.EmbeddedDB.Tests.Entities;
using ProtoBuf;

namespace Droog.EmbeddedDB.Tests.Sources {
    public class BDBWrapper<T> : IEnumerable<T>, IDisposable where T : IEntity {

        private readonly BTreeDatabase _db;

        public BDBWrapper() {
            var path = BerkeleyDBDataSource.GetStorageDir<T>();
            var dbConfig = new BTreeDatabaseConfig {
                Duplicates = DuplicatesPolicy.NONE,
                ErrorPrefix = "ex_csharp_access",
                Creation = CreatePolicy.IF_NEEDED,
                CacheSize = new CacheInfo(0, 64 * 1024, 1),
                PageSize = 8 * 1024
            };

            _db = BTreeDatabase.Open(path, dbConfig);
        }

        public uint Count { get { return _db.Stats().nKeys; } }

        public T this[int id] {
            get {
                var key = new DatabaseEntry(BitConverter.GetBytes(id));
                var record = _db.Get(key);
                using(var ms = new MemoryStream(record.Value.Data)) {
                    return Serializer.DeserializeWithLengthPrefix<T>(ms, PrefixStyle.Base128, 1);
                }
            }
            set {
                using(var ms = new MemoryStream()) {
                    Serializer.SerializeWithLengthPrefix(ms, value, PrefixStyle.Base128, 1);
                    ms.Position = 0;
                    var key = new DatabaseEntry(BitConverter.GetBytes(id));
                    var data = new DatabaseEntry(ms.ToArray());
                    _db.Put(key, data);
                }
            }
        }

        public IEnumerator<T> GetEnumerator() {
            using(var cursor = _db.Cursor()) {
                foreach(var kvp in cursor) {
                    using(var ms = new MemoryStream(kvp.Value.Data)) {
                        yield return Serializer.DeserializeWithLengthPrefix<T>(ms, PrefixStyle.Base128, 1);
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public void Dispose() {
            _db.Dispose();
        }
    }
}