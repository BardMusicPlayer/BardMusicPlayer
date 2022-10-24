using BardMusicPlayer.Maestro;
using BardMusicPlayer.Maestro.Events;
using BardMusicPlayer.Maestro.Performance;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Seer;
using BardMusicPlayer.Seer.Events;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Linq;

namespace BardMusicPlayer.Ui.Controls
{
    /// <summary>
    /// Interaktionslogik für BardView.xaml
    /// </summary>
    public partial class BardView : UserControl
    {
        public BardView()
        {
            InitializeComponent();

            this.DataContext = this;
            Bards = new ObservableCollection<Performer>();

            BmpMaestro.Instance.OnPerformerChanged      += OnPerfomerChanged;
            BmpMaestro.Instance.OnTrackNumberChanged    += OnTrackNumberChanged;
            BmpMaestro.Instance.OnOctaveShiftChanged    += OnOctaveShiftChanged;
            BmpMaestro.Instance.OnSongLoaded            += OnSongLoaded;
            BmpMaestro.Instance.OnPerformerUpdate       += OnPerformerUpdate;
            BmpSeer.Instance.PlayerNameChanged          += OnPlayerNameChanged;
            BmpSeer.Instance.InstrumentHeldChanged      += OnInstrumentHeldChanged;
            BmpSeer.Instance.HomeWorldChanged           += OnHomeWorldChanged;
            Globals.Globals.OnConfigReload              += Globals_OnConfigReload;
            Globals_OnConfigReload(null, null);
        }

        private void Globals_OnConfigReload(object sender, EventArgs e)
        {
            Autoequip_CheckBox.IsChecked = BmpPigeonhole.Instance.EnsembleAutoEquip;
        }

        public ObservableCollection<Performer> Bards { get; private set; }

        public Performer SelectedBard { get; set; }

        private void OnPerfomerChanged(object sender, bool e)
        {
            this.Bards = new ObservableCollection<Performer>(BmpMaestro.Instance.GetAllPerformers());
            this.Dispatcher.BeginInvoke(new Action(() => this.BardsList.ItemsSource = Bards));
        }

        private void OnTrackNumberChanged(object sender, TrackNumberChangedEvent e)
        {
            UpdateList();
        }

        private void OnOctaveShiftChanged(object sender, OctaveShiftChangedEvent e)
        {
            UpdateList();
        }

        private void OnSongLoaded(object sender, SongLoadedEvent e)
        {
            UpdateList();
        }

        private void OnPerformerUpdate(object sender, PerformerUpdate e)
        {
            UpdateList();
        }

        private void OnPlayerNameChanged(PlayerNameChanged e)
        {
            UpdateList();
        }

        private void OnHomeWorldChanged(HomeWorldChanged e)
        {
            UpdateList();
        }

        private void OnInstrumentHeldChanged(InstrumentHeldChanged e)
        {
            UpdateList();
        }

        private void UpdateList()
        {
            this.Bards = new ObservableCollection<Performer>(BmpMaestro.Instance.GetAllPerformers());
            this.Dispatcher.BeginInvoke(new Action(() => this.BardsList.ItemsSource = Bards));
        }

        private void OpenInstrumentButton_Click(object sender, RoutedEventArgs e)
        {
            BmpMaestro.Instance.EquipInstruments();
        }

        private void CloseInstrumentButton_Click(object sender, RoutedEventArgs e)
        {
            BmpMaestro.Instance.StopLocalPerformer();
            BmpMaestro.Instance.UnEquipInstruments();
        }

        private void BardsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine(this.BardsList.SelectedItem);
        }

