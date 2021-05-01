using BardMusicPlayer.Notate.Objects;
using System.Collections.Generic;

namespace BardMusicPlayer.Catalog
{
    public interface IPlaylist : IEnumerable<MMSong>
    {
        /// <summary>
        /// This adds a song to the playlist.
        /// </summary>
        /// <param name="song"></param>
        void Add(MMSong song);

        /// <summary>
        /// This song adds a song to the playlist at the given index.
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="song"></param>
        void Add(int idx, MMSong song);

        /// <summary>
        /// This moves a song within the playlist to the given index.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        void Move(int source, int target);

        /// <summary>
        /// This removes the song at the given index.
        /// </summary>
        /// <param name="idx"></param>
        void Remove(int idx);

        /// <summary>
        /// Returns the name of the playlist.
        /// </summary>
        /// <returns></returns>
        string GetName();

        /// <summary>
        /// Sets the name of the playlist.
        /// </summary>
        /// <param name="name"></param>
        void SetName(string name);
    }
}
