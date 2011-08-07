using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Droog.EmbeddedDB.Tests.Entities;
using Lucene.Net.Analysis;

namespace Droog.EmbeddedDB.Tests.Sources {
    public static class LuceneDataSource {
        public static LuceneDB<T> WriteAll<T>(IEnumerable<T> data) where T : IEntity {
            var dir = GetStorageDir<T>();
            if(Directory.Exists(dir)) {
                throw new Exception(string.Format("directory {0} already exists", dir));
            }
            var db = Open<T>();
            foreach(var entity in data) {
                db[entity.Id] = entity;
            }
            db.Commit();
            return db;
        }

        public static LuceneDB<T> Open<T>() where T : IEntity {
            return new LuceneDB<T>();
        }

        public static string GetStorageDir<T>() {
            return GetStorageDir(typeof(T));
        }

        public static string GetStorageDir(Type t) {
            return Util.Entity.GetFilename(t) + ".lucene";
        }
    }
}

