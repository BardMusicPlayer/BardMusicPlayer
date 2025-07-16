/*
 * Copyright(c) 2007-2020 Ryan Wilson syndicated.life@gmail.com (http://syndicated.life/)
 * Licensed under the MIT license. See https://github.com/FFXIVAPP/sharlayan/blob/master/LICENSE.md for full license information.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Files.Structures;
using BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Models;
using BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Models.Structures;
using Newtonsoft.Json;

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Utilities
{
    internal sealed class APIHelper
    {
        public static readonly JsonSerializerSettings SerializerSettings = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Populate
        };

        private readonly MemoryHandler memoryHandler;

        public APIHelper(MemoryHandler memoryHandler)
        {
            this.memoryHandler = memoryHandler;
        }

        public IEnumerable<Signature> GetSignatures()
        {
            var jsonStream =
                new MemoryStream(
                    (byte[])Files.Signatures.Signatures.ResourceManager.GetObject(memoryHandler.GameRegion
                        .ToString()) ?? Array.Empty<byte>());
            using var reader = new StreamReader(jsonStream);
            var json = reader.ReadToEnd();
            var signatures = JsonConvert.DeserializeObject<IEnumerable<Signature>>(json, SerializerSettings);
            var enumerable = signatures as Signature[] ?? signatures.ToArray();
            foreach (var signature in enumerable) signature.MemoryHandler = memoryHandler;

            return enumerable;
        }

        public StructuresContainer GetStructures()
        {
            var jsonStream =
                new MemoryStream(
                    (byte[])Structures.ResourceManager.GetObject(memoryHandler.GameRegion
                        .ToString()) ?? Array.Empty<byte>());
            using var reader = new StreamReader(jsonStream);
            var json = reader.ReadToEnd();
            return JsonConvert.DeserializeObject<StructuresContainer>(json, SerializerSettings);
        }
    }
}