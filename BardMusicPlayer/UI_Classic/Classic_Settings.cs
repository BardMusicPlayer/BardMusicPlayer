using BardMusicPlayer.Maestro;
using BardMusicPlayer.Pigeonhole;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace BardMusicPlayer.Ui.Classic
{
    public partial class Classic_MainView : UserControl
    {
        /// <summary>
        /// load the settings
        /// </summary>
        private void LoadConfig(bool reload = false)
        {
            this.AutoPlay_CheckBox.IsChecked = BmpPigeonhole.Instance.PlaylistAutoPlay;

            this.AMPInFrontBox.IsChecked = BmpPigeonhole.Instance.BringBMPtoFront;

            //Playback
            this.HoldNotesBox.IsChecked = BmpPigeonhole.Instance.HoldNotes;
            this.ForcePlaybackBox.IsChecked = BmpPigeonhole.Instance.ForcePlayback;

            //Don't call this on reload
            if (!reload)
            {
                MIDI_Input_DeviceBox.Items.Clear();
                MIDI_Input_DeviceBox.ItemsSource = Maestro.Utils.MidiInput.ReloadMidiInputDevices();
                this.MIDI_Input_DeviceBox.SelectedIndex = BmpPigeonhole.Instance.MidiInputDev + 1;
            }
            LiveMidiDelay.IsChecked = BmpPigeonhole.Instance.LiveMidiPlayDelay;

            //Misc
            this.Autostart_source.SelectedIndex = BmpPigeonhole.Instance.AutostartMethod;
            this.AutoequipDalamud.IsChecked = BmpPigeonhole.Instance.UsePluginForInstrumentOpen;

            //Local orchestra
            this.LocalOrchestraBox.IsChecked = BmpPigeonhole.Instance.LocalOrchestra;
            this.AutoEquipBox.IsChecked = BmpPigeonhole.Instance.AutoEquipBards;
            this.KeepTrackSettingsBox.IsChecked = BmpPigeonhole.Instance.EnsembleKeepTrackSetting;
            this.IgnoreProgchangeBox.IsChecked = BmpPigeonhole.Instance.IgnoreProgChange;
        }

        private void AMPInFrontBox_Checked(object sender, RoutedEventArgs e)
        {
            BmpPigeonhole.Instance.BringBMPtoFront = AMPInFrontBox.IsChecked ?? false;
        }

        #region Playback
        private void Hold_Notes_Checked(object sender, RoutedEventArgs e)
        {
            BmpPigeonhole.Instance.HoldNotes = HoldNotesBox.IsChecked ?? false;
        }

        private void Force_Playback_Checked(object sender, RoutedEventArgs e)
        {
            BmpPigeonhole.Instance.ForcePlayback = ForcePlaybackBox.IsChecked ?? false;
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

        private void LiveMidiDelay_Checked(object sender, RoutedEventArgs e)
        {
            BmpPigeonhole.Instance.LiveMidiPlayDelay = (LiveMidiDelay.IsChecked ?? false);
        }
        #endregion

        #region Misc
        private void Autostart_source_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int d = Autostart_source.SelectedIndex;
            BmpPigeonhole.Instance.AutostartMethod = (int)d;
        }

        #region Local orchestra
        private void LocalOrchestraBox_Checked(object sender, RoutedEventArgs e)
        {
            BmpPigeonhole.Instance.LocalOrchestra = LocalOrchestraBox.IsChecked ?? false;
        }

        private void AutoEquipBox_Checked(object sender, RoutedEventArgs e)
        {
            BmpPigeonhole.Instance.AutoEquipBards = AutoEquipBox.IsChecked ?? false;
            Globals.Globals.ReloadConfig();
        }

        private void KeepTrackSettingsBox_Checked(object sender, RoutedEventArgs e)
        {
            BmpPigeonhole.Instance.EnsembleKeepTrackSetting = KeepTrackSettingsBox.IsChecked ?? false;
        }

        private void IgnoreProgchangeBox_Checked(object sender, RoutedEventArgs e)
        {
            BmpPigeonhole.Instance.IgnoreProgChange = IgnoreProgchangeBox.IsChecked ?? false;
        }
        #endregion
    }
}
