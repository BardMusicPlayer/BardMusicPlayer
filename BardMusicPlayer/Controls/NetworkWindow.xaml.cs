using System;
using System.Windows;
using System.Windows.Controls;
using BardMusicPlayer.Jamboree;
using BardMusicPlayer.Jamboree.Events;

namespace BardMusicPlayer.Controls
{
    /// <summary>
    /// The songbrowser but much faster than the BMP 1.x had
    /// </summary>
    public partial class NetworkControl : UserControl
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
            string Token = e.Token;
            this.Dispatcher.BeginInvoke(new Action(() => PartyToken_Text.Text = Token));
        }

        private void Instance_PartyDebugLog(object sender, PartyDebugLogEvent e)
        {
            string logtext = e.LogString;
            this.Dispatcher.BeginInvoke(new Action(() => this.PartyLog_Text.Text = this.PartyLog_Text.Text + logtext));
        }

        private void Join_Click(object sender, RoutedEventArgs e)
        {
            string token = PartyToken_Text.Text;
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
