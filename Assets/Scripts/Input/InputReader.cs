using UnityEngine;
using UnityEngine.InputSystem;

namespace ChronoHeist.Input
{
    public class InputReader : ScriptableObject, InputSystem_Actions.IGameActions, InputSystem_Actions.IUIActions
    {
        private InputSystem_Actions _input;

        public System.Action clickEvent;

        public void Initialize()
        {
            _input = new InputSystem_Actions();

            _input.Game.SetCallbacks(this);
            _input.UI.SetCallbacks(this);

            SetGameInput();
        }

        public void SetGameInput()
        {
            _input.Game.Enable();
            _input.UI.Disable();
        }

        public void SetUIInput()
        {
            _input.UI.Enable();
            _input.Game.Disable();
        }

        private void OnDisable()
        {
            if (_input != null)
            {
                _input.Game.Disable();
                _input.UI.Disable();
                _input.Dispose();
                _input = null;
            }
        }

        public Vector2 GetMousePosition()
        {
            return Mouse.current.position.ReadValue();
        }

        public void OnSelect(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                clickEvent?.Invoke();
            }
        }

        public void OnNewaction(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}