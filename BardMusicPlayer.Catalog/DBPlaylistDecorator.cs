using BardMusicPlayer.Notate.Objects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace BardMusicPlayer.Catalog
{
    public sealed class DBPlaylistDecorator : IPlaylist
    {
        private readonly DBPlaylist target;

        internal DBPlaylistDecorator(DBPlaylist target) => this.target = target ?? throw new NullReferenceException();

        void IPlaylist.Add(MMSong song)
        {
            this.target.Songs.Add(song);
        }

        void IPlaylist.Add(int idx, MMSong song)
        {
            this.target.Songs.Insert(idx, song);
        }

        IEnumerator<MMSong> IEnumerable<MMSong>.GetEnumerator()
        {
            return this.target.Songs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.target.Songs.GetEnumerator();
        }

        string IPlaylist.GetName()
        {
            return this.target.Name;
        }

        void IPlaylist.Move(int sourceIdx, int targetIdx)
        {
            List<MMSong> contents = this.target.Songs;
            MMSong moveMe = contents[sourceIdx];
            contents.RemoveAt(sourceIdx);
            contents.Insert(targetIdx, moveMe);
        }

        void IPlaylist.Remove(int idx)
        {
            this.target.Songs.RemoveAt(idx);
        }

        void IPlaylist.SetName(string name)
        {
            this.target.Name = name ?? throw new ArgumentNullException();
        }

        internal DBPlaylist GetDBPlaylist() => this.target;
    }
}
