using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace FFBardMusicPlayer.FFXIV
{
    internal class FFXIVConfigFile
    {
        public class ConfigDictionary : Dictionary<string, string>
        {
        }

        public ConfigDictionary FFXIVConfig = new ConfigDictionary();
        public ConfigDictionary BootConfig = new ConfigDictionary();

        public FFXIVConfigFile()
        {
            //string subDirName = Registry.LocalMachine.OpenSubKey("Computer\HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\SquareEnix\FINAL FANTASY XIV - A Realm Reborn");
            var doc = FFXIVDocsResolver.GetPath();
            var dirPath = Path.Combine(new[] { doc, "My Games" });
            foreach (var dir in Directory.GetDirectories(dirPath, "FINAL FANTASY XIV - *"))
            {
                var ffc = Path.Combine(new[] { dir, "FFXIV.cfg" });
                if (File.Exists(ffc))
                {
                    FFXIVConfig = ParseConfigFile(ffc);
                }

                var ffbc = Path.Combine(new[] { dir, "FFXIV_BOOT.cfg" });
                if (File.Exists(ffbc))
                {
                    BootConfig = ParseConfigFile(ffbc);
                }
            }
        }

        private ConfigDictionary ParseConfigFile(string path)
        {
            var configData = new ConfigDictionary();
            using (var reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (Regex.Match(line, @"^(\w+)\t(.*)$") is Match match)
                    {
                        var key = match.Groups[1].Value;
                        var val = match.Groups[2].Value;
                        if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(val))
                        {
                            configData[key] = val;
                        }
                    }
                }
            }

            return configData;
        }
    }
}