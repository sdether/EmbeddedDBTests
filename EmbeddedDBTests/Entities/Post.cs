using System;
using System.Xml.Serialization;
using ProtoBuf;

namespace Droog.EmbeddedDB.Tests.Entities {
    [ProtoContract]
    [XmlRoot("row")]
    public class Post : IEntity {
        [ProtoMember(1)]
        [XmlAttribute]
        public int Id { get; set; }

        [ProtoIgnore]
        [XmlAttribute]
        public byte PostTypeId {
            get { return (byte)PostType; }
            set { PostType = (PostType)value; }
        }

        [XmlIgnore]
        [ProtoMember(2,IsRequired = true)]
        public PostType PostType { get; set; }
        [ProtoMember(3)]
        [XmlAttribute]
        public int ParentId;
        [ProtoMember(4)]
        [XmlAttribute]
        public int AcceptedAnswerId;
        [ProtoMember(5)]
        [XmlAttribute]
        public DateTime CreationDate;
        [ProtoMember(6)]
        [XmlAttribute]
        public int Score;
        [ProtoMember(7)]
        [XmlAttribute]
        public int ViewCount;
        [ProtoMember(8)]
        [XmlAttribute]
        public string Body;
        [ProtoMember(9)]
        [XmlAttribute]
        public int OwnerUserId;
        [ProtoMember(10)]
        [XmlAttribute]
        public int LastEditorUserId;
        [ProtoMember(11)]
        [XmlAttribute]
        public string LastEditorDisplayname;
        [ProtoMember(12)]
        [XmlAttribute]
        public DateTime LastEditDate;
        [ProtoMember(13)]
        [XmlAttribute]
        public DateTime LastActivityDate;
        [ProtoMember(14)]
        [XmlAttribute]
        public DateTime CommunityOwnedDate;
        [ProtoMember(15)]
        [XmlAttribute]
        public DateTime ClosedDate;
        [ProtoMember(16)]
        [XmlAttribute]
        public string Title;
        [ProtoMember(17)]
        [XmlAttribute]
        public string Tags;
        [ProtoMember(18)]
        [XmlAttribute]
        public int AnswerCount;
        [ProtoMember(19)]
        [XmlAttribute]
        public int CommentCount;
        [ProtoMember(20)]
        [XmlAttribute]
        public int FavoriteCount;
    }

    [ProtoContract]
    public enum PostType : byte {
        [ProtoEnum]
        Invalid = 0,
        [ProtoEnum]
        Question = 1,
        [ProtoEnum]
        Answer = 2
    }
}
