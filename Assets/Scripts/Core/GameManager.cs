using System.Collections.Generic;
using ChronoHeist.Input;
using UnityEngine;

namespace ChronoHeist.Core
{
    public class GameManager : Manager<GameManager>
    {
        [Header("Input")]
        [SerializeField]
        private InputReader _input;
        
        [Header("Managers")]
        [Tooltip("The managers that are initialized by order")]
        [SerializeField]
        private List<Manager> _managersToInitialize;

        
        public override void InitializeManager()
        {
            _input.EnableInput();
            
            foreach (var manager in _managersToInitialize)
            {
                manager.InitializeManager();
            }
        }

        private void Start()
        {
            InitializeManager();            
        }
    }
}