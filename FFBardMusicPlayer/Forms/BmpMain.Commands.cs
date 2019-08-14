using Sharlayan.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static FFBardMusicPlayer.Controls.BmpPlayer;

namespace FFBardMusicPlayer.Forms {
	public partial class BmpMain {

		private BmpChatListener chatListener = new BmpChatListener();
		BmpConfirmConductor confirmConductor = new BmpConfirmConductor();

		private void SetupCommands() {
			chatListener["conduct|con"] = ChatCommandConduct;
			chatListener["play|pl"] = ChatCommandPlay;
			chatListener["pause|pa"] = ChatCommandPause;
			chatListener["stop|st"] = ChatCommandStop;
			chatListener["load|ld"] = ChatCommandLoad;
			chatListener["track|tr"] = ChatCommandTrack;
			chatListener["note|nt"] = ChatCommandNote;
			chatListener["open|op"] = ChatCommandOpen;
			chatListener["close|cl"] = ChatCommandClose;
			chatListener["command|cmd"] = ChatCommandCommand;
			chatListener["delay|dl"] = ChatCommandDelay;
			chatListener["seek|sk"] = ChatCommandSeek;
			chatListener["loop|lp"] = ChatCommandLoop;
			chatListener["octaveshift|os"] = ChatCommandOctaveShift;
			chatListener["speedshift|ss"] = ChatCommandSpeedShift;

			confirmConductor.OnConfirmConductor += delegate (object o, string name) {
				if((o as BmpConfirmConductor).DialogResult == DialogResult.Yes) {
					ConfirmSetConductor(name);
				}
			};
		}

		private bool IsCommandPermitted(BmpChatListener.Command cmd) {
			if(Orchestra.IsConductorName(cmd.sender)) {
				if(cmd.sender == FFXIV.memory.currentPlayer.CurrentPlayer.Name) {
					// Disable making the same fucking command to yourself as conductor
					return false;
				}
				return cmd.IsCommandListenChannel();
			} else {
				if(Properties.Settings.Default.ForceListen) {
					if(cmd.IsCommandListenChannel()) {
						return true;
					}
				}
			}
			return false;
		}


		private bool ChatCommandConduct(BmpChatListener.Command cmd) {
			if(!cmd.IsCommandListenChannel()) {
				return false;
			}
			if(Orchestra.IsConductorName(cmd.sender)) {
				if(cmd.param == "off") {
					this.ConfirmSetConductor(string.Empty);
				} else {
					// Same conductor, idk what i wanna do with this
				}
			} else {
				if(Properties.Settings.Default.ForceListen) {
					this.ConfirmSetConductor(cmd.sender);
				} else {
					this.BringFront();
					confirmConductor.ConductorName = cmd.sender;
					confirmConductor.ShowDialog(this);
				}
			}
			return true;
		}

		private void ConfirmSetConductor(string conductorName) {
			Console.WriteLine(string.Format("ConfirmSetConductor: {0}", conductorName));
			if(conductorName is string) {
				Orchestra.ConductorName = conductorName;

				bool isme = Orchestra.IsConductorName(FFXIV.memory.currentPlayer.CurrentPlayer.Name);
				if(Orchestra.IsConducting) {
					Player.Status = (isme ? PlayerStatus.Conducting : PlayerStatus.PerformerMulti);
				} else {
					Player.Status = PlayerStatus.PerformerSolo;
				}

				this.UpdatePerformance();
			}
		}

