#if UNITY_EDITOR

using System.Collections.Generic;
using ChronoHeist.Node;
using UnityEngine;
using UnityEditor;

public class NodeEditor : EditorWindow
{
    private int _gridWidth = 10;
    private int _gridHeight = 10;
    private float _cellSize = 50f;

    private GridCellData[,] _gridData;
    private NodeDataSO _currentLevelData;

    [MenuItem("Tools/Level Editor")]
    public static void ShowWindow()
    {
        var a = GetWindow<NodeEditor>("Level Editor");
    }

    private void OnEnable()
    {
        InitializeGridData();
    }

    private void InitializeGridData(bool force = false)
    {
        if (_gridData == null || _gridData.GetLength(0) != _gridWidth || _gridData.GetLength(1) != _gridHeight || force)
        {
            _gridData = new GridCellData[_gridWidth, _gridHeight];

            for (int x = 0; x < _gridWidth; x++)
            {
                for (int y = 0; y < _gridHeight; y++)
                {
                    _gridData[x, y] = new GridCellData();
                }
            }
        }
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        {
            GUILayout.Label("Settings:", EditorStyles.miniLabel);
            _gridWidth = EditorGUILayout.IntField(_gridWidth, EditorStyles.toolbarTextField, GUILayout.Width(40));
            GUILayout.Label("x", EditorStyles.miniLabel);
            _gridHeight = EditorGUILayout.IntField(_gridHeight, EditorStyles.toolbarTextField, GUILayout.Width(40));
            GUILayout.Label("y", EditorStyles.miniLabel);

            if (GUILayout.Button("Reset", EditorStyles.toolbarButton))
            {
                InitializeGridData(true);
                float calculatedWidth = (_gridWidth * _cellSize) + 40;
                float calculatedHeight = (_gridHeight * _cellSize) + 40 + 25;
                Vector2 newSize = new Vector2(calculatedWidth, calculatedHeight);
                this.minSize = newSize;
                this.maxSize = newSize;
            }
            if (GUILayout.Button("Save", EditorStyles.toolbarButton)) SaveLevel();
            if (GUILayout.Button("Load", EditorStyles.toolbarButton)) LoadLevel();
        }
        GUILayout.EndHorizontal();

        InitializeGridData();

        Rect workArea = GUILayoutUtility.GetRect(
            (_gridWidth * _cellSize) + 40, (_gridHeight * _cellSize) + 40,
            GUILayout.ExpandWidth(false),
            GUILayout.ExpandHeight(false)
        );

        EditorGUI.DrawRect(workArea, new Color(0.15f, 0.15f, 0.15f));

        DrawGridSection(workArea);
    }

    private void DrawGridSection(Rect workArea)
    {
        float contentWidth = _gridWidth * _cellSize + 40;
        float contentHeight = _gridHeight * _cellSize + 40;
        Rect viewRect = new Rect(0, 0, contentWidth, contentHeight);

        GUILayout.BeginArea(workArea);

        HandleGridEvents(viewRect);
        DrawGrid(viewRect);

        GUILayout.EndArea();
    }

    private void DrawGrid(Rect viewRect)
    {
        float startX = 20;
        float startY = 20;

        for (int x = 0; x < _gridWidth; x++)
        {
            for (int y = 0; y < _gridHeight; y++)
            {
                Rect cellRect = new Rect(startX + x * _cellSize, startY + y * _cellSize, _cellSize, _cellSize);

                Handles.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                Handles.DrawWireDisc(cellRect.center, Vector3.forward, 0);
                EditorGUI.DrawRect(new Rect(cellRect.x, cellRect.y, _cellSize, 1), Color.grey);
                EditorGUI.DrawRect(new Rect(cellRect.x, cellRect.y, 1, _cellSize), Color.grey);

                GenerateCellVisual(x, y, cellRect);
            }
        }

        float gridRealWidth = _gridWidth * _cellSize;
        float gridRealHeight = _gridHeight * _cellSize;
        EditorGUI.DrawRect(new Rect(startX + gridRealWidth, startY, 1, gridRealHeight), Color.grey);
        EditorGUI.DrawRect(new Rect(startX, startY + gridRealHeight, gridRealWidth, 1), Color.grey);
    }

