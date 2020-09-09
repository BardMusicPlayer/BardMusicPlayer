using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Sanford.Multimedia.Midi;

namespace FFBardMusicPlayer.Controls {
	public partial class BmpSettings : UserControl {

		#region API Stuff

		[System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

		[System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
		private static extern IntPtr SendMessage(IntPtr hWnd, Int32 wMsg, Int32 wParam, ref Point lParam);

		[System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
		public static extern bool ReleaseCapture();

		[System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
		public static extern int GetScrollPos(IntPtr hWnd, int nBar);

		[System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
		public static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);

		[System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
		private static extern bool GetScrollRange(IntPtr hWnd, int nBar, out int lpMinPos, out int lpMaxPos);

		[System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
		public static extern int RegisterWindowMessage(string message);

		private const int WM_USER = 0x400;
		private const int SB_HORZ = 0x0;
		private const int SB_VERT = 0x1;
		private const int EM_SETSCROLLPOS = WM_USER + 222;
		private const int EM_GETSCROLLPOS = WM_USER + 221;
		private const int WM_NCLBUTTONDOWN = 0xA1;
		private const int HT_CAPTION = 0x2;

		public static readonly int WM_SHOWME = RegisterWindowMessage("WM_SHOWME");

		#endregion

		public EventHandler<bool> OnForcedOpen;
		public EventHandler OnKeyboardTest;
		
		public EventHandler<MidiInput> OnMidiInputChange;
		List<MidiInput> midiInputs = new List<MidiInput>();

		public BmpSettings() {
			InitializeComponent();

			List<ChatChannel> channelList = new List<ChatChannel> {
				new ChatChannel("000D"),
				new ChatChannel("000E"),
				new ChatChannel("000F"),
				new ChatChannel("0018"),
				new ChatChannel("0010"),
				new ChatChannel("0011"),
				new ChatChannel("0012"),
				new ChatChannel("0013"),
				new ChatChannel("0014"),
				new ChatChannel("0015"),
				new ChatChannel("0016"),
				new ChatChannel("0017"),
				new ChatChannel("0025"),
				new ChatChannel("0065"),
				new ChatChannel("0066"),
				new ChatChannel("0067"),
				new ChatChannel("0068"),
				new ChatChannel("0069"),
				new ChatChannel("006A"),
				new ChatChannel("006B"),
			};
			ChatChannel selectChannel = null;
			foreach(ChatChannel chan in channelList) {
				if(chan.code == Properties.Settings.Default.ListenChannel) {
					selectChannel = chan;
					break;
				}
			}
			ListenChatList.DataSource = channelList;
			ListenChatList.SelectedItem = selectChannel;
			ListenChatList.SelectedValueChanged += delegate (object sender, EventArgs e) {
				Properties.Settings.Default.ListenChannel = (ListenChatList.SelectedItem as ChatChannel).code;
			};
			KeyboardTest.Click += delegate (object o, EventArgs e) {
				OnKeyboardTest?.Invoke(o, e);
			};
			SignatureFolder.Click += delegate (object o, EventArgs e) {
				Process.Start(Sharlayan.Reader.JsonPath);
			};

			RefreshMidiInput();

			string midiInput = Properties.Settings.Default.MidiInputDev;
			if(!string.IsNullOrEmpty(Program.programOptions.MidiInput)) {
				midiInput = Program.programOptions.MidiInput;
			}
			SetMidiInput(midiInput);
			SettingMidiInput.SelectedValueChanged += SettingMidiInput_SelectedValueChanged;

			UpdateSlowPlayToggle();
		}

		private void SettingMidiInput_SelectedValueChanged(object sender, EventArgs e) {
			if(GetMidiInput() is MidiInput input) {
				Properties.Settings.Default.MidiInputDev = input.name;
				SetMidiInput(input.name);
			}
		}

		private void SlowPlayToggle_CheckedChanged(object sender, EventArgs e) {
			UpdateSlowPlayToggle();
		}

		private void ForceOpenToggle_CheckedChanged(object sender, EventArgs e) {
			bool check = (sender as CheckBox).Checked;
			Properties.Settings.Default.ForcedOpen = check;
			Properties.Settings.Default.Save();
			OnForcedOpen?.Invoke(this, check);
		}

		private void UpdateSlowPlayToggle() {
			bool c = SlowPlayToggle.Checked;
			SettingHoldNotes.Enabled = !c;
		}

		public MidiInput SetMidiInput(string device) {
			MidiInput input = midiInputs[0];
			foreach(MidiInput inp in midiInputs) {
				if(inp.name == device) {
					Console.WriteLine("Found input: " + inp.name);
					input = inp;
					break;
				}
			}
			if(input != null) {
				Properties.Settings.Default.MidiInputDev = input.name;
				SettingMidiInput.SelectedItem = input;
				OnMidiInputChange?.Invoke(this, input);
				return input;
			}
			return null;
		}

		public MidiInput GetMidiInput() {
			if(SettingMidiInput.SelectedValue is MidiInput input) {
				return input;
			}
			return null;
		}
		public void RefreshMidiInput() {

			// Refresh list of Midi input devices
			midiInputs.Clear();
			midiInputs.Add(new MidiInput("None", -1));
			for(int i = 0; i < InputDevice.DeviceCount; i++) {
				MidiInCaps cap = InputDevice.GetDeviceCapabilities(i);
				midiInputs.Add(new MidiInput(cap.name, i));
			}
			SettingMidiInput.DataSource = midiInputs;
		}

		// Links

		private void SiteLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			Process.Start(Program.urlBase);
		}

		private void DiscordLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			Process.Start(Program.urlBase + "discord/");
		}

		private void AboutLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			BmpAbout about = new BmpAbout();
			about.ShowDialog(this);
		}
    }

    public class MidiInput {
		public string name = string.Empty;
		public int id = 0;
		public MidiInput(string n, int i) {
			name = n;
			id = i;
		}
		public override string ToString() {
			return name;
		}
	}

	public class ChatChannel {
		public string code = string.Empty;
		public ChatChannel(string c) {
			code = c;
		}
		public override string ToString() {
			switch(code) {
				case "000D":
					return "Tell";
				case "000E":
					return "Party";
				case "000F":
					return "Alliance";
				case "0010":
					return "Linkshell#1";
				case "0011":
					return "Linkshell#2";
				case "0012":
					return "Linkshell#3";
				case "0013":
					return "Linkshell#4";
				case "0014":
					return "Linkshell#5";
				case "0015":
					return "Linkshell#6";
				case "0016":
					return "Linkshell#7";
				case "0017":
					return "Linkshell#8";
				case "0018":
					return "Free company";
				case "0025":
					return "CW Linkshell#1";
				case "0065":
					return "CW Linkshell#2";
				case "0066":
					return "CW Linkshell#3";
				case "0067":
					return "CW Linkshell#4";
				case "0068":
					return "CW Linkshell#5";
				case "0069":
					return "CW Linkshell#6";
				case "006A":
					return "CW Linkshell#7";
				case "006B":
					return "CW Linkshell#8";
				default:
					return string.Empty;
			}
		}
	}
}
