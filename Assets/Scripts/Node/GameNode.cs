using System.Collections.Generic;
using UnityEngine;

namespace ChronoHeist.Node
{
    public class GameNode : MonoBehaviour
    {
        public Vector2Int index;
        
        public List<GameNode> neighbors = new List<GameNode>();
    }
}