using System.Collections.Generic;
using ChronoHeist.Node;
using UnityEngine;

namespace ChronoHeist.Core
{
    public class GridManager : Manager<GridManager>
    {
        [Header("Config")]
        [SerializeField]
        private NodeDataSO _nodeDataSo;

        [Header("Settings")]
        [SerializeField]
        private float _cellSize = 1f;
        [SerializeField]
        private Vector2 _startOffset = Vector2.zero;

        [Header("Prefabs")]
        [SerializeField]
        private GameObject _circlePrefab;
        [SerializeField]
        private GameObject _linePrefab;

        private CellType[,] _runtimeGrid;

        private Dictionary<Vector2Int, GameNode> _nodeLookup = new Dictionary<Vector2Int, GameNode>();

        private void Start()
        {
            if (_nodeDataSo != null)
            {
                GenerateGrid();
            }
            else
            {
                Logger.Error(this, "Node Data SO is null");
            }
        }

        private void GenerateGrid()
        {
            _nodeLookup.Clear();

            InitializeRuntimeGrid();

            for (int x = 0; x < _nodeDataSo.width; x++)
            {
                for (int y = 0; y < _nodeDataSo.height; y++)
                {
                    SpawnCell(x, y);
                }
            }

            ConnectNodesDfs();
        }

        private void ConnectNodesDfs()
        {
            foreach (var kvp in _nodeLookup)
            {
                Vector2Int index = kvp.Key;
                GameNode node = kvp.Value;

                foreach (Vector2Int dir in CHRLibrary.Directions)
                {
                    TraceAndConnect(node, index, dir);
                }
            }
        }

        private void TraceAndConnect(GameNode node, Vector2Int index, Vector2Int dir)
        {
            Vector2Int current = index + dir;

            while (CHRLibrary.IsInsideGrid(current.x, current.y, _nodeDataSo.width, _nodeDataSo.height))
            {
                CellType cell = _runtimeGrid[current.x, current.y];

                switch (cell)
                {
                    case CellType.Empty:
                        return;

                    case CellType.Circle:
                        if (_nodeLookup.TryGetValue(current, out GameNode neighbor))
                        {
                            if (!node.neighbors.Contains(neighbor))
                            {
                                node.neighbors.Add(neighbor);
                            }

                            if (!neighbor.neighbors.Contains(node))
                            {
                                neighbor.neighbors.Add(node);
                            }
                        }
                        return;

                    case CellType.Line:
                        current += dir;
                        continue;
                }
            }
        }

        private void InitializeRuntimeGrid()
        {
            _runtimeGrid = new CellType[_nodeDataSo.width, _nodeDataSo.height];

            for (int i = 0; i < _nodeDataSo.cellContainer.Count; i++)
            {
                int x = i / _nodeDataSo.width;
                int y = i % _nodeDataSo.height;

                _runtimeGrid[x, y] = _nodeDataSo.cellContainer[i];
            }
        }

        private void SpawnCell(int x, int y)
        {
            CellType type = _runtimeGrid[x, y];

            if (type == CellType.Empty) return;

            Vector3 position = new Vector3(x * _cellSize, 0.1f, y * _cellSize).ConvertVector() + (Vector3)_startOffset;

            GameObject instance = null;

            if (type == CellType.Circle)
            {
                instance = Instantiate(_circlePrefab, position, Quaternion.identity, transform);
                instance.name = $"Circle_{x}_{y}";

                GameNode node = instance.GetComponent<GameNode>();
                Vector2Int index = new Vector2Int(x, y);

                node.index = index;
                _nodeLookup.Add(index, node);
            }
            else if (type == CellType.Line)
            {
                instance = Instantiate(_linePrefab, position, Quaternion.identity, transform);
                instance.name = $"Line_{x}_{y}";

                float angle = CHRLibrary.GetLineAngel(x, y, _nodeDataSo.width, _nodeDataSo.height, _runtimeGrid);
                instance.transform.rotation = Quaternion.Euler(0, angle, 0);
            }
        }


        private void OnDrawGizmos()
        {
            if (_nodeDataSo == null) return;

            Gizmos.color = new Color(0, 1, 1, 0.3f);

            for (int x = 0; x < _nodeDataSo.width; x++)
            {
                for (int y = 0; y < _nodeDataSo.height; y++)
                {
                    Vector3 pos = new Vector3(x * _cellSize, 0, y * _cellSize) + (Vector3)_startOffset;
                    Gizmos.DrawWireCube(pos, Vector3.one * _cellSize * 0.9f);
                }
            }
        }
    }
}