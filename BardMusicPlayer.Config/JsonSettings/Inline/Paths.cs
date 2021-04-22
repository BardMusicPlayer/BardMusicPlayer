using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace BardMusicPlayer.Config.JsonSettings.Inline
{
    /// <summary>
    ///     Class that determines paths.
    /// </summary>
    internal static class Paths
    {
#pragma warning disable CS0169 // The field 'Paths._cacheprogress' is never used
        private static Task _cacheprogress;
#pragma warning restore CS0169 // The field 'Paths._cacheprogress' is never used


        /// <summary>
        ///     Gives the path to windows dir, most likely to be 'C:/Windows/'
        /// </summary>
        /// <summary>
        ///     The path to the entry exe.
        /// </summary>
        public static FileInfo ExecutingExe => new FileInfo((Assembly.GetEntryAssembly()
#if NETSTANDARD1_6
            ?? throw new NotSupportedException("Cant support fallback of ExecutingExe in NETSTANDARD1.6")).Location);
#else
                                                             ?? Assembly.GetExecutingAssembly())?.Location);
#endif
        /// <summary>
        ///     The config dir inside user profile.
        /// </summary>
        public static DirectoryInfo ConfigDirectory => new DirectoryInfo(Path.Combine(Environment.ExpandEnvironmentVariables("%USERPROFILE%"), "autoload/"));

        /// <summary>
        ///     The config file inside user profile.
        /// </summary>
        public static FileInfo ConfigFile(string configname) => new FileInfo(Path.Combine(ConfigDirectory.FullName, Environment.MachineName + $".{configname}.json"));


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
        ///     Checks the ability to create and write to a file in the supplied directory.
        /// </summary>
        /// <param name="directory">String representing the directory path to check.</param>
        /// <returns>True if successful; otherwise false.</returns>
        public static bool IsDirectoryWritable(this DirectoryInfo directory)
        {
            var success = false;
            var fullPath = directory + "toster.txt";

            if (directory.Exists)
                try
                {
                    using (var fs = new FileStream(fullPath, FileMode.CreateNew,
                        FileAccess.Write))
                    {
                        fs.WriteByte(0xff);
                    }

                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                        success = true;
                    }
                }
                catch (Exception)
                {
                    success = false;
                }

            return success;
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
        ///     Combines the file name with the dir of <see cref="Paths.ExecutingExe" />, resulting in path of a file inside the
        ///     directory of the executing exe file.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static DirectoryInfo CombineToExecutingBaseDir(string filename)
        {
            if (ExecutingExe.DirectoryName != null)
                return new DirectoryInfo(Path.Combine(ExecutingDirectory.FullName, filename));
            return null;
        }

        /// <summary>
        ///     Compares two FileSystemInfos the right way.
        /// </summary>
        /// <returns></returns>
        public static bool CompareTo(this FileSystemInfo fi, FileSystemInfo fi2) { return NormalizePath(fi.FullName, true).Equals(NormalizePath(fi2.FullName, true), StringComparison.Ordinal); }

        /// <summary>
        ///     Compares two FileSystemInfos the right way.
        /// </summary>
        /// <returns></returns>
        public static bool CompareTo(string fi, string fi2) { return NormalizePath(fi, true).Equals(NormalizePath(fi2, true), StringComparison.Ordinal); }

        /// <summary>
        ///     Normalizes path to prepare for comparison or storage
        /// </summary>
        public static string NormalizePath(string path, bool forComparsion = false)
        {
            string validBackslash = "\\";
            string invalidBackslash = "/";
#if CROSSPLATFORM
            //override default backslash that is used in windows.
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                validBackslash = "/";
                invalidBackslash = "\\";
            }
#endif

            path = path
                .Replace(invalidBackslash, validBackslash)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            if (forComparsion)
            {
#if CROSSPLATFORM
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    path = path.ToUpperInvariant();
#else
                path = path.ToUpperInvariant();
#endif
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

#if CROSSPLATFORM
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                //is root, fix.
                if ((path.Length == 2) && (path[1] == ':') && char.IsLetter(path[0]))
                    path = path + validBackslash;
            }
#endif
            return path;
        }

        ///
        /// Consts defined in WINBASE.H
        ///
        private enum MoveFileFlags
        {
            MOVEFILE_REPLACE_EXISTING = 1,
            MOVEFILE_COPY_ALLOWED = 2,
            MOVEFILE_DELAY_UNTIL_REBOOT = 4,
            MOVEFILE_WRITE_THROUGH = 8
        }


        /// <summary>
        /// Marks the file for deletion during next system reboot
        /// </summary>
        /// <param name="lpExistingFileName">The current name of the file or directory on the local computer.</param>
        /// <param name="lpNewFileName">The new name of the file or directory on the local computer.</param>
        /// <param name="dwFlags">MoveFileFlags</param>
        /// <returns>bool</returns>
        /// <remarks>http://msdn.microsoft.com/en-us/library/aa365240(VS.85).aspx</remarks>
        [System.Runtime.InteropServices.DllImportAttribute("kernel32.dll", EntryPoint = "MoveFileEx")]
        private static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, MoveFileFlags dwFlags);

        public static FileInfo MarkForDeletion(FileInfo file)
        {
            MarkForDeletion(file.FullName);
            return file;
        }

        public static string MarkForDeletion(string filename)
        {
            if (File.Exists(filename) == false)
                return filename;
            //Usage for marking the file to delete on reboot
            MoveFileEx(filename, null, MoveFileFlags.MOVEFILE_DELAY_UNTIL_REBOOT);
            return filename;
        }

        /// <summary>
        ///     Removes or replaces all illegal characters for path in a string.
        /// </summary>
        public static string RemoveIllegalPathCharacters(string filename, string replacewith = "") => string.Join(replacewith, filename.Split(Path.GetInvalidFileNameChars()));

        public class FilePathEqualityComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y) { return Paths.CompareTo(x, y); }

            public int GetHashCode(string obj) { return Paths.NormalizePath(obj, true).GetHashCode(); }
        }

        public class FileInfoPathEqualityComparer : IEqualityComparer<FileSystemInfo>
        {
            public bool Equals(FileSystemInfo x, FileSystemInfo y) { return Paths.CompareTo(x, y); }

            public int GetHashCode(FileSystemInfo obj) { return Paths.NormalizePath(obj.FullName, true).GetHashCode(); }
        }
    }
}