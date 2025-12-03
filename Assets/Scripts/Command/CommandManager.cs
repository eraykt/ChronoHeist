using System.Collections.Generic;
using ChronoHeist.Command;
using ChronoHeist.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ChronoHeist.Core
{
    public class CommandManager : Manager<CommandManager>
    {
        [SerializeField]
        private InputReader _input;
        
        private Stack<ICommand> _commandHistory = new Stack<ICommand>();

        public override void InitializeManager()
        {
            _input.GameActions.Undo.performed += HandleUndo;
        }
        
        public void ExecuteCommand(ICommand command)
        {
            command.Execute();
            _commandHistory.Push(command);
        }

        public void UndoLastCommand()
        {
            if (_commandHistory.Count > 0)
            {
                var command = _commandHistory.Pop();
                command.Undo();
            }
        }

        public void ClearHistory()
        {
            _commandHistory.Clear();
        }

        private void HandleUndo(InputAction.CallbackContext callbackContext)
        {
            if (TurnManager.Instance.CurrentState == TurnManager.TurnState.PlayerTurn)
            {
                UndoLastCommand();
            }
        }

        private void OnDisable()
        {
            _input.GameActions.Undo.performed -= HandleUndo;
        }
    }
}