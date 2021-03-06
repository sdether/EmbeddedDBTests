﻿using System;
using System.Collections.Generic;
using System.IO;
using Droog.EmbeddedDB.Tests.Entities;
using log4net;

namespace Droog.EmbeddedDB.Tests.Sources {
    public static class BerkeleyDBDataSource {

        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static BDBWrapper<T> WriteAll<T>(IEnumerable<T> data) where T : IEntity {
            var dir = GetStorageDir<T>();
            if(Directory.Exists(dir)) {
                throw new Exception(string.Format("directory {0} already exists", dir));
            }
            var db = Open<T>();
            foreach(var entity in data) {
                db[entity.Id] = entity;
            }
            return db;
        }

        public static BDBWrapper<T> Open<T>() where T : IEntity {
            return new BDBWrapper<T>();
        }

        public static string GetStorageDir<T>() {
            return GetStorageDir(typeof(T));
        }

        public static string GetStorageDir(Type t) {
            return Util.Entity.GetFilename(t) + ".bdb";
        }
    }
}
