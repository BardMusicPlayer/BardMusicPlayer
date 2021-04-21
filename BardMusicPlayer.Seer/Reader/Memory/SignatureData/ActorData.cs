using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BardMusicPlayer.Seer.Reader.Memory.SignatureData {

    [Serializable]
    public class ActorData {
        public string name = string.Empty;
        public uint id = 0;
        public uint jobid = 0;
        public uint perfid = 0;
        public int level = 0;
    };
    [Serializable]
    public class ActorDataList : Dictionary<uint, ActorData> {
        public ActorDataList() { }
        public ActorDataList(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
    
    [Serializable]
    public class SigActorsData {
        public ActorDataList removedActors = new ActorDataList();
        public ActorDataList currentActors = new ActorDataList();
        public ActorDataList addedActors = new ActorDataList();
    };
}
