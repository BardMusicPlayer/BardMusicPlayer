/*
 * Copyright(c) 2017 Belash
 * Licensed under the MIT license. See https://github.com/Nucs/JsonSettings/blob/master/LICENSE for full license information.
 */

using System;
using System.IO;

namespace BardMusicPlayer.Config.JsonSettings.Inline
{
    public static class Files
    {
        public static FileStream AttemptOpenFile(this FileInfo file, FileMode filemode = FileMode.Open, FileAccess fileaccess = FileAccess.Read, FileShare fileshare = FileShare.None, bool @throw = false)
        {
            return AttemptOpenFile(file?.FullName, filemode, fileaccess, fileshare);
        }

        /// <summary>
        ///     Attempts to open a file with given arguments.
        /// </summary>
        /// <param name="file">path to the file.</param>
        /// <param name="filemode"></param>
        /// <param name="fileaccess"></param>
        /// <param name="fileshare"></param>
        /// <param name="throw">Should this be silent and return null on exception or throw?</param>
        /// <returns>The filestream. null if UnauthorizedAccess or IOException</returns>
        public static FileStream AttemptOpenFile(string file, FileMode filemode = FileMode.Open, FileAccess fileaccess = FileAccess.Read, FileShare fileshare = FileShare.None, bool @throw = false)
        {
            if (string.IsNullOrEmpty(file)) throw new ArgumentException("message", nameof(file));

            FileStream stream = null;

            //if file doesnt exist and filemode won't create one - return null.
            if (File.Exists(file) == false && filemode != FileMode.Create && filemode != FileMode.CreateNew && filemode != FileMode.OpenOrCreate)
                return null;

            //ensure if theres a parent folder - to create it.
            string parent = Path.GetDirectoryName(file);

            try
            {
                if (parent != null)
                    Directory.CreateDirectory(parent);

                return File.Open(file, filemode, fileaccess, fileshare);
            }
            catch (IOException e)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                if (@throw)
                    throw;
                return null;
            }
            catch (UnauthorizedAccessException)
            {
                if (@throw)
                    throw;
                return null;
            }
        }

#if NETSTANDARD1_6

        public static void Close(this Stream str) {
            str.Dispose();
        }

#endif
    }
}