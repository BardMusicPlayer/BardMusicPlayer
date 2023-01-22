namespace Sanford.Multimedia.Midi.Sanford.Collections.Generic.UndoableList
{
    internal interface ICommand
    {
        void Execute();
        void Undo();
    }
}
