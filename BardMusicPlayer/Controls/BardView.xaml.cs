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

        private int _maxTracks;

        public int MaxTracks { 
            get => _maxTracks; 
            set {
                _maxTracks = value;
                RaisePropertyChanged(nameof(MaxTracks));
            } }
        

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

        BmpMaestro.Instance.OnPerformerChanged += OnPerformerChanged;
        // BmpMaestro.Instance.OnTrackNumberChanged += OnTrackNumberChanged;
        // BmpMaestro.Instance.OnOctaveShiftChanged += OnOctaveShiftChanged;
        BmpMaestro.Instance.OnSongLoaded       += OnSongLoaded;
        BmpMaestro.Instance.OnPerformerUpdate  += OnPerformerUpdate;
        BmpSeer.Instance.PlayerNameChanged     += OnPlayerNameChanged;
        BmpSeer.Instance.InstrumentHeldChanged += OnInstrumentHeldChanged;
        BmpSeer.Instance.HomeWorldChanged      += OnHomeWorldChanged;
    }

    public BardViewModel Bards { get; } = new();

    private Performer? SelectedBard { get; set; }

    private void OnPerformerChanged(object? sender, bool e) { UpdateList(); }
    // private void OnTrackNumberChanged(object? sender, TrackNumberChangedEvent e) { UpdateView(); }
    // private void OnOctaveShiftChanged(object? sender, OctaveShiftChangedEvent e) { UpdateView(); }
    private void OnSongLoaded(object? sender, SongLoadedEvent e) { Bards.MaxTracks = e.MaxTracks; UpdateView(); }
    private void OnPerformerUpdate(object? sender, PerformerUpdate e) { UpdateView(); }
    private void OnPlayerNameChanged(PlayerNameChanged e) { UpdateView(); }
    private void OnHomeWorldChanged(HomeWorldChanged e) { UpdateView(); }

    private void OnInstrumentHeldChanged(InstrumentHeldChanged e)
    {
        UpdateView();
    }

    /// <summary>
    /// Update the contents of the item only
    /// </summary>
    private void UpdateView()
    {
        Dispatcher.BeginInvoke(new Action(() =>
        {
            BardsList.Items.Refresh();
        }));
    }

    /// <summary>
    /// Add/Remove an element from the list
    /// </summary>
    private void UpdateList()
    {
        Dispatcher.BeginInvoke(new Action(() =>
        {
            var tempPerf = BardsList.Items.OfType<Performer>().ToList();
            var comparator = BmpMaestro.Instance.GetAllPerformers().Except(tempPerf).ToList();
            foreach (var p in comparator)
                BardsList.Items.Add(p);
            comparator = tempPerf.Except(BmpMaestro.Instance.GetAllPerformers()).ToList();
            foreach (var p in comparator)
                BardsList.Items.Remove(p);
            BardsList.Items.Refresh();
        }));
    }

    #region Drag&Drop
    /// <summary>
    /// Drag & Drop Start
    /// </summary>
    private Point _startPoint;

    private void BardsListItemPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _startPoint = e.GetPosition(null);
    }

    private void BardsListItem_MouseMove(object sender, MouseEventArgs e)
    {
        var mousePos = e.GetPosition(null);
        var diff = _startPoint - mousePos;
        if (e.LeftButton == MouseButtonState.Pressed &&
            (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
             Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
        {
            if (sender is ListViewItem celltext)
            {
                DragDrop.DoDragDrop(BardsList, celltext, DragDropEffects.Move);
                e.Handled = true;
            }
        }
    }

    /// <summary>
    /// Called when there is a drop
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BardsListItem_Drop(object sender, DragEventArgs e)
    {
        var draggedObject = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;
        var targetObject = (ListViewItem)sender;

        var drag = draggedObject?.Content as Performer;
        var drop = targetObject.Content as Performer;
        var dragIdx = BardsList.Items.IndexOf(drag);
        var dropIdx = BardsList.Items.IndexOf(drop);

        if (drag == drop)
            return;

        var newBardsList = new SortedDictionary<int, Performer?>();
        var index = 0;

        foreach (var p in BardsList.Items)
        {
            if ((Performer)p == drag)
                continue;

            if ((Performer)p == drop)
            {
                if (dropIdx < dragIdx)
                {
                    newBardsList.Add(index, drag); index++;
                    newBardsList.Add(index, drop); index++;
                }
                else if (dropIdx > dragIdx)
                {
                    newBardsList.Add(index, drop); index++;
                    newBardsList.Add(index, drag); index++;
                }
            }
            else
            {
                newBardsList.Add(index, p as Performer);
                index++;
            }
        }

        BardsList.ItemsSource = null;
        BardsList.Items.Clear();

        foreach (var p in newBardsList)
        {
            BardsList.Items.Add(p.Value);
        }

        newBardsList.Clear();

        // Update the track list order
        UpdateTrackListOrder();
    }

    private void UpdateTrackListOrder()
    {
        var trackNumber = 1;
        foreach (Performer performer in BardsList.Items)
        {
            performer.TrackNumber = trackNumber;
            trackNumber++;
        }
    }
    #endregion

    /* Track UP/Down */
    private void TrackNumericUpDown_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (sender is TrackNumericUpDown ctl) ctl.OnValueChanged += OnValueChanged;
    }

    private static void OnValueChanged(object? sender, int s)
    {
        var game = (sender as TrackNumericUpDown)?.DataContext as Performer;
        BmpMaestro.Instance.SetTracknumber(game, s);

        if (sender is TrackNumericUpDown ctl) ctl.OnValueChanged -= OnValueChanged;

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

    /// <summary>
    /// Helper class
    /// </summary>
    public class PerformerSettingData
    {
        public string CID { get; set; } = "None";
        public int OrderNum { get; set; } = -1;
        public string Name { get; set; } = "";
        public int Track { get; set; }
        public long AffinityMask { get; set; }
        public bool IsHost { get; set; }
        public bool IsInGameSoundEnabled { get; set; } = true;
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
        var performerDataList =
            JsonConvert.DeserializeObject<List<PerformerSettingData>>(new UTF8Encoding(true).GetString(data));

        if (performerDataList != null)
        {
            foreach (var pconfig in performerDataList)
            {

                var p = BardsList.Items.OfType<Performer>().ToList().Where(perf => perf.game.ConfigId.Equals(pconfig.CID));
                if (!p.Any())
                {
                    p = BardsList.Items.OfType<Performer>().ToList().Where(perf => perf.game.PlayerName.Equals(pconfig.Name));
                    if (!p.Any())
                        continue;
                }

                p.First().TrackNumber = pconfig.Track;
                if (pconfig.AffinityMask != 0)
                    p.First().game.SetAffinity(pconfig.AffinityMask);
                
                if (p.First().game.SetSoundOnOff(pconfig.IsInGameSoundEnabled).Result)
                    p.First().game.SoundOn = pconfig.IsInGameSoundEnabled;
            }

            var tempPerf = new List<Performer>(BardsList.Items.OfType<Performer>().ToList());

            //Reorder if there is no -1 OrderNum
            if (performerDataList.All(p => p.OrderNum != -1))
            {
                performerDataList.Sort((x, y) => x.OrderNum.CompareTo(y.OrderNum));
                BardsList.Items.Clear();
                foreach (var p in performerDataList.Select(pconfig => tempPerf.Where(perf => perf.game.ConfigId.Equals(pconfig.CID))).Where(p => p.Any()))
                {
                    BardsList.Items.Add(p.First());
                }
            }
            //Set the host performer
            /*var host_perf = pdatalist.Where(p => p.IsHost);
            if (host_perf.Count() != 0)
            {
                BmpMaestro.Instance.SetHostBard(tempPerf.Find(p => p.game.ConfigId == host_perf.First().CID));
                BmpMaestro.Instance.SetTracknumber(tempPerf.Find(p => p.game.ConfigId == host_perf.First().CID), host_perf.First().Track);
            }*/
            tempPerf.Clear();
            performerDataList.Clear();

            if (!BmpPigeonhole.Instance.EnsembleKeepTrackSetting)
            {
                BmpPigeonhole.Instance.EnsembleKeepTrackSetting = true;
            }

            tempPerf = BardsList.Items.OfType<Performer>().ToList();
            foreach (var perf in tempPerf.Where(perf => !perf.game.SoundOn && !perf.HostProcess))
            {
                MuteCheckBox.IsChecked = true;
            }

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

        var performerDataList = BardsList.Items.OfType<Performer>()
            .ToList()
            .Select(performer => new PerformerSettingData
            {
                OrderNum             = BardsList.Items.IndexOf(performer),
                CID                  = performer.game.ConfigId,
                Name                 = performer.game.PlayerName,
                Track                = performer.TrackNumber,
                AffinityMask         = performer.game.GetAffinity(),
                IsHost               = performer.HostProcess,
                IsInGameSoundEnabled = performer.game.SoundOn
            })
            .ToList();
        var t = JsonConvert.SerializeObject(performerDataList);
        var content = new UTF8Encoding(true).GetBytes(t);

        var fileStream = File.Create(openFileDialog.FileName);
        fileStream.Write(content, 0, content.Length);
        fileStream.Close();
        performerDataList.Clear();
    }

    private void GfxLow_CheckBox_Checked(object sender, RoutedEventArgs e)
    {
        //foreach (var p in Bards.Where(p => p.game.GfxSettingsLow != GfxLowCheckBox.IsChecked))
        foreach (var p in BardsList.Items.OfType<Performer>().ToList().Where(p => p.game.GfxSettingsLow != GfxLowCheckBox.IsChecked))
        {
            p.game.GfxSettingsLow = GfxLowCheckBox.IsChecked ?? false;
            p.game.GfxSetLow(GfxLowCheckBox.IsChecked ?? false);
        }
    }

    private void Mute_CheckBox_Checked(object sender, RoutedEventArgs e)
    {
        foreach (var p in BardsList.Items.OfType<Performer>().ToList().Where(p => !p.HostProcess))
        {
            p.game.SoundOn = !MuteCheckBox.IsChecked ?? false;
            p.game.SetSoundOnOff(!MuteCheckBox.IsChecked ?? false);
        }
    }

    /// <summary>
    /// Window pos load button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ArrangeWindow_Click(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter      = "WindowLayout | *.txt",
            Multiselect = true
        };

        if (openFileDialog.ShowDialog() != true)
            return;

        ArrangeWindows(openFileDialog.FileName);
    }

    /// <summary>
    /// Arrange the window position and size
    /// </summary>
    private void ArrangeWindows(string filename)
    {
        if (BardsList.Items.OfType<Performer>().ToList().Count == 0)
            return;

        var y = 0;
        var size_x = 0;
        var size_y = 0;
        var reader = new StreamReader(filename);
        var input = reader.ReadLine();
        if (input != null && input.Split(':')[0].Contains("Size"))
        {
            size_x = Convert.ToInt32(input.Split(':')[1].Split('x')[0]);
            size_y = Convert.ToInt32(input.Split(':')[1].Split('x')[1]);
        }

        while (reader.ReadLine() is { } line)
        {
            var x = 0;
            for (var i = 0; i < line.Length;)
            {
                var value = line[i] + line[i + 1].ToString();
                i += 2;
                if (value != "--")
                {
                    var bard = BardsList.Items.OfType<Performer>().ToList().FirstOrDefault(p => p.TrackNumber == Convert.ToInt32(value));
                    if (bard == null)
                        continue;
                    bard.game.SetWindowPosAndSize(x, y, size_x, size_y, true);
                }
                x += size_x;
            }
            y += size_y;
        }
        reader.Close();
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
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null) return null;
        var text = value.ToString();
        var width = double.Parse(parameter?.ToString() ?? string.Empty);
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

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}