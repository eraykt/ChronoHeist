using System.Collections.Generic;
using ChronoHeist.Core;
using ChronoHeist.Node;
using UnityEngine;

namespace ChronoHeist.Enemy
{
    public class EnemyController : MonoBehaviour, IMovable
    {
        public GameNode CurrentNode { get; private set; }
        
        public void Initialize(GameNode startNode)
        {
            CurrentNode = startNode;
            transform.position = new Vector3(startNode.transform.position.x, 0.0f, startNode.transform.position.z);
        }

        public GameNode CalculatePath(GameNode targetNode)
        {
            List<GameNode> path = Pathfinding.GetPath(CurrentNode, targetNode);
            if (path != null && path.Count > 0)
            {
                GameNode nextStep = path[0];
                return nextStep;                
            }
            return null;
        }
        
        public void MoveToNode(GameNode targetNode, System.Action onMoveEnded)
        {
            CurrentNode = targetNode;
            transform.position = new Vector3(targetNode.transform.position.x, 0.0f, targetNode.transform.position.z);
            onMoveEnded?.Invoke();
        }
    }
}