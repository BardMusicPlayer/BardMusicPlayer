using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using BardMusicPlayer.DalamudBridge;
using BardMusicPlayer.Maestro.Old;
using BardMusicPlayer.Maestro.Old.Events;
using BardMusicPlayer.Maestro.Old.Performance;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Seer;
using BardMusicPlayer.Seer.Events;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace BardMusicPlayer.Controls;

/// <summary>
/// Interaction logic for BardView.xaml
/// </summary>
public partial class BardView
{
    public sealed class BardViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Performer> _bardList = new();

        private int _maxTracks;

        public int MaxTracks { 
            get => _maxTracks; 
            set {
                _maxTracks = value;
                RaisePropertyChanged(nameof(MaxTracks));
            } }


        public ObservableCollection<Performer> BardList
        {
            get => _bardList;
            set
            {
                _bardList = value;
                RaisePropertyChanged(nameof(BardList));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public BardView()
    {
        InitializeComponent();

        DataContext = this;
        //Bards       = new BardViewModel(); // initialize the Bards property

        BmpMaestro.Instance.OnPerformerChanged   += OnPerformerChanged;
        // BmpMaestro.Instance.OnTrackNumberChanged += OnTrackNumberChanged;
        // BmpMaestro.Instance.OnOctaveShiftChanged += OnOctaveShiftChanged;
        BmpMaestro.Instance.OnSongLoaded         += OnSongLoaded;
        BmpMaestro.Instance.OnPerformerUpdate    += OnPerformerUpdate;
        BmpSeer.Instance.PlayerNameChanged       += OnPlayerNameChanged;
        BmpSeer.Instance.InstrumentHeldChanged   += OnInstrumentHeldChanged;
        BmpSeer.Instance.HomeWorldChanged        += OnHomeWorldChanged;
        Globals.Globals.OnConfigReload           += Globals_OnConfigReload;
        Globals_OnConfigReload(null, null);
    }

    private void Globals_OnConfigReload(object? sender, EventArgs? e)
    {
        AutoEquipCheckBox.IsChecked = BmpPigeonhole.Instance.AutoEquipBards;
    }

    //private ObservableCollection<Performer> Bards { get; set; }
    public BardViewModel Bards { get; } = new();

    private Performer? SelectedBard { get; set; }

    private void OnPerformerChanged(object? sender, bool e)
    {
        // Bards = new ObservableCollection<Performer>(BmpMaestro.Instance.GetAllPerformers());
        // Dispatcher.BeginInvoke(new Action(() => BardsList.ItemsSource = Bards));
        Bards.BardList = new ObservableCollection<Performer>(BmpMaestro.Instance.GetAllPerformers());
    }

    // private void OnTrackNumberChanged(object? sender, TrackNumberChangedEvent e)
    // {
    //     UpdateList();
    // }
    //
    // private void OnOctaveShiftChanged(object? sender, OctaveShiftChangedEvent e)
    // {
    //     UpdateList();
    // }

    private void OnSongLoaded(object? sender, SongLoadedEvent e)
    {
        Bards.MaxTracks = e.MaxTracks;

        UpdateList();
    }

    private void OnPerformerUpdate(object? sender, PerformerUpdate e)
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
        // Bards = new ObservableCollection<Performer>(BmpMaestro.Instance.GetAllPerformers());
        // Dispatcher.BeginInvoke(new Action(() => BardsList.ItemsSource = Bards));
        Dispatcher.BeginInvoke(new Action(() => BardsList.Items.Refresh()));
    }

    private void BardsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Console.WriteLine(BardsList.SelectedItem);
    }

