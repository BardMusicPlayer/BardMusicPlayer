using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FFBardMusicPlayer.Controls;
using System.IO;

using FFBardMusicCommon;

namespace FFBardMusicPlayer {
	static class BmpSocketClientHelper {

		public static BmpPerformerSettingData PerformerToSettingData(PerformerControl ctl) {
			return new BmpPerformerSettingData {
				actorId = (uint) ctl.Id,
				ss = ctl.SpeedShift,
				os = ctl.OctaveShift,
				tn = ctl.TrackNum,
			};
		}

		public static BmpPlayerStatusData PerformerToStatusData(Controls.BmpPlayer ctl) {
			return new BmpPlayerStatusData {
				play = ctl.Player.IsPlaying,
				tick = ctl.Player.CurrentTick,
				loop = ctl.Loop,
			};
		}

		public static BmpPlayerFileData PerformerToFileData(Controls.BmpPlayer ctl) {
			string file = ctl.Player.LoadedFilename;
			return new BmpPlayerFileData {
				filename = Path.GetFileName(file),
				data = File.ReadAllBytes(file),
			};
		}
	}

}
