/*
 * Copyright(c) 2021 MoogleTroupe, trotlinebeercan
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BardMusicPlayer.Updater.Util
{
    [DataContract]
    internal struct BmpVersion
    {
        [DataMember]
        public bool beta { get; set; }
        [DataMember]
        public string commit { get; set; }
        [DataMember]
        public int build { get; set; }
        [DataMember]
        public string entryDll { get; set; }
        [DataMember]
        public string entryClass { get; set; }
        [DataMember]
        public List<BmpVersionItem> items { get; set; }
    }
    [DataContract]
    internal struct BmpVersionItem
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
}
