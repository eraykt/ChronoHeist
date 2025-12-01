using System.Collections.Generic;
using UnityEngine;

public class NodeDataSO : ScriptableObject
{
    public int width;
    public int height;

    public List<CellType> cellContainer;
}
