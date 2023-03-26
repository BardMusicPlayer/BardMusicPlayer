using System.Windows;
using System.Windows.Controls;
using BardMusicPlayer.Maestro.Old;
using BardMusicPlayer.Maestro.Old.Utils;
using BardMusicPlayer.Pigeonhole;

namespace BardMusicPlayer.UI_Classic;

public partial class ClassicMainView
{
    /// <summary>
    /// load the settings
    /// </summary>
    private void LoadConfig(bool reload = false)
    {
        AutoPlayCheckBox.IsChecked = BmpPigeonhole.Instance.PlaylistAutoPlay;

        //Playback
        HoldNotesBox.IsChecked           = BmpPigeonhole.Instance.HoldNotes;
        ForcePlaybackBox.IsChecked       = BmpPigeonhole.Instance.ForcePlayback;
        IgnoreProgramChangeBox.IsChecked = BmpPigeonhole.Instance.IgnoreProgChange;

        //Don't call this on reload
        if (!reload)
        {
            MidiInputDeviceBox.Items.Clear();
            MidiInputDeviceBox.ItemsSource   = MidiInput.ReloadMidiInputDevices();
            MidiInputDeviceBox.SelectedIndex = BmpPigeonhole.Instance.MidiInputDev + 1;
        }
        LiveMidiDelay.IsChecked = BmpPigeonhole.Instance.LiveMidiPlayDelay;

        //Misc
        AutostartSource.SelectedIndex = BmpPigeonhole.Instance.AutostartMethod;
        HypnotoadBox.IsChecked        = BmpPigeonhole.Instance.UsePluginForInstrumentOpen;

        //Local orchestra
        AutoEquipBox.IsChecked         = BmpPigeonhole.Instance.AutoEquipBards;
        KeepTrackSettingsBox.IsChecked = BmpPigeonhole.Instance.EnsembleKeepTrackSetting;
        
        //UI
        EnableDarkMode.IsChecked = BmpPigeonhole.Instance.DarkStyle;

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
    
    private void IgnoreProgramChangeBox_Checked(object sender, RoutedEventArgs e)
    {
        BmpPigeonhole.Instance.IgnoreProgChange = IgnoreProgramChangeBox.IsChecked ?? false;
    }
    
    private void MIDI_Input_Device_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var d = (KeyValuePair<int, string>)MidiInputDeviceBox.SelectedItem;
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
        BmpPigeonhole.Instance.LiveMidiPlayDelay = LiveMidiDelay.IsChecked ?? false;
    }
    #endregion

    #region Misc
    private void Autostart_Source_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var d = AutostartSource.SelectedIndex;
        BmpPigeonhole.Instance.AutostartMethod = d;
    }

    private void Hypnotoad_Checked(object sender, RoutedEventArgs e)
    {
        BmpPigeonhole.Instance.UsePluginForInstrumentOpen = HypnotoadBox.IsChecked ?? false;
    }
    #endregion

    #region Local orchestra

    private void AutoEquipBox_Checked(object sender, RoutedEventArgs e)
    {
        BmpPigeonhole.Instance.AutoEquipBards = AutoEquipBox.IsChecked ?? false;
        Globals.Globals.ReloadConfig();
    }

    private void KeepTrackSettingsBox_Checked(object sender, RoutedEventArgs e)
    {
        BmpPigeonhole.Instance.EnsembleKeepTrackSetting = KeepTrackSettingsBox.IsChecked ?? false;
    }
    #endregion
    
    #region UI
    private void EnableDarkMode_Checked(object sender, RoutedEventArgs e)
    {
        BmpPigeonhole.Instance.DarkStyle = EnableDarkMode.IsChecked ?? false;

        if (!BmpPigeonhole.Instance.DarkStyle)
            MainWindow.LightModeStyle();
        else
            MainWindow.DarkModeStyle();
    }
    #endregion
}