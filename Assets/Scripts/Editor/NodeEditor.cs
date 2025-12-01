#if UNITY_EDITOR

using System.Collections.Generic;
using ChronoHeist.Node;
using UnityEngine;
using UnityEditor;

public class NodeEditor : EditorWindow
{
    private int gridWidth = 10;
    private int gridHeight = 10;
    private float cellSize = 30f;

    private CellType[,] gridData;
    private NodeDataSO currentLevelData;

    private readonly Vector2Int[] directions = {
        Vector2Int.up, 
        Vector2Int.down, 
        Vector2Int.left, 
        Vector2Int.right
    };

    [MenuItem("Tools/My Level Editor")]
    public static void ShowWindow()
    {
        var a = GetWindow<NodeEditor>("Level Editor");
    }

    private void OnEnable()
    {
        if (gridData == null) gridData = new CellType[gridWidth, gridHeight];
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        {
            GUILayout.Label("Settings:", EditorStyles.miniLabel);
            gridWidth = EditorGUILayout.IntField(gridWidth, EditorStyles.toolbarTextField, GUILayout.Width(40));
            GUILayout.Label("x", EditorStyles.miniLabel);
            gridHeight = EditorGUILayout.IntField(gridHeight, EditorStyles.toolbarTextField, GUILayout.Width(40));
            GUILayout.Label("y", EditorStyles.miniLabel);

            if (GUILayout.Button("Reset", EditorStyles.toolbarButton))
            {
                gridData = new CellType[gridWidth, gridHeight];
                float calculatedWidth = (gridWidth * cellSize) + 40;
                float calculatedHeight = (gridHeight * cellSize) + 40 + 25;
                Vector2 newSize = new Vector2(calculatedWidth, calculatedHeight);
                this.minSize = newSize;
                this.maxSize = newSize;
            }
            if (GUILayout.Button("Save", EditorStyles.toolbarButton)) SaveLevel();
            if (GUILayout.Button("Load", EditorStyles.toolbarButton)) LoadLevel();
        }
        GUILayout.EndHorizontal();

        if (gridData == null || gridData.GetLength(0) != gridWidth || gridData.GetLength(1) != gridHeight)
        {
            gridData = new CellType[gridWidth, gridHeight];
        }

        Rect workArea = GUILayoutUtility.GetRect(
            (gridWidth * cellSize) + 40, (gridHeight * cellSize) + 40,
            GUILayout.ExpandWidth(false),
            GUILayout.ExpandHeight(false)
        );

        EditorGUI.DrawRect(workArea, new Color(0.15f, 0.15f, 0.15f));

        DrawGridSection(workArea);
    }

    private void DrawGridSection(Rect workArea)
    {
        float contentWidth = gridWidth * cellSize + 40;
        float contentHeight = gridHeight * cellSize + 40;
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

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Rect cellRect = new Rect(startX + x * cellSize, startY + y * cellSize, cellSize, cellSize);

                Handles.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                Handles.DrawWireDisc(cellRect.center, Vector3.forward, 0);
                EditorGUI.DrawRect(new Rect(cellRect.x, cellRect.y, cellSize, 1), Color.grey);
                EditorGUI.DrawRect(new Rect(cellRect.x, cellRect.y, 1, cellSize), Color.grey);

                GenerateCellVisual(x, y, cellRect);
            }
        }

        float gridRealWidth = gridWidth * cellSize;
        float gridRealHeight = gridHeight * cellSize;
        EditorGUI.DrawRect(new Rect(startX + gridRealWidth, startY, 1, gridRealHeight), Color.grey);
        EditorGUI.DrawRect(new Rect(startX, startY + gridRealHeight, gridRealWidth, 1), Color.grey);
    }

    private void GenerateCellVisual(int x, int y, Rect cellRect)
    {
        CellType type = gridData[x, y];

        switch (type)
        {
            case CellType.Circle:
                Handles.color = Color.green;
                Handles.DrawSolidDisc(cellRect.center, Vector3.forward, cellSize * 0.35f);
                break;

            case CellType.Line:
                float angle = GetSmartAngle(x, y);

                Matrix4x4 originalMatrix = GUI.matrix;
                GUIUtility.RotateAroundPivot(angle, cellRect.center);

                Rect lineRect = new Rect(cellRect.x + 2, cellRect.y + (cellSize / 2) - (4f / 2), cellSize, 4f);
                EditorGUI.DrawRect(lineRect, new Color(1f, 0.4f, 0.4f));

                GUI.matrix = originalMatrix;
                break;
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
            int x = Mathf.FloorToInt(relativeMousePos.x / cellSize);
            int y = Mathf.FloorToInt(relativeMousePos.y / cellSize);

            if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight)
            {
                if (e.type == EventType.MouseDown && e.button == 0)
                {
                    gridData[x, y] = (gridData[x, y] == CellType.Circle) ? CellType.Empty : CellType.Circle;
                    e.Use();
                    Repaint();
                }

                if ((e.type == EventType.MouseDrag || e.type == EventType.MouseDown) && e.button == 1)
                {
                    gridData[x, y] = CellType.Line;
                    e.Use();
                    Repaint();
                }

                if ((e.type == EventType.MouseDrag || e.type == EventType.MouseDown) && e.button == 2)
                {
                    gridData[x, y] = CellType.Empty;
                    e.Use();
                    Repaint();
                }
            }
        }
    }

    private float GetSmartAngle(int x, int y)
    {
        foreach (var dir in directions) // check circle first 
        {
            int cx = x + dir.x;
            int cy = y + dir.y;
            if (IsInside(cx, cy) && gridData[cx, cy] == CellType.Circle)
                return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        }
        foreach (var dir in directions) // than check line
        {
            int cx = x + dir.x;
            int cy = y + dir.y;
            if (IsInside(cx, cy) && gridData[cx, cy] == CellType.Line)
                return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        }
        return 0f;
    }

    private bool IsInside(int x, int y) => x >= 0 && x < gridWidth && y >= 0 && y < gridHeight;

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
        data.width = gridWidth;
        data.height = gridHeight;
        data.cellContainer = new List<CellType>(gridWidth * gridHeight);

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                data.cellContainer.Add(gridData[x, y]);
            }
        }

        AssetDatabase.CreateAsset(data, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        currentLevelData = data;

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

        currentLevelData = loadedLevel;
        
        gridWidth = loadedLevel.width;
        gridHeight = loadedLevel.height;
        gridData = new CellType[gridWidth, gridHeight];

        for (int i = 0; i < loadedLevel.cellContainer.Count; i++)
        {
            int x = i / gridWidth;
            int y = i % gridHeight;
            
            gridData[x, y] = loadedLevel.cellContainer[i];
        }
        
        Repaint();
        Logger.Success(this, "Level has been loaded successfully!");
    }
}
#endif