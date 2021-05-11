/*
 * Copyright(c) 2021 isaki
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using BardMusicPlayer.Notate.Song;

namespace BardMusicPlayer.Catalog.Tests
{
    [TestClass, DeploymentItem("Resources\\test.mid")]
    public class CatalogManagerTest : CatalogTestBase
    {
        // Resource name
        private const string TEST_SONG_FILENAME = "test.mid";

        // Current tags in the file are "To The Edge" and "testing"
        private const string TEST_SONG_TAG_A = "Test Tag 1";
        private const string TEST_SONG_TAG_B = "Test Tag 2";

        // Test playlist name
        private const string PLAYLIST_NAME = "Test Playlist";

        [TestMethod]
        public void TestSongBasics()
        {
            BmpSong song = LoadTestSong();
            Assert.IsNull(song.Id);

            using (BmpCatalog test = this.CreateCatalogManager())
            {
                test.SaveSong(song);
                Assert.IsNotNull(song.Id);

                BmpSong resultTitle = test.GetSong(song.Title);
                Assert.AreEqual(song.Id, resultTitle.Id);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(BmpCatalogException))]
        public void TestDuplicateSong()
        {
            BmpSong songA = LoadTestSong();
            Assert.IsNull(songA.Id);

            BmpSong songB = LoadTestSong();
            Assert.IsNull(songB.Id);

            using (BmpCatalog test = this.CreateCatalogManager())
            {
                test.SaveSong(songA);
                Assert.IsNotNull(songA.Id);

                test.SaveSong(songB);
            }
        }

        [TestMethod]
        public void TestSongListing()
        {
            BmpSong song = LoadTestSong();
            Assert.IsNull(song.Id);

            using (BmpCatalog test = this.CreateCatalogManager())
            {
                test.SaveSong(song);
                Assert.IsNotNull(song.Id);

                IList<string> resultAll = test.GetSongTitles();
                Assert.AreEqual(1, resultAll.Count);
                Assert.AreEqual(song.Title, resultAll[0]);
            }
        }

        [TestMethod]
        public void TestPlaylistFromTag()
        {
            BmpSong song = LoadTestSong();
            Assert.IsNull(song.Id);

            song.Tags.Add("Test Tag 1");
            song.Tags.Add("Test Tag 2");

            using (BmpCatalog test = this.CreateCatalogManager())
            {
                test.SaveSong(song);
                Assert.IsNotNull(song.Id);

                IPlaylist tagPlaylist = test.CreatePlaylistFromTag(TEST_SONG_TAG_A);
                Assert.IsNotNull(tagPlaylist);

                List<BmpSong> allTags = new List<BmpSong>();
                foreach (BmpSong entry in tagPlaylist)
                {
                    allTags.Add(entry);
                }

                Assert.AreEqual(1, allTags.Count);
                Assert.AreEqual(song.Id, allTags[0].Id);

                tagPlaylist = test.CreatePlaylistFromTag(TEST_SONG_TAG_B);
                Assert.IsNotNull(tagPlaylist);

                allTags.Clear();
                foreach (BmpSong entry in tagPlaylist)
                {
                    allTags.Add(entry);
                }

                Assert.AreEqual(1, allTags.Count);
                Assert.AreEqual(song.Id, allTags[0].Id);
            }
        }

        [TestMethod]
        public void TestPlaylistBasics()
        {
            BmpSong song = LoadTestSong();
            Assert.IsNull(song.Id);

            using (BmpCatalog test = this.CreateCatalogManager())
            {
                test.SaveSong(song);
                Assert.IsNotNull(song.Id);

                IPlaylist playlist = test.CreatePlaylist(PLAYLIST_NAME);
                Assert.IsNotNull(playlist);
                Assert.IsTrue(playlist is BmpPlaylistDecorator);
                Assert.AreEqual(PLAYLIST_NAME, playlist.GetName());

                // Internal things...
                BmpPlaylist backingData = ((BmpPlaylistDecorator)playlist).GetBmpPlaylist();
                Assert.IsNull(backingData.Id);

                playlist.Add(song);

                test.SavePlaylist(playlist);
                Assert.IsNotNull(backingData.Id);

                IPlaylist result = test.GetPlaylist(PLAYLIST_NAME);
                Assert.IsNotNull(result);
                Assert.AreEqual(backingData.Id, ((BmpPlaylistDecorator)playlist).GetBmpPlaylist().Id);
            }
        }

        [TestMethod]
        public void TestPlaylistListing()
        {
            BmpSong song = LoadTestSong();
            Assert.IsNull(song.Id);

            using (BmpCatalog test = this.CreateCatalogManager())
            {
                test.SaveSong(song);
                Assert.IsNotNull(song.Id);

                IPlaylist playlist = test.CreatePlaylist(PLAYLIST_NAME);
                Assert.IsNotNull(playlist);

                playlist.Add(song);

                test.SavePlaylist(playlist);

                IList<string> names = test.GetPlaylistNames();
                Assert.AreEqual(1, names.Count);
                Assert.AreEqual(playlist.GetName(), names[0]);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(BmpCatalogException))]
        public void TestDuplicatePlaylist()
        {
            BmpSong song = LoadTestSong();
            Assert.IsNull(song.Id);

            using (BmpCatalog test = this.CreateCatalogManager())
            {
                test.SaveSong(song);
                Assert.IsNotNull(song.Id);

                IPlaylist playlistA = test.CreatePlaylist(PLAYLIST_NAME);
                Assert.IsNotNull(playlistA);

                IPlaylist playlistB = test.CreatePlaylist(PLAYLIST_NAME);
                Assert.IsNotNull(playlistB);

                test.SavePlaylist(playlistA);
                test.SavePlaylist(playlistB);
            }
        }

        private BmpCatalog CreateCatalogManager()
        {
            string dbPath = this.GetDBPath();
            return BmpCatalog.CreateInstance(dbPath);
        }

        private static BmpSong LoadTestSong() => BmpSong.OpenMidiFile(TEST_SONG_FILENAME).GetAwaiter().GetResult();
    }
}
