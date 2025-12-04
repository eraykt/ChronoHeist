using ChronoHeist.Node;

namespace ChronoHeist
{
    public interface IMovable
    {
        public void MoveToNode(GameNode targetNode, System.Action onMoveComplete);
    }
}
