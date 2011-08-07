using System;
using Droog.EmbeddedDB.Tests.Entities;
using Droog.EmbeddedDB.Tests.Sources;
using Droog.EmbeddedDB.Tests.Util;
using NUnit.Framework;
using log4net;

namespace Droog.EmbeddedDB.Tests {

    [TestFixture]
    public class LuceneTests {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [Test]
        public void Bin_to_LuceneDB() {
            _log.DebugFormat("Converting User binary blob to Berkeley DB");
            LuceneDB<User> users = null;
            var elapsed = Diagnostics.Time(() => {
                users = LuceneDataSource.WriteAll(BinaryDataSource.All<User>());
            });
            _log.DebugFormat("Read {0} user records @ {1:0,0}records/second", users.Count, users.Count / elapsed.TotalSeconds);
            users.Dispose();
        }

        [Test]
        public void Read_users() {
            var i = 0;
            var users = LuceneDataSource.Open<User>();
            var elapsed = Diagnostics.Time(() => {
                foreach(var user in users) {
                    i++;
                    if(i % 10000 == 0) {
                        _log.DebugFormat("{0}, id: {1}, name: {2}", i, user.Id, user.DisplayName);
                    }
                }
            });
            _log.DebugFormat("Read {0} user records @ {1:0,0}records/second", i, i / elapsed.TotalSeconds);
        }

        [Test]
        public void ReadUser() {
            var db = LuceneDataSource.Open<User>();
            var user = db[104066];
            Console.WriteLine("id: {0}, name: {1}", user.Id, user.DisplayName);
        }
    }
}
