using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

using Sharlayan;
using System.Threading;

namespace FFBardMusicPlayer {
	public partial class BmpProcessSelect : Form {

		private bool hasAutoSelected = false;
		public Process selectedProcess;
		public EventHandler<Process> OnSelectProcess;

		public class MultiboxProcess {
			public Process process;
			public string characterName;
			public string characterId;
		}
		public List<MultiboxProcess> multiboxProcesses = new List<MultiboxProcess>();
		public bool useLocalOrchestra;

		private BackgroundWorker processWorker = new BackgroundWorker();
		private AutoResetEvent processCancelled = new AutoResetEvent(false);


		public BmpProcessSelect() {
			InitializeComponent();
			processWorker.DoWork += ButtonLabelTask;
			processWorker.WorkerSupportsCancellation = true;
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			ControlPaint.DrawBorder(e.Graphics, ClientRectangle, Color.Black, ButtonBorderStyle.Solid);
		}

		private void ButtonLabelTask(object o, DoWorkEventArgs e) {
			// Update the button label with the character name for FFXIV processes
			Dictionary<Process, Button> buttons = new Dictionary<Process, Button>();
			foreach(Button button in ProcessList.Controls) {
				if(button != null) {
					Process proc = (button.Tag as Process);
					if(proc != null) {
						buttons[proc] = button;
					}
				}
			}
			LocalOrchestraCheck.Invoke(t => t.Visible = false);
			multiboxProcesses.Clear();
			// Loop through all buttons and set the name
			while(buttons.Count > 0) {
				KeyValuePair<Process, Button> proc = buttons.First();
				Process process = proc.Key;
				Button button = proc.Value;
				buttons.Remove(process);
				if(process.ProcessName == "ffxiv_dx11") {
					MemoryHandler.Instance.SetProcess(new Sharlayan.Models.ProcessModel {
						Process = process,
						IsWin64 = true,
					});
					var scanTask = false;
					while(Scanner.Instance.IsScanning) {
						if(processWorker.CancellationPending) {
							scanTask = true;
							break;
						}
					}
					if(scanTask) {
						break;
					}
					string name = "(Unknown)";
					string id = string.Empty;
					if(Reader.CanGetPlayerInfo()) {
						string name2 = Reader.GetCurrentPlayer().CurrentPlayer.Name;
						if(string.IsNullOrEmpty(name2)) {
							name = string.Format("{0} (?)", button.Text);
						} else {
							name = string.Format("{0} ({1})", name2, process.Id);
						}
					}
					if(Reader.CanGetCharacterId()) {
						id = Reader.GetCharacterId();
					}
					button.Invoke(t => t.Text = name);
					multiboxProcesses.Add(new MultiboxProcess {
						process = process,
						characterName = name,
						characterId = id,
					});
					MemoryHandler.Instance.UnsetProcess();
				}
			}

			MemoryHandler.Instance.UnsetProcess();
			processCancelled.Set();

			// FIXME enable this after testing
			LocalOrchestraCheck.Invoke(t => t.Visible = (multiboxProcesses.Count > 1));
		}

		public void RefreshList() {
			RefreshList(this, EventArgs.Empty);
		}
		public void RefreshList(object o, EventArgs a) {
			ProcessList.Controls.Clear();
			processWorker.CancelAsync();

			// Get all processes instead
			if(AllProcessCheck.Checked) {
				HeaderText.Text = "Select process:";
				foreach(Process process in Process.GetProcesses()) {
					if(process.MainWindowHandle == IntPtr.Zero) {
						continue;
					}
					List<string> badNames = new List<string> { "FFBardMusicPlayer", "explorer" };
					if(badNames.Contains(process.ProcessName)) {
						continue;
					}
					string debug = string.Format("{0} - {1}", process.ProcessName, process.MainWindowTitle);
					int width = ProcessList.Size.Width;
					int height = 20;
					Button button = new Button() {
						Text = debug,
						Size = new Size(width, height),
						Margin = new Padding(0),
						FlatStyle = FlatStyle.Popup,
						Tag = process,
						TextAlign = ContentAlignment.MiddleCenter,
					};
					button.Click += Button_Click;
					ProcessList.Controls.Add(button);
				}
				return;
			}
			// Get a list of all ffxiv_dx11 processes
			List<Process> processes = new List<Process>(Process.GetProcessesByName("ffxiv_dx11"));

			// If specified window title, select first one
			if(!string.IsNullOrEmpty(Program.programOptions.HookWindowTitle)) {
				foreach(Process proc in Process.GetProcesses()) {
					if(proc.MainWindowTitle == Program.programOptions.HookWindowTitle) {
						processes = new List<Process>() { proc };
						break;
					}
				}
			}
			else if(Program.programOptions.HookPid != 0) {
				Console.WriteLine(string.Format("{0}", Program.programOptions.HookPid));
				Process proc = Process.GetProcessById(Program.programOptions.HookPid);
				if(proc != null) {
					processes = new List<Process>() { proc };
				}
			}

			// Mutex that shit
			foreach(Process process in processes.ToList()) {
				Mutex mutex = new Mutex(true, string.Format("bard-music-player-{0}", process.Id));
				if(!mutex.WaitOne(TimeSpan.Zero, true)) {
					processes.Remove(process);
				}
			}
			if(processes.Count == 0) {
				HeaderText.Text = "No FFXIV processes found.\nMake sure you run with DX11 on.";
				return;

			} else if(processes.Count == 1 && !hasAutoSelected) {
				DialogResult = DialogResult.Yes;
				selectedProcess = processes[0];
				OnSelectProcess?.Invoke(this, selectedProcess);
				hasAutoSelected = true;
				return;

			} else {
				HeaderText.Text = "Select FFXIV process:";

				foreach(Process process in processes) {
					string debug = string.Format("{0} ({1})", process.ProcessName, process.Id);
					int width = ProcessList.Size.Width - (ProcessList.Padding.Left + ProcessList.Padding.Right);
					int height = 35;
					Button button = new Button() {
						Text = debug,
						Size = new Size(width, height),
						Margin = new Padding(0),
						FlatStyle = FlatStyle.Popup,
						Tag = process,
						TextAlign = ContentAlignment.MiddleCenter,
					};
					button.Click += Button_Click;
					ProcessList.Controls.Add(button);
				}
				CancelProcessWorkerSync();
				if(!processWorker.IsBusy) {
					processWorker.RunWorkerAsync();
				}
			}
		}

		public void CancelProcessWorkerSync() {
			if(processWorker.IsBusy) {
				processWorker.CancelAsync();
				processCancelled.WaitOne();
			}
		}

		// Button click
		private void Button_Click(object sender, EventArgs e) {
			CancelProcessWorkerSync();
			DialogResult = DialogResult.Yes;

			Process process = (sender as Button).Tag as Process;

			if(LocalOrchestraCheck.Visible) {
				useLocalOrchestra = LocalOrchestraCheck.Checked;
				for(int i = multiboxProcesses.Count - 1; i >= 0; i--) {
					if(multiboxProcesses[i].process == process) {
						multiboxProcesses.RemoveAt(i);
					}
				}
			}

			selectedProcess = process;
			OnSelectProcess?.Invoke(this, process);
		}

		// Form buttons
		private void RefreshButton_Click(object sender, EventArgs e) {
			RefreshList();
		}

		private void AllProcessCheck_CheckedChanged(object sender, EventArgs e) {
			RefreshList();
		}

		private void CancelButton_Click(object sender, EventArgs e) {
			CancelProcessWorkerSync();
			DialogResult = DialogResult.No;
		}
	}
}