    private void BardsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        SelectedBard = BardsList.SelectedItem as Performer;

    }

    /* Track UP/Down */
    private void TrackNumericUpDown_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (sender is TrackNumericUpDown ctl) ctl.OnValueChanged += OnValueChanged;
    }

    private static void OnValueChanged(object? sender, int s)
    {
        var game = (sender as TrackNumericUpDown)?.DataContext as Performer;
        BmpMaestro.Instance.SetTracknumber(game, s);
        if (BmpPigeonhole.Instance.AutoEquipBards)
        {
            _ = game?.ReplaceInstrument();
        }
    }

    /* Octave UP/Down */
    private void OctaveControl_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (sender is OctaveNumericUpDown ctl) ctl.OnValueChanged += OnOctaveValueChanged;
    }

    private static void OnOctaveValueChanged(object? sender, int s)
    {
        var performer = (sender as OctaveNumericUpDown)?.DataContext as Performer;
        BmpMaestro.Instance.SetOctaveshift(performer, s);
    }

    private void HostChecker_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is CheckBox ctl && (!ctl.IsChecked ?? false))
            return;

        var game = (sender as CheckBox)?.DataContext as Performer;
        BmpMaestro.Instance.SetHostBard(game);
    }

    private void PerformerEnabledChecker_Checked(object sender, RoutedEventArgs e)
    {
        var ctl = sender as CheckBox;
        if (ctl?.DataContext is Performer game)
        {
            var isEnabled = ctl.IsChecked ?? false;
            switch (isEnabled)
            {
                case true when BmpPigeonhole.Instance.AutoEquipBards:
                    _ = game.ReplaceInstrument();
                    break;
                case false when BmpPigeonhole.Instance.AutoEquipBards:
                    game.CloseInstrument();
                    break;
            }
            game.PerformerEnabled = ctl.IsChecked ?? false;
        }
    }

    private void Bard_MouseClick(object sender, MouseButtonEventArgs e)
    {
        if ((sender as Button)?.DataContext is not Performer bard)
            return;

        SelectedBard = bard; // Set the SelectedBard property to the clicked performer
        var bardExtSettings = new BardExtSettingsWindow(SelectedBard);
        bardExtSettings.Activate();
        bardExtSettings.Visibility = Visibility.Visible;

        e.Handled = true;
    }

    private void Bard_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            if ((sender as TextBlock)?.DataContext is not Performer bard)
                return;

            SelectedBard = bard; // Set the SelectedBard property to the clicked performer
            var bardExtSettings = new BardExtSettingsWindow(SelectedBard);
            bardExtSettings.Activate();
            bardExtSettings.Visibility = Visibility.Visible;
        }
    }

    private void AutoEquip_CheckBox_Checked(object sender, RoutedEventArgs e)
    {
        BmpPigeonhole.Instance.AutoEquipBards = AutoEquipCheckBox.IsChecked ?? false;
        Globals.Globals.ReloadConfig();
    }

    /// <summary>
    /// Helper class
    /// </summary>
    public class PerformerSettingData
    {
        public string Name { get; init; } = "";
        public int Track { get; init; }
        public long AffinityMask { get; init; }
    }

    /// <summary>
    /// load the performer config file
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Load_Performer_Settings(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter      = "Performer Config | *.cfg",
            Multiselect = true
        };

        if (openFileDialog.ShowDialog() != true)
            return;

        var memoryStream = new MemoryStream();
        var fileStream = File.Open(openFileDialog.FileName, FileMode.Open);
        fileStream.CopyTo(memoryStream);
        fileStream.Close();

        var data = memoryStream.ToArray();
        var performerDataList = JsonConvert.DeserializeObject<List<PerformerSettingData>>(new UTF8Encoding(true).GetString(data));

        if (performerDataList != null)
        {
            foreach (var performerConfig in performerDataList)
            {
                //var p = Bards.Where(perf => perf.game.PlayerName.Equals(performerConfig.Name));
                var p = Bards.BardList.Where(perf => perf.game.PlayerName.Equals(performerConfig.Name));
                var performers = p as Performer[] ?? p.ToArray();
                if (!performers.Any())
                    continue;

                performers.First().TrackNumber = performerConfig.Track;
                if (performerConfig.AffinityMask != 0)
                    performers.First().game.SetAffinity(performerConfig.AffinityMask);
            }
        }

        if (!BmpPigeonhole.Instance.EnsembleKeepTrackSetting)
        {
            BmpPigeonhole.Instance.EnsembleKeepTrackSetting = true;
            Globals.Globals.ReloadConfig();
        }
    }

    /// <summary>
    /// save the performer config file
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Save_Performer_Settings(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new SaveFileDialog
        {
            Filter = "Performer Config | *.cfg"
        };

        if (openFileDialog.ShowDialog() != true)
            return;

        //var performerDataList = Bards.Select(performer => new PerformerSettingData { Name = performer.game.PlayerName, Track = performer.TrackNumber, AffinityMask = performer.game.GetAffinity() }).ToList();
        var performerDataList = Bards.BardList.Select(performer => new PerformerSettingData { Name = performer.game.PlayerName, Track = performer.TrackNumber, AffinityMask = performer.game.GetAffinity() }).ToList();
        var t = JsonConvert.SerializeObject(performerDataList);
        var content = new UTF8Encoding(true).GetBytes(t);

        var fileStream = File.Create(openFileDialog.FileName);
        fileStream.Write(content, 0, content.Length);
        fileStream.Close();
    }

    private void GfxLow_CheckBox_Checked(object sender, RoutedEventArgs e)
    {
        //foreach (var p in Bards.Where(p => p.game.GfxSettingsLow != GfxLowCheckBox.IsChecked))
        foreach (var p in Bards.BardList.Where(p => p.game.GfxSettingsLow != GfxLowCheckBox.IsChecked))
        {
            p.game.GfxSettingsLow = GfxLowCheckBox.IsChecked ?? false;
            p.game.GfxSetLow(GfxLowCheckBox.IsChecked ?? false);
        }
    }

    /// <summary>
    /// Button context menu routine
    /// </summary>
    private void MenuButton_PreviewMouseLeftButtonDown(object sender, RoutedEventArgs e)
    {
        if (sender is Button rectangle)
        {
            var contextMenu = rectangle.ContextMenu;
            if (contextMenu != null)
            {
                contextMenu.PlacementTarget = rectangle;
                contextMenu.Placement       = PlacementMode.Bottom;
                contextMenu.IsOpen          = true;
            }
        }
    }
}

public class FontSizeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null) return null;
        var text = value.ToString();
        var width = double.Parse(parameter.ToString() ?? string.Empty);
        double fontSize = 13;
        var formattedText = new FormattedText(
            text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
            new Typeface("RobotoRegularFont"), fontSize, Brushes.Black, 1.0);

        while (formattedText.Width > width && fontSize > 1)
        {
            fontSize--;
            formattedText.SetFontSize(fontSize);
        }

        return fontSize;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}