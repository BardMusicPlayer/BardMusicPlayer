using Stylet;

namespace BardMusicPlayer.Ui.ViewModels.Dialogue
{
    public class InputViewModel : Screen
    {
        public InputViewModel(string header) { Header = header; }

        public string Header { get; }

        public string? Input { get; set; }
    }
}