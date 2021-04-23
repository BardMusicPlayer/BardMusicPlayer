using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BardMusicPlayer
{
    [DataContract]
    internal struct UpdateInfo
    {
        [DataMember]
        public bool deprecated { get; set; }
        [DataMember]
        public string deprecatedMessage { get; set; }
        [DataMember]
        public string deprecatedTitle { get; set; }
        [DataMember]
        public string newsUrl { get; set; }
        [DataMember]
        public List<string> versionPaths { get; set; }
    }
    [DataContract]
    internal struct Version
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
        public string extra { get; set; }
        [DataMember]
        public List<VersionItem> items { get; set; }
    }
    [DataContract]
    internal struct VersionItem
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
