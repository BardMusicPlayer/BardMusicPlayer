/*
 * Copyright(c) 2021 MoogleTroupe, isaki
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Collections.Generic;
using BardMusicPlayer.Transmogrify.Song;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BardMusicPlayer.Coffer.Tests
{
    [TestClass]
    [DeploymentItem("Resources\\test.mid")]
    public class CofferManagerTest : CofferTestBase
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
            var song = LoadTestSong();
            Assert.IsNull(song.Id);

            using (var test = CreateCofferManager())
            {
                test.SaveSong(song);
                Assert.IsNotNull(song.Id);

                var resultTitle = test.GetSong(song.Title);
                Assert.AreEqual(song.Id, resultTitle.Id);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(BmpCofferException))]
        public void TestDuplicateSong()
        {
            var songA = LoadTestSong();
            Assert.IsNull(songA.Id);

            var songB = LoadTestSong();
            Assert.IsNull(songB.Id);

            using (var test = CreateCofferManager())
            {
                test.SaveSong(songA);
                Assert.IsNotNull(songA.Id);

                test.SaveSong(songB);
            }
        }

        [TestMethod]
        public void TestSongListing()
        {
            var song = LoadTestSong();
            Assert.IsNull(song.Id);

            using (var test = CreateCofferManager())
            {
                test.SaveSong(song);
                Assert.IsNotNull(song.Id);

                var resultAll = test.GetSongTitles();
                Assert.AreEqual(1, resultAll.Count);
                Assert.AreEqual(song.Title, resultAll[0]);
            }
        }

        [TestMethod]
        public void TestPlaylistFromTag()
        {
            var song = LoadTestSong();
            Assert.IsNull(song.Id);

            song.Tags.Add(TEST_SONG_TAG_A);
            song.Tags.Add(TEST_SONG_TAG_B);

            using (var test = CreateCofferManager())
            {
                test.SaveSong(song);
                Assert.IsNotNull(song.Id);

                var tagPlaylist = test.CreatePlaylistFromTag(TEST_SONG_TAG_A);
                Assert.IsNotNull(tagPlaylist);

                var allTags = new List<BmpSong>();
                foreach (var entry in tagPlaylist)
                {
                    allTags.Add(entry);
                }

                Assert.AreEqual(1, allTags.Count);
                Assert.AreEqual(song.Id, allTags[0].Id);

                tagPlaylist = test.CreatePlaylistFromTag(TEST_SONG_TAG_B);
                Assert.IsNotNull(tagPlaylist);

                allTags.Clear();
                foreach (var entry in tagPlaylist)
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
            var song = LoadTestSong();
            Assert.IsNull(song.Id);

            using (var test = CreateCofferManager())
            {
                test.SaveSong(song);
                Assert.IsNotNull(song.Id);

                var playlist = test.CreatePlaylist(PLAYLIST_NAME);
                Assert.IsNotNull(playlist);
                Assert.IsTrue(playlist is BmpPlaylistDecorator);
                Assert.AreEqual(PLAYLIST_NAME, playlist.GetName());

                // Internal things...
                var backingData = ((BmpPlaylistDecorator) playlist).GetBmpPlaylist();
                Assert.IsNull(backingData.Id);

                playlist.Add(song);

                test.SavePlaylist(playlist);
                Assert.IsNotNull(backingData.Id);

                var result = test.GetPlaylist(PLAYLIST_NAME);
                Assert.IsNotNull(result);
                Assert.AreEqual(backingData.Id, ((BmpPlaylistDecorator) playlist).GetBmpPlaylist().Id);
            }
        }

        [TestMethod]
        public void TestPlaylistListing()
        {
            var song = LoadTestSong();
            Assert.IsNull(song.Id);

            using (var test = CreateCofferManager())
            {
                test.SaveSong(song);
                Assert.IsNotNull(song.Id);

                var playlist = test.CreatePlaylist(PLAYLIST_NAME);
                Assert.IsNotNull(playlist);

                playlist.Add(song);

                test.SavePlaylist(playlist);

                var names = test.GetPlaylistNames();
                Assert.AreEqual(1, names.Count);
                Assert.AreEqual(playlist.GetName(), names[0]);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(BmpCofferException))]
        public void TestDuplicatePlaylist()
        {
            var song = LoadTestSong();
            Assert.IsNull(song.Id);

            using (var test = CreateCofferManager())
            {
                test.SaveSong(song);
                Assert.IsNotNull(song.Id);

                var playlistA = test.CreatePlaylist(PLAYLIST_NAME);
                Assert.IsNotNull(playlistA);

                var playlistB = test.CreatePlaylist(PLAYLIST_NAME);
                Assert.IsNotNull(playlistB);

                test.SavePlaylist(playlistA);
                test.SavePlaylist(playlistB);
            }
        }

        private BmpCoffer CreateCofferManager()
        {
            var dbPath = GetDBPath();
            return BmpCoffer.CreateInstance(dbPath);
        }

        private static BmpSong LoadTestSong() => BmpSong.OpenMidiFile(TEST_SONG_FILENAME).GetAwaiter().GetResult();
    }
}