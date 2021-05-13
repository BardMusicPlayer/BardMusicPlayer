/*
 * Copyright(c) 2021 MoogleTroupe, isaki
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.IO;
using LiteDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BardMusicPlayer.Coffer.Tests
{
    public class CofferTestBase
    {
        private string dbpath;

        protected CofferTestBase()
        {
            this.dbpath = null;
        }

        [TestInitialize]
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

        [TestCleanup]
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

        protected LiteDatabase CreateDatabase()
        {
            LiteDatabase ret = new LiteDatabase(this.dbpath);
            BmpCoffer.MigrateDatabase(ret);
            return ret;
        }

        protected string GetDBPath() => this.dbpath;
    }
}
