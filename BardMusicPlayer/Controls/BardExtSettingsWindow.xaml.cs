/*
 * Copyright(c) 2023 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using BardMusicPlayer.DalamudBridge;
using BardMusicPlayer.Maestro.Old;
using BardMusicPlayer.Maestro.Old.Performance;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Quotidian.Structs;

namespace BardMusicPlayer.Controls;

/// <summary>
/// Interaction logic for BardExtSettingsWindow.xaml
/// </summary>
public sealed partial class BardExtSettingsWindow
{
    private readonly Performer? _performer;
    private readonly List<CheckBox> _cpuBoxes = new();

    public BardExtSettingsWindow(Performer? performer)
    {
        _performer = performer;
        InitializeComponent();
        Title = "Settings for: " + _performer?.PlayerName;

        SongTitlePostType.SelectedIndex = 0;
        SongTitleChatType.SelectedIndex = 0;
        ChatType.SelectedIndex          = 0;

        //Get the values for the song parsing bard
        var tpBard = BmpMaestro.Instance.GetSongTitleParsingBard();
        if (tpBard.Value != null)
        {
            if (_performer != null && tpBard.Value.game.Pid == _performer.game.Pid)
            {
                SongTitleChatPrefix.Text = tpBard.Key.prefix;

                if (tpBard.Key.channelType.ChannelCode == ChatMessageChannelType.Say.ChannelCode)
                    SongTitleChatType.SelectedIndex = 0;
                else if (tpBard.Key.channelType.ChannelCode == ChatMessageChannelType.Yell.ChannelCode)
                    SongTitleChatType.SelectedIndex = 1;
                else if (tpBard.Key.channelType.ChannelCode == ChatMessageChannelType.Party.ChannelCode)
                    SongTitleChatType.SelectedIndex = 2;

                SongTitlePostType.SelectedIndex = tpBard.Key.channelType.Equals(ChatMessageChannelType.None) ? 0 : 1;
            }
        }

        LyricsTrackNr.Value = performer?.SingerTrackNr.ToString();
        GfxTest.IsChecked   = _performer?.game.GfxSettingsLow;
        SoundOn.IsChecked   = _performer?.game.SoundOn;
        PopulateCpuTab();
    }

    private void SongTitle_Post_Type_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var chanType = SongTitleChatType.SelectedIndex switch
        {
            0 => ChatMessageChannelType.Say,
            1 => ChatMessageChannelType.Yell,
            2 => ChatMessageChannelType.Party,
            _ => ChatMessageChannelType.None
        };

        switch (SongTitlePostType.SelectedIndex)
        {
            case 0:
                BmpMaestro.Instance.SetSongTitleParsingBard(ChatMessageChannelType.None, "", null);
                break;
            case 1:
                BmpMaestro.Instance.SetSongTitleParsingBard(chanType, SongTitleChatPrefix.Text, _performer);
                break;
        }
    }

    private void PostSongTitle_Click(object sender, RoutedEventArgs e)
    {
        if (_performer?.SongName == "")
            return;

        var chanType = SongTitleChatType.SelectedIndex switch
        {
            0 => ChatMessageChannelType.Say,
            1 => ChatMessageChannelType.Yell,
            2 => ChatMessageChannelType.Party,
            _ => ChatMessageChannelType.None
        };

        if (_performer?.SongName != null)
        {
            var songName = $"{SongTitleChatPrefix.Text} {_performer.SongName.Replace('_', ' ')} {SongTitleChatPrefix.Text}";
            _performer.game.SendText(chanType, songName);
        }
    }

    private void ChatInputText_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Return)
        {
            var chanType = ChatType.SelectedIndex switch
            {
                0 => ChatMessageChannelType.Say,
                1 => ChatMessageChannelType.Yell,
                2 => ChatMessageChannelType.Shout,
                3 => ChatMessageChannelType.Party,
                4 => ChatMessageChannelType.FreeCompany,
                _ => ChatMessageChannelType.None
            };
            var text = new string(ChatInputText.Text.ToCharArray());
            _performer?.game.SendText(chanType, text);
            ChatInputText.Text = "";
        }
    }

    private void Lyrics_TrackNr_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (sender is NumericUpDown ctl) ctl.OnValueChanged += Lyrics_TrackNr_OnValueChanged;
    }

    private void Lyrics_TrackNr_OnValueChanged(object? sender, int s)
    {
        if (_performer != null) _performer.SingerTrackNr    =  s;
        if (sender is NumericUpDown ctl) ctl.OnValueChanged -= Lyrics_TrackNr_OnValueChanged;
    }

    #region CPU-Tab
    private void PopulateCpuTab()
    {
        //Get the our application's process.
        var _ = _performer?.game.Process;

        //Get the processor count of our machine.
        var cpuCount = Environment.ProcessorCount;
        if (_performer != null)
        {
            var affinityMask = (long)_performer.game.GetAffinity();

            var res = (int)Math.Ceiling(cpuCount / (double)3);
            var idx = 1;
            for (var col = 0; col != 3; col++)
            {
                CpuDisplay.ColumnDefinitions.Add(new ColumnDefinition());
                
                for (var i = 0; i != res + 1; i++)
                {
                    if (idx == cpuCount+1)
                        break;
                    if (CpuDisplay.RowDefinitions.Count < res +1)
                        CpuDisplay.RowDefinitions.Add(new RowDefinition());
                    var uc = new CheckBox
                    {
                        Name       = "CPU" + idx,
                        Content    = "CPU" + idx,
                        Foreground = !BmpPigeonhole.Instance.DarkStyle ? Brushes.Black : Brushes.White
                    };
                    if ((affinityMask & (1 << idx-1)) > 0) //-1 since we count at 1
                        uc.IsChecked = true;
                    _cpuBoxes.Add(uc);
                    CpuDisplay.Children.Add(uc);
                    Grid.SetRow(uc, i);
                    Grid.SetColumn(uc, CpuDisplay.ColumnDefinitions.Count - 1);
                    idx++;
                }
            }
        }
    }

    private void Save_CPU_Click(object sender, RoutedEventArgs e)
    {
        ulong mask = 0;
        var idx = 0;
        foreach (var box in _cpuBoxes)
        {
            if (box.IsChecked != null && (bool)box.IsChecked)
                mask += 0b1ul << idx;
            else
                mask += 0b0ul << idx;
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
            _performer?.game.SetAffinity((long)mask);
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
        if ((bool)GfxTest.IsChecked!)
        {
            if (_performer!.game.GfxSettingsLow)
                return;
            if (!_performer.game.GfxSetLow(true).Result)
                _performer.game.SetGfxLow();
            _performer.game.GfxSettingsLow = true;
        }
        else
        {
            if (!_performer!.game.GfxSettingsLow)
                return;
            if(!_performer.game.GfxSetLow(false).Result)
                _performer.game.RestoreGfxSettings();
            _performer.game.GfxSettingsLow = false;
        }
    }

    private void SoundOn_Checked(object sender, RoutedEventArgs e)
    {
        if ((bool)SoundOn.IsChecked!)
        {
            if (_performer!.game.SoundOn)
                return;
            if (!_performer.game.SetSoundOnOff(true).Result)
                _performer.game.SetSoundOnOffLegacy(true);
            _performer.game.SoundOn = true;
        }
        else
        {
            if (!_performer!.game.SoundOn)
                return;
            if (!_performer.game.SetSoundOnOff(false).Result)
                _performer.game.SetSoundOnOffLegacy(false);
            _performer.game.SoundOn = false;
        }
    }
}