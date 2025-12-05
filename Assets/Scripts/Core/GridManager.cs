using System.Collections.Generic;
using ChronoHeist.Enemy;
using ChronoHeist.Node;
using ChronoHeist.Player;
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
        [SerializeField]
        private GameObject _playerPrefab;
        [SerializeField]
        private GameObject _enemyPrefab;
        [SerializeField]
        private GameObject _goldPrefab;
        [SerializeField]
        private GameObject _exitPrefab;

        private Transform _gridContainer;

        private GridCellData[,] _runtimeGrid;

        private Dictionary<Vector2Int, GameNode> _nodeLookup = new Dictionary<Vector2Int, GameNode>();

        public override void InitializeManager()
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

            GameObject gridContainer = new GameObject("GridContainer");
            _gridContainer = gridContainer.transform;

            for (int x = 0; x < _nodeDataSo.width; x++)
            {
                for (int y = 0; y < _nodeDataSo.height; y++)
                {
                    SpawnCell(x, y);
                }
            }

            ConnectNodesDfs();

            EventManager.TriggerEvent(new EventManager.OnGridGenerationFinished());
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
                CellStructure cell = _runtimeGrid[current.x, current.y].Structure;

                switch (cell)
                {
                    case CellStructure.Empty:
                        return;

                    case CellStructure.Node:
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

                    case CellStructure.Line:
                        current += dir;
                        continue;
                }
            }
        }

        private void InitializeRuntimeGrid()
        {
            _runtimeGrid = new GridCellData[_nodeDataSo.width, _nodeDataSo.height];

            for (int i = 0; i < _nodeDataSo.cellContainer.Count; i++)
            {
                int x = i / _nodeDataSo.width;
                int y = i % _nodeDataSo.height;

                _runtimeGrid[x, y] = _nodeDataSo.cellContainer[i];
            }
        }

        private void SpawnCell(int x, int y)
        {
            CellStructure structure = _runtimeGrid[x, y].Structure;

            if (structure == CellStructure.Empty) return;

            Vector3 position = new Vector3(x * _cellSize, 0.1f, y * _cellSize).ConvertVector() + (Vector3)_startOffset;

            GameObject instance = null;

            if (structure == CellStructure.Node)
            {
                instance = Instantiate(_circlePrefab, position, Quaternion.identity, _gridContainer);
                instance.name = $"Node_{x}_{y}";

                GameNode node = instance.GetComponent<GameNode>();
                Vector2Int index = new Vector2Int(x, y);

                node.index = index;
                _nodeLookup.Add(index, node);
            }
            else if (structure == CellStructure.Line)
            {
                instance = Instantiate(_linePrefab, position, Quaternion.identity, _gridContainer);
                instance.name = $"Line_{x}_{y}";

                float angle = CHRLibrary.GetLineAngle(x, y, _nodeDataSo.width, _nodeDataSo.height, _runtimeGrid);
                instance.transform.rotation = Quaternion.Euler(0, angle, 0);
            }

            if (_runtimeGrid[x, y].Contents.Count > 0)
            {
                if (_runtimeGrid[x, y].ContainsContent(CellContent.PlayerSpawn))
                {
                    var player = Instantiate(_playerPrefab).GetComponent<PlayerController>();
                    player.Initialize(instance?.GetComponent<GameNode>());
                }

                if (_runtimeGrid[x, y].ContainsContent(CellContent.EnemySpawn))
                {
                    var enemy = Instantiate(_enemyPrefab).GetComponent<EnemyController>();
                    enemy.Initialize(instance?.GetComponent<GameNode>());
                    TurnManager.Instance.RegisterEnemy(enemy);
                }

                if (_runtimeGrid[x, y].ContainsContent(CellContent.Gold))
                {
                    GameObject itemObj = Instantiate(_goldPrefab, position, Quaternion.identity, _gridContainer);

                    GameNode nodeScript = instance?.GetComponent<GameNode>();
                    nodeScript.OccupyingItem = itemObj;

                    GameManager.Instance.MaxGoldCount++;
                }
                
                if (_runtimeGrid[x, y].ContainsContent(CellContent.ExitPoint))
                {
                    Instantiate(_exitPrefab, position, Quaternion.identity, _gridContainer);

                    if (instance != null && instance.TryGetComponent(out GameNode gameNode))
                    {
                        gameNode.IsExit = true;
                    }
                }
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