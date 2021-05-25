/*
 * Copyright(c) 2021 MoogleTroupe, trotlinebeercan
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Windows;
using System.Windows.Controls;
using BardMusicPlayer.Ui.Notifications;
using BardMusicPlayer.Ui.ViewModels.Settings;
using BardMusicPlayer.Ui.ViewModels.SongEditor;
using Stylet;
using StyletIoC;

namespace BardMusicPlayer.Ui.ViewModels
{
    public class MainViewModel : Conductor<IScreen>.StackNavigation,
        IHandle<NavigateBackNotification>,
        IHandle<EditSongNotification>
    {
        public MainViewModel(IContainer ioc, IEventAggregator events)
        {
            events.Subscribe(this);

            TopPage  = ioc.Get<TopPageViewModel>();
            Bards    = ioc.Get<BardViewModel>();
            Settings = ioc.Get<SettingsViewModel>();

            ActiveItem = TopPage;
        }

        public IScreen Bards { get; }

        public IScreen Settings { get; }

        public IScreen TopPage { get; }

        public void Handle(EditSongNotification message)
        {
            var songEditor = new SongEditorViewModel(message.Song);
            ActivateItem(songEditor);
        }

        public void Handle(NavigateBackNotification message) { GoBack(); }

        public void Navigate(object sender, RoutedEventArgs e)
        {
            if (sender is Button { Tag: IScreen screen }) ActivateItem(screen);
        }
    }
}