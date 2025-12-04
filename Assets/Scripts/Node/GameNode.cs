using System.Collections.Generic;
using UnityEngine;

namespace ChronoHeist.Node
{
    public class GameNode : MonoBehaviour
    {
        public Vector2Int index;
        
        public List<GameNode> neighbors = new List<GameNode>();

        [Header("Visuals")]
        [SerializeField]
        private Renderer _renderer;

        [SerializeField]
        private Color _defaultColor = Color.white;
        
        [SerializeField]
        private Color _highlightColor = Color.green;
        
        private MaterialPropertyBlock _propertyBlock;
        
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

        private void Awake()
        {
            _propertyBlock = new MaterialPropertyBlock();
        }

        public void SetHighlight(bool isActive)
        {
            _renderer.GetPropertyBlock(_propertyBlock);
            Color targetColor  = isActive ? _highlightColor : _defaultColor;
            _propertyBlock.SetColor(BaseColorId, targetColor);
            _renderer.SetPropertyBlock(_propertyBlock);
        }
        
    }
}