        private void BardsList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SelectedBard = BardsList.SelectedItem as Performer;

        }

        /* Track UP/Down */
        private void TrackNumericUpDown_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TrackNumericUpDown ctl = sender as TrackNumericUpDown;
            ctl.OnValueChanged += OnValueChanged;
        }

        private void OnValueChanged(object sender, int s)
        {
            Performer game = (sender as TrackNumericUpDown).DataContext as Performer;
            BmpMaestro.Instance.SetTracknumber(game, s);
        }

        /* Octave UP/Down */
        private void OctaveControl_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OctaveNumericUpDown ctl = sender as OctaveNumericUpDown;
            ctl.OnValueChanged += OnOctaveValueChanged;
        }

        private void OnOctaveValueChanged(object sender, int s)
        {
            Performer performer = (sender as OctaveNumericUpDown).DataContext as Performer;
            BmpMaestro.Instance.SetOctaveshift(performer, s);
        }

        private void HostChecker_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox ctl = sender as CheckBox;
            if (!ctl.IsChecked ?? false)
                return;

            var game = (sender as CheckBox).DataContext as Performer;
            BmpMaestro.Instance.SetHostBard(game);
        }

        private void PerfomerEnabledChecker_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox ctl = sender as CheckBox;
            var game = (sender as CheckBox).DataContext as Performer;
            game.PerformerEnabled = ctl.IsChecked ?? false;
        }

        private void Bard_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (SelectedBard == null)
                    return;

                BardExtSettingsWindow bardExtSettings = new BardExtSettingsWindow(SelectedBard);
                bardExtSettings.Activate();
                bardExtSettings.Visibility = Visibility.Visible;
            }
        }


        private void Autoequip_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            BmpPigeonhole.Instance.EnsembleAutoEquip = Autoequip_CheckBox.IsChecked ?? false;
            Globals.Globals.ReloadConfig();
        }

        /// <summary>
        /// load the performer config file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Load_Performer_Settings(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Performerconfig | *.cfg",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() != true)
                return;

            List<PerformerSettingData> pdatalist = new List<PerformerSettingData>();
            MemoryStream memoryStream = new MemoryStream();
            FileStream fileStream = File.Open(openFileDialog.FileName, FileMode.Open);
            fileStream.CopyTo(memoryStream);
            fileStream.Close();

            var data = memoryStream.ToArray();
            pdatalist = JsonConvert.DeserializeObject<List<PerformerSettingData>>(new UTF8Encoding(true).GetString(data));

            foreach (var pconfig in pdatalist)
            {
                var p = Bards.Where(perf => perf.game.PlayerName.Equals(pconfig.Name));
                if (p.Count() == 0)
                    continue;

                p.First().TrackNumber = pconfig.Track;
                if (pconfig.AffinityMask != 0)
                    p.First().game.SetAffinity(pconfig.AffinityMask);
            }
        }

        /// <summary>
        /// save the performer config file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_Performer_Settings(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Performerconfig | *.cfg"
            };

            if (openFileDialog.ShowDialog() != true)
                return;

            List<PerformerSettingData> pdatalist = new List<PerformerSettingData>();
            foreach (var performer in Bards)
            {
                PerformerSettingData pdata = new PerformerSettingData();
                pdata.Name = performer.game.PlayerName;
                pdata.Track = performer.TrackNumber;
                pdata.AffinityMask = (long)performer.game.GetAffinity();
                pdatalist.Add(pdata);
            }
            var t = JsonConvert.SerializeObject(pdatalist);
            byte[] content = new UTF8Encoding(true).GetBytes(t);

            FileStream fileStream = File.Create(openFileDialog.FileName);
            fileStream.Write(content, 0, content.Length);
            fileStream.Close();
        }

        /// <summary>
        /// Button context menu routine
        /// </summary>
        private void MenuButton_PreviewMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            Button rectangle = sender as Button;
            ContextMenu contextMenu = rectangle.ContextMenu;
            contextMenu.PlacementTarget = rectangle;
            contextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            contextMenu.IsOpen = true;
        }
    }

    /// <summary>
    /// Helperclass
    /// </summary>
    public class PerformerSettingData
    {
        public string Name { get; set; } = "";
        public int Track { get; set; } = 0;
        public long AffinityMask { get; set; } = 0;
    }
}
