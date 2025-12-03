using UnityEngine;
using UnityEngine.InputSystem;

namespace ChronoHeist.Input
{
    public class InputReader : ScriptableObject
    {
        private InputSystem_Actions _input;

        public InputSystem_Actions.GameActions GameActions => _input.Game;
        public InputSystem_Actions.UIActions UIActions => _input.UI;
        
        public void OnEnable()
        {
            if (!Application.isPlaying) return;

            if (!InputIsValid())
            {
                CreateInput();
            }
        }

        private void CreateInput()
        {
            _input = new InputSystem_Actions();
            SetGameInput();
        }

        public void EnableInput()
        {
            if (!InputIsValid())
            {
                CreateInput();
            }

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
        
        public Vector2 GetMousePosition()
        {
            return Mouse.current.position.ReadValue();
        }
        
        private bool InputIsValid()
        {
            return _input != null;
        }

        private void DisableInput()
        {
            if (_input != null)
            {
                _input.Game.Disable();
                _input.UI.Disable();
                _input.Dispose();
                _input = null;
            }
        }
        
        private void OnDisable()
        {
            if (!Application.isPlaying) return;

            DisableInput();
        }
    }
}