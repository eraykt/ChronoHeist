using System.Collections.Generic;
using ChronoHeist.Command;
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

        private List<GameNode> _highlightedNodes = new List<GameNode>();

        private void OnEnable()
        {
            EventManager.RegisterEvent<EventManager.OnPlayerInitialized>(OnPlayerInitialized);
            EventManager.RegisterEvent<EventManager.OnGridGenerationFinished>(OnGridGenerationFinished);
        }

        private void OnGridGenerationFinished(EventManager.OnGridGenerationFinished obj)
        {
            CurrentState = TurnState.PlayerTurn;
            HighlightAvailableMoves();
            Logger.Info(this, "Game Started. Player's turn");
        }

        private void OnPlayerInitialized(EventManager.OnPlayerInitialized obj)
        {
            _currentPlayer = obj.player;
        }

        private void HighlightAvailableMoves()
        {
            ClearHighlights();

            foreach (GameNode node in _currentPlayer.CurrentNode.neighbors)
            {
                node.SetHighlight(true);
                _highlightedNodes.Add(node);
            }
        }

        private void ClearHighlights()
        {
            foreach (var node in _highlightedNodes)
            {
                node.SetHighlight(false);
            }

            _highlightedNodes.Clear();
        }

        public void OnNodeInteracted(GameNode targetNode)
        {
            if (CurrentState != TurnState.PlayerTurn)
            {
                return;
            }

            if (IsValidMove(targetNode))
            {
                ChangeState(TurnState.Execution);

                ICommand moveCommand = new MoveCommand(
                    _currentPlayer,
                    _currentPlayer.CurrentNode,
                    targetNode,
                    OnPlayerMoveCompleted
                );

                CommandManager.Instance.ExecuteCommand(moveCommand);
            }
        }

        private void OnPlayerMoveCompleted()
        {
            Logger.Success(this, "Player Moved");
            ChangeState(TurnState.PlayerTurn);
            HighlightAvailableMoves();
        }

        private bool IsValidMove(GameNode target)
        {
            return _currentPlayer.CurrentNode.neighbors.Contains(target);
        }

        private void ChangeState(TurnState newState)
        {
            CurrentState = newState;
        }

        private void OnDisable()
        {
            EventManager.DeregisterEvent<EventManager.OnPlayerInitialized>(OnPlayerInitialized);
            EventManager.DeregisterEvent<EventManager.OnGridGenerationFinished>(OnGridGenerationFinished);
        }
    }
}