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
        PlayerSpawn,
        EnemySpawn
    }

    [System.Serializable]
    public class GridCellData
    {
        public CellStructure Structure = CellStructure.Empty;
        public List<CellContent> Contents = new List<CellContent>();
        
        public bool ContainsContent(CellContent content)
        {
            return Contents.Contains(content);
        }

        public void AddContent(CellContent content)
        {
            if (!ContainsContent(content))
            {
                Contents.Add(content);
            }
        }

        public void RemoveContent(CellContent content)
        {
            if (Contents.Contains(content))
            {
                Contents.Remove(content);
            }
        }
    }
}