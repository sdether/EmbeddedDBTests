using System;
using System.Linq;
using Droog.EmbeddedDB.Tests.Entities;
using Droog.EmbeddedDB.Tests.Sources;
using Droog.EmbeddedDB.Tests.Util;
using NUnit.Framework;
using log4net;

namespace Droog.EmbeddedDB.Tests {

    [TestFixture]
    public class BerkeleyDBTests {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [Test]
        public void Bin_to_BerkeleyDB() {
            _log.DebugFormat("Converting User binary blob to Berkeley DB");
            BDBWrapper<User> users = null;
            var elapsed = Diagnostics.Time(() => {
                users = BerkeleyDBDataSource.WriteAll(BinaryDataSource.All<User>());
            });
            _log.DebugFormat("Read {0} user records @ {1:0,0}records/second", users.Count, users.Count / elapsed.TotalSeconds);
            users.Dispose();
        }

        [Test]
        public void Read_users() {
            var i = 0;
            var users = BerkeleyDBDataSource.Open<User>();
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
        public void Users_starting_with_A() {
            var users = BerkeleyDBDataSource.Open<User>();
            foreach(var user in users.Where(Foo)) {
                Console.WriteLine("User: {0}",user.DisplayName);
            }
        }

        private bool Foo(User u) {
            return u.DisplayName.StartsWith("A");
        }

        [Test]
        public void Read_posts() {
            var i = 0;
            var posts = BerkeleyDBDataSource.Open<Post>();
            var elapsed = Diagnostics.Time(() => {
                foreach(var post in posts) {
                    i++;
                    if(i % 10000 == 0) {
                        _log.DebugFormat("{0}, id: {1}, name: {2}", i, post.Id, post.PostType);
                    }
                }
            });
            _log.DebugFormat("Read {0} posts @ {1:0,0}records/second", i, i / elapsed.TotalSeconds);
        }
    }
}
