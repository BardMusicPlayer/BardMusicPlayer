/*
 * Copyright(c) 2021 isaki
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using Newtonsoft.Json;
using System.Text;

namespace BardMusicPlayer.Jamboree
{
    public sealed class JsonSerializationAdapter : ISerializationAdapter
    {
        /// <summary>
        /// Creates a new instance of the JSON serialization adapter.
        /// </summary>
        public JsonSerializationAdapter() { }

        ///<inheritdoc/>
        public T Decode<T>(byte[] buffer, int offset, int length)
        {
            // Ideally, we'd use a stream based approach instead of buffers and strings, but that can be done later.
            string json = Encoding.UTF8.GetString(buffer);
            return JsonConvert.DeserializeObject<T>(json);
        }

        ///<inheritdoc/>
        public byte[] Encode(object obj)
        {
            string json = JsonConvert.SerializeObject(obj);
            return Encoding.UTF8.GetBytes(json);
        }
    }
}
