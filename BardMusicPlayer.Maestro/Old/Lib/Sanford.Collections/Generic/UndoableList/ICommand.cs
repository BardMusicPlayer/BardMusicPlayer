namespace BardMusicPlayer.Maestro.Old.Lib.Sanford.Collections.Generic.UndoableList
{
    internal interface ICommand
    {
        void Execute();
        void Undo();
    }
}