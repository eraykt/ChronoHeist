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
        
        private List<ICommand> _commandHistory = new List<ICommand>();

        private int _historyIndex;
        public int CurrentIndex => _historyIndex;
        public int MaxHistoryCount => _commandHistory.Count;

        public override void InitializeManager()
        {
            _input.GameActions.Undo.performed += HandleUndo;
        }
        
        public void RegisterCommand(ICommand command)
        {
            if (_historyIndex < _commandHistory.Count)
            {
                _commandHistory.RemoveRange(_historyIndex, _commandHistory.Count - _historyIndex);
            }

            _commandHistory.Add(command);
            _historyIndex++;
        }
        
        public void StepBack()
        {
            if (_historyIndex > 0)
            {
                _historyIndex--;
                _commandHistory[_historyIndex].Undo();
            }
        }
        
        public void StepForward()
        {
            if (_historyIndex < _commandHistory.Count)
            {
                _commandHistory[_historyIndex].Execute(true);
                _historyIndex++;
            }
        }

        public void ScrubToTurn(int targetIndex)
        {
            while (_historyIndex > targetIndex)
            {
                StepBack();
            }

            while (_historyIndex < targetIndex)
            {
                StepForward();
            }
        }

        public void ClearHistory()
        {
            _commandHistory.Clear();
            _historyIndex = 0;
        }

        private void HandleUndo(InputAction.CallbackContext callbackContext)
        {
            if (TurnManager.Instance.CurrentState == TurnManager.TurnState.PlayerTurn)
            {
                StepBack();
            }
        }

        private void OnDisable()
        {
            _input.GameActions.Undo.performed -= HandleUndo;
        }
    }
}