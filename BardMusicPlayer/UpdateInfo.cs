using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace BardMusicPlayer
{
    [DataContract]
    internal class UpdateInfo
    {
        internal const int BmpUpdaterVersion = 1;
        internal static Version BmpUiVersion { get; set; }
        [DataContract]
        internal struct Item
        {
            [DataMember]
            public bool load { get; set; }
            [DataMember]
            public string source { get; set; }
            [DataMember]
            public string destination { get; set; }
            [DataMember]
            public string sha256 { get; set; }
        }
        [DataContract]
        internal struct Version
        {
            [DataMember]
            public bool beta { get; set; }
            [DataMember]
            public string commit { get; set; }
            [DataMember]
            public long timeStamp { get; set; }
            [DataMember]
            public string entryDll { get; set; }
            [DataMember]
            public string entryClass { get; set; }
            [DataMember]
            public string extra { get; set; }
            [DataMember]
            public bool disabled { get; set; }
            [DataMember]
            public List<Item> items { get; set; }
        }
        [DataMember]
        public List<Version> versions { get; set; }
        [DataMember]
        public bool deprecated { get; set; }
        [DataMember]
        public string deprecatedMessage { get; set; }
    }

    internal static class UpdateInfoExtensions
    {
        internal static T DeserializeFromJson<T>(this string json)
        {
            var deserializer = new DataContractJsonSerializer(typeof(T));
            using var stream = new MemoryStream(Encoding.Unicode.GetBytes(json));
            return (T)deserializer.ReadObject(stream);
        }
        internal static string SerializeToJson<T>(this T type)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            using var stream = new MemoryStream();
            serializer.WriteObject(stream, type);
            stream.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
