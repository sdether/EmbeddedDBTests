using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Droog.EmbeddedDB.Tests.Util;
using log4net;

namespace Droog.EmbeddedDB.Tests.Sources {
    public static class XmlDataSource {

        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static IEnumerable<T> All<T>() where T : class {
            return ReadAll(typeof(T)).Cast<T>();
        }

        public static IEnumerable ReadAll(Type type) {
            var datasource = Path.Combine(ConfigurationManager.AppSettings["path.stackoverflow"], GetFilename(type));
            if(!File.Exists(datasource)) {
                throw new Exception(string.Format("no such data source: {0}", datasource));
            }
            var serializer = new XmlSerializer(type);
            using(var stream = new FileStream(datasource, FileMode.Open)) {
                using(var reader = new XmlTextReader(stream)) {
                    while(reader.Read()) {
                        if(reader.NodeType != XmlNodeType.Element) {
                            continue;
                        }
                        if(reader.Name != "row") {
                            continue;
                        }
                        yield return serializer.Deserialize(reader);
                    }
                }
            }
        }

        public static string GetFilename<T>() {
            return GetFilename(typeof(T));
        }
        public static string GetFilename(Type t) {
            return Entity.GetFilename(t) + ".xml";
        }
    }
}
