using System.Collections.Generic;
using BardMusicPlayer.Ui.Notifications;
using Stylet;
using StyletIoC;

namespace BardMusicPlayer.Ui.ViewModels.Settings
{
    public class SettingsViewModel : Conductor<IScreen>
    {
        private readonly IEventAggregator _events;

        public SettingsViewModel(IContainer ioc, IEventAggregator events)
        {
            _events = events;

            About    = ioc.Get<AboutPageViewModel>();
            Advanced = ioc.Get<AdvancedPageViewModel>();
            General  = ioc.Get<GeneralPageViewModel>();

            Pages = new Dictionary<IScreen, string>()
            {
                [General]  = "General",
                [About]    = "About",
                [Advanced] = "Advanced Settings"
            };
        }

        public Dictionary<IScreen, string> Pages { get; }

        public IScreen About { get; }

        public IScreen Advanced { get; }

        public IScreen General { get; }

        public void GoBack() { _events.Publish(new NavigateBackNotification()); }
    }
}