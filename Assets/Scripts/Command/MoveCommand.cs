using ChronoHeist.Node;
using ChronoHeist.Player;

namespace ChronoHeist.Command
{
    public class MoveCommand : ICommand
    {
        private readonly PlayerController _movingPlayer;
        private readonly GameNode _fromNode;
        private readonly GameNode _toNode;
        private readonly System.Action _onComplete;

        public MoveCommand(PlayerController movingPlayer, GameNode fromNode, GameNode toNode, System.Action onComplete)
        {
            _movingPlayer = movingPlayer;
            _fromNode = fromNode;
            _toNode = toNode;
            _onComplete = onComplete;
        }

        public void Execute()
        {
            _movingPlayer.MoveToNode(_toNode, _onComplete);
        }

        public void Undo()
        {
            _movingPlayer.MoveToNode(_fromNode, _onComplete);
        }
    }
}