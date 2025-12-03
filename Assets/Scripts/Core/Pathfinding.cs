using System.Collections.Generic;
using ChronoHeist.Node;

namespace ChronoHeist.Core
{
    public static class Pathfinding
    {
        public static List<GameNode> GetPath(GameNode start, GameNode end)
        {
            if (start == null || end == null) return new List<GameNode>();
            if (start == end)  return new List<GameNode>();

            Queue<GameNode>  queue = new Queue<GameNode>();
            Dictionary<GameNode, GameNode> map = new Dictionary<GameNode, GameNode>();
            map[start] = null;
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                
                if (current == end)
                {
                    return ConstructPath(map, start, end);
                }

                foreach (GameNode currentNeighbor in current.neighbors)
                {
                    if (!map.ContainsKey(currentNeighbor))
                    {
                        queue.Enqueue(currentNeighbor);
                        map[currentNeighbor] = current;
                    }
                }
            }
            
            return new List<GameNode>();
        }

        private static List<GameNode> ConstructPath(Dictionary<GameNode, GameNode> map, GameNode start, GameNode end)
        {
            List<GameNode> path = new List<GameNode>();

            var current = end;

            while (current != start)
            {
                path.Add(current);
                current = map[current];
            }
            
            path.Reverse();
            return path;
        }
    }
}