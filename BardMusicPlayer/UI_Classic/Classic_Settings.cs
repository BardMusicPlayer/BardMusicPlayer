using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BardMusicPlayer.Maestro.Old.Utils;
using BardMusicPlayer.Maestro.Old;
using BardMusicPlayer.Pigeonhole;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;

namespace BardMusicPlayer.UI_Classic;

public partial class Classic_MainView
{
    /// <summary>
    /// load the settings
    /// </summary>
    private void LoadConfig(bool reload = false)
    {
        AutoPlay_CheckBox.IsChecked   = BmpPigeonhole.Instance.PlaylistAutoPlay;

        //Playback
        HoldNotesBox.IsChecked        = BmpPigeonhole.Instance.HoldNotes;
        ForcePlaybackBox.IsChecked    = BmpPigeonhole.Instance.ForcePlayback;
        IgnoreProgChangeBox.IsChecked = BmpPigeonhole.Instance.IgnoreProgChange;

        //Don't call this on reload
        if (!reload)
        {
            MIDI_Input_DeviceBox.Items.Clear();
            MIDI_Input_DeviceBox.ItemsSource   = MidiInput.ReloadMidiInputDevices();
            MIDI_Input_DeviceBox.SelectedIndex = BmpPigeonhole.Instance.MidiInputDev + 1;
        }
        LiveMidiDelay.IsChecked        = BmpPigeonhole.Instance.LiveMidiPlayDelay;

        //Misc
        Autostart_source.SelectedIndex = BmpPigeonhole.Instance.AutostartMethod;
        AutoequipDalamud.IsChecked     = BmpPigeonhole.Instance.UsePluginForInstrumentOpen;

        //Local orchestra
        AutoEquipBox.IsChecked         = BmpPigeonhole.Instance.AutoEquipBards;
        KeepTrackSettingsBox.IsChecked = BmpPigeonhole.Instance.EnsembleKeepTrackSetting;
        
        //UI
        EnableDarkMode.IsChecked       = BmpPigeonhole.Instance.DarkStyle;

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
    
    private void IgnoreProgChangeBox_Checked(object sender, RoutedEventArgs e)
    {
        BmpPigeonhole.Instance.IgnoreProgChange = IgnoreProgChangeBox.IsChecked ?? false;
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
        BmpPigeonhole.Instance.LiveMidiPlayDelay = LiveMidiDelay.IsChecked ?? false;
    }
    #endregion

    #region Misc
    private void Autostart_source_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var d = Autostart_source.SelectedIndex;
        BmpPigeonhole.Instance.AutostartMethod = d;
    }

    private void AutoequipDalamud_Checked(object sender, RoutedEventArgs e)
    {
        BmpPigeonhole.Instance.UsePluginForInstrumentOpen = AutoequipDalamud.IsChecked ?? false;
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
        var paletteHelper = new PaletteHelper();
        var theme = paletteHelper.GetTheme();
        
        if (BmpPigeonhole.Instance.DarkStyle) {
            theme.SetBaseTheme(Theme.Dark);
            theme.SetPrimaryColor(Colors.DarkOrange);
            theme.SetSecondaryColor(Colors.Blue);
            //theme.PrimaryMid = new ColorPair(Colors.Brown, Colors.White);
            paletteHelper.SetTheme(theme);
        }
        else
        {
            theme.SetBaseTheme(Theme.Light);
            theme.SetPrimaryColor(Colors.LightSlateGray);
            theme.SetSecondaryColor(Colors.Green);
            //theme.PrimaryMid = new ColorPair(Colors.Brown, Colors.White);
            paletteHelper.SetTheme(theme);
        }
    }
    #endregion
}