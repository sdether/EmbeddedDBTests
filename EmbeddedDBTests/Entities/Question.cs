using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Droog.EmbeddedDB.Tests.Entities {
    [ProtoContract]
    public class Question : IEntity {
        [ProtoMember(1)]
        public int Id { get; set; }
        [ProtoMember(2)]
        public Answer[] Answers;
        [ProtoMember(4)]
        public Answer AcceptedAnswer;
        [ProtoMember(5)]
        public DateTime CreationDate;
        [ProtoMember(6)]
        public int Score;
        [ProtoMember(7)]
        public int ViewCount;
        [ProtoMember(8)]
        public string Body;
        [ProtoMember(9)]
        public Member Owner;
        [ProtoMember(10)]
        public Member LastEditor;
        [ProtoMember(12)]
        public DateTime LastEditDate;
        [ProtoMember(13)]
        public DateTime LastActivityDate;
        [ProtoMember(14)]
        public DateTime CommunityOwnedDate;
        [ProtoMember(15)]
        public DateTime ClosedDate;
        [ProtoMember(16)]
        public string Title;
        [ProtoMember(17)]
        public string Tags;
        [ProtoMember(20)]
        public int FavoriteCount;
    }
}
