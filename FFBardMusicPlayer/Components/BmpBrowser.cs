using FFBardMusicCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace FFBardMusicPlayer.Components
{
    public partial class BmpBrowser : ListBox
    {
        #region Midi classes

        public class MidiFileName
        {
            public string FileName { get; } = string.Empty;

            public string ShortFileName => Path.GetFileName(FileName);

            public string TinyFileName => Path.GetFileNameWithoutExtension(FileName);

            public string RelativeFileName
            {
                get
                {
                    var songDirectory = Properties.Settings.Default.SongDirectory;
                    var fileDirectory = Path.GetDirectoryName(FileName) ?? string.Empty;

                    return Path.GetFullPath(songDirectory).Contains(fileDirectory)
                        ? Path.Combine(songDirectory, Path.GetFileName(FileName))
                        : Path.GetFullPath(FileName);
                }
            }

            public MidiFileName() { }

            public MidiFileName(string f) { FileName = f; }

            // Return the full relative path
            public string RelativePath => Path.GetDirectoryName(RelativeFileName);

            // Return a compressed version of the path
            public string CompressedPath
            {
                get
                {
                    var path = RelativePath;
                    var paths = path.Split(Path.DirectorySeparatorChar).ToList();
                    if (paths.Count == 1)
                    {
                        return paths.First();
                    }

                    path =  "../".Repeat(paths.Count - 1);
                    path += paths.Last();
                    return path;
                }
            }

            public int PathDepth
            {
                get
                {
                    var path = Path.GetDirectoryName(RelativeFileName);
                    var paths = path.Split(Path.DirectorySeparatorChar).ToList();
                    return paths.Count - 1;
                }
            }
        }

        public class MidiFile
        {
            public MidiFileName FileName { get; }

            public bool Enabled { get; set; }

            public MidiFile(string f, bool e = true)
            {
                FileName = new MidiFileName(f);
                Enabled  = e;
            }
        }

        public class MidiList : List<MidiFile>
        {
        }

        public MidiList List { get; } = new MidiList();

        public string FilenameFilter { get; set; } = string.Empty;

        #endregion

        public EventHandler<BmpMidiEntry> OnMidiFileSelect;
        private readonly FileSystemWatcher fileWatcher = new FileSystemWatcher();

        public BmpBrowser()
        {
            InitializeComponent();
            Setup();
        }

        public BmpBrowser(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
            Setup();
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            EnterFile();
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);

            if (Focused)
            {
                return;
            }

            if (SelectedItem is MidiFile file)
            {
                if (!file.Enabled)
                {
                    return;
                }
            }

            var ft = (int) Math.Round((double) (Height / ItemHeight)) / 2;
            var i = SelectedIndex - ft;
            TopIndex = i.Clamp(0, Items.Count);
        }

        private void Setup()
        {
            var songDir = Properties.Settings.Default.SongDirectory;
            if (Directory.Exists(songDir))
            {
                fileWatcher.Changed             += (sender, e) => { this.Invoke(t => t.RefreshList()); };
                fileWatcher.Path                =  songDir;
                fileWatcher.Filter              =  "*.*";
                fileWatcher.EnableRaisingEvents =  true;
            }

            RefreshList();
        }

        private void DrawItemEvent(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
            {
                return;
            }

            if (Items[e.Index] is MidiFile file)
            {
                var selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

                var myBrush = file.Enabled ? Brushes.Black : Brushes.Gray;
                var myBrush2 = Brushes.Gray;
                Brush sBrush = new SolidBrush(BackColor);
                if (file.Enabled)
                {
                    if (selected)
                    {
                        e.Graphics.FillRectangle(Brushes.LightGray, e.Bounds);
                    }
                    else
                    {
                        e.Graphics.FillRectangle(sBrush, e.Bounds);
                        myBrush2 = sBrush;
                    }
                }

                var sfmt = StringFormat.GenericDefault;
                sfmt.LineAlignment = StringAlignment.Far;

                if (file.Enabled)
                {
                    var path = file.FileName.CompressedPath;
                    var filename = file.FileName.ShortFileName;
                    var fmt = $"└ {path}{Path.DirectorySeparatorChar}{filename}";

                    var lastSlash = fmt.LastIndexOf(Path.DirectorySeparatorChar);
                    if (lastSlash > 0)
                    {
                        var bound = e.Bounds;
                        e.Graphics.DrawString(path, e.Font, myBrush2, bound, sfmt);
                        bound.X += (int) e.Graphics.MeasureString(path, e.Font).Width;
                        e.Graphics.DrawString(filename, e.Font, myBrush, bound, sfmt);
                    }
                }
                else
                {
                    e.Graphics.DrawString(file.FileName.FileName, e.Font, myBrush, e.Bounds, sfmt);
                }
            }
        }

        public bool IsFilenameValid(string path)
        {
            try
            {
                Path.GetFileName(path);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        public List<string> GetSongFiles()
        {
            var fileNames = new List<string>();
            var dir = Properties.Settings.Default.SongDirectory;
            if (Directory.Exists(dir))
            {
                foreach (var file in Directory.EnumerateFiles(dir, "*.m*", SearchOption.AllDirectories)
                    .Where(s => s.ToLower().EndsWith(".mid") || s.ToLower().EndsWith(".mmsong")))
                {
                    fileNames.Add(file);
                }
            }

            return fileNames;
        }

        public void RefreshList()
        {
            var pathDepth = string.Empty;
            MidiFile singleEntry = null;
            MidiFile focusEntry = null;

            // this function is called so many times. since we're clearing the list each time,
            // the UI element is resetting it's selected index and forcing the selected item to always be 0
            // storing the previously selected index should be sufficient, for now
            var previouslySelectedIndex = SelectedIndex;

            if (!string.IsNullOrEmpty(FilenameFilter))
            {
                if (!IsFilenameValid(FilenameFilter))
                {
                    List.Clear();
                    Items.Clear();
                    return;
                }

                var filename = FilenameFilter.ToLower();
                if (filename.EndsWith(".mid") || filename.EndsWith(".mmsong"))
                {
                    singleEntry = Items.Cast<MidiFile>()
                        .FirstOrDefault(file => 
                            file.Enabled && 
                            file.FileName.RelativeFileName == FilenameFilter);
                }
            }

            List.Clear();
            Items.Clear();

            foreach (var file in GetSongFiles().Select(path => new MidiFile(path)))
            {
                if (!string.IsNullOrEmpty(FilenameFilter))
                {
                    var f1 = file.FileName.TinyFileName.ToUpper();
                    var f2 = file.FileName.ShortFileName.ToUpper();
                    var f3 = FilenameFilter.ToUpper();
                    if (singleEntry != null)
                    {
                        if (file.FileName.RelativeFileName == singleEntry.FileName.RelativeFileName)
                        {
                            focusEntry = file;
                        }
                    }
                    else if (!f1.Contains(f3))
                    {
                        continue;
                    }
                }

                List.Add(file);

                var pd = file.FileName.PathDepth;
                if (pd > 0)
                {
                    var newPath = file.FileName.CompressedPath;
                    if (newPath != pathDepth)
                    {
                        Items.Add(new MidiFile(newPath, false));
                    }

                    pathDepth = newPath;
                }

                Items.Add(file);
            }

            var c = Items.Count;
            if (c == 0)
            {
                Items.Add(new MidiFile("No Midi files found. Make sure your song files are", false));
                Items.Add(new MidiFile("in a \"songs\" sub-folder next to the executable.", false));
            }

            if (focusEntry != null)
            {
                SelectedItem = focusEntry;
            }
            else
            {
                if (SelectedIndex == -1 && Items.Count > 0)
                {
                    // don't allow this to pass the current index limits. this could change, say, when the user is using the search feature
                    SelectedIndex = previouslySelectedIndex < Items.Count
                        ? previouslySelectedIndex
                        : Items.Count - 1;
                }
            }
        }

        // Own

        public void EnterFile()
        {
            if (SelectedItem is MidiFile file && file.Enabled)
            {
                OnMidiFileSelect?.Invoke(this, new BmpMidiEntry(file.FileName.RelativeFileName));
            }
        }

        public bool SelectFile(string filename)
        {
            MidiFile halfMatch = null;
            MidiFile fullMatch = null;

            var f1 = filename.ToUpper();
            foreach (var file in Items.Cast<MidiFile>().Where(file => file.Enabled))
            {
                var f2 = file.FileName.ShortFileName.ToUpper();
                if (f2.StartsWith(f1) && halfMatch == null)
                {
                    halfMatch = file;
                }

                var f3 = file.FileName.RelativeFileName.ToUpper();
                if (f3.Equals(f1) && fullMatch == null)
                {
                    fullMatch = file;
                }
            }

            if (fullMatch != null)
            {
                this.Invoke(t => t.SelectedItem = fullMatch);
                return true;
            }

            if (halfMatch != null)
            {
                this.Invoke(t => t.SelectedItem = halfMatch);
                return true;
            }

            return false;
        }

        public void NextFile(int step = 1)
        {
            if (Items.Count <= 0)
                return;

            var i = SelectedIndex + step;
            if (i >= Items.Count)
            {
                i = 0;
            }

            if (Items[i] is MidiFile file && !file.Enabled)
            {
                if (i < Items.Count - 1)
                {
                    i++;
                }
            }

            SelectedIndex = i;
        }

        public void PreviousFile(int step = 1)
        {
            if (Items.Count <= 0)
                return;
            
            var i = SelectedIndex - step;
            if (i < 0)
            {
                i = Items.Count - 1;
            }

            if (Items[i] is MidiFile file && !file.Enabled)
            {
                if (i > 0)
                {
                    i--;
                }
            }

            SelectedIndex = i;
        }
    }
}