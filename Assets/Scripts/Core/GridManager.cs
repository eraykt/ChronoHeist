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
        private float _cellSize = 1f; // Oyun dünyasında kareler arası boşluk
        [SerializeField]
        private Vector2 _startOffset = Vector2.zero; // Gridin başlama pozisyonu
        [SerializeField]
        private Transform _container;

        [Header("Prefabs")]
        [SerializeField]
        private GameObject _circlePrefab;
        [SerializeField]
        private GameObject _linePrefab;

        private CellType[,] _runtimeGrid;

        private readonly Vector2Int[] _directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        private void Start()
        {
            if (_nodeDataSo != null)
            {
                GenerateGrid();
            }
            else
            {
                Debug.LogError("GridManager: NodeDataSO is missing!");
            }
        }

        private void GenerateGrid()
        {
            InitializeRuntimeGrid();

            // 2. Şimdi gridi gez ve objeleri oluştur
            for (int x = 0; x < _nodeDataSo.width; x++)
            {
                for (int y = 0; y < _nodeDataSo.height; y++)
                {
                    SpawnCell(x, y);
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
                instance = Instantiate(_circlePrefab, position, Quaternion.identity, _container);
                instance.name = $"Circle_{x}_{y}";
            }
            else if (type == CellType.Line)
            {
                instance = Instantiate(_linePrefab, position, Quaternion.identity, _container);
                instance.name = $"Line_{x}_{y}";

                float angle = GetSmartAngle(x, y);
                instance.transform.rotation = Quaternion.Euler(0, angle, 0);
            }
        }

        private float GetSmartAngle(int x, int y)
        {
            foreach (var dir in _directions)
            {
                int cx = x + dir.x;
                int cy = y + dir.y;
                if (IsInside(cx, cy) && _runtimeGrid[cx, cy] == CellType.Circle)
                {
                    return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                }
            }

            foreach (var dir in _directions)
            {
                int cx = x + dir.x;
                int cy = y + dir.y;
                if (IsInside(cx, cy) && _runtimeGrid[cx, cy] == CellType.Line)
                {
                    return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                }
            }
            return 0f;
        }

        private bool IsInside(int x, int y)
        {
            return x >= 0 && x < _nodeDataSo.width && y >= 0 && y < _nodeDataSo.height;
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