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
        
        public void Execute(bool replay)
        {
            for (int i = 0; i < _commands.Count; i++)
            {
                _commands[i].Execute(replay);
            }
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
