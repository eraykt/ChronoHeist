using System.Collections;
using System.Collections.Generic;
using ChronoHeist.Command;
using ChronoHeist.Enemy;
using ChronoHeist.Node;
using ChronoHeist.Player;
using UnityEngine;

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
        private List<EnemyController> _enemyList = new List<EnemyController>();

        private List<GameNode> _highlightedNodes = new List<GameNode>();

        private TurnBatchCommand _currentCommandBatch;

        public override void InitializeManager()
        {
            EventManager.RegisterEvent<EventManager.OnPlayerInitialized>(OnPlayerInitialized);
            EventManager.RegisterEvent<EventManager.OnGridGenerationFinished>(OnGridGenerationFinished);
        }

        public void RegisterEnemy(EnemyController enemy)
        {
            _enemyList.Add(enemy);
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

                _currentCommandBatch = new TurnBatchCommand();

                ICommand moveCommand = new MoveCommand(
                    _currentPlayer,
                    _currentPlayer.CurrentNode,
                    targetNode,
                    OnPlayerMoveCompleted,
                    HighlightAvailableMoves
                );
                
                _currentCommandBatch.AddCommand(moveCommand);
                
                moveCommand.Execute();
            }
        }

        private void OnPlayerMoveCompleted()
        {
            Logger.Success(this, "Player Moved");
            ClearHighlights();
            ChangeState(TurnState.EnemyTurn);

            StartCoroutine(ProcessEnemyTurns());

            //ChangeState(TurnState.PlayerTurn);
            //HighlightAvailableMoves();
        }
        
        private IEnumerator ProcessEnemyTurns()
        {
            foreach (EnemyController enemy in _enemyList)
            {
                if (enemy == null) continue;

                bool enemyFinished = false;
                
                GameNode targetNode = enemy.CalculatePath(_currentPlayer.CurrentNode);

                ICommand enemyMove = new MoveCommand(
                    enemy, 
                    enemy.CurrentNode, 
                    targetNode, 
                    () => enemyFinished = true,
                    null
                    );
                
                enemyMove.Execute();
                _currentCommandBatch.AddCommand(enemyMove);

                yield return new WaitUntil(() => enemyFinished);

                yield return new WaitForSeconds(1.0f);
            }
            
            CommandManager.Instance.RegisterCommand(_currentCommandBatch);
            
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