using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Sharlayan;

namespace FFBardMusicPlayer.Forms
{
    public partial class BmpProcessSelect : Form
    {
        private bool hasAutoSelected;
        public Process SelectedProcess;
        public EventHandler<Process> OnSelectProcess;

        public class MultiboxProcess
        {
            public Process Process;
            public string CharacterName;
            public string CharacterId;
            public bool HostProcess;
        }

        public List<MultiboxProcess> MultiboxProcesses = new List<MultiboxProcess>();
        public bool UseLocalOrchestra;
        private readonly BackgroundWorker processWorker = new BackgroundWorker();
        private readonly AutoResetEvent processCancelled = new AutoResetEvent(false);

        public BmpProcessSelect()
        {
            InitializeComponent();

            LocalOrchestraCheck.Visible = false;

            processWorker.DoWork                     += ButtonLabelTask;
            processWorker.WorkerSupportsCancellation =  true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            ControlPaint.DrawBorder(e.Graphics, ClientRectangle, Color.Black, ButtonBorderStyle.Solid);
        }

        private void ButtonLabelTask(object o, DoWorkEventArgs e)
        {
            // Update the button label with the character name for FFXIV processes
            var buttons = new Dictionary<Process, Button>();
            foreach (Button button in ProcessList.Controls)
            {
                if (button?.Tag is Process proc)
                {
                    buttons[proc] = button;
                }
            }

            LocalOrchestraCheck.Invoke(t => t.Visible = false);
            MultiboxProcesses.Clear();
            // Loop through all buttons and set the name
            while (buttons.Count > 0)
            {
                var proc = buttons.First();
                var process = proc.Key;
                var button = proc.Value;
                buttons.Remove(process);
                if (process.ProcessName == "ffxiv_dx11")
                {
                    MemoryHandler.Instance.SetProcess(new Sharlayan.Models.ProcessModel
                    {
                        Process = process,
                        IsWin64 = true
                    });
                    var scanTask = false;
                    while (Scanner.Instance.IsScanning)
                    {
                        if (processWorker.CancellationPending)
                        {
                            scanTask = true;
                            break;
                        }
                    }

                    if (scanTask)
                    {
                        break;
                    }

                    var name = "(Unknown)";
                    var origName = string.Empty;
                    var id = string.Empty;
                    if (Reader.CanGetPlayerInfo())
                    {
                        origName = Reader.GetCurrentPlayer().CurrentPlayer.Name;
                        name     = string.IsNullOrEmpty(origName) ? $"{button.Text} (?)" : $"{origName} ({process.Id})";
                    }

                    if (Reader.CanGetCharacterId())
                    {
                        id = Reader.GetCharacterId();
                    }

                    button.Invoke(t => t.Text = name);
                    MultiboxProcesses.Add(new MultiboxProcess
                    {
                        Process       = process,
                        CharacterName = origName,
                        CharacterId   = id
                    });
                    MemoryHandler.Instance.UnsetProcess();
                }
            }

            MemoryHandler.Instance.UnsetProcess();
            processCancelled.Set();

            // FIXME enable this after testing
            LocalOrchestraCheck.Invoke(t => t.Visible = true);
            //LocalOrchestraCheck.Invoke(t => t.Visible = (multiboxProcesses.Count > 1));
        }

        public void RefreshList() { RefreshList(this, EventArgs.Empty); }

