using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFMemoryParser {

    [Serializable]
    public class ActorData {
        public string name = string.Empty;
        public uint id = 0;
        public uint jobid = 0;
        public uint perfid = 0;
        public int level = 0;
    };
    [Serializable]
    public class ActorDataList : Dictionary<uint, ActorData> { }
    
    [Serializable]
    public class SigActorsData {
        public ActorDataList removedActors = new ActorDataList();
        public ActorDataList currentActors = new ActorDataList();
        public ActorDataList addedActors = new ActorDataList();
    };
}
