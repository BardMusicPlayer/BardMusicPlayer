using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FFBardMusicCommon;
using FFBardMusicPlayer.Components;
using System.Diagnostics;
using System.IO;

namespace FFBardMusicPlayer.Controls {
	public partial class BmpExplorer : UserControl {

		private bool ignoreTrackChange = false;
		private bool initState = true;
		public EventHandler<bool> OnBrowserVisibleChange;
		public EventHandler<BmpMidiEntry> OnBrowserSelect;

		private Timer selectFlashingTimer = new Timer();

		public bool SongBrowserVisible {
			get { return SongBrowser.Visible; }
			set {
				OnBrowserVisibleChange?.Invoke(this, value);
				SongBrowser.Visible = value;
			}
		}

		private bool PlayAllTracksEffect {
			set {
				if(SelectorTrack != null) {
					SelectorTrack.Enabled = !value;
				}
			}
		}

		static private Tuple<int, int, int>[] colors = {
			Tuple.Create( 255, 207, 135),
			Tuple.Create( 207, 135, 255 ),
			Tuple.Create( 135, 255, 207 ),
		};

		public BmpExplorer() {
			InitializeComponent();
			
			selectFlashingTimer.Tick += delegate (object o, EventArgs a) {
				Random random = new Random();
				int min = 0, max = colors.Length;
				Tuple<int, int, int> color = colors[random.Next(min, max)];
				SelectorSong.BackColor = Color.FromArgb(color.Item1, color.Item2, color.Item3);
			};
			selectFlashingTimer.Interval = 100;
			selectFlashingTimer.Start();

			SongBrowser.OnMidiFileSelect += SongBrowser_EnterFile;
			SelectorTrack.ValueChanged += delegate (object o, EventArgs e) {
				if(!ignoreTrackChange) {
					this.EnterFile();
				}
			};

			MusicReload.Click += delegate (object sender, EventArgs e) {
				SongBrowser.RefreshList();
				this.EnterFile();
			};

			MusicReload.MouseDown += delegate (object sender, MouseEventArgs e) {
				if(e.Button == MouseButtons.Middle) {
					string dir = Path.GetDirectoryName(Application.ExecutablePath);
					string path = Path.Combine(dir, Properties.Settings.Default.SongDirectory);
					if(Directory.Exists(path)) {
						Process.Start(path);
					}
				}
			};

			SelectorSong.GotFocus += delegate (object sender, EventArgs e) {
				if(!SongBrowserVisible) {
					SongBrowserVisible = true;
				}
			};
			SelectorSong.LostFocus += delegate (object sender, EventArgs e) {
				if(!SongBrowser.Focused) {
					SongBrowserVisible = false;
					SelectorTrack.Focus();
				}
			};

			SongBrowser.LostFocus += delegate (object sender, EventArgs e) {
				if(!SelectorSong.Focused && !SelectorTrack.Focused && !MusicReload.Focused) {
					SongBrowserVisible = false;
				}
			};
			SongBrowser.MouseWheel += delegate (object sender, MouseEventArgs e) {
				BmpBrowser browser = (sender as BmpBrowser);
				if(browser != null) {
					if(e.Delta > 0) {
						browser.PreviousFile();
					} else {
						browser.NextFile();
					}
					((HandledMouseEventArgs) e).Handled = true;
				}
			};
			SelectorSong.OnHandledKeyDown += delegate (object sender, KeyEventArgs e) {
				switch(e.KeyCode) {
					case Keys.Up: {
						SongBrowser.PreviousFile();
						break;
					}
					case Keys.Down: {
						SongBrowser.NextFile();
						break;
					}
					case Keys.PageUp: {
						SongBrowser.PreviousFile(5);
						break;
					}
					case Keys.PageDown: {
						SongBrowser.NextFile(5);
						break;
					}
					case Keys.Enter: {
						if(!SongBrowserVisible) {
							SongBrowserVisible = true;
						} else {
							SongBrowser.EnterFile();
							SelectorTrack.Focus();
						}
						break;
					}
					case Keys.Tab:
					case Keys.Escape: {
						SongBrowserVisible = false;
						SelectorTrack.Focus();
						e.Handled = true;
						break;
					}
				}
			};
			SelectorTrack.KeyDown += delegate (object sender, KeyEventArgs e) {
				switch(e.KeyCode) {
					case Keys.Enter: {
						SelectorSong.Focus();
						e.Handled = true;
						e.SuppressKeyPress = true;
						break;
					}
					case Keys.Escape: {
						break;
					}
				}
			};

			SelectorSong.OnTextChange += delegate (object sender, string text) {
				SongBrowser.FilenameFilter = text;
				SongBrowser.RefreshList();
			};

			PlayAllTracksEffect = Properties.Settings.Default.PlayAllTracks;
		}

		public bool SelectFile(string file) {
			bool sel = SongBrowser.SelectFile(file);
			if(!sel) {
				SongBrowser.ClearSelected();
			}
			return sel;
		}

		public void SelectTrack(int track) {
			if(track >= 0) {
				SelectorTrack.Maximum = track + 1;
				ignoreTrackChange = true;
				SelectorTrack.Value = track;
				ignoreTrackChange = false;
			}
		}

		public void EnterFile() {
			SongBrowser.Invoke(t => t.EnterFile());
		}

		public void SetTrackName(string name) {
			SelectorSong.Text = name;
		}

		public void SetTrackNums(int track, int maxtrack) {
			// Set max before value so max isn't 0 and the program whines
			if(maxtrack == 0) {
				return;
			}
			SelectorTrack.Maximum = maxtrack;
			if(track <= maxtrack) {
				ignoreTrackChange = true;
				SelectorTrack.Value = track;
				ignoreTrackChange = false;
			}
		}

		private void SongBrowser_EnterFile(object sender, BmpMidiEntry file) {

			if(initState) {
				selectFlashingTimer.Stop();
				SelectorSong.BackColor = Color.White;
				SelectorSong.Text = string.Empty;
			}

			BmpMidiEntry entry = new BmpMidiEntry(file.FilePath.FilePath, decimal.ToInt32(SelectorTrack.Value));
			OnBrowserSelect?.Invoke(this, entry);
			
			SelectorTrack.Focus();
		}

		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);

			if(e.KeyCode == Keys.Escape) {
				SongBrowserVisible = false;
			}
		}

		protected override void OnMouseWheel(MouseEventArgs e) {
			base.OnMouseWheel(e);

			if(SongBrowserVisible) {
				if(e.Delta > 0) {
					SongBrowser.PreviousFile();
				} else {
					SongBrowser.NextFile();
				}
			}
		}

		private void PlayAllTracks_CheckedChanged(object sender, EventArgs e) {
			PlayAllTracksEffect = PlayAllTracks.Checked;
		}
	}
}
