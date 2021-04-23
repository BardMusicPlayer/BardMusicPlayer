using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace BardMusicPlayer
{
    internal static class Extensions
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
