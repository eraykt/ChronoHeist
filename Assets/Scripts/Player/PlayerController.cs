using ChronoHeist.Node;
using UnityEngine;

namespace ChronoHeist.Player
{
    public class PlayerController : MonoBehaviour
    {
        public GameNode CurrentNode { get; private set; }

        public void Initialize(GameNode startingNode)
        {
            CurrentNode = startingNode;
            transform.position = new Vector3(startingNode.transform.position.x, 0.0f, startingNode.transform.position.z);
            
            EventManager.TriggerEvent(new EventManager.OnPlayerInitialized(this));
        }

        public void MoveToNode(GameNode targetNode, System.Action onMoveEnded = null)
        {
            CurrentNode = targetNode;
            transform.position = new Vector3(targetNode.transform.position.x, 0.0f, targetNode.transform.position.z);
            onMoveEnded?.Invoke();
        }
    }
}