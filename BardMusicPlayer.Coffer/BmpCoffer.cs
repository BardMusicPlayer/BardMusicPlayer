/*
 * Copyright(c) 2021 MoogleTroupe, isaki
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BardMusicPlayer.Quotidian.Structs;
using BardMusicPlayer.Transmogrify.Song;
using LiteDB;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

namespace BardMusicPlayer.Coffer
{
    public sealed class BmpCoffer : IDisposable
    {
        private static BmpCoffer _instance;

        /// <summary>
        /// Initializes the default coffer instance
        /// </summary>
        /// <param name="filename">full path to the coffer file</param>
        public static void Initialize(string filename)
        {
            if (Initialized) return;

            _instance = CreateInstance(filename);
        }

        /// <summary>
        /// Returns true if the default coffer instance is initialized
        /// </summary>
        public static bool Initialized => _instance != null;

        /// <summary>
        /// Gets the default coffer instance
        /// </summary>
        public static BmpCoffer Instance =>
            _instance ?? throw new BmpCofferException("This coffer must be initialized first.");

        private readonly LiteDatabase dbi;
        private bool disposedValue;

        /// <summary>
        /// Internal constructor; this object is constructed with a factory pattern.
        /// </summary>
        /// <param name="dbi"></param>
        private BmpCoffer(LiteDatabase dbi)
        {
            this.dbi      = dbi;
            disposedValue = false;
        }

        /// <summary>
        /// Create a new instance of the BmpCoffer manager based on the given LiteDB database.
        /// </summary>
        /// <param name="dbPath"></param>
        /// <returns></returns>
        internal static BmpCoffer CreateInstance(string dbPath)
        {
            var mapper = new BsonMapper();
            mapper.RegisterType
            (
                group => group.Index,
                bson => Instrument.Parse(bson.AsInt32)
            );
            mapper.RegisterType
            (
                group => group.Index,
                bson => InstrumentTone.Parse(bson.AsInt32)
            );
            mapper.RegisterType
            (
                group => group.Index,
                bson => OctaveRange.Parse(bson.AsInt32)
            );
            mapper.RegisterType
            (
                tempoMap => SerializeTempoMap(tempoMap),
                bson => DeserializeTempoMap(bson.AsBinary)
            );
            mapper.RegisterType
            (
                trackChunk => SerializeTrackChunk(trackChunk),
                bson => DeserializeTrackChunk(bson.AsBinary)
            );

            var dbi = new LiteDatabase(dbPath, mapper);
            MigrateDatabase(dbi);

            return new BmpCoffer(dbi);
        }

        /// <summary>
        /// Serializes a TempoMap from DryWetMidi.
        /// </summary>
        /// <param name="tempoMap"></param>
        /// <returns></returns>
        private static byte[] SerializeTempoMap(TempoMap tempoMap)
        {
            var midiFile = new MidiFile(new TrackChunk());
            midiFile.ReplaceTempoMap(tempoMap);
            using var memoryStream = new MemoryStream();
            midiFile.Write(memoryStream);
            var bson = memoryStream.ToArray();
            memoryStream.Dispose();
            return bson;
        }

        /// <summary>
        /// Deserializes a TempoMap from DryWetMidi.
        /// </summary>
        /// <param name="bson"></param>
        /// <returns></returns>
        private static TempoMap DeserializeTempoMap(byte[] bson)
        {
            using var memoryStream = new MemoryStream(bson);
            var midiFile = MidiFile.Read(memoryStream);
            var tempoMap = midiFile.GetTempoMap().Clone();
            memoryStream.Dispose();
            return tempoMap;
        }

        /// <summary>
        /// Serializes a TrackChunk from DryWetMidi.
        /// </summary>
        /// <param name="trackChunk"></param>
        /// <returns></returns>
        private static byte[] SerializeTrackChunk(TrackChunk trackChunk)
        {
            var midiFile = new MidiFile(trackChunk);
            using var memoryStream = new MemoryStream();
            midiFile.Write(memoryStream);
            var bson = memoryStream.ToArray();
            memoryStream.Dispose();
            return bson;
        }

        /// <summary>
        /// Deserializes a TrackChunk from DryWetMidi.
        /// </summary>
        /// <param name="bson"></param>
        /// <returns></returns>
        private static TrackChunk DeserializeTrackChunk(byte[] bson)
        {
            using var memoryStream = new MemoryStream(bson);
            var midiFile = MidiFile.Read(memoryStream);
            var trackChunk = midiFile.GetTrackChunks().First();
            memoryStream.Dispose();
            return trackChunk;
        }

        /// <summary>
        /// This creates a playlist containing songs that match the given tag.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public IPlaylist CreatePlaylistFromTag(string tag)
        {
            if (tag == null) throw new ArgumentNullException();

            var songCol = GetSongCollection();

            // TODO: This is brute force and not memory efficient; there has to be a better
            // way to do this, but my knowledge of LINQ and BsonExpressions isn't there yet.
            var allSongs = songCol.FindAll();
            var songList = new List<BmpSong>();

            foreach (var entry in allSongs)
            {
                if (TagMatches(tag, entry)) songList.Add(entry);
            }

            if (songList.Count == 0) return null;

            var dbList = new BmpPlaylist()
            {
                Name  = tag,
                Songs = songList,
                Id    = null
            };

            return new BmpPlaylistDecorator(dbList);
        }

        /// <summary>
        /// This creates a new empty playlist with the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IPlaylist CreatePlaylist(string name)
        {
            if (name == null) throw new ArgumentNullException();

            var dbList = new BmpPlaylist()
            {
                Songs = new List<BmpSong>(),
                Name  = name,
                Id    = null
            };

            return new BmpPlaylistDecorator(dbList);
        }

        /// <summary>
        /// This retrieves a playlist with the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>The playlist if found or null if no matching playlist exists.</returns>
        public IPlaylist GetPlaylist(string name)
        {
            if (name == null) throw new ArgumentNullException();

            var playlists = GetPlaylistCollection();

            // We guarantee uniqueness in index and code, therefore
            // there should be one and only one list.
            var dbList = playlists.Query()
                .Include(x => x.Songs)
                .Where(x => x.Name == name)
                .Single();

            return dbList != null ? new BmpPlaylistDecorator(dbList) : null;
        }

        /// <summary>
        /// This retrieves the names of all saved playlists.
        /// </summary>
        /// <returns></returns>
        public IList<string> GetPlaylistNames()
        {
            var playlists = GetPlaylistCollection();

            // Want to ensure we don't pull in the trackchunk data.
            return playlists.Query()
                .Select<string>(x => x.Name)
                .ToList();
        }

        /// <summary>
        /// This retrieves the song with the given title.
        /// </summary>
        /// <param name="title"></param>
        /// <returns>The song if found or null if no matching song exists.</returns>
        public BmpSong GetSong(string title)
        {
            if (title == null) throw new ArgumentNullException();

            var songCol = GetSongCollection();

            return songCol.FindOne(x => x.Title == title);
        }

        /// <summary>
        /// This retrieves the titles of all saved songs.
        /// </summary>
        /// <returns></returns>
        public IList<string> GetSongTitles()
        {
            var songCol = GetSongCollection();

            return songCol.Query()
                .Select<string>(x => x.Title)
                .ToList();
        }

        /// <summary>
        /// This saves a song.
        /// </summary>
        /// <param name="song"></param>
        /// <exception cref="BmpCofferException">This is thrown if a title conflict occurs on save.</exception>
        public void SaveSong(BmpSong song)
        {
            if (song == null) throw new ArgumentNullException();

            var songCol = GetSongCollection();

            try
            {
                if (song.Id == null)
                {
                    song.Id = ObjectId.NewObjectId();
                    //TODO: Fix this to get a real unique idendifier
                    var results = songCol.Find(x => x.Title.Equals(song.Title));
                    if (results.Count() > 0)
                    {
                        songCol.Update(song);
                        return;
                    }
                    songCol.Insert(song);
                }
                else
                    songCol.Update(song);
            }
            catch (LiteException e)
            {
                throw new BmpCofferException(e.Message, e);
            }
        }

        /// <summary>
        /// This saves a playlist.
        /// </summary>
        /// <param name="songList"></param>
        /// <exception cref="BmpCofferException">This is thrown if a name conflict occurs on save.</exception>
        public void SavePlaylist(IPlaylist songList)
        {
            if (songList.GetType() != typeof(BmpPlaylistDecorator))
                throw new Exception("Unsupported implementation of IPlaylist");

            var playlists = GetPlaylistCollection();

            var dbList = ((BmpPlaylistDecorator) songList).GetBmpPlaylist();

            try
            {
                if (dbList.Id == null)
                {
                    dbList.Id = ObjectId.NewObjectId();
                    playlists.Insert(dbList);
                }
                else
                    playlists.Update(dbList);
            }
            catch (LiteException e)
            {
                throw new BmpCofferException(e.Message, e);
            }
        }

        /// <summary>
        /// This deletes a playlist. TODO: Make sure all data is erased
        /// </summary>
        /// <param name="songList"></param>
        /// <exception cref="BmpCofferException">This is thrown if a name conflict occurs on save.</exception>
        public void DeletePlaylist(IPlaylist songList)
        {
            if (songList.GetType() != typeof(BmpPlaylistDecorator))
                throw new Exception("Unsupported implementation of IPlaylist");

            var playlists = GetPlaylistCollection();

            var dbList = ((BmpPlaylistDecorator)songList).GetBmpPlaylist();

            try
            {
                if (dbList.Id != null)
                {
                    playlists.Delete(dbList.Id);
                }
            }
            catch (LiteException e)
            {
                throw new BmpCofferException(e.Message, e);
            }
        }

        /// <summary>
        /// Generated by VS2019.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose method.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing) dbi.Dispose();

                disposedValue = true;
            }
        }

        /// <summary>
        /// Utility method.
        /// </summary>
        /// <returns></returns>
        private ILiteCollection<BmpPlaylist> GetPlaylistCollection() =>
            dbi.GetCollection<BmpPlaylist>(Constants.PLAYLIST_COL_NAME);

        /// <summary>
        /// Utility method.
        /// </summary>
        /// <returns></returns>
        private ILiteCollection<BmpSong> GetSongCollection() => dbi.GetCollection<BmpSong>(Constants.SONG_COL_NAME);

        /// <summary>
        /// Tag matching algorithm.
        /// </summary>
        /// <param name="search"></param>
        /// <param name="song"></param>
        /// <returns></returns>
        private static bool TagMatches(string search, BmpSong song)
        {
            var ret = false;
            var tags = song.Tags;

            if (tags != null && tags.Count > 0)
            {
                for (var i = 0; i < tags.Count; ++i)
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

        /// <summary>
        /// Database creation/migration method.
        /// </summary>
        /// <param name="dbi"></param>
        internal static void MigrateDatabase(LiteDatabase dbi)
        {
            // This method exists to provide a way to have different versions of key
            // API objects, such as MMSong, and a way to migrate that data (or nuke it
            // if required).

            // Currently, we are version 1, so the only thing to do is to inject the requisite metadata.
            var schemaData = dbi.GetCollection<LiteDBSchema>(Constants.SCHEMA_COL_NAME);
            var dataCount = schemaData.Count();

            if (dataCount > 1) throw new Exception("Invalid schema collection in database");

            bool insertRequired;
            if (dataCount == 0)
                insertRequired = true;
            else
            {
                var result = schemaData.FindOne(x => true);
                if (result.Version == Constants.SCHEMA_VERSION)
                    insertRequired = false;
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
            var songs = dbi.GetCollection<BmpSong>(Constants.SONG_COL_NAME);
            songs.EnsureIndex(x => x.Title, true);
            songs.EnsureIndex(x => x.Tags);

            // Create the custom playlist collection and add indicies
            var playlists = dbi.GetCollection<BmpPlaylist>(Constants.PLAYLIST_COL_NAME);
            playlists.EnsureIndex(x => x.Name, true);
        }
    }
}