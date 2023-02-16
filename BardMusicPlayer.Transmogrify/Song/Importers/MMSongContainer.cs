#region

using System.Collections.Generic;

#endregion

namespace BardMusicPlayer.Transmogrify.Song.Importers;

public sealed class MMSongContainer
{
    public List<MMSong> songs = new();

    public MMSongContainer()
    {
        var s = new MMSong();
        songs.Add(s);
    }
}

public sealed class MMSong
{
    public List<MMBards> bards = new();
    public List<MMLyrics> lyrics = new();
    public string title { get; set; } = "";
    public string description { get; set; } = "";
}

public class MMBards
{
    public Dictionary<int, int> sequence = new();
    public int instrument { get; set; } = 0;
}

public class MMLyrics
{
    public Dictionary<int, string> lines = new();
    public Dictionary<int, int> sequence = new();
    public string description { get; set; } = "";
}