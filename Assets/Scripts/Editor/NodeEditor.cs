using UnityEditor;
using UnityEngine;

public class NodeEditor : EditorWindow
{
    private int gridWidth = 10;
    private int gridHeight = 10;
    private float cellSize = 30f;
    private float padding = 20f;

    public enum CellType
    {
        Empty,
        Circle,
        Line
    }

    private CellType[,] gridData;

    [MenuItem("Tools/My Level Editor")]
    public static void ShowWindow()
    {
        GetWindow<NodeEditor>("Level Editor");
    }

    private void OnEnable()
    {
        if (gridData == null)
        {
            gridData = new CellType[gridWidth, gridHeight];
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("Level Design Grid", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        gridWidth = EditorGUILayout.IntField("Width", gridWidth);
        gridHeight = EditorGUILayout.IntField("Height", gridHeight);
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Reset Grid"))
        {
            gridData = new CellType[gridWidth, gridHeight];
        }

        GUILayout.Space(10);

        if (gridData == null || gridData.GetLength(0) != gridWidth || gridData.GetLength(1) != gridHeight)
        {
            gridData = new CellType[gridWidth, gridHeight];
        }

        HandleEvents();
        DrawGrid();
    }

    private void DrawGrid()
    {
        Rect gridRect = new Rect(padding, 60, gridWidth * cellSize, gridHeight * cellSize);

        EditorGUI.DrawRect(gridRect, new Color(0.2f, 0.2f, 0.2f));

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Rect cellRect = new Rect(padding + x * cellSize, 60 + y * cellSize, cellSize, cellSize);

                Handles.color = Color.black;
                Handles.DrawWireDisc(cellRect.center, Vector3.forward, 0);
                GenerateCellVisual(x, y, cellRect);

                EditorGUI.DrawRect(new Rect(cellRect.x, cellRect.y, cellSize, 1), Color.grey);
                EditorGUI.DrawRect(new Rect(cellRect.x, cellRect.y, 1, cellSize), Color.grey);
            }
        }

        EditorGUI.DrawRect(new Rect(gridRect.xMax, gridRect.y, 1, gridRect.height), Color.grey);
        EditorGUI.DrawRect(new Rect(gridRect.x, gridRect.yMax, gridRect.width, 1), Color.grey);
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
                float lineHeight = 4f;
                Rect lineRect = new Rect(cellRect.x + 2, cellRect.y + (cellSize / 2) - (lineHeight / 2), cellSize - 4, lineHeight);
                EditorGUI.DrawRect(lineRect, new Color(1f, 0.4f, 0.4f));
                break;
        }
    }

    private void HandleEvents()
    {
        Event e = Event.current;

        Rect gridArea = new Rect(padding, 60, gridWidth * cellSize, gridHeight * cellSize);

        if (gridArea.Contains(e.mousePosition))
        {
            Vector2 relativeMousePos = e.mousePosition - new Vector2(padding, 60);
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
            }
        }
    }
}