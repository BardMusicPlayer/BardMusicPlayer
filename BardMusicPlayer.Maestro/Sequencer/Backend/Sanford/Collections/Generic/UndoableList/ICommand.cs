namespace BardMusicPlayer.Maestro.Sequencer.Backend.Sanford.Collections.Generic.UndoableList
{
    internal interface ICommand
    {
        void Execute();
        void Undo();
    }
}