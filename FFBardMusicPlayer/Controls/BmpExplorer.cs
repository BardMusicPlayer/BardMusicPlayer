using System;
using System.Drawing;
using System.Windows.Forms;
using FFBardMusicCommon;
using FFBardMusicPlayer.Components;
using System.Diagnostics;
using System.IO;

namespace FFBardMusicPlayer.Controls
{
    public partial class BmpExplorer : UserControl
    {
        private bool ignoreTrackChange;
        private readonly bool initState = true;
        public EventHandler<bool> OnBrowserVisibleChange;
        public EventHandler<BmpMidiEntry> OnBrowserSelect;
        private readonly Timer selectFlashingTimer = new Timer();

        public bool SongBrowserVisible
        {
            get => SongBrowser.Visible;
            set
            {
                OnBrowserVisibleChange?.Invoke(this, value);
                SongBrowser.Visible = value;
            }
        }

        private bool PlayAllTracksEffect
        {
            set
            {
                if (SelectorTrack != null)
                {
                    SelectorTrack.Enabled = !value;
                }
            }
        }

        private static readonly Tuple<int, int, int>[] Colors =
        {
            Tuple.Create(255, 207, 135),
            Tuple.Create(207, 135, 255),
            Tuple.Create(135, 255, 207)
        };

        public BmpExplorer()
        {
            InitializeComponent();

            selectFlashingTimer.Tick += delegate
            {
                var random = new Random();
                int min = 0, max = Colors.Length;
                var color = Colors[random.Next(min, max)];
                SelectorSong.BackColor = Color.FromArgb(color.Item1, color.Item2, color.Item3);
            };
            selectFlashingTimer.Interval = 100;
            selectFlashingTimer.Start();

            SongBrowser.OnMidiFileSelect += SongBrowser_EnterFile;
            SelectorTrack.ValueChanged += delegate
            {
                if (!ignoreTrackChange)
                {
                    EnterFile();
                }
            };

            MusicReload.Click += delegate
            {
                SongBrowser.RefreshList();
                EnterFile();
            };

            MusicReload.MouseDown += delegate(object sender, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Middle)
                {
                    var dir = Path.GetDirectoryName(Application.ExecutablePath);
                    var path = Path.Combine(dir, Properties.Settings.Default.SongDirectory);
                    if (Directory.Exists(path))
                    {
                        Process.Start(path);
                    }
                }
            };

            SelectorSong.GotFocus += delegate
            {
                if (!SongBrowserVisible)
                {
                    SongBrowserVisible = true;
                }
            };
            SelectorSong.LostFocus += delegate
            {
                if (!SongBrowser.Focused)
                {
                    SongBrowserVisible = false;
                    SelectorTrack.Focus();
                }
            };

            SongBrowser.LostFocus += delegate
            {
                if (!SelectorSong.Focused && !SelectorTrack.Focused && !MusicReload.Focused)
                {
                    SongBrowserVisible = false;
                }
            };
            SongBrowser.MouseWheel += delegate(object sender, MouseEventArgs e)
            {
                if (sender is BmpBrowser browser)
                {
                    if (e.Delta > 0)
                    {
                        browser.PreviousFile();
                    }
                    else
                    {
                        browser.NextFile();
                    }

                    ((HandledMouseEventArgs) e).Handled = true;
                }
            };
            SelectorSong.OnHandledKeyDown += delegate(object sender, KeyEventArgs e)
            {
                switch (e.KeyCode)
                {
                    case Keys.Up:
                    {
                        SongBrowser.PreviousFile();
                        break;
                    }
                    case Keys.Down:
                    {
                        SongBrowser.NextFile();
                        break;
                    }
                    case Keys.PageUp:
                    {
                        SongBrowser.PreviousFile(5);
                        break;
                    }
                    case Keys.PageDown:
                    {
                        SongBrowser.NextFile(5);
                        break;
                    }
                    case Keys.Enter:
                    {
                        if (!SongBrowserVisible)
                        {
                            SongBrowserVisible = true;
                        }
                        else
                        {
                            SongBrowser.EnterFile();
                            SelectorTrack.Focus();
                        }

                        break;
                    }
                    case Keys.Tab:
                    case Keys.Escape:
                    {
                        SongBrowserVisible = false;
                        SelectorTrack.Focus();
                        e.Handled = true;
                        break;
                    }
                }
            };
            SelectorTrack.KeyDown += delegate(object sender, KeyEventArgs e)
            {
                switch (e.KeyCode)
                {
                    case Keys.Enter:
                    {
                        SelectorSong.Focus();
                        e.Handled          = true;
                        e.SuppressKeyPress = true;
                        break;
                    }
                    case Keys.Escape:
                    {
                        break;
                    }
                }
            };

            SelectorSong.OnTextChange += delegate(object sender, string text)
            {
                SongBrowser.FilenameFilter = text;
                SongBrowser.RefreshList();
            };

            PlayAllTracksEffect = Properties.Settings.Default.PlayAllTracks;
        }

        public bool SelectFile(string file)
        {
            var sel = SongBrowser.SelectFile(file);
            if (!sel)
            {
                SongBrowser.ClearSelected();
            }

            return sel;
        }

        public void SelectTrack(int track)
        {
            if (track >= 0)
            {
                SelectorTrack.Maximum = track + 1;
                ignoreTrackChange     = true;
                SelectorTrack.Value   = track;
                ignoreTrackChange     = false;
            }
        }

        public void EnterFile() { SongBrowser.Invoke(t => t.EnterFile()); }

        public void SetTrackName(string name) { SelectorSong.Text = name; }

        public void SetTrackNums(int track, int maxtrack)
        {
            // Set max before value so max isn't 0 and the program whines
            if (maxtrack == 0)
            {
                return;
            }

            SelectorTrack.Maximum = maxtrack;
            if (track <= maxtrack)
            {
                ignoreTrackChange   = true;
                SelectorTrack.Value = track;
                ignoreTrackChange   = false;
            }
        }

        private void SongBrowser_EnterFile(object sender, BmpMidiEntry file)
        {
            if (initState)
            {
                selectFlashingTimer.Stop();
                SelectorSong.BackColor = Color.White;
                SelectorSong.Text      = string.Empty;
            }

            var entry = new BmpMidiEntry(file.FilePath.FilePath, decimal.ToInt32(SelectorTrack.Value));
            OnBrowserSelect?.Invoke(this, entry);

            // make sure the filter is reset when a song is selected
            SongBrowser.FilenameFilter = string.Empty;
            SongBrowser.RefreshList();

            SelectorTrack.Focus();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.Escape)
            {
                SongBrowserVisible = false;
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (SongBrowserVisible)
            {
                if (e.Delta > 0)
                {
                    SongBrowser.PreviousFile();
                }
                else
                {
                    SongBrowser.NextFile();
                }
            }
        }

        private void PlayAllTracks_CheckedChanged(object sender, EventArgs e)
        {
            PlayAllTracksEffect = PlayAllTracks.Checked;
        }
    }
}