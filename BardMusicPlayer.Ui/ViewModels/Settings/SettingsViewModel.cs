using System.Collections.Generic;
using Stylet;
using StyletIoC;

namespace BardMusicPlayer.Ui.ViewModels.Settings
{
    public class SettingsViewModel : Conductor<IScreen>
    {
        public SettingsViewModel(IContainer ioc)
        {
            About    = ioc.Get<AboutPageViewModel>();
            Advanced = ioc.Get<AdvancedPageViewModel>();
            General  = ioc.Get<GeneralPageViewModel>();

            Pages = new()
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
    }
}