		private bool ChatCommandPlay(BmpChatListener.Command cmd) {
			if(IsCommandPermitted(cmd)) {
				Player.Player.Play();
			}
			return true;
		}
		private bool ChatCommandPause(BmpChatListener.Command cmd) {
			if(IsCommandPermitted(cmd)) {
				Player.Player.Pause();
			}
			return true;
		}
		private bool ChatCommandStop(BmpChatListener.Command cmd) {
			if(IsCommandPermitted(cmd)) {
				Explorer.EnterFile();
			}
			return true;
		}
		private bool ChatCommandLoad(BmpChatListener.Command cmd) {
			if(IsCommandPermitted(cmd)) {
				if(Explorer.SelectFile(cmd.param)) {
					Explorer.EnterFile();
				}
			}
			return true;
		}
		private bool ChatCommandNote(BmpChatListener.Command cmd) {
			if(IsCommandPermitted(cmd)) {
				FFXIV.PlayPerformanceNote(cmd.param);
			}
			return true;
		}
		private bool ChatCommandOpen(BmpChatListener.Command cmd) {
			if(IsCommandPermitted(cmd)) {

				Performance.Instrument instrument = Player.PreferredInstrument;

				if(!string.IsNullOrEmpty(cmd.param)) {
					if(!FFXIV.GetPerformanceInstrument(cmd.param, out instrument)) {
						Log(string.Format("Cannot open [\"{0}\"] - that isn't an instrument.", cmd.param));
						return false;
					}
				}

				if(!FFXIV.GetHotkeyForInstrument(instrument, out FFXIVKeybindDat.Keybind keybind)) {
					Log(string.Format("Cannot open [{0}] - that performance action isn't placed on your hotbar.", instrument.ToString()));
					return false;
				}
				if(keybind is FFXIVKeybindDat.Keybind && keybind.GetKey() != Keys.None) {
					if(FFXIV.IsPlayerJobReady()) {
						FFXIV.hook.SendSyncKeybind(keybind);
					}
				} else {
					Log(string.Format("Cannot open [{0}] - no keybind is assigned to that hotbar slot.", instrument.ToString()));
				}
			}
			return true;
		}
		private bool ChatCommandClose(BmpChatListener.Command cmd) {
			if(IsCommandPermitted(cmd)) {
				FFXIV.UnequipPerformance();
			}
			return true;
		}
		private bool ChatCommandCommand(BmpChatListener.Command cmd) {
			if(IsCommandPermitted(cmd)) {
				if(!string.IsNullOrEmpty(cmd.param)) {
					if(Properties.Settings.Default.PlayLyrics) {
						FFXIV.SendChatString(cmd.param);
					}
				}
			}
			return true;
		}
		private bool ChatCommandDelay(BmpChatListener.Command cmd) {
			if(IsCommandPermitted(cmd)) {
				if(int.TryParse(cmd.param, out int delay)) {
					delay = delay.Clamp(-500, 500);

					if(FFXIV.IsPerformanceReady() && Player.Player.IsPlaying) {
						Player.Player.Seek(-delay);
					}
				}
			}
			return true;
		}
		private bool ChatCommandSeek(BmpChatListener.Command cmd) {
			if(IsCommandPermitted(cmd)) {
				if(int.TryParse(cmd.param, out int delay)) {
					delay = delay.Clamp(-500, 500);

					if(FFXIV.IsPerformanceReady() && Player.Player.IsPlaying) {
						Player.Player.Seek(delay);
					}
				}
			}
			return true;
		}
		private bool ChatCommandTrack(BmpChatListener.Command cmd) {
			if(IsCommandPermitted(cmd)) {
				if(int.TryParse(cmd.param, out int track)) {
					Explorer.SelectTrack(track.Clamp(0, Player.Player.MaxTrack));
				}
			}
			return true;
		}
		private bool ChatCommandLoop(BmpChatListener.Command cmd) {
			if(IsCommandPermitted(cmd)) {
				if(string.IsNullOrEmpty(cmd.param)) {
					Player.Loop = !Player.Loop;
				} else {
					if(bool.TryParse(cmd.param, out bool loop)) {
						Player.Loop = loop;
					}
				}
			}
			return true;
		}
		private bool ChatCommandOctaveShift(BmpChatListener.Command cmd) {
			if(IsCommandPermitted(cmd)) {
				if(int.TryParse(cmd.param, out int os)) {
					Player.OctaveShift = os;
				}
			}
			return true;
		}
		private bool ChatCommandSpeedShift(BmpChatListener.Command cmd) {
			if(IsCommandPermitted(cmd)) {
				if(float.TryParse(cmd.param, out float ss)) {
					Player.SpeedShift = ss;
				}
			}
			return true;
		}
	}
}