    private void GenerateCellVisual(int x, int y, Rect cellRect)
    {
        GridCellData type = _gridData[x, y];

        switch (type.Structure)
        {
            case CellStructure.Node:
                Handles.color = Color.green;
                Handles.DrawSolidDisc(cellRect.center, Vector3.forward, _cellSize * 0.35f);
                break;

            case CellStructure.Line:
                float angle = CHRLibrary.GetLineAngel(x, y, _gridWidth, _gridHeight, _gridData);

                Matrix4x4 originalMatrix = GUI.matrix;
                GUIUtility.RotateAroundPivot(angle, cellRect.center);

                Rect lineRect = new Rect(cellRect.x + 2, cellRect.y + (_cellSize / 2) - (4f / 2), _cellSize, 4f);
                EditorGUI.DrawRect(lineRect, new Color(1f, 0.4f, 0.4f));

                GUI.matrix = originalMatrix;
                break;
        }

        if (type.Contents.Count > 0)
        {
            // Merkeze göre ne kadar köşeye gideceğiz?
            float offset = _cellSize * 0.25f;

            foreach (var content in type.Contents)
            {
                Vector2 drawPos = cellRect.center;
                string labelText = "";
                Color labelColor = Color.white;

                switch (content)
                {
                    case CellContent.PlayerSpawn:
                        labelText = "P";
                        labelColor = Color.cyan;
                        drawPos += new Vector2(-offset, -offset);
                        break;

                    case CellContent.EnemySpawn:
                        labelText = "E";
                        labelColor = Color.red;
                        drawPos += new Vector2(offset, -offset);
                        break;
                }
                
                Handles.color = new Color(0, 0, 0, 0.8f);
                Handles.DrawSolidDisc(drawPos, Vector3.forward, 8f);

                GUIStyle style = new GUIStyle();
                style.normal.textColor = labelColor;
                style.fontSize = 11;
                style.fontStyle = FontStyle.Bold;
                style.alignment = TextAnchor.MiddleCenter;

                float iconSize = 16f;
                Rect labelRect = new Rect(drawPos.x - (iconSize/2), drawPos.y - (iconSize/2), iconSize, iconSize);
                GUI.Label(labelRect, labelText, style);
            }
        }
    }

    private void HandleGridEvents(Rect viewRect)
        {
            Event e = Event.current;
            float startX = 20;
            float startY = 20;

            if (viewRect.Contains(e.mousePosition))
            {
                Vector2 relativeMousePos = e.mousePosition - new Vector2(startX, startY);
                int x = Mathf.FloorToInt(relativeMousePos.x / _cellSize);
                int y = Mathf.FloorToInt(relativeMousePos.y / _cellSize);

                if (x >= 0 && x < _gridWidth && y >= 0 && y < _gridHeight)
                {
                    if (e.type == EventType.MouseDown && e.button == 0)
                    {
                        _gridData[x, y].Structure = (_gridData[x, y].Structure == CellStructure.Node) ? CellStructure.Empty : CellStructure.Node;
                        e.Use();
                        Repaint();
                    }

                    if ((e.type == EventType.MouseDrag || e.type == EventType.MouseDown) && e.button == 1)
                    {
                        _gridData[x, y].Structure = CellStructure.Line;
                        e.Use();
                        Repaint();
                    }

                    if ((e.type == EventType.MouseDrag || e.type == EventType.MouseDown) && e.button == 2)
                    {
                        _gridData[x, y].Structure = CellStructure.Empty;
                        e.Use();
                        Repaint();
                    }
                }
            }
        }

        private void SaveLevel()
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Save Level",
                "Level",
                "asset",
                "Save Level",
                "Assets/Level Data");

            if (string.IsNullOrEmpty(path)) return;

            NodeDataSO data = CreateInstance<NodeDataSO>();
            data.width = _gridWidth;
            data.height = _gridHeight;
            data.cellContainer = new List<GridCellData>(_gridWidth * _gridHeight);

            for (int x = 0; x < _gridWidth; x++)
            {
                for (int y = 0; y < _gridHeight; y++)
                {
                    data.cellContainer.Add(_gridData[x, y]);
                }
            }

            AssetDatabase.CreateAsset(data, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            _currentLevelData = data;

            Logger.Success(this, "New level node has been saved successfully!");
            GUIUtility.ExitGUI();
        }

        private void LoadLevel()
        {
            string path = EditorUtility.OpenFilePanel("Load Level", "Assets/Level Data", "asset");

            if (string.IsNullOrEmpty(path)) return;

            string relativePath = FileUtil.GetProjectRelativePath(path);

            NodeDataSO loadedLevel = AssetDatabase.LoadAssetAtPath<NodeDataSO>(relativePath);
            if (loadedLevel == null)
            {
                Logger.Error(this, "Could not load level node! (NodeDataSO == null)");
                return;
            }

            _currentLevelData = loadedLevel;

            _gridWidth = loadedLevel.width;
            _gridHeight = loadedLevel.height;
            _gridData = new GridCellData[_gridWidth, _gridHeight];

            for (int i = 0; i < loadedLevel.cellContainer.Count; i++)
            {
                int x = i / _gridWidth;
                int y = i % _gridHeight;

                _gridData[x, y] = loadedLevel.cellContainer[i];
            }

            Repaint();
            Logger.Success(this, "Level has been loaded successfully!");
        }
    }
#endif