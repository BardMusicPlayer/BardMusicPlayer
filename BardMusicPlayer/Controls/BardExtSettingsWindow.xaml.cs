using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BardMusicPlayer.DalamudBridge;
using BardMusicPlayer.Maestro;
using BardMusicPlayer.Maestro.Performance;
using BardMusicPlayer.Quotidian.Structs;

namespace BardMusicPlayer.Controls;

/// <summary>
/// Interaction logic for BardExtSettingsWindow.xaml
/// </summary>
public sealed partial class BardExtSettingsWindow
{
    private Performer _performer;
    private List<CheckBox> _cpuBoxes = new();

    public BardExtSettingsWindow(Performer performer)
    {
        _performer = performer;
        InitializeComponent();
        Title = "Settings for: " + _performer.PlayerName;

        Songtitle_Post_Type.SelectedIndex = 0;
        Songtitle_Chat_Type.SelectedIndex = 0;
        Chat_Type.SelectedIndex           = 0;

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
                else if (tpBard.Key.channelType.ChannelCode == ChatMessageChannelType.Party.ChannelCode)
                    Songtitle_Chat_Type.SelectedIndex = 2;

                Songtitle_Post_Type.SelectedIndex = tpBard.Key.channelType.Equals(ChatMessageChannelType.None) ? 0 : 1;
            }
        }

        Lyrics_TrackNr.Value = performer.SingerTrackNr.ToString();
        GfxTest.IsChecked    = _performer.game.GfxSettingsLow;
        PopulateCPUTab();
    }

    private void SongTitle_Post_Type_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var chanType = Songtitle_Chat_Type.SelectedIndex switch
        {
            0 => ChatMessageChannelType.Say,
            1 => ChatMessageChannelType.Yell,
            2 => ChatMessageChannelType.Party,
            _ => ChatMessageChannelType.None
        };

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

        var chanType = Songtitle_Chat_Type.SelectedIndex switch
        {
            0 => ChatMessageChannelType.Say,
            1 => ChatMessageChannelType.Yell,
            2 => ChatMessageChannelType.Party,
            _ => ChatMessageChannelType.None
        };
        var songName = $"{Songtitle_Chat_Prefix.Text} {_performer.SongName} {Songtitle_Chat_Prefix.Text}";
        _performer.game.SendText(chanType, songName);
    }

    private void ChatInputText_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Return)
        {
            var chanType = Chat_Type.SelectedIndex switch
            {
                0 => ChatMessageChannelType.Say,
                1 => ChatMessageChannelType.Yell,
                2 => ChatMessageChannelType.Shout,
                3 => ChatMessageChannelType.Party,
                4 => ChatMessageChannelType.FC,
                _ => ChatMessageChannelType.None
            };
            var text = new string(ChatInputText.Text.ToCharArray());
            _performer.game.SendText(chanType, text);
            ChatInputText.Text = "";
        }
    }

    private void Lyrics_TrackNr_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (sender is NumericUpDown ctl) ctl.OnValueChanged += Lyrics_TrackNr_OnValueChanged;
    }

    private void Lyrics_TrackNr_OnValueChanged(object sender, int s)
    {
        _performer.SingerTrackNr = s;
        if (sender is NumericUpDown ctl) ctl.OnValueChanged -= Lyrics_TrackNr_OnValueChanged;
    }

    #region CPU-Tab
    private void PopulateCPUTab()
    {
        //Get the our application's process.
        var process = _performer.game.Process;

        //Get the processor count of our machine.
        var cpuCount = Environment.ProcessorCount;
        var AffinityMask = (long)_performer.game.GetAffinity();

        var res = (int)Math.Ceiling(cpuCount / (double)3);
        var idx = 1;
        for (var col = 0; col != 3; col++)
        {
            CPUDisplay.ColumnDefinitions.Add(new ColumnDefinition());
                
            for (var i = 0; i != res + 1; i++)
            {
                if (idx == cpuCount+1)
                    break;
                if (CPUDisplay.RowDefinitions.Count < res +1)
                    CPUDisplay.RowDefinitions.Add(new RowDefinition());
                var uc = new CheckBox
                {
                    Name    = "CPU" + idx,
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
        var idx = 0;
        foreach (var box in _cpuBoxes)
        {
            if (box.IsChecked != null && (bool)box.IsChecked)
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
        foreach (var box in _cpuBoxes)
        {
            box.IsChecked = false;
        }
    }

    private void Reset_CPU_Click(object sender, RoutedEventArgs e)
    {
        foreach (var box in _cpuBoxes)
        {
            box.IsChecked = true;
        }
    }

    #endregion

    private void GfxTest_Checked(object sender, RoutedEventArgs e)
    {
        if (GfxTest.IsChecked != null && (bool)GfxTest.IsChecked)
        {
            if (_performer.game.GfxSettingsLow)
                return;
            _performer.game.GfxSetLow(true);
            _performer.game.GfxSettingsLow = true;
        }
        else
        {
            if (!_performer.game.GfxSettingsLow)
                return;
            _performer.game.GfxSetLow(false);
            _performer.game.GfxSettingsLow = false;
        }
    }
}