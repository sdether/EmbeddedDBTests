using System;
using System.Collections.Generic;
using System.Linq;
using Droog.EmbeddedDB.Tests.Entities;
using Droog.EmbeddedDB.Tests.Sources;
using Droog.EmbeddedDB.Tests.Util;
using Droog.Firkin;
using NUnit.Framework;
using log4net;

namespace Droog.EmbeddedDB.Tests {

    [TestFixture]
    public class FirkinTests {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [Test]
        public void Bin_to_Firkin() {
            _log.DebugFormat("Converting User binary blob to firkin DB");
            FirkinDictionary<int, User> users = null;
            var elapsed = Diagnostics.Time(() => {
                users = FirkinDataSource.WriteAll(BinaryDataSource.All<User>());
            });
            _log.DebugFormat("Read {0} user records @ {1:0,0}records/second", users.Count, users.Count / elapsed.TotalSeconds);
            _log.DebugFormat("Converting User binary blob to firkin DB");
            FirkinDictionary<int, Post> posts = null;
            elapsed = Diagnostics.Time(() => {
                posts = FirkinDataSource.WriteAll(BinaryDataSource.All<Post>());
            });
            _log.DebugFormat("Read {0} post records @ {1:0,0}records/second", posts.Count, posts.Count / elapsed.TotalSeconds);
        }

        [Test]
        public void Read_users() {
            var i = 0;
            var users = FirkinDataSource.Open<User>();
            var elapsed = Diagnostics.Time(() => {
                foreach(var user in users.Values) {
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
            var posts = FirkinDataSource.Open<Post>();
            var elapsed = Diagnostics.Time(() => {
                foreach(var post in posts.Values) {
                    i++;
                    if(i % 10000 == 0) {
                        _log.DebugFormat("{0}, id: {1}, name: {2}", i, post.Id, post.PostType);
                    }
                }
            });
            _log.DebugFormat("Read {0} posts @ {1:0,0}records/second", i, i / elapsed.TotalSeconds);
        }

        [Test]
        public void Create_QnA_projection() {
            var users = FirkinDataSource.Open<User>();
            var posts = FirkinDataSource.Open<Post>();
            var answers = (from post in posts.Values
                           where post.PostType == PostType.Answer
                           let pair = new { AnswerId = post.Id, QuestionId = post.ParentId }
                           group pair by pair.QuestionId into g
                           select new { QuestionId = g.Key, Answers = g.Select(x => x.AnswerId).ToArray() }
                          ).ToDictionary(x => x.QuestionId, x => x.Answers);
            var members = users.Values.Select(x => x.AsMember()).ToDictionary(x => x.Id);
            var questions = from post in posts.Values
                            where post.PostType == PostType.Question
                            select post.AsQuestion(GetAnswers(posts, members, answers, post.Id), members);
            BinaryDataSource.WriteAll(questions);
        }

        private List<Answer> GetAnswers(FirkinDictionary<int, Post> posts, Dictionary<int, Member> members, Dictionary<int, int[]> answers, int id) {
            if(!answers.ContainsKey(id)) {
                return new List<Answer>();
            }
            return (from postId in answers[id]
                    let post = posts[postId]
                    where post != null
                    select post.AsAnswer(members)
                   ).ToList();
        }

        public static void CreateBinaryFileFromXml(Type type) {
            var data = XmlDataSource.ReadAll(type);
            _log.DebugFormat("Reading and writing '{0}'", type.Name);
            BinaryDataSource.WriteAll(type, data);
        }

    }
}
