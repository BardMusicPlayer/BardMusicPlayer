/*
 * Copyright(c) 2017 Eli Belash
 * Licensed under the MIT license. See https://github.com/Nucs/JsonSettings/blob/master/LICENSE for full license information.
 */

using System;
using System.IO;
using System.Reflection;

namespace BardMusicPlayer.Pigeonhole.JsonSettings.Inline
{
    /// <summary>
    ///     Class that determines paths.
    /// </summary>
    internal static class Paths
    {
        /// <summary>
        ///     Gives the path to windows dir, most likely to be 'C:/Windows/'
        /// </summary>
        /// <summary>
        ///     The path to the entry exe.
        /// </summary>
        public static FileInfo ExecutingExe => new ((Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly())?.Location);

        /// <summary>
        ///     The path to the entry exe's directory.
        /// </summary>
        public static DirectoryInfo ExecutingDirectory
        {
            get
            {
                try
                {
                    return ExecutingExe.Directory;
                }
                catch
                {
                    return new DirectoryInfo(Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetEntryAssembly().CodeBase).Path)));
                }
            }
        }

        /// <summary>
        ///     Combines the file name with the dir of <see cref="Paths.ExecutingExe" />, resulting in path of a file inside the
        ///     directory of the executing exe file.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static FileInfo CombineToExecutingBase(string filename)
        {
            if (ExecutingExe.DirectoryName != null)
                return new FileInfo(Path.Combine(ExecutingDirectory.FullName, filename));
            return null;
        }

        /// <summary>
        ///     Normalizes path to prepare for comparison or storage
        /// </summary>
        public static string NormalizePath(string path, bool forComparsion = false)
        {
            string validBackslash = "\\";
            string invalidBackslash = "/";

            path = path
                .Replace(invalidBackslash, validBackslash)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            if (forComparsion)
            {
                path = path.ToUpperInvariant();
            }

            if (path.Contains(validBackslash))
                if (Uri.IsWellFormedUriString(path, UriKind.RelativeOrAbsolute))
                    try
                    {
                        path = Path.GetFullPath(new Uri(path).LocalPath);
                    }
                    catch
                    {
                        // ignored
                    }

            return path;
        }
    }
}