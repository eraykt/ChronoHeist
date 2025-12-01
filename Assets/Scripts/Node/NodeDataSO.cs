using System.Collections.Generic;
using UnityEngine;

namespace ChronoHeist.Node
{
    public class NodeDataSO : ScriptableObject
    {
        public int width;
        public int height;

        public List<CellType> cellContainer;
    }
}