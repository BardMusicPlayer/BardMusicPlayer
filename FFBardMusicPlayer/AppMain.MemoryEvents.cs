using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

using Sharlayan;
using Sharlayan.Core;
using Sharlayan.Models.ReadResults;

namespace FFBardMusicPlayer {
	public partial class AppMain {
		
		/*
		private bool readyPerformance;
		private bool readyCharacter;

		private void SetupMemoryEvents() {
			memory.OnCurrentPlayerLogin += OnCurrentPlayerLogin;
			memory.OnCurrentPlayerLogout += OnCurrentPlayerLogout;
			memory.OnCurrentPlayerJobChange += OnCurrentPlayerJobChange;
			memory.OnChatReceived += OnChatReceived;
			memory.OnPerformanceChanged += OnPerformanceChanged;
		}


		private void OnCurrentPlayerLogin(object o, CurrentPlayerResult res) {

			if(string.IsNullOrEmpty(id)) {
				Log("Character ID was not detected.");
				string newid = Properties.Settings.Default.LastCharId;
				if(!string.IsNullOrEmpty(newid)) {
					Log(string.Format("Using last loaded character ID: [{0}]", newid));
					if(o is FFXIVMemory mem) {
						mem.CharacterID = newid;
					}
				}
				Log("Manually select ID by right-clicking the keyboard.");
			}
			if(Reader.CanGetWorld()) {
				Log(string.Format("Current world: [{0}]", Reader.GetWorld()));
			}
			readyCharacter = true;

			UpdatePlayer();
		}
		private void OnCurrentPlayerLogout(object o, CurrentPlayerResult res) {
			CurrentPlayer cp = res.CurrentPlayer;
			Log(string.Format("Character {0} logged out.", cp.Name));
			readyCharacter = false;
		}
		private void OnChatReceived(object o, ChatLogItem arg) {
			
			string format = FormattedChat(arg);
			if(!string.IsNullOrEmpty(format)) {
				log.Info(format);
				Console.WriteLine(format);
			}

			if(chatListener.ProcessChat(arg)) {
				return;
			}

			Chat(arg);
		}

		private void OnCurrentPlayerJobChange(object o, CurrentPlayerResult res) {
			UpdatePlayer();
		}
        
		*/
	}
}
