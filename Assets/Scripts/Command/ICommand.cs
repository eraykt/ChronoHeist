namespace ChronoHeist.Command
{
    public interface ICommand
    {
        void Execute(bool replay);
        void Undo();
    }
}