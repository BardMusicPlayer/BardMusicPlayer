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

		public BmpExplorer() {
			InitializeComponent();
			
			selectFlashingTimer.Tick += delegate (object o, EventArgs a) {
				Random random = new Random();
				int min = 180, max = 240;
				SelectorSong.BackColor = Color.FromArgb(random.Next(min, max), random.Next(min, max), random.Next(min, max));
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

		}

		public bool SelectFile(string file) {
			return SongBrowser.SelectFile(file);
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
			SongBrowser.EnterFile();
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
				SelectorTrack.Value = track;
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
	}
}
