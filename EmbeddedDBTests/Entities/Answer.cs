using System;
using ProtoBuf;

namespace Droog.EmbeddedDB.Tests.Entities {
    [ProtoContract]
    public class Answer : IEntity {
        [ProtoMember(1)]
        public int Id { get; set; }
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
    }
}