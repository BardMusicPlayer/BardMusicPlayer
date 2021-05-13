/*
 * Copyright(c) 2021 MoogleTroupe, isaki
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BardMusicPlayer.Catalog.Tests
{
    [TestClass]
    public class LiteDBSchemaTest
    {
        [TestMethod]
        public void TestBasics()
        {
            int testId = 100;
            byte testVersion = 40;

            LiteDBSchema test = new LiteDBSchema()
            {
                Id = testId,
                Version = testVersion
            };

            Assert.AreEqual(testId, test.Id);
            Assert.AreEqual(testVersion, test.Version);
        }
    }
}
