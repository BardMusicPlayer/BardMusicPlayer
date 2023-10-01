/*
 * Copyright(c) 2023 MoogleTroupe, isaki, GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Quotidian.Structs;
using BardMusicPlayer.Transmogrify.Song;
using LiteDB;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

namespace BardMusicPlayer.Coffer;

public sealed class BmpCoffer : IDisposable
{
    private static BmpCoffer _instance;

    public event Action OnPlaylistDataUpdated;

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

    private readonly LiteDatabase _dbi;
    private bool _disposedValue;

    /// <summary>
    /// Internal constructor; this object is constructed with a factory pattern.
    /// </summary>
    /// <param name="dbi"></param>
    private BmpCoffer(LiteDatabase dbi)
    {
        _dbi           = dbi;
        _disposedValue = false;
    }

    #region MainRoutines: Create / Load / Save / CleanUp
    /// <summary>
    /// Generates the <see cref="BsonMapper"/>
    /// </summary>
    /// <returns> <see cref="BsonMapper"/> </returns>
    private static BsonMapper GenerateMapper()
    {
        var mapper = new BsonMapper();
        mapper.RegisterType(static group => group.Index, static bson => Instrument.Parse(bson.AsInt32));
        mapper.RegisterType(static group => group.Index, static bson => InstrumentTone.Parse(bson.AsInt32));
        mapper.RegisterType(static group => group.Index, static bson => OctaveRange.Parse(bson.AsInt32));
        mapper.RegisterType(static tempoMap => SerializeTempoMap(tempoMap), static bson => DeserializeTempoMap(bson.AsBinary));
        mapper.RegisterType(static trackChunk => SerializeTrackChunk(trackChunk), static bson => DeserializeTrackChunk(bson.AsBinary));
        return mapper;
    }

    /// <summary>
    /// Create a new instance of the <see cref="BmpCoffer"/> manager based on the given LiteDB database.
    /// </summary>
    /// <param name="dbPath"></param>
    /// <returns> <see cref="BmpCoffer"/> </returns>
    private static BmpCoffer CreateInstance(string dbPath)
    {
        var dbi = new LiteDatabase("filename=" + dbPath + "; journal = false", GenerateMapper());
        MigrateDatabase(dbi);

        return new BmpCoffer(dbi);
    }

    /// <summary>
    /// Loads a LiteDB database from file
    /// </summary>
    /// <param name="file"></param>
    public void LoadNew(string file)
    {
        _dbi.Dispose();
        var dbi = new LiteDatabase(@"filename=" + file + "; journal = false", GenerateMapper()); //turn journal off, for big containers
        MigrateDatabase(dbi);

        _instance = new BmpCoffer(dbi);
    }

    /// <summary>
    /// Exports the current LiteDB database to a new file
    /// </summary>
    /// <param name="filename"></param>
    public void Export(string filename)
    {
        var t = new LiteDatabase(filename);
        var names = _dbi.GetCollectionNames();
        foreach (var name in names)
        {
            var col2 = _dbi.GetCollection(name);
            var col = t.GetCollection(name);
            try
            {
                col.InsertBulk(col2.FindAll());
            }
            catch
            {
                // ignored
            }
        }

        t.Dispose();
    }

    /// <summary>
    /// Cleans the database
    /// <para/>
    /// Removes unbound songs and rebuild the LiteDB.
    /// </summary>
    public void CleanUpDb()
    {
        //Try it and catch if the log file can't be removed
        try
        {
            //Check if we have songs without a playlist
            var differenceQuery = GetSongCollection().Query().Select(x => x.Id).ToList()
                .Except(from x in GetPlaylistCollection().Query().ToArray()
                    from y in x.Songs
                    select y.Id).ToList();
            //and remove them
            foreach (var id in differenceQuery)
                GetSongCollection().Delete(id);

            differenceQuery.Clear();

            _dbi.Checkpoint();
            _dbi.Rebuild();
        }
        catch
        {
            // ignored
        }
    }
    #endregion

    #region Serializations
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
    private static byte[] SerializeTrackChunk(MidiChunk trackChunk)
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
        //In case we have more than 1 chunk per track, combine them
        if (!midiFile.GetTrackChunks().Any())
            return new TrackChunk();

        var trackChunk = midiFile.GetTrackChunks().Merge();
        memoryStream.Dispose();
        return trackChunk;
    }
    #endregion

    #region Playlist

    /// <summary>
    /// This creates a playlist containing songs that match the given tag.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns><see cref="IPlaylist"/></returns>
    public IPlaylist CreatePlaylistFromTag(string tag)
    {
        if (tag == null)
        {
            throw new ArgumentNullException();
        }

        var songCol = GetSongCollection();

        // TODO: This is brute force and not memory efficient; there has to be a better
        // way to do this, but my knowledge of LINQ and BsonExpressions isn't there yet.
        var allSongs = songCol.FindAll();
        var songList = allSongs.Where(entry => TagMatches(tag, entry)).ToList();

        if (songList.Count == 0)
        {
            return null;
        }

        var dbList = new BmpPlaylist
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
    /// <returns><see cref="IPlaylist"/></returns>
    public static IPlaylist CreatePlaylist(string name)
    {
        if (name == null)
        {
            throw new ArgumentNullException();
        }

        var dbList = new BmpPlaylist
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
        if (name == null)
        {
            throw new ArgumentNullException();
        }

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
            .Select(x => x.Name)
            .ToList();
    }

    /// <summary>
    /// This saves a playlist.
    /// </summary>
    /// <param name="songList"></param>
    /// <exception cref="BmpCofferException">This is thrown if a name conflict occurs on save.</exception>
    public void SavePlaylist(IPlaylist songList)
    {
        if (songList.GetType() != typeof(BmpPlaylistDecorator))
        {
            throw new Exception("Unsupported implementation of IPlaylist");
        }

        var playlists = GetPlaylistCollection();

        var dbList = ((BmpPlaylistDecorator)songList).GetBmpPlaylist();

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
            OnPlaylistDataUpdated?.Invoke();
        }
        catch (LiteException e)
        {
            throw new BmpCofferException(e.Message, e);
        }
    }

    /// <summary>
    /// This deletes a playlist.
    /// </summary>
    /// <param name="songList"></param>
    public void DeletePlaylist(IPlaylist songList)
    {
        if (songList.GetType() != typeof(BmpPlaylistDecorator))
        {
            throw new Exception("Unsupported implementation of IPlaylist");
        }

        var playlists = GetPlaylistCollection();

        var dbList = ((BmpPlaylistDecorator)songList).GetBmpPlaylist();

        try
        {
            if (dbList.Id != null)
            {
                foreach (var song in dbList.Songs)
                    DeleteSong(song);
                playlists.Delete(dbList.Id);
                OnPlaylistDataUpdated?.Invoke();
            }
        }
        catch (LiteException e)
        {
            throw new BmpCofferException(e.Message, e);
        }
    }
    #endregion

    #region Songs

    /// <summary>
    /// This retrieves the song with the given title.
    /// </summary>
    /// <param name="title"></param>
    /// <returns>The song if found or null if no matching song exists.</returns>
    public BmpSong GetSong(string title)
    {
        if (title == null)
        {
            throw new ArgumentNullException();
        }

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
            .Select(x => x.Title)
            .ToList();
    }

    /// Simple check if song is in database
    /// <param name="song"></param>
    /// <param name="strict"></param>
    /// <returns></returns>
    public bool IsSongInDatabase(BmpSong song, bool strict = true)
    {
        if (song == null)
        {
            throw new ArgumentNullException();
        }

        var songCol = GetSongCollection();
        var sList = strict ? songCol.Find(x => x.Title == song.Title) : songCol.Find(x => x.Title.StartsWith(song.Title));

        var inList = false;
        foreach (var s in sList)
        {
            if (s.TrackContainers.Count != song.TrackContainers.Count)
            {
                for (var i = 0; i != s.TrackContainers.Count; i++)
                {
                    if (s.TrackContainers[i].SourceTrackChunk.GetNotes().Count == song.TrackContainers[i].SourceTrackChunk.GetNotes().Count)
                        inList = true;
                }
            }
            else
                inList = true;

            const float tolerance = 0.0001f; // define a small tolerance value
            if (Math.Abs(s.Duration.TotalMilliseconds - song.Duration.TotalMilliseconds) > tolerance)
                inList = true;
        }
        return inList;
    }

    /// <summary>
    /// This saves a song.
    /// </summary>
    /// <param name="song"></param>
    /// <exception cref="BmpCofferException">This is thrown if a title conflict occurs on save.</exception>
    public void SaveSong(BmpSong song)
    {
        if (song == null)
        {
            throw new ArgumentNullException();
        }

        var songCol = GetSongCollection();
        try
        {
            if (song.Id == null)
            {
                //TODO: Fix this if more than one song with the name exists
                var results = songCol.Find(x => x.Title.Equals(song.Title));
                var bmpSongs = results as BmpSong[] ?? results.ToArray();
                if (bmpSongs.Any())
                {
                    //Get the ID from the found song and update the data
                    song.Id = bmpSongs.First().Id;
                    songCol.Update(song);
                    return;
                }

                song.Id = ObjectId.NewObjectId();
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
    /// This deletes a song.
    /// </summary>
    /// <param name="song"></param>
    /// <exception cref="BmpCofferException">This is thrown if a name conflict occurs on save.</exception>
    public void DeleteSong(BmpSong song)
    {
        if (song == null) throw new ArgumentNullException();

        //Check if the song is in use in other playlists
        if ((from x in GetPlaylistCollection().Query().ToArray()
                from y in x.Songs
                where y.Id.Equals(song.Id)
                select y.Id).Count() > 1)
            return;

        //if not, remove it
        var songCol = GetSongCollection();
        try
        {
            if (song.Id == null)
                return;

            var results = songCol.Find(x => x.Id.Equals(song.Id));
            if (results.Any())
                songCol.Delete(song.Id);
        }
        catch (LiteException e)
        {
            throw new BmpCofferException(e.Message, e);
        }
    }

    #endregion

    /// <summary>
    /// Generated by VS2019.
    /// </summary>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Dispose method.
    /// </summary>
    /// <param name="disposing"></param>
    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _dbi.Dispose();
            }

            _disposedValue = true;
        }
    }

    /// <summary>
    /// Utility method.
    /// </summary>
    /// <returns></returns>
    private ILiteCollection<BmpPlaylist> GetPlaylistCollection()
    {
        return _dbi.GetCollection<BmpPlaylist>(Constants.PLAYLIST_COL_NAME);
    }

    /// <summary>
    /// Utility method.
    /// </summary>
    /// <returns></returns>
    private ILiteCollection<BmpSong> GetSongCollection()
    {
        return _dbi.GetCollection<BmpSong>(Constants.SONG_COL_NAME);
    }

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

        if (tags is { Count: > 0 })
        {
            if (tags.Any(t => string.Equals(search, t, StringComparison.OrdinalIgnoreCase)))
            {
                ret = true;
            }
        }

        return ret;
    }

    /// <summary>
    /// Database creation/migration method.
    /// </summary>
    /// <param name="dbi"></param>
    private static void MigrateDatabase(ILiteDatabase dbi)
    {
        // This method exists to provide a way to have different versions of key
        // API objects, such as MMSong, and a way to migrate that data (or nuke it
        // if required).

        // Currently, we are version 1, so the only thing to do is to inject the requisite metadata.
        var schemaData = dbi.GetCollection<LiteDBSchema>(Constants.SCHEMA_COL_NAME);
        var dataCount = schemaData.Count();

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

        // Create the song collection and add indices
        var songs = dbi.GetCollection<BmpSong>(Constants.SONG_COL_NAME);
        songs.EnsureIndex(x => x.Title);
        songs.EnsureIndex(x => x.Tags);

        // Create the custom playlist collection and add indices
        var playlists = dbi.GetCollection<BmpPlaylist>(Constants.PLAYLIST_COL_NAME);
        playlists.EnsureIndex(x => x.Name, unique: true);
    }
}