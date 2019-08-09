using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Configuration;
using System.ComponentModel;

namespace FFBardMusicCommon {


	[Serializable]
	public class BmpMidiEntryPath {
		private string midiFilePath = string.Empty;
		public string FilePath {
			get {
				return midiFilePath;
			}
		}

		public BmpMidiEntryPath() {
			midiFilePath = "";
		}

		public BmpMidiEntryPath(string path = "") {
			midiFilePath = path;
		}
		public override string ToString() {
			return Path.GetFileName(midiFilePath);
		}
	}

	[Serializable]
	public class BmpMidiEntryTrack {
		private int midiTrack = 0;
		public int Track {
			get { return midiTrack; }
		}
		public string TrackString {
			get {
				return string.Format("{0}", midiTrack);
			}
		}

		public BmpMidiEntryTrack() {

		}

		public BmpMidiEntryTrack(int track = 0) {
			midiTrack = track;
		}
		public override string ToString() {
			return string.Format("t{0}", midiTrack);
		}
	}

	[Serializable]
	public class BmpMidiEntry {

		private BmpMidiEntryPath midiFilePath = new BmpMidiEntryPath();
		public BmpMidiEntryPath FilePath {
			get { return midiFilePath; }
		}

		private BmpMidiEntryTrack midiTrack = new BmpMidiEntryTrack();
		public BmpMidiEntryTrack Track {
			get { return midiTrack; }
		}

		public BmpMidiEntry() {
			midiFilePath = new BmpMidiEntryPath();
			midiTrack = new BmpMidiEntryTrack();
		}

		public BmpMidiEntry(string filename, int track = -1) {
			if(track == -1) {
				track = 0;

				Match m = Regex.Match(filename, @"(^[^;\n\r]+)(?:;?(\d+)?)");
				if(m.Success && m.Groups.Count == 3) {
					filename = m.Groups[1].Value;
                    if(!string.IsNullOrEmpty(m.Groups[2].Value)) {
                        track = int.Parse(m.Groups[2].Value);
                    }
                }
			}

			midiFilePath = new BmpMidiEntryPath(filename);
			midiTrack = new BmpMidiEntryTrack(track);
		}

		public override string ToString() {
			return midiFilePath.FilePath + ";" + midiTrack.TrackString;
		}
	}

	[Serializable]
	public class BmpMidiList : List<BmpMidiEntry> {
		public BmpMidiList() : base() { }
		public BmpMidiList(List<BmpMidiEntry> list) : base(list) { }
	}
	
	public sealed class BmpMidiListSettings : ApplicationSettingsBase {
		[UserScopedSetting]
		[SettingsSerializeAs(SettingsSerializeAs.Binary)]
		[DefaultSettingValue("")]
		public BmpMidiList MidiList {
			get { return (BmpMidiList) this[nameof(MidiList)]; }
			set { this[nameof(MidiList)] = value; }
		}
	}
}
