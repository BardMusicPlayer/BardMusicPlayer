/*
 * Copyright(c) 2017 Eli Belash
 * Licensed under the MIT license. See https://github.com/Nucs/JsonSettings/blob/master/LICENSE for full license information.
 */

using System;
using System.IO;

namespace BardMusicPlayer.Pigeonhole.JsonSettings.Inline
{
    internal static class Files
    {
        /// <summary>
        ///     Attempts to open a file with given arguments.
        /// </summary>
        /// <param name="file">path to the file.</param>
        /// <param name="filemode"></param>
        /// <param name="fileaccess"></param>
        /// <param name="fileshare"></param>
        /// <param name="throw">Should this be silent and return null on exception or throw?</param>
        /// <returns>The filestream. null if UnauthorizedAccess or IOException</returns>
        internal static FileStream AttemptOpenFile(string file, FileMode filemode = FileMode.Open, FileAccess fileaccess = FileAccess.Read, FileShare fileshare = FileShare.None, bool @throw = false)
        {
            if (string.IsNullOrEmpty(file)) throw new ArgumentException("message", nameof(file));
            
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
            catch (IOException)
            {
                return null;
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
        }
    }
}