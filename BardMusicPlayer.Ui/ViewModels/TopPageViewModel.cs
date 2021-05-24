using Stylet;
using StyletIoC;

namespace BardMusicPlayer.Ui.ViewModels
{
    public class TopPageViewModel : Screen
    {
        public TopPageViewModel(IContainer ioc) { Instruments = ioc.Get<BardViewModel>(); }

        public BardViewModel Instruments { get; }
    }
}