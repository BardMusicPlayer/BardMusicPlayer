using Stylet;

namespace BardMusicPlayer.Ui.ViewModels.Dialogue
{
    public class InputViewModel : Screen
    {
        private string _input;

        public InputViewModel(string header) { Header = header; }

        public string Header { get; }

        public string Input
        {
            get => _input;
            set => SetAndNotify(ref _input, value);
        }
    }
}