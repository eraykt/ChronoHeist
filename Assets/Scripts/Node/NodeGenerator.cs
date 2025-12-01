using UnityEngine;

namespace ChronoHeist.Node
{
    public class NodeGenerator : MonoBehaviour
    {

        [SerializeField]
        private GameObject _linePrefab;

        [SerializeField]
        private GameObject _nodePrefab;


        private void Start()
        {
            GenerateNodes();
        }

        private void GenerateNodes()
        { }
    }
}