using System.Collections.Generic;

namespace ChronoHeist.Command
{
    public class TurnBatchCommand : ICommand
    {
        private List<ICommand> _commands = new List<ICommand>();

        public void AddCommand(ICommand command)
        {
            _commands.Add(command);
        }
        
        public void Execute()
        {
                
        }
        
        public void Undo()
        {
            for (int i = _commands.Count - 1; i >= 0; i--)
            {
                _commands[i].Undo();
            }    
        }
    }
}
