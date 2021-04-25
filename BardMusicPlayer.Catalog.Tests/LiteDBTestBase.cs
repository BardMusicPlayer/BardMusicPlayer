using BardMusicPlayer.Notate.Objects;
using LiteDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace BardMusicPlayer.Catalog
{
    public abstract class LiteDBTestBase
    {
        private string dbpath;
        protected LiteDBTestBase()
        {
            this.dbpath = null;
        }

        [TestInitializeAttribute]
        public void Initialize()
        {
            // We cannot use Path.GetTempFileName() as this creates a zero byte file on disk.
            // This causes LiteDB to fail to load the file. As such, we need to create our
            // own temporary file path and allow LiteDB to do the actual file creation.
            string tempDir = Path.GetTempPath();
            long unixEpoch = DateTimeOffset.Now.ToUnixTimeSeconds();

            this.dbpath = tempDir + this.GetType().Name + "." + unixEpoch.ToString() + ".db";
            Console.WriteLine("LiteDB path: " + this.dbpath);
        }

        [TestCleanupAttribute]
        public void TearDown()
        {
            if (this.dbpath != null)
            {
                try
                {
                    File.Delete(this.dbpath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                finally
                {
                    this.dbpath = null;
                }
            }
        }

        protected abstract void OnInit();

        protected abstract void OnTearDown();

        protected LiteDatabase CreateDatabase()
        {
            LiteDatabase ret = new LiteDatabase(this.dbpath);
            CatalogManager.MigrateDatabase(ret);
            return ret;
        }
    }
}
