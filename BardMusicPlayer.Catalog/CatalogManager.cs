using BardMusicPlayer.Notate.Objects;
using LiteDB;
using System;
using System.Collections.Generic;

namespace BardMusicPlayer.Catalog
{
    public sealed class CatalogManager : IDisposable
    {
        private readonly LiteDatabase dbi;
        private bool disposedValue;

        private CatalogManager(LiteDatabase dbi)
        {
            this.dbi = dbi;
            this.disposedValue = false;
        }

        public static CatalogManager CreateInstance(string dbPath)
        {
            var dbi = new LiteDatabase(dbPath);
            MigrateDatabase(dbi);

            return new CatalogManager(dbi);
        }

        public IPlaylist CreatePlaylistFromTag(string tag)
        {
            if (tag == null)
            {
                throw new ArgumentNullException();
            }

            var songCol = this.GetSongCollection();

            // TODO: This is brute force and not memory efficient; there has to be a better
            // way to do this, but my knowledge of LINQ and BsonExpressions isn't there yet.
            var allSongs = songCol.FindAll();
            var songList = new List<MMSong>();

            foreach (var entry in allSongs)
            {
                if (TagMatches(tag, entry))
                {
                    songList.Add(entry);
                }
            }

            if (songList.Count == 0)
            {
                return null;
            }

            var dbList = new DBPlaylist()
            {
                Name = tag,
                Songs = songList,
                Id = null
            };

            return new DBPlaylistDecorator(dbList);
        }

        public IPlaylist CreatePlaylist(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException();
            }

            var dbList = new DBPlaylist()
            {
                Songs = new List<MMSong>(),
                Name = name,
                Id = null
            };

            return new DBPlaylistDecorator(dbList);
        }

        public IPlaylist GetPlaylist(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException();
            }

            var playlists = this.GetPlaylistCollection();

            // We guarantee uniqueness in index and code, therefore
            // there should be one and only one list.
            var dbList = playlists.Query()
                .Include(x => x.Songs)
                .Where(x => x.Name == name)
                .Single();

            return (dbList != null) ? new DBPlaylistDecorator(dbList) : null;
        }

        public IList<string> GetPlaylistNames()
        {
            var playlists = this.GetPlaylistCollection();

            // Want to ensure we don't pull in the mmsong data.
            return playlists.Query()
                .Select<string>(x => x.Name)
                .ToList();
        }

        public MMSong GetSong(string title)
        {
            if (title == null)
            {
                throw new ArgumentNullException();
            }

            var songCol = this.GetSongCollection();

            return songCol.FindOne(x => x.title == title);
        }

        public IList<string> GetSongTitles()
        {
            var songCol = this.GetSongCollection();

            return songCol.Query()
                .Select<string>(x => x.title)
                .ToList();
        }

        public void SaveSong(MMSong song)
        {
            if (song == null)
            {
                throw new ArgumentNullException();
            }

            var songCol = this.GetSongCollection();

            try
            {
                if (song.Id == null)
                {
                    song.Id = ObjectId.NewObjectId();
                    songCol.Insert(song);
                }
                else
                {
                    songCol.Update(song);
                }
            }
            catch (LiteException e)
            {
                throw new CatalogException(e.Message, e);
            }
        }

        public void SavePlaylist(IPlaylist songList)
        {
            if (songList.GetType() != typeof(DBPlaylistDecorator))
            {
                throw new Exception("Unsupported implementation of IPlaylist");
            }

            var playlists = this.GetPlaylistCollection();

            var dbList = ((DBPlaylistDecorator)songList).GetDBPlaylist();

            try
            {
                if (dbList.Id == null)
                {
                    dbList.Id = ObjectId.NewObjectId();
                    playlists.Insert(dbList);
                }
                else
                {
                    playlists.Update(dbList);
                }
            }
            catch (LiteException e)
            {
                throw new CatalogException(e.Message, e);
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.dbi.Dispose();
                }

                disposedValue = true;
            }
        }

        private ILiteCollection<DBPlaylist> GetPlaylistCollection()
        {
            return this.dbi.GetCollection<DBPlaylist>(Constants.PLAYLIST_COL_NAME);
        }

        private ILiteCollection<MMSong> GetSongCollection()
        {
            return this.dbi.GetCollection<MMSong>(Constants.SONG_COL_NAME);
        }

        private static bool TagMatches(string search, MMSong song)
        {
            bool ret = false;
            var tags = song.tags;

            if (tags != null && tags.Length > 0)
            {
                for (int i = 0; i < tags.Length; ++i)
                {
                    if (string.Equals(search, tags[i], StringComparison.OrdinalIgnoreCase))
                    {
                        ret = true;
                        break;
                    }
                }
            }

            return ret;
        }

        internal static void MigrateDatabase(LiteDatabase dbi)
        {
            // This method exists to provide a way to have different versions of key
            // API objects, such as MMSong, and a way to migrate that data (or nuke it
            // if required).

            // Currently, we are version 1, so the only thing to do is to inject the requisite metadata.
            var schemaData = dbi.GetCollection<LiteDBSchema>(Constants.SCHEMA_COL_NAME);
            int dataCount = schemaData.Count();

            if (dataCount > 1)
            {
                throw new Exception("Invalid schema collection in database");
            }

            bool insertRequired;
            if (dataCount == 0)
            {
                insertRequired = true;
            }
            else
            {
                var result = schemaData.FindOne(x => true);
                if (result.Version == Constants.SCHEMA_VERSION)
                {
                    insertRequired = false;
                }
                else
                {
                    schemaData.DeleteAll();
                    insertRequired = true;
                }
            }

            if (insertRequired)
            {
                var schema = new LiteDBSchema();
                schemaData.Insert(schema);
            }

            // Create the song collection and add indicies
            var songs = dbi.GetCollection<MMSong>(Constants.SONG_COL_NAME);
            songs.EnsureIndex(x => x.title, unique: true);
            songs.EnsureIndex(x => x.tags);

            // Create the custom playlist collection and add indicies
            var playlists = dbi.GetCollection<DBPlaylist>(Constants.PLAYLIST_COL_NAME);
            playlists.EnsureIndex(x => x.Name, unique: true);
        }
    }
}
