/*
 * Copyright(c) 2021 MoogleTroupe, isaki
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Collections.Generic;
using BardMusicPlayer.Transmogrify.Song;
using LiteDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BardMusicPlayer.Coffer.Tests
{

    [TestClass]
    public class BmpPlaylistTest : CofferTestBase
    {
        [TestMethod]
        public void TestSerialization()
        {
            string playlistName = "Test Playlist";
            ObjectId playlistId = ObjectId.NewObjectId();
            BmpPlaylist test = new BmpPlaylist()
            {
                Name = playlistName,
                Songs = new List<BmpSong>(),
                Id = playlistId
            };

            BmpPlaylist saved;

            using (var dbi = this.CreateDatabase())
            {
                var collection = dbi.GetCollection<BmpPlaylist>(Constants.PLAYLIST_COL_NAME);
                collection.Insert(test);

                saved = collection.Query()
                    .Include(x => x.Songs)
                    .Where(x => x.Id.Equals(playlistId))
                    .First();
            }

            Assert.IsNotNull(saved);
            Assert.AreEqual(playlistName, saved.Name);
            Assert.AreEqual(playlistId, saved.Id);
            Assert.IsNotNull(saved.Songs);
            Assert.AreEqual(0, saved.Songs.Count);
        }
    }
}
