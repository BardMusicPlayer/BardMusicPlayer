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

        protected CofferTestBase() { dbpath = null; }

        [TestInitialize]
        public void Initialize()
        {
            // We cannot use Path.GetTempFileName() as this creates a zero byte file on disk.
            // This causes LiteDB to fail to load the file. As such, we need to create our
            // own temporary file path and allow LiteDB to do the actual file creation.
            var tempDir = Path.GetTempPath();
            var unixEpoch = DateTimeOffset.Now.ToUnixTimeSeconds();

            dbpath = tempDir + GetType().Name + "." + unixEpoch.ToString() + ".db";
            Console.WriteLine("LiteDB path: " + dbpath);
        }

        [TestCleanup]
        public void TearDown()
        {
            if (dbpath != null)
            {
                try
                {
                    File.Delete(dbpath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                finally
                {
                    dbpath = null;
                }
            }
        }

        protected LiteDatabase CreateDatabase()
        {
            var ret = new LiteDatabase(dbpath);
            BmpCoffer.MigrateDatabase(ret);
            return ret;
        }

        protected string GetDBPath() => dbpath;
    }
}