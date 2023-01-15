using BardMusicPlayer.Maestro;
using BardMusicPlayer.Maestro.Performance;
using BardMusicPlayer.DalamudBridge;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BardMusicPlayer.Quotidian.Structs;

namespace BardMusicPlayer.Ui.Controls
{
    /// <summary>
    /// Interaktionslogik für BardExtSettingsWindow.xaml
    /// </summary>
    public sealed partial class BardExtSettingsWindow : Window
    {
        private Performer _performer = null;
        private List<CheckBox> _cpuBoxes = new List<CheckBox>();

        public BardExtSettingsWindow(Performer performer)
        {
            _performer = performer;
            InitializeComponent();
            Title = "Settings for: " + _performer.PlayerName;

            Songtitle_Post_Type.SelectedIndex = 0;
            Songtitle_Chat_Type.SelectedIndex = 0;
            Chat_Type.SelectedIndex = 0;

            //Get the values for the song parsing bard
            var tpBard = BmpMaestro.Instance.GetSongTitleParsingBard();
            if (tpBard.Value != null)
            {
                if (tpBard.Value.game.Pid == _performer.game.Pid)
                {
                    Songtitle_Chat_Prefix.Text = tpBard.Key.prefix;

                    if (tpBard.Key.channelType.ChannelCode == ChatMessageChannelType.Say.ChannelCode)
                        Songtitle_Chat_Type.SelectedIndex = 0;
                    else if (tpBard.Key.channelType.ChannelCode == ChatMessageChannelType.Yell.ChannelCode)
                        Songtitle_Chat_Type.SelectedIndex = 1;
                    else if (tpBard.Key.channelType.ChannelCode == ChatMessageChannelType.Shout.ChannelCode)
                        Songtitle_Chat_Type.SelectedIndex = 2;

                    Songtitle_Post_Type.SelectedIndex = tpBard.Key.channelType.Equals(ChatMessageChannelType.None) ? 0 : 1;
                }
            }

            this.Lyrics_TrackNr.Value = performer.SingerTrackNr.ToString();
            GfxTest.IsChecked = _performer.game.GfxSettingsLow;
            PopulateCPUTab();
        }

        private void Songtitle_Post_Type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChatMessageChannelType chanType = ChatMessageChannelType.None;
            switch (Songtitle_Chat_Type.SelectedIndex)
            {
                case 0:
                    chanType = ChatMessageChannelType.Say;
                    break;
                case 1:
                    chanType = ChatMessageChannelType.Yell;
                    break;
                case 2:
                    chanType = ChatMessageChannelType.Shout;
                    break;
            }

            switch (Songtitle_Post_Type.SelectedIndex)
            {
                case 0:
                    BmpMaestro.Instance.SetSongTitleParsingBard(ChatMessageChannelType.None, "", null);
                    break;
                case 1:
                    BmpMaestro.Instance.SetSongTitleParsingBard(chanType, Songtitle_Chat_Prefix.Text, _performer);
                    break;
            }
        }

        private void PostSongTitle_Click(object sender, RoutedEventArgs e)
        {
            if (_performer.SongName == "")
                return;

            ChatMessageChannelType chanType = ChatMessageChannelType.None;
            switch (Songtitle_Chat_Type.SelectedIndex)
            {
                case 0:
                    chanType = ChatMessageChannelType.Say;
                    break;
                case 1:
                    chanType = ChatMessageChannelType.Yell;
                    break;
            }
            string songName = $"{Songtitle_Chat_Prefix.Text} {_performer.SongName} {Songtitle_Chat_Prefix.Text}";
            GameExtensions.SendText(_performer.game, chanType, songName);
        }

        private void ChatInputText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                ChatMessageChannelType chanType = ChatMessageChannelType.None;
                switch (Chat_Type.SelectedIndex)
                {
                    case 0:
                        chanType = ChatMessageChannelType.Say;
                        break;
                    case 1:
                        chanType = ChatMessageChannelType.Yell;
                        break;
                    case 2:
                        chanType = ChatMessageChannelType.Group;
                        break;
                    case 3:
                        chanType = ChatMessageChannelType.FC;
                        break;
                    case 4:
                        chanType = ChatMessageChannelType.None;
                        break;
                }
                string text = new string(ChatInputText.Text.ToCharArray());
                GameExtensions.SendText(_performer.game, chanType, text);
                ChatInputText.Text = "";
            }
        }

        private void Lyrics_TrackNr_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            NumericUpDown ctl = sender as NumericUpDown;
            ctl.OnValueChanged += Lyrics_TrackNr_OnValueChanged;
        }

        private void Lyrics_TrackNr_OnValueChanged(object sender, int s)
        {
            _performer.SingerTrackNr = s;
            NumericUpDown ctl = sender as NumericUpDown;
            ctl.OnValueChanged -= Lyrics_TrackNr_OnValueChanged;
        }

    #region CPU-Tab
        private void PopulateCPUTab()
        {
            //Get the our application's process.
            Process process = _performer.game.Process;

            //Get the processor count of our machine.
            int cpuCount = Environment.ProcessorCount;
            long AffinityMask = (long)_performer.game.GetAffinity();

            int res = (int)Math.Ceiling((double)cpuCount / (double)3);
            int idx = 1;
            for (int col = 0; col != 3; col++)
            {
                CPUDisplay.ColumnDefinitions.Add(new ColumnDefinition());
                
                for (int i = 0; i != res + 1; i++)
                {
                    if (idx == cpuCount+1)
                        break;
                    if (CPUDisplay.RowDefinitions.Count < res +1)
                        CPUDisplay.RowDefinitions.Add(new RowDefinition());
                    var uc = new CheckBox
                    {
                        Name = "CPU" + idx,
                        Content = "CPU" + idx
                    };
                    if ((AffinityMask & (1 << idx-1)) > 0) //-1 since we count at 1
                        uc.IsChecked = true;
                    _cpuBoxes.Add(uc);
                    CPUDisplay.Children.Add(uc);
                    Grid.SetRow(uc, i);
                    Grid.SetColumn(uc, CPUDisplay.ColumnDefinitions.Count - 1);
                    idx++;
                }
            }
        }

        private void Save_CPU_Click(object sender, RoutedEventArgs e)
        {
            long mask = 0;
            int idx = 0;
            foreach (CheckBox box in _cpuBoxes)
            {
                if ((bool)box.IsChecked)
                    mask += 0b1 << idx;
                else
                    mask += 0b0 << idx;
                idx++;
            }
            //If mask == 0 show an error
            if (mask == 0)
            {
                var result = MessageBox.Show("No CPU was selected", "Error Affinity", MessageBoxButton.OK, MessageBoxImage.Error);
                if (result == MessageBoxResult.OK)
                    return;
            }
            else
                _performer.game.SetAffinity(mask);
        }

        private void Clear_CPU_Click(object sender, RoutedEventArgs e)
        {
            foreach (CheckBox box in _cpuBoxes)
            {
                box.IsChecked = false;
            }
        }

        private void Reset_CPU_Click(object sender, RoutedEventArgs e)
        {
            foreach (CheckBox box in _cpuBoxes)
            {
                box.IsChecked = true;
            }
        }

        #endregion

        private void GfxTest_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)GfxTest.IsChecked)
            {
                if (_performer.game.GfxSettingsLow)
                    return;
                GameExtensions.GfxSetLow(_performer.game, true);
                _performer.game.GfxSettingsLow = true;
            }
            else
            {
                if (!_performer.game.GfxSettingsLow)
                    return;
                GameExtensions.GfxSetLow(_performer.game, false);
                _performer.game.GfxSettingsLow = false;
            }
        }
    }
}