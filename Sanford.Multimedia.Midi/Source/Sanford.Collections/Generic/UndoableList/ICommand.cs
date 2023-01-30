namespace Sanford.Collections.Generic
{
    internal interface ICommand
    {
        void Execute();
        void Undo();
    }
}