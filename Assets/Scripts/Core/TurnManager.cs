using System;
using ChronoHeist.Node;
using ChronoHeist.Player;

namespace ChronoHeist.Core
{
    public class TurnManager : Manager<TurnManager>
    {
        public enum TurnState
        {
            PlayerTurn,
            EnemyTurn,
            Execution,
            Pause
        }

        public TurnState CurrentState { get; private set; }

        private PlayerController _currentPlayer;

        private void Start()
        {
            _currentPlayer = GridManager.Instance.Player;
            CurrentState = TurnState.PlayerTurn;

            Logger.Info(this, "Game Started. Player's turn");
        }

        public void OnNodeInteracted(GameNode targetNode)
        {
            if (CurrentState != TurnState.PlayerTurn)
            {
                return;
            }

            if (!_currentPlayer)
            {
                _currentPlayer = GridManager.Instance.Player;
            }

            if (IsValidMove(targetNode))
            {
                ChangeState(TurnState.Execution);
                _currentPlayer.MoveToNode(targetNode, OnPlayerMoveCompleted);
            }
        }

        private void OnPlayerMoveCompleted()
        {
            Logger.Success(this, "Player Moved");
            ChangeState(TurnState.PlayerTurn);
        }

        private bool IsValidMove(GameNode target)
        {
            return _currentPlayer.CurrentNode.neighbors.Contains(target);
        }

        private void ChangeState(TurnState newState)
        {
            CurrentState = newState;
        }
    }
}