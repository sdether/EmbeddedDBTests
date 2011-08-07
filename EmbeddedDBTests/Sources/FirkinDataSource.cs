using System;
using System.Collections.Generic;
using System.IO;
using Droog.EmbeddedDB.Tests.Entities;
using Droog.EmbeddedDB.Tests.Util;
using Droog.Firkin;
using Droog.Firkin.Serialization;
using ProtoBuf;

namespace Droog.EmbeddedDB.Tests.Sources {
    public static class FirkinDataSource {

        private const long USER_SIZE = 20 * 1024 * 1024;
        private const long POST_SIZE = 200 * 1024 * 1024;
        private const long QUESTION_SIZE = 100 * 1024 * 1024;

        private static readonly string _userTypename = typeof(User).ToString();
        private static readonly string _postTypename = typeof(Post).ToString();
        private static readonly string _questionTypename = typeof(Question).ToString();
        static FirkinDataSource() {
            SerializerRepository.RegisterStreamSerializer(new ProtoBufStreamSerializer<User>());
            SerializerRepository.RegisterStreamSerializer(new ProtoBufStreamSerializer<Post>());
            SerializerRepository.RegisterStreamSerializer(new ProtoBufStreamSerializer<Question>());
        }

        public static FirkinDictionary<int, T> WriteAll<T>(IEnumerable<T> data) where T : IEntity {
            var dir = GetStorageDir<T>();
            if(Directory.Exists(dir)) {
                throw new Exception(string.Format("directory {0} already exists", dir));
            }
            var dictionary = Open<T>();
            foreach(var entity in data) {
                dictionary[entity.Id] = entity;
            }
            return dictionary;
        }

        public static FirkinDictionary<int, T> Open<T>() {
            var t = typeof(T);
            long maxSize = t == typeof(User)
                               ? USER_SIZE
                               : t == typeof(Post)
                                     ? POST_SIZE
                                     : t == typeof(Question)
                                           ? QUESTION_SIZE
                                           : 10 * 1024 * 1024;
            return new FirkinDictionary<int, T>(
                 GetStorageDir<T>(),
                maxSize,
                SerializerRepository.GetByteArraySerializer<int>(),
                SerializerRepository.GetStreamSerializer<T>());
        }

        public static string GetStorageDir<T>() {
            return GetStorageDir(typeof(T));
        }

        public static string GetStorageDir(Type t) {
            return Util.Entity.GetFilename(t) + ".firkin";
        }
    }

    public class ProtoBufStreamSerializer<T> : IStreamSerializer<T> {
        public void Serialize(Stream destination, T value) {
            Serializer.SerializeWithLengthPrefix(destination, value, PrefixStyle.Base128, 1);
        }

        public T Deserialize(Stream source) {
            return Serializer.DeserializeWithLengthPrefix<T>(source, PrefixStyle.Base128, 1);
        }
    }


}
