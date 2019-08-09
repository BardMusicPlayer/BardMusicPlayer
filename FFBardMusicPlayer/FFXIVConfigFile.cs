using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FFBardMusicPlayer {
	class FFXIVConfigFile {

		public class ConfigDictionary : Dictionary<string, string> { }

		public ConfigDictionary ffxivConfig = new ConfigDictionary();
		public ConfigDictionary bootConfig = new ConfigDictionary();

		public FFXIVConfigFile() {

			//string subDirName = Registry.LocalMachine.OpenSubKey("Computer\HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\SquareEnix\FINAL FANTASY XIV - A Realm Reborn");
			string doc = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			string dirPath = Path.Combine(new string[] { doc, "My Games" });
			foreach(string dir in Directory.GetDirectories(dirPath, "FINAL FANTASY XIV - *")) {
				string ffc = Path.Combine(new string[] { dir, "FFXIV.cfg" });
				if(File.Exists(ffc)) {
					ffxivConfig = ParseConfigFile(ffc);
				}
				string ffbc = Path.Combine(new string[] { dir, "FFXIV_BOOT.cfg" });
				if(File.Exists(ffbc)) {
					bootConfig = ParseConfigFile(ffbc);
				}
			}
		}

		private ConfigDictionary ParseConfigFile(string path) {
			ConfigDictionary configData = new ConfigDictionary();
			using(StreamReader reader = new StreamReader(path)) {
				string line = string.Empty;
				while((line = reader.ReadLine()) != null) {
					if(Regex.Match(line, @"^(\w+)\t(.*)$") is Match match) {
						string key = match.Groups[1].Value;
						string val = match.Groups[2].Value;
						if(!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(val)) {
							configData[key] = val;
						}
					}
				}
			}
			return configData;
		}
	}
}
