using FFBardMusicCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFBardMusicPlayer.Components {
	public partial class BmpBrowser : ListBox {

		#region Midi classes
		public class MidiFileName {
			private string fileName = string.Empty;
			public string FileName {
				get { return fileName; }
			}
			public string ShortFileName {
				get { return Path.GetFileName(fileName); }
			}
			public string TinyFileName {
				get { return Path.GetFileNameWithoutExtension(fileName); }
			}

			public string RelativeFileName {
				get {
					string sp = Properties.Settings.Default.SongDirectory;
					string file1 = Path.GetFullPath(fileName);
					string path2 = Path.GetFullPath(sp);
					if(file1.IndexOf(path2) != 0) {
						return file1;
					}
					return file1.Remove(0, path2.Length - sp.Length);
				}
			}

			public MidiFileName() {

			}
			public MidiFileName(string f) {
				fileName = f;
			}
			// Return the full relative path
			public string RelativePath {
				get {
					return Path.GetDirectoryName(RelativeFileName);
				}
			}
			// Return a compressed version of the path
			public string CompressedPath {
				get {
					string path = RelativePath;
					List<string> paths = path.Split(Path.DirectorySeparatorChar).ToList();
					if(paths.Count == 1) {
						return paths.First();
					}
					path = "../".Repeat(paths.Count - 1);
					path += paths.Last();
					return path;
				}
			}

			public int PathDepth {
				get {
					string path = Path.GetDirectoryName(RelativeFileName);
					List<string> paths = path.Split(Path.DirectorySeparatorChar).ToList();
					return paths.Count - 1;
				}
			}
		}
		public class MidiFile {
			private MidiFileName fileName = new MidiFileName();
			public MidiFileName FileName {
				get {
					return fileName;
				}
			}

			private bool enabled = true;
			public bool Enabled {
				get {
					return enabled;
				}
				set {
					enabled = value;
				}
			}

			public MidiFile(string f, bool e = true) {
				fileName = new MidiFileName(f);
				enabled = e;
			}
		}

		public class MidiList : List<MidiFile> { }

		private MidiList midis = new MidiList();
		public MidiList List {
			get {
				return midis;
			}
		}

		private string filenameFilter = string.Empty;
		public string FilenameFilter {
			get { return filenameFilter; }
			set {
				filenameFilter = value;
			}
		}

		#endregion

		public EventHandler<BmpMidiEntry> OnMidiFileSelect;
		private FileSystemWatcher fileWatcher = new FileSystemWatcher();

		public BmpBrowser() {
			InitializeComponent();
			Setup();
		}
		public BmpBrowser(IContainer container) {
			container.Add(this);
			InitializeComponent();
			Setup();
		}

		protected override void OnMouseDoubleClick(MouseEventArgs e) {
			base.OnMouseDoubleClick(e);
			EnterFile();
		}

		protected override void OnVisibleChanged(EventArgs e) {
			base.OnVisibleChanged(e);
			if(this.Visible) {
				this.RefreshList();
			}
		}

		protected override void OnSelectedIndexChanged(EventArgs e) {
			base.OnSelectedIndexChanged(e);

			if(this.Focused) {
				return;
			}
			if(this.SelectedItem is MidiFile file) {
				if(!file.Enabled) {
					return;
				}
			}

			int ft = (int) Math.Round((double) (this.Height / this.ItemHeight)) / 2;
			int i = (this.SelectedIndex) - ft;
			this.TopIndex = i.Clamp(0, this.Items.Count);
		}

		private void Setup() {
			string songDir = Properties.Settings.Default.SongDirectory;
			if(Directory.Exists(songDir)) {
				fileWatcher.Changed += (object sender, FileSystemEventArgs e) => {
					this.Invoke(t => t.RefreshList());
				};
				fileWatcher.Path = songDir;
				fileWatcher.Filter = "*.*";
				fileWatcher.EnableRaisingEvents = true;
			}

			this.RefreshList();
		}

		private void DrawItemEvent(object sender, DrawItemEventArgs e) {
			if(e.Index < 0) {
				return;
			}
			if(this.Items[e.Index] is MidiFile file) {
				bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

				Brush myBrush = file.Enabled ? Brushes.Black : Brushes.Gray;
				Brush myBrush2 = Brushes.Gray;
				Brush sBrush = new SolidBrush(this.BackColor);
				if(file.Enabled) {
					if(selected) {
						e.Graphics.FillRectangle(Brushes.LightGray, e.Bounds);
					} else {
						e.Graphics.FillRectangle(sBrush, e.Bounds);
						myBrush2 = sBrush;
					}
				}
				StringFormat sfmt = StringFormat.GenericDefault;
				sfmt.LineAlignment = StringAlignment.Far;


				if(file.Enabled) {
					string path = file.FileName.CompressedPath;
					string filename = file.FileName.ShortFileName;
					string fmt = string.Format("└ {0}{1}{2}", path, Path.DirectorySeparatorChar, filename);

					int lastSlash = fmt.LastIndexOf(Path.DirectorySeparatorChar);
					if(lastSlash > 0) {
						Rectangle bound = e.Bounds;
						e.Graphics.DrawString(path, e.Font, myBrush2, bound, sfmt);
						bound.X += (int) e.Graphics.MeasureString(path, e.Font).Width;
						e.Graphics.DrawString(filename, e.Font, myBrush, bound, sfmt);
					}
				} else {
					e.Graphics.DrawString(file.FileName.FileName, e.Font, myBrush, e.Bounds, sfmt);
				}
			}
		}

		public bool IsFilenameValid(string path) {
			try {
				Path.GetFileName(path);
				return true;

			} catch(ArgumentException) {
				return false;
			}
		}

		public List<string> GetSongFiles() {
			List<string> fileNames = new List<string>();
			string dir = Properties.Settings.Default.SongDirectory;
			if(Directory.Exists(dir)) {
				foreach(string file in Directory.EnumerateFiles(dir, "*.m*", SearchOption.AllDirectories).Where(s => s.ToLower().EndsWith(".mid") || s.ToLower().EndsWith(".mmsong"))) {
					fileNames.Add(file);
				}
			}
			return fileNames;
		}

		public void RefreshList() {

			string pathDepth = string.Empty;
			MidiFile singleEntry = null;
			MidiFile focusEntry = null;

			if(!string.IsNullOrEmpty(filenameFilter)) {
				if(!IsFilenameValid(FilenameFilter)) {
					midis.Clear();
					this.Items.Clear();
					return;
				} else {
					if(filenameFilter.ToLower().EndsWith(".mid")||filenameFilter.ToLower().EndsWith(".mmsong")) {
						foreach(MidiFile file in this.Items) {
							if(file.Enabled) {
								if(file.FileName.RelativeFileName == filenameFilter) {
									singleEntry = file;
									break;
								}
							}
						}
					}
				}
			}

			midis.Clear();
			this.Items.Clear();

			foreach(string path in GetSongFiles()) {
				MidiFile file = new MidiFile(path);

				if(!string.IsNullOrEmpty(filenameFilter)) {
					string f1 = file.FileName.TinyFileName.ToUpper();
					string f2 = file.FileName.ShortFileName.ToUpper();
					string f3 = filenameFilter.ToUpper();
					if(singleEntry != null) {
						if(file.FileName.RelativeFileName == singleEntry.FileName.RelativeFileName) {
							focusEntry = file;
						}
					} else if(singleEntry == null) {
						if(!f1.Contains(f3)) {
							continue;
						}
					}
				}

				midis.Add(file);

				int pd = file.FileName.PathDepth;
				if(pd > 0) {
					string newPath = file.FileName.CompressedPath;
					if(newPath != pathDepth) {
						this.Items.Add(new MidiFile(newPath, false));
					}
					pathDepth = newPath;
				}

				this.Items.Add(file);
			}

			int c = this.Items.Count;
			if(c == 0) {
				this.Items.Add(new MidiFile("No Midi files found. Make sure your song files are", false));
				this.Items.Add(new MidiFile("in a \"songs\" sub-folder next to the executable.", false));
				
			}

			if(focusEntry != null) {
				this.SelectedItem = focusEntry;
			} else {
				if(this.SelectedIndex == -1) {
					if(this.Items.Count > 0) {
						this.SelectedIndex = 0;
					}
				}
			}
		}



		// Own

		public void EnterFile() {
			if(this.SelectedItem != null) {
				if(this.SelectedItem is MidiFile file) {
					if(file.Enabled) {
						OnMidiFileSelect?.Invoke(this, new BmpMidiEntry(file.FileName.RelativeFileName));
					}
				}
			}
		}

		public bool SelectFile(string filename) {
			MidiFile halfMatch = null;
			MidiFile fullMatch = null;

			string f1 = filename.ToUpper();
			foreach(MidiFile file in this.Items) {
				if(file.Enabled) {
					string f2 = file.FileName.ShortFileName.ToUpper();
					if(f2.StartsWith(f1) && halfMatch == null) {
						halfMatch = file;
					}
					string f3 = file.FileName.RelativeFileName.ToUpper();
					if(f3.Equals(f1) && fullMatch == null) {
						fullMatch = file;
					}
				}
			}
			if(fullMatch != null) {
				this.Invoke(t => t.SelectedItem = fullMatch);
				return true;
			}
			if(halfMatch != null) {
				this.Invoke(t => t.SelectedItem = halfMatch);
				return true;
			}
			return false;
		}

		public void NextFile(int step = 1) {
			if(this.Items.Count <= 0) {
				return;
			}
			int i = this.SelectedIndex + step;
			if(i >= this.Items.Count) {
				i = 0;
			}
			if(this.Items[i] is MidiFile file) {
				if(!file.Enabled) {
					if(i < (this.Items.Count - 1)) {
						i++;
					}
				}
			}
			this.SelectedIndex = i;
		}
		public void PreviousFile(int step = 1) {
			if(this.Items.Count <= 0) {
				return;
			}
			int i = this.SelectedIndex - step;
			if(i < 0) {
				i = this.Items.Count - 1;
			}

			if(this.Items[i] is MidiFile file) {
				if(!file.Enabled) {
					if(i > 0) {
						i--;
					}
				}
			}

			this.SelectedIndex = i;
		}
	}
}
