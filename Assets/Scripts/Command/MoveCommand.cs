using ChronoHeist.Core;
using ChronoHeist.Node;
using ChronoHeist.Player;

namespace ChronoHeist.Command
{
    public class MoveCommand : ICommand
    {
        private readonly IMovable _currentMoving;
        private readonly GameNode _fromNode;
        private readonly GameNode _toNode;
        private readonly System.Action _onComplete;
        private readonly System.Action _onUndoComplete;

        public MoveCommand(IMovable currentMoving, GameNode fromNode, GameNode toNode, System.Action onComplete, System.Action onUndoComplete)
        {
            _currentMoving = currentMoving;
            _fromNode = fromNode;
            _toNode = toNode;
            _onComplete = onComplete;
            _onUndoComplete = onUndoComplete;
        }

        public void Execute(bool replay)
        {
            _currentMoving.MoveToNode(_toNode, !replay ? _onComplete : null);
            
            if (_currentMoving is PlayerController)
            {
                GameManager.Instance.StepCount++;
            }
        }

        public void Undo()
        {
            _currentMoving.MoveToNode(_fromNode, _onUndoComplete);
            
            if (_currentMoving is PlayerController)
            {
                GameManager.Instance.StepCount--;
            }
        }
    }
}