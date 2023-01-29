using System;
using System.Windows;
using BardMusicPlayer.Jamboree;
using BardMusicPlayer.Jamboree.Events;

namespace BardMusicPlayer.Controls
{
    /// <summary>
    /// The song browser but much faster than the BMP 1.x had
    /// </summary>
    public partial class NetworkControl
    {
        public NetworkControl()
        {
            InitializeComponent();
            //NetEvents
            BmpJamboree.Instance.OnPartyCreated += Instance_PartyCreated;
            BmpJamboree.Instance.OnPartyDebugLog += Instance_PartyDebugLog;
        }

        private void Instance_PartyCreated(object sender, PartyCreatedEvent e)
        {
            var Token = e.Token;
            Dispatcher.BeginInvoke(new Action(() => PartyToken_Text.Text = Token));
        }

        private void Instance_PartyDebugLog(object sender, PartyDebugLogEvent e)
        {
            var logtext = e.LogString;
            Dispatcher.BeginInvoke(new Action(() => PartyLog_Text.Text += logtext));
        }

        private void Join_Click(object sender, RoutedEventArgs e)
        {
            var token = PartyToken_Text.Text;
            PartyToken_Text.Text = "Please wait...";
            BmpJamboree.Instance.JoinParty(token, 0, "Test Player"); // BmpMaestro.Instance.GetHostGame().PlayerName);
        }

        private void Leave_Click(object sender, RoutedEventArgs e)
        {
            BmpJamboree.Instance.LeaveParty();
        }
        private void ForcePlay_Click(object sender, RoutedEventArgs e)
        {
            BmpJamboree.Instance.SendPerformanceStart();
        }


    }
}
