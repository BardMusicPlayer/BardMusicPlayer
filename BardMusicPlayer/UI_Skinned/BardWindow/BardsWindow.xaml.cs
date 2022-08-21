using BardMusicPlayer.Ui.Globals.SkinContainer;
using System;
using System.Windows;
using System.Windows.Input;

namespace BardMusicPlayer.Ui.Skinned
{
    /// <summary>
    /// Only a container window
    /// </summary>
    public partial class BardsWindow : Window
    {
        public BardsWindow()
        {
            InitializeComponent();
            ApplySkin();
            SkinContainer.OnNewSkinLoaded += SkinContainer_OnNewSkinLoaded;
        }

        #region Skinning
        private void SkinContainer_OnNewSkinLoaded(object sender, EventArgs e)
        { ApplySkin(); }

        public void ApplySkin()
        {
            this.BARDS_TOP_LEFT.Fill = SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_TOP_LEFT_CORNER];
            this.BARDS_TOP_TILE.Fill = SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_TOP_TILE];
            this.BARDS_TOP_RIGHT.Fill = SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_TOP_RIGHT_CORNER];

            this.BARDS_BOTTOM_LEFT_CORNER.Fill = SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_BOTTOM_LEFT_CORNER];
            this.BARDS_BOTTOM_TILE.Fill = SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_BOTTOM_TILE];
            this.BARDS_BOTTOM_RIGHT_CORNER.Fill = SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_BOTTOM_RIGHT_CORNER];

            this.BARDS_LEFT_TILE.Fill = SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_LEFT_TILE];
            this.BARDS_RIGHT_TILE.Fill = SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_RIGHT_TILE];

            this.Close_Button.Background = SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_CLOSE_SELECTED];
            this.Close_Button.Background.Opacity = 0;

        }
        #endregion

        #region Titlebar functions and buttons
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
