using System.Linq;
using BardMusicPlayer.Ui.Utilities;
using Stylet;

namespace BardMusicPlayer.Ui.ViewModels.Dialogue
{
    public class DialogueViewModel : Screen
    {
        public DialogueViewModel(params string[] inputHeaders)
        {
            Inputs = inputHeaders
                .Select(header => new InputViewModel(header))
                .ToBindableCollection();
        }

        public BindableCollection<InputViewModel> Inputs { get; set; }
    }
}