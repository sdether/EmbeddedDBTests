using System;
using ProtoBuf;

namespace Droog.EmbeddedDB.Tests.Entities {
    [ProtoContract]
    public class Member : IEntity {
        [ProtoMember(1)]
        public int Id { get; set; }
        [ProtoMember(2)]
        public int Reputation;
        [ProtoMember(3)]
        public DateTime CreationDate;
        [ProtoMember(4)]
        public string DisplayName;
        [ProtoMember(5)]
        public string EmailHash;
        [ProtoMember(6)]
        public DateTime LastAccessDate;
        [ProtoMember(7)]
        public string WebsiteUrl;
        [ProtoMember(8)]
        public string Location;
        [ProtoMember(9)]
        public int Age;
        [ProtoMember(10)]
        public string AboutMe;
        [ProtoMember(11)]
        public int Views;
        [ProtoMember(12)]
        public int UpVotes;
        [ProtoMember(13)]
        public int DownVotes;
    }
}