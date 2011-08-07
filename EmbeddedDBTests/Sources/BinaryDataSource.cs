using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Droog.EmbeddedDB.Tests.Entities;
using Droog.EmbeddedDB.Tests.Util;
using ProtoBuf;
using log4net;

namespace Droog.EmbeddedDB.Tests.Sources {
    public static class BinaryDataSource {

        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static IEnumerable<T> ReadAll<T>(string path) where T : class {
            using(var stream = File.OpenRead(path)) {
                var i = 0;
                while(stream.Position + 1 < stream.Length) {
                    var item = Serializer.DeserializeWithLengthPrefix<T>(stream, PrefixStyle.Base128, 1);
                    if(item == null) {
                        break;
                    }
                    i++;
                    if(i % 10000 == 0) {
                        _log.DebugFormat("read record {0}", i);
                    }

                    yield return item;
                }
            }
        }

        public static IEnumerable<T> All<T>() where T : class {
            using(var stream = File.OpenRead(GetFilename<T>())) {
                var i = 0;
                while(stream.Position + 1 < stream.Length) {
                    var item = Serializer.DeserializeWithLengthPrefix<T>(stream, PrefixStyle.Base128, 1);
                    if(item == null) {
                        break;
                    }
                    i++;
                    if(i % 10000 == 0) {
                        _log.DebugFormat("read record {0}", i);
                    }
                    yield return item;
                }
            }
        }

        public static void WriteAll<T>(IEnumerable<T> data) {
            WriteAll(typeof(T), data);
        }

        public static void WriteAll(Type type, IEnumerable data) {
            if(File.Exists(GetFilename(type))) {
                throw new Exception(string.Format("binary file already exists: {0}", GetFilename(type)));
            }
            _log.DebugFormat("Writing to '{0}'", GetFilename(type));
            var i = 0;
            var elapsed = Diagnostics.Time(() => {
                using(var stream = File.OpenWrite(GetFilename(type))) {
                    foreach(var entity in data) {
                        i++;
                        Serializer.NonGeneric.SerializeWithLengthPrefix(stream, entity, PrefixStyle.Base128, 1);
                        if(i % 100000 == 0) {
                            _log.DebugFormat("wrote {0} {1} records", i, type.Name);
                        }
                    }
                }
            });
            _log.DebugFormat("Read/Wrote {0} {1} records @ {2:0,0}records/second", i, type.Name, i / elapsed.TotalSeconds);
        }


        public static string GetFilename<T>() {
            return GetFilename(typeof(T));
        }
        public static string GetFilename(Type t) {
            return Entity.GetFilename(t) + ".bin";
        }
    }
}
