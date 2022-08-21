using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BardMusicPlayer.Transmogrify.Song.Importers
{
    public class MMSongContainer
    {
        public List<MMSong> songs = new List<MMSong>();
        public MMSongContainer()
        {
            var s = new MMSong();
            songs.Add(s);
        }
    }

    public class MMSong
    {
        public List<MMBards> bards = new List<MMBards>();
        public List<MMLyrics> lyrics = new List<MMLyrics>();
        public string title{ get; set; } ="";
        public string description { get; set; } = "";
    }

    public class MMBards
    {
        public int instrument { get; set; } = 0;
        public Dictionary<int, int> sequence = new Dictionary<int, int>();
    }

    public class MMLyrics
    {
        public string description { get; set; } = "";
        public Dictionary<int, string> lines = new Dictionary<int, string>();
        public Dictionary<int, int> sequence = new Dictionary<int, int>();
    }

}
