using BardMusicPlayer.Notate.Objects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace BardMusicPlayer.Catalog.Tests
{
    [TestClass, DeploymentItem("Resources\\To_The_Edge.mmsong")]
    public class CatalogManagerTest : CatalogTestBase
    {
        // Resource name
        private const string TEST_SONG_FILENAME = "To_The_Edge.mmsong";

        // Current tags in the file are "To The Edge" and "testing"
        private const string TEST_SONG_TAG_A = "testing";
        private const string TEST_SONG_TAG_B = "To The Edge";

        // Test playlist name
        private const string PLAYLIST_NAME = "Test Playlist";

        [TestMethod]
        public void TestSongBasics()
        {
            MMSong song = LoadTestSong();
            Assert.IsNull(song.Id);

            using (CatalogManager test = this.CreateCatalogManager())
            {
                test.SaveSong(song);
                Assert.IsNotNull(song.Id);

                MMSong resultTitle = test.GetSong(song.title);
                Assert.AreEqual(song.Id, resultTitle.Id);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(CatalogException))]
        public void TestDuplicateSong()
        {
            MMSong songA = LoadTestSong();
            Assert.IsNull(songA.Id);

            MMSong songB = LoadTestSong();
            Assert.IsNull(songB.Id);

            using (CatalogManager test = this.CreateCatalogManager())
            {
                test.SaveSong(songA);
                Assert.IsNotNull(songA.Id);

                test.SaveSong(songB);
            }
        }

        [TestMethod]
        public void TestSongListing()
        {
            MMSong song = LoadTestSong();
            Assert.IsNull(song.Id);

            using (CatalogManager test = this.CreateCatalogManager())
            {
                test.SaveSong(song);
                Assert.IsNotNull(song.Id);

                IList<string> resultAll = test.GetSongTitles();
                Assert.AreEqual(1, resultAll.Count);
                Assert.AreEqual(song.title, resultAll[0]);
            }
        }

        [TestMethod]
        public void TestPlaylistFromTag()
        {
            MMSong song = LoadTestSong();
            Assert.IsNull(song.Id);

            using (CatalogManager test = this.CreateCatalogManager())
            {
                test.SaveSong(song);
                Assert.IsNotNull(song.Id);

                IPlaylist tagPlaylist = test.CreatePlaylistFromTag(TEST_SONG_TAG_A);
                Assert.IsNotNull(tagPlaylist);

                List<MMSong> allTags = new List<MMSong>();
                foreach (MMSong entry in tagPlaylist)
                {
                    allTags.Add(entry);
                }

                Assert.AreEqual(1, allTags.Count);
                Assert.AreEqual(song.Id, allTags[0].Id);

                tagPlaylist = test.CreatePlaylistFromTag(TEST_SONG_TAG_B);
                Assert.IsNotNull(tagPlaylist);

                allTags.Clear();
                foreach (MMSong entry in tagPlaylist)
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
            MMSong song = LoadTestSong();
            Assert.IsNull(song.Id);

            using (CatalogManager test = this.CreateCatalogManager())
            {
                test.SaveSong(song);
                Assert.IsNotNull(song.Id);

                IPlaylist playlist = test.CreatePlaylist(PLAYLIST_NAME);
                Assert.IsNotNull(playlist);
                Assert.IsTrue(playlist is DBPlaylistDecorator);
                Assert.AreEqual(PLAYLIST_NAME, playlist.GetName());

                // Internal things...
                DBPlaylist backingData = ((DBPlaylistDecorator)playlist).GetDBPlaylist();
                Assert.IsNull(backingData.Id);

                playlist.Add(song);

                test.SavePlaylist(playlist);
                Assert.IsNotNull(backingData.Id);

                IPlaylist result = test.GetPlaylist(PLAYLIST_NAME);
                Assert.IsNotNull(result);
                Assert.AreEqual(backingData.Id, ((DBPlaylistDecorator)playlist).GetDBPlaylist().Id);
            }
        }

        [TestMethod]
        public void TestPlaylistListing()
        {
            MMSong song = LoadTestSong();
            Assert.IsNull(song.Id);

            using (CatalogManager test = this.CreateCatalogManager())
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
        [ExpectedException(typeof(CatalogException))]
        public void TestDuplicatePlaylist()
        {
            MMSong song = LoadTestSong();
            Assert.IsNull(song.Id);

            using (CatalogManager test = this.CreateCatalogManager())
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

        private CatalogManager CreateCatalogManager()
        {
            string dbPath = this.GetDBPath();
            return CatalogManager.CreateInstance(dbPath);
        }

        private static MMSong LoadTestSong() => MMSong.Open(TEST_SONG_FILENAME);
    }
}
