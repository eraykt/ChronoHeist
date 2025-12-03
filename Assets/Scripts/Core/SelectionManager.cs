using ChronoHeist.Input;
using ChronoHeist.Node;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ChronoHeist.Core
{
    public class SelectionManager : Manager<SelectionManager>
    {
        [SerializeField]
        private InputReader _input;

        [SerializeField]
        private LayerMask _nodeLayer;

        [SerializeField]
        private Camera _mainCamera;

        private void OnEnable()
        {
            _input.EnableInput();
            _input.GameActions.Select.performed += HandleClick;
        }
        
        private void HandleClick(InputAction.CallbackContext callbackContext)
        {
            Vector2 mousePos = _input.GetMousePosition();
            Ray ray = _mainCamera.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _nodeLayer))
            {
                if (hit.collider.TryGetComponent(out GameNode clickedNode))
                {
                    Logger.Info(this, $"Selected Node: {clickedNode.index}");
                    TurnManager.Instance.OnNodeInteracted(clickedNode);
                }
            }
        }
        
        private void OnDisable()
        {
            _input.GameActions.Select.performed -= HandleClick;
        }
    }
}