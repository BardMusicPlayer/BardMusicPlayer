using BardMusicPlayer.Ui.Globals.SkinContainer;
using System;
using System.Windows;
using System.Windows.Input;
using BardMusicPlayer.Jamboree;
using BardMusicPlayer.Jamboree.Events;

namespace BardMusicPlayer.Ui.Skinned
{
    /// <summary>
    /// Interaktionslogik für BardsWindow.xaml
    /// </summary>
    public partial class NetworkPlayWindow : Window
    {
        public NetworkPlayWindow()
        {
            InitializeComponent();
            ApplySkin();
            SkinContainer.OnNewSkinLoaded += SkinContainer_OnNewSkinLoaded;
        }
#region Loading window, apply skindata and buttons
        private void SkinContainer_OnNewSkinLoaded(object sender, EventArgs e)
        { ApplySkin(); }

        public void ApplySkin()
        {
            this.NETWORK_TOP_LEFT.Fill = SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_TOP_LEFT_CORNER];
            this.NETWORK_TOP_TILE.Fill = SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_TOP_TILE];
            this.NETWORK_TOP_RIGHT.Fill = SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_TOP_RIGHT_CORNER];

            this.NETWORK_BOTTOM_LEFT_CORNER.Fill = SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_BOTTOM_LEFT_CORNER];
            this.NETWORK_BOTTOM_TILE.Fill = SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_BOTTOM_TILE];
            this.NETWORK_BOTTOM_RIGHT_CORNER.Fill = SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_BOTTOM_RIGHT_CORNER];

            this.NETWORK_LEFT_TILE.Fill = SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_LEFT_TILE];
            this.NETWORK_RIGHT_TILE.Fill = SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_RIGHT_TILE];

            this.Close_Button.Background = SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_CLOSE_SELECTED];
            this.Close_Button.Background.Opacity = 0;

        }
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Close_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }
        private void Close_Button_Down(object sender, MouseButtonEventArgs e)
        {
            this.Close_Button.Background.Opacity = 1;
        }
        private void Close_Button_Up(object sender, MouseButtonEventArgs e)
        {
            this.Close_Button.Background.Opacity = 0;
        }
#endregion



    }
}