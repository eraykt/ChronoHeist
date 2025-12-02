using ChronoHeist.Node;
using UnityEngine;

public static class CHRLibrary
{
    public static readonly Vector2Int[] Directions =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    // Right is Z, down is X according to the camera
    public static Vector3 ConvertVector(this Vector3 vector)
    {
        return new Vector3(vector.z, vector.y, vector.x);
    }

    public static bool IsInsideGrid(int x, int y, int width, int height)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    public static float GetLineAngel(int x, int y, int width, int height, GridCellData[,] gridData)
    {
        foreach (var dir in Directions) 
        {
            int cx = x + dir.x;
            int cy = y + dir.y;
        
            if (IsInsideGrid(cx, cy, width, height) && gridData[cx, cy].Structure == CellStructure.Node)
            {
                return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            }
        }

        foreach (var dir in Directions)
        {
            int cx = x + dir.x;
            int cy = y + dir.y;
            if (IsInsideGrid(cx, cy, width, height) && gridData[cx, cy].Structure == CellStructure.Line)
            {
                return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            }
        }
        return 0f;
    }
}