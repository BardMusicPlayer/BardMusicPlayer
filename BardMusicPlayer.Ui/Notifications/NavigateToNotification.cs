using Stylet;

namespace BardMusicPlayer.Ui.Notifications
{
    public class NavigateToNotification
    {
        public NavigateToNotification(IScreen screen) { Screen = screen; }

        public IScreen Screen { get; }
    }
}