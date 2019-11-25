using Sharlayan;
using Sharlayan.Core;
using Sharlayan.Models.ReadResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FFBardMusicPlayer {
	public class BmpChatListener {
		Dictionary<string, Func<Command, bool>> commandList = new Dictionary<string, Func<Command, bool>>();
		public Func<Command, bool> this[string key] {
			set {
				commandList[key] = value;
			}
		}
		public class Command {
			public string sender = string.Empty;
			public string command = string.Empty;
			public string target = string.Empty;
			public string param = string.Empty;
			public string code = string.Empty;

			private static string[] standardChannels = {
				"000D", "000E", "000F", "0010", "0011", "0012",
				"0013", "0014", "0015", "0016", "0017", "0018"
			};
			public bool IsCommandStandardChannel() {
				return (standardChannels.Contains(code));
			}

			public bool IsCommandListenChannel() {
				string chan = Properties.Settings.Default.ListenChannel;
				if(!string.IsNullOrEmpty(chan) && chan.Equals(code)) {
					return true;
				}
				return false;
			}
		}

		private static bool GetCommand(string text, out Command command) {
			command = new Command();

			//                         (skip prefix junk like stars)
			//                                          (get name until :)
			//                                                     (check for <b.xxx> message)
			//                                                                            (get optional "<t>")
			//                                                                                              (get optional parameters)
			Regex regex = new Regex(@"(?:^(?:[^a-zA-Z0-9]*)([^:\n]+?):)?(?:<b\.([a-zA-Z0-9]+)>)(?: ""([^""]+)"")?(?: (.+))?$");
			Match match = regex.Match(text);
			if(match.Success) {
				if(match.Groups.Count == 5) {
					command.sender = match.Groups[1].Value;
					command.command = match.Groups[2].Value;
					command.target = match.Groups[3].Value;
					command.param = match.Groups[4].Value;
				}
			}
			return match.Success;
		}

		public Func<bool> GetChatCommand(ChatLogItem item) {

			if(GetCommand(item.Line, out Command command)) {
				command.code = item.Code;

				if(!string.IsNullOrEmpty(command.target)) {
					if(Reader.CanGetPlayerInfo()) {
						CurrentPlayerResult res = Reader.GetCurrentPlayer();
						if(!command.target.Equals(res.CurrentPlayer.Name)) {
							return null;
						}
					}
				}

				return new Func<bool>(() => FindChatCommand(command.command).Invoke(command));
			}
			return null;
		}
		public Func<bool> GetChatCommand(string cmdString, string cmdCode = "") {

			if(GetCommand(cmdString, out Command command)) {
				command.code = cmdCode;

				return new Func<bool>(() => FindChatCommand(command.command).Invoke(command));
			}
			return null;
		}
		private Func<Command, bool> FindChatCommand(string cmd) {

			foreach(string cmdString in commandList.Keys) {
				string cmd1 = cmdString;
				string cmd2 = cmdString;
				if(cmdString.Contains('|')) {
					string[] str = cmdString.Split(new char[] { '|' });
					if(str.Length == 2) {
						cmd1 = str[0];
						cmd2 = str[1];
					}
				}
				if(cmd1 == cmd || cmd2 == cmd) {
					return commandList[cmdString];
				}
			}
			return null;
		}
	}
}
