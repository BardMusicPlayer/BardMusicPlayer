using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFBardMusicPlayer.Controls {
	public partial class BmpStatistics : UserControl {

		Timer npsTimer = new Timer();

		int npsInternalTemp = 0;
		int npsInternalCount = 0;

		public BmpStatistics() {
			InitializeComponent();
			npsTimer.Interval = 1000;
			npsTimer.Tick += NpsTimer_Tick;
		}

		private void NpsTimer_Tick(object sender, EventArgs e) {
			npsInternalCount = (npsInternalCount + npsInternalTemp) / 2;
			npsInternalTemp = 0;

			npsCount.Invoke(t => t.Text = npsInternalCount.ToString());
		}

		public void Restart() {
			npsTimer.Stop();
			npsTimer.Start();
		}
		public void AddNoteCount() {
			npsInternalTemp++;
		}

		public void SetBpmCount(int bpm) {
			bpmCount.Invoke(t => t.Text = bpm.ToString());
		}
		public void SetTotalTrackCount(int trackcount) {
			trkCount.Invoke(t => t.Text = trackcount.ToString());
		}
		public void SetTotalNoteCount(int notecount) {
			ncCount.Invoke(t => t.Text = notecount.ToString());
		}
		public void SetTrackNoteCount(int notecount) {
			nscCount.Invoke(t => t.Text = notecount.ToString());
		}
		public void SetChordDetectedCount(int chordcount) {
			chordCount.Invoke(t => t.Text = chordcount.ToString());
		}
	}
}
