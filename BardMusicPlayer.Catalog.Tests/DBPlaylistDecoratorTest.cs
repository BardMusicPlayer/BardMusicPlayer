using BardMusicPlayer.Notate.Objects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace BardMusicPlayer.Catalog.Tests
{
    [TestClass]
    public class DBPlaylistDecoratorTest
    {
        [TestMethod]
        public void TestAdd()
        {

        }

        private static DBPlaylistDecorator CreateTestPlaylist()
        {
            DBPlaylist decorateMe = new DBPlaylist()
            {
                Name = "Test Playlist",
                Id = null,
                Songs = new List<MMSong>()
            };

            return new DBPlaylistDecorator(decorateMe);
        }
    }
}
