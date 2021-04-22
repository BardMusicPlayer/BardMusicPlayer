using BardMusicPlayer.Notate.Objects;
using System.Collections.Generic;

namespace BardMusicPlayer.Catalog
{
    public interface IPlaylist : IEnumerable<MMSong>
    {
        void Add(MMSong song);

        void Add(int idx, MMSong song);

        void Move(int source, int target);

        void Remove(int idx);

        string GetName();

        void SetName(string name);
    }
}
