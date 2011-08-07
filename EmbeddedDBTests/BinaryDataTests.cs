using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Linq;
using Droog.EmbeddedDB.Tests.Entities;
using Droog.EmbeddedDB.Tests.Sources;
using Droog.EmbeddedDB.Tests.Util;
using NUnit.Framework;
using ProtoBuf;
using log4net;

namespace Droog.EmbeddedDB.Tests {

    [TestFixture]
    public class BinaryDataTests {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [Test]
        public void Post_Xml_to_Bin() {
            _log.DebugFormat("Converting Post xml to protobuf ");
            try {
                CreateBinaryFileFromXml(typeof(Post));
            } catch(Exception e) {
                _log.Warn(e.Message);
            }
        }

        [Test]
        public void User_Xml_to_Bin() {
            _log.DebugFormat("Converting User xml to protobuf");
            try {
                CreateBinaryFileFromXml(typeof(User));
            } catch(Exception e) {
                _log.Warn(e.Message);
            }
        }

        [Test]
        public void Read_users() {
            var i = 0;
            var elapsed = Diagnostics.Time(() => {

                foreach(var user in BinaryDataSource.All<User>()) {
                    i++;
                    if(i % 10000 == 0) {
                        _log.DebugFormat("{0}, id: {1}, name: {2}", i, user.Id, user.DisplayName);
                    }
                }
            });
            _log.DebugFormat("Read {0} user records @ {1:0,0}records/second", i, i / elapsed.TotalSeconds);
        }

        [Test]
        public void Read_posts() {
            var i = 0;
            var elapsed = Diagnostics.Time(() => {

                foreach(var post in BinaryDataSource.All<Post>()) {
                    i++;
                    if(i % 100000 == 0 && post.PostType == PostType.Question) {
                        _log.DebugFormat("{0}, id: {1}, name: {2}", i, post.Id, post.Title);
                    }
                }
            });
            _log.DebugFormat("Read {0} posts @ {1:0,0}records/second", i, i / elapsed.TotalSeconds);
        }

        [Test]
        public void Find_user() {
            var elapsed = Diagnostics.Time(() => {
                var user = XmlDataSource.All<User>()
                                .Where(x => x.DisplayName == "Arne Claassen")
                                .First();
                Console.WriteLine("Name: {0}\r\nAbout:\r\n{1}", user.DisplayName, user.AboutMe);
            });
            _log.DebugFormat("Found user in {0:0.00}ms", elapsed.TotalMilliseconds);
        }

        [Test,Ignore("this will take forever!")]
        public void Create_QnA_projection() {

            // since there are no indicies, this will iterate over the binary data source once for every post, i.e. this could take hours to run!
            var members = BinaryDataSource.All<User>().Select(x => x.AsMember()).ToDictionary(x => x.Id);
            var questions = from questionPost in BinaryDataSource.All<Post>()
                            where questionPost.PostType == PostType.Question
                            let answers = (from post in BinaryDataSource.All<Post>()
                                           where post.PostType == PostType.Answer && post.ParentId == questionPost.Id
                                           select post.AsAnswer(members)
                                          )
                                .ToList()
                            select questionPost.AsQuestion(answers, members);
            BinaryDataSource.WriteAll(questions);
        }

        public static void CreateBinaryFileFromXml(Type type) {
            var data = XmlDataSource.ReadAll(type);
            _log.DebugFormat("Reading and writing '{0}'", type.Name);
            BinaryDataSource.WriteAll(type, data);
        }

    }
}
