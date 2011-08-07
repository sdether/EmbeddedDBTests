using System;
using System.Collections.Generic;
using System.Linq;
using Droog.EmbeddedDB.Tests.Entities;

namespace Droog.EmbeddedDB.Tests.Util {
    public static class Entity {
        public static string GetFilename<T>() {
            return GetFilename(typeof(T));
        }
        public static string GetFilename(Type t) {
            return t.Name.ToLower() + "s";
        }

        public static Question AsQuestion(this Post post, List<Answer> answers, Dictionary<int, Member> members) {
            return new Question {
                Id = post.Id,
                AcceptedAnswer = answers.Where(x => x.Id == post.AcceptedAnswerId).FirstOrDefault(),
                Answers = answers.ToArray(),
                Body = post.Body,
                ClosedDate = post.ClosedDate,
                CommunityOwnedDate = post.CommunityOwnedDate,
                CreationDate = post.CreationDate,
                FavoriteCount = post.FavoriteCount,
                LastActivityDate = post.LastActivityDate,
                LastEditDate = post.LastEditDate,
                LastEditor = members[post.LastEditorUserId],
                Owner = members[post.OwnerUserId],
                Score = post.Score,
                ViewCount = post.ViewCount,
                Tags = post.Tags,
                Title = post.Title
            };
        }
        public static Answer AsAnswer(this Post post, Dictionary<int,Member> members ) {
            return new Answer {
                Id = post.Id,
                Body = post.Body,
                CreationDate = post.CreationDate,
                LastActivityDate = post.LastActivityDate,
                LastEditDate = post.LastEditDate,
                LastEditor = members[post.LastEditorUserId],
                Owner = members[post.OwnerUserId],
                Score = post.Score,
                ViewCount = post.ViewCount
            };    
        }
        public static Member AsMember(this User user) {
            return new Member {
                Id = user.Id,
                Age = user.Age,
                CreationDate = user.CreationDate,
                DisplayName = user.DisplayName,
                EmailHash = user.EmailHash,
                LastAccessDate = user.LastAccessDate,
                Location = user.Location,
                Reputation = user.Reputation,
                WebsiteUrl = user.WebsiteUrl
            };
        }

    }
}