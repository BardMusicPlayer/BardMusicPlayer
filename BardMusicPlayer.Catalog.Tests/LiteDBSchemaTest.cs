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
