using System.Windows;
using System.Windows.Controls;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Maestro;
using System.Collections.Generic;
using System;
using BardMusicPlayer.Siren;
using BardMusicPlayer.Ui.Globals.SkinContainer;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UI.Resources;

namespace BardMusicPlayer.Ui.Skinned
{
    /// <summary>
    /// Interaktionslogik für Settings.xaml
    /// </summary>
    public partial class SettingsView : Window
    {
        /// <summary>
        /// Helper class to get skin preview working
        /// </summary>
        public class SkinData
        {
            private string _Title;
            public string Title
            {
                get { return this._Title; }
                set { this._Title = value; }
            }

            private BitmapImage _ImageData;
            public BitmapImage ImageData
            {
                get { return this._ImageData; }
                set { this._ImageData = value; }
            }
        }

        public SettingsView()
        {
            InitializeComponent();
            ApplySkin();
            SkinContainer.OnNewSkinLoaded += SkinContainer_OnNewSkinLoaded;

            //Design Tab
            ClassicSkin.IsChecked = BmpPigeonhole.Instance.ClassicUi;

            //Playback Tab
            this.HoldNotesBox.IsChecked = BmpPigeonhole.Instance.HoldNotes;
            this.ForcePlaybackBox.IsChecked = BmpPigeonhole.Instance.ForcePlayback;
            MIDI_Input_DeviceBox.Items.Clear();
            MIDI_Input_DeviceBox.ItemsSource = Maestro.Utils.MidiInput.ReloadMidiInputDevices();
            this.MIDI_Input_DeviceBox.SelectedIndex = BmpPigeonhole.Instance.MidiInputDev + 1;
            AutoPlayBox.IsChecked = BmpPigeonhole.Instance.PlaylistAutoPlay;
            LiveMidiDelay.IsChecked = BmpPigeonhole.Instance.LiveMidiPlayDelay;
            this.AutoequipSoloBard.IsChecked = BmpPigeonhole.Instance.SoloBardAutoEquip;

            //Local Orchestra Tab
            this.LocalOrchestraBox.IsChecked = BmpPigeonhole.Instance.LocalOrchestra;
            this.AutoEquipBox.IsChecked = BmpPigeonhole.Instance.EnsembleAutoEquip;
            this.KeepTrackSettingsBox.IsChecked = BmpPigeonhole.Instance.EnsembleKeepTrackSetting;
            this.StartBardIndividuallyBox.IsChecked = BmpPigeonhole.Instance.EnsembleStartIndividual;

            //Syncsettings Tab
            Autostart_source.SelectedIndex = BmpPigeonhole.Instance.AutostartMethod;
            this.MidiBardComp.IsChecked = BmpPigeonhole.Instance.MidiBardCompatMode;

            //Misc Tab
            SirenVolume.Value = BmpSiren.Instance.GetVolume();
            this.AMPInFrontBox.IsChecked = BmpPigeonhole.Instance.BringBMPtoFront;

            //Path Tab
            SongsDir.Text = BmpPigeonhole.Instance.SongDirectory;
            SkinsDir.Text = BmpPigeonhole.Instance.SkinDirectory;

            //Load the skin previews
            if (Directory.Exists(BmpPigeonhole.Instance.SkinDirectory))
            {
                string[] files = Directory.EnumerateFiles(BmpPigeonhole.Instance.SkinDirectory, "*.wsz", SearchOption.TopDirectoryOnly).ToArray();
                Parallel.ForEach(files, file =>
                {
                    string name = Path.GetFileNameWithoutExtension(file);
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        var bmap = ((Skinned_MainView)System.Windows.Application.Current.MainWindow.DataContext).ExtractBitmapFromZip(file, "Screenshot.png");
                        if (bmap == null)
                            bmap = ((Skinned_MainView)System.Windows.Application.Current.MainWindow.DataContext).ExtractBitmapFromZip(file, "MAIN.BMP");
                        this.SkinPreviewBox.Items.Add(new SkinData { Title = name, ImageData = bmap });
                    }));
                });
            }
        }

        #region Window design and buttons
        /// <summary>
        /// Triggered by the SkinLoader, if a new skin was loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SkinContainer_OnNewSkinLoaded(object sender, EventArgs e)
        { ApplySkin(); }

        /// <summary>
        /// Applies a skin
        /// </summary>
        public void ApplySkin()
        {
            this.BARDS_TOP_LEFT.Fill = SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_TOP_LEFT_CORNER];
            this.BARDS_TOP_TILE.Fill = SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_TOP_TILE];
            this.BARDS_TOP_RIGHT.Fill = SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_TOP_RIGHT_CORNER];

            this.BARDS_BOTTOM_LEFT_CORNER.Fill = SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_BOTTOM_LEFT_CORNER];
            this.BARDS_BOTTOM_TILE.Fill = SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_BOTTOM_TILE];
            this.BARDS_BOTTOM_RIGHT_CORNER.Fill = SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_BOTTOM_RIGHT_CORNER];

            this.BARDS_LEFT_TILE.Fill = SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_LEFT_TILE];
            this.BARDS_RIGHT_TILE.Fill = SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_RIGHT_TILE];

            this.Close_Button.Background = SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_CLOSE_SELECTED];
            this.Close_Button.Background.Opacity = 0;

        }

        /// <summary>
        /// if a mousedown event was triggered by the title bar, dragmove the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        /// <summary>
        /// The close button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// if mouse button down on close button, change bitmap
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_Button_Down(object sender, MouseButtonEventArgs e)
        {
            this.Close_Button.Background.Opacity = 1;
        }
        /// <summary>
        /// if mouse button up on close button, change bitmap
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_Button_Up(object sender, MouseButtonEventArgs e)
        {
            this.Close_Button.Background.Opacity = 0;
        }
        #endregion

        #region DesignTab controls
        /// <summary>
        /// The classic skin checkbox action
        /// </summary>
        private void ClassicSkin_Checked(object sender, RoutedEventArgs e)
        {
            BmpPigeonhole.Instance.ClassicUi = ClassicSkin.IsChecked ?? true;
        }

        /// <summary>
        /// skinpreview doubleclick: change skin
        /// </summary>
        private void SkinPreviewBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SkinData d = SkinPreviewBox.SelectedItem as SkinData;
            if (d == null)
                return;
            string fileName = BmpPigeonhole.Instance.SkinDirectory + d.Title + ".wsz";
            ((Skinned_MainView)System.Windows.Application.Current.MainWindow.DataContext).LoadSkin(fileName);
            BmpPigeonhole.Instance.LastSkin = fileName;
        }
        #endregion

        #region PlaybackTab controls
        private void Hold_Notes_Checked(object sender, RoutedEventArgs e)
        {
            BmpPigeonhole.Instance.HoldNotes = (HoldNotesBox.IsChecked ?? false);
        }

        private void Force_Playback_Checked(object sender, RoutedEventArgs e)
        {
            BmpPigeonhole.Instance.ForcePlayback = (ForcePlaybackBox.IsChecked ?? false);
        }

        private void MIDI_Input_Device_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var d = (KeyValuePair<int, string>)MIDI_Input_DeviceBox.SelectedItem;
            BmpPigeonhole.Instance.MidiInputDev = d.Key;
            if (d.Key == -1)
            {
                BmpMaestro.Instance.CloseInputDevice();
                return;
            }

            BmpMaestro.Instance.OpenInputDevice(d.Key);
        }

        private void AutoPlay_Checked(object sender, RoutedEventArgs e)
        {
            BmpPigeonhole.Instance.PlaylistAutoPlay = (AutoPlayBox.IsChecked ?? false);
        }

        private void LiveMidiDelay_Checked(object sender, RoutedEventArgs e)
        {
            BmpPigeonhole.Instance.LiveMidiPlayDelay = (LiveMidiDelay.IsChecked ?? false);
        }

        private void AutoequipSoloBard_Checked(object sender, RoutedEventArgs e)
        {
            BmpPigeonhole.Instance.SoloBardAutoEquip = AutoequipSoloBard.IsChecked ?? false;
        }
        #endregion

        #region Local orchestra controls
        private void LocalOrchestraBox_Checked(object sender, RoutedEventArgs e)
        {
            BmpPigeonhole.Instance.LocalOrchestra = LocalOrchestraBox.IsChecked ?? false;
        }

        private void AutoEquipBox_Checked(object sender, RoutedEventArgs e)
        {
            BmpPigeonhole.Instance.EnsembleAutoEquip = AutoEquipBox.IsChecked ?? false;
        }

        private void KeepTrackSettingsBox_Checked(object sender, RoutedEventArgs e)
        {
            BmpPigeonhole.Instance.EnsembleKeepTrackSetting = KeepTrackSettingsBox.IsChecked ?? false;
        }

        private void StartBardIndividually_Checked(object sender, RoutedEventArgs e)
        {
            BmpPigeonhole.Instance.EnsembleStartIndividual = StartBardIndividuallyBox.IsChecked ?? false;
        }
        #endregion

        #region SyncsettingsTab controls
        private void Autostart_source_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int d = Autostart_source.SelectedIndex;
            BmpPigeonhole.Instance.AutostartMethod = (int)d;
        }
        private void MidiBard_Checked(object sender, RoutedEventArgs e)
        {
            BmpPigeonhole.Instance.MidiBardCompatMode = MidiBardComp.IsChecked ?? false;
        }
        #endregion

        #region Miscsettings Tab controls
        private void SirenVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var g = SirenVolume.Value;
            BmpSiren.Instance.SetVolume((float)g);
        }

        private void AMPInFrontBox_Checked(object sender, RoutedEventArgs e)
        {
            BmpPigeonhole.Instance.BringBMPtoFront = AMPInFrontBox.IsChecked ?? false;
        }
        #endregion

        #region PathTab controls
        private void SongsDir_TextChanged(object sender, TextChangedEventArgs e)
        {
            BmpPigeonhole.Instance.SongDirectory = SongsDir.Text+ (SongsDir.Text.EndsWith("\\") ? "" : "\\");
        }

        private void SongsDir_Button_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new FolderPicker();

            if (Directory.Exists(BmpPigeonhole.Instance.SongDirectory))
                dlg.InputPath = Path.GetFullPath(BmpPigeonhole.Instance.SongDirectory);
            else
                dlg.InputPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            if (dlg.ShowDialog() == true)
            {
                string path = dlg.ResultPath;
                if (!Directory.Exists(path))
                    return;

                path = path + (path.EndsWith("\\") ? "" : "\\");
                SongsDir.Text = path;
                BmpPigeonhole.Instance.SongDirectory = path;
            }
        }

        private void SkinsDir_TextChanged(object sender, TextChangedEventArgs e)
        {
            BmpPigeonhole.Instance.SkinDirectory = SkinsDir.Text + (SkinsDir.Text.EndsWith("\\") ? "" : "\\");
        }

        private void SkinsDir_Button_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new FolderPicker();

            if (Directory.Exists(BmpPigeonhole.Instance.SkinDirectory))
                dlg.InputPath = Path.GetFullPath(BmpPigeonhole.Instance.SkinDirectory);
            else
                dlg.InputPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            if (dlg.ShowDialog() == true)
            {
                string path = dlg.ResultPath;
                if (!Directory.Exists(path))
                    return;

                path = path + (path.EndsWith("\\") ? "" : "\\");
                SkinsDir.Text = path;
                BmpPigeonhole.Instance.SkinDirectory = path;
            }
        }
        #endregion
    }
}
