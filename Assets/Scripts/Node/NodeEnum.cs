using System.Collections.Generic;

namespace ChronoHeist.Node
{
    public enum CellStructure
    {
        Empty,
        Node,
        Line
    }

    public enum CellContent
    {
        None,
        PlayerSpawn,
        EnemySpawn
    }

    [System.Serializable]
    public class GridCellData
    {
        public CellStructure Structure = CellStructure.Empty;
        public List<CellContent> Contents = new List<CellContent>();
    }
}