        public void RefreshList(object o, EventArgs a)
        {
            ProcessList.Controls.Clear();
            processWorker.CancelAsync();

            // Get all processes instead
            if (AllProcessCheck.Checked)
            {
                HeaderText.Text = "Select process:";
                foreach (var process in Process.GetProcesses())
                {
                    if (process.MainWindowHandle == IntPtr.Zero)
                    {
                        continue;
                    }

                    var badNames = new List<string> { "FFBardMusicPlayer", "explorer" };
                    if (badNames.Contains(process.ProcessName))
                    {
                        continue;
                    }

                    var debug = $"{process.ProcessName} - {process.MainWindowTitle}";
                    var width = ProcessList.Size.Width - 20;
                    var height = 20;
                    var button = new Button()
                    {
                        Text      = debug,
                        Size      = new Size(width, height),
                        Margin    = new Padding(0),
                        FlatStyle = FlatStyle.Popup,
                        Tag       = process,
                        TextAlign = ContentAlignment.MiddleCenter
                    };
                    button.Click += Button_Click;
                    ProcessList.Controls.Add(button);
                }

                return;
            }

            // Get a list of all ffxiv_dx11 processes
            var processes = new List<Process>(Process.GetProcessesByName("ffxiv_dx11"));

            // If specified window title, select first one
            if (!string.IsNullOrEmpty(Program.ProgramOptions.HookWindowTitle))
            {
                foreach (var proc in Process.GetProcesses())
                {
                    if (proc.MainWindowTitle == Program.ProgramOptions.HookWindowTitle)
                    {
                        processes = new List<Process>() { proc };
                        break;
                    }
                }
            }
            else if (Program.ProgramOptions.HookPid != 0)
            {
                Console.WriteLine($"{Program.ProgramOptions.HookPid}");
                var proc = Process.GetProcessById(Program.ProgramOptions.HookPid);
                processes = new List<Process>() { proc };
            }

            // Mutex that shit
            foreach (var process in processes.ToList())
            {
                var mutex = new Mutex(true, $"bard-music-player-{process.Id}");
                if (!mutex.WaitOne(TimeSpan.Zero, true))
                {
                    processes.Remove(process);
                }
            }

            if (processes.Count == 0)
            {
                HeaderText.Text = "No FFXIV processes found.\nMake sure you run with DX11 on.";
            }
            else if (processes.Count == 1 && !hasAutoSelected)
            {
                DialogResult    = DialogResult.Yes;
                SelectedProcess = processes[0];
                OnSelectProcess?.Invoke(this, SelectedProcess);
                hasAutoSelected = true;
            }
            else
            {
                HeaderText.Text = "Select FFXIV process:";

                foreach (var process in processes)
                {
                    var debug = $"{process.ProcessName} ({process.Id})";
                    var width = ProcessList.Size.Width - (ProcessList.Padding.Left + ProcessList.Padding.Right);
                    var height = 35;
                    var button = new Button()
                    {
                        Text      = debug,
                        Size      = new Size(width, height),
                        Margin    = new Padding(0),
                        FlatStyle = FlatStyle.Popup,
                        Tag       = process,
                        TextAlign = ContentAlignment.MiddleCenter
                    };
                    button.Click += Button_Click;
                    ProcessList.Controls.Add(button);
                }

                CancelProcessWorkerSync();
                if (!processWorker.IsBusy)
                {
                    processWorker.RunWorkerAsync();
                }
            }
        }

        public void CancelProcessWorkerSync()
        {
            if (processWorker.IsBusy)
            {
                processWorker.CancelAsync();
                processCancelled.WaitOne();
            }
        }

        // Button click
        private void Button_Click(object sender, EventArgs e)
        {
            CancelProcessWorkerSync();
            DialogResult = DialogResult.Yes;

            var process = (sender as Button).Tag as Process;

            UseLocalOrchestra = LocalOrchestraCheck.Checked || ModifierKeys == Keys.Shift;

            if (UseLocalOrchestra)
            {
                for (var i = MultiboxProcesses.Count - 1; i >= 0; i--)
                {
                    if (MultiboxProcesses[i].Process == process)
                    {
                        MultiboxProcesses[i].HostProcess = true;
                    }
                }
            }

            SelectedProcess = process;
            OnSelectProcess?.Invoke(this, process);
        }

        // Form buttons
        private void RefreshButton_Click(object sender, EventArgs e) { RefreshList(); }

        private void AllProcessCheck_CheckedChanged(object sender, EventArgs e) { RefreshList(); }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            CancelProcessWorkerSync();
            DialogResult = DialogResult.No;
        }
    }
}