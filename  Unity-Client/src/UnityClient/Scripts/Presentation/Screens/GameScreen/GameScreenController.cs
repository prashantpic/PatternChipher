using PatternCipher.Client.Core.Events;
using PatternCipher.Client.Application.Commands;
using PatternCipher.Client.Domain.Events;
using PatternCipher.Client.Domain.ValueObjects; // For GridPosition
using PatternCipher.Client.Presentation.Input; // For IGameInputReceiver
using PatternCipher.Client.Presentation.Screens.Common;
using UnityEngine;
using System; // For Action

namespace PatternCipher.Client.Presentation.Screens.GameScreen
{
    public class GameScreenController : BaseScreenController, IGameInputReceiver
    {
        [SerializeField]
        private GameScreenView gameScreenView;

        // Dependencies (to be injected or found)
        private GlobalEventBus _eventBus;
        private PlayerMoveCommandHandler _playerMoveCommandHandler; // Example, could be a service

        // Placeholder for actual Command Handler or Service
        private class PlayerMoveCommandHandler
        {
            public void Handle(ProcessPlayerMoveCommand command) { /* Placeholder */ }
        }


        protected override void Awake()
        {
            base.Awake();
            // Typically, dependencies are injected via a DI container or Service Locator
            // For simplicity, let's assume GlobalEventBus is a singleton
            _eventBus = GlobalEventBus.Instance;
            _playerMoveCommandHandler = new PlayerMoveCommandHandler(); // Placeholder instantiation
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (_eventBus != null)
            {
                _eventBus.Subscribe<PlayerMoveMadeEvent>(OnPlayerMoveMade);
                _eventBus.Subscribe<LevelCompletedEvent>(OnLevelCompleted);
                _eventBus.Subscribe<TileInteractionFeedbackEvent>(OnTileInteractionFeedback);
                _eventBus.Subscribe<GridInitializedEvent>(OnGridInitialized); // Assuming GameScreenView needs initial grid data
            }

            if (gameScreenView != null)
            {
                gameScreenView.OnPauseButtonClick.AddListener(HandlePauseButtonPressed);
                gameScreenView.OnHintButtonClick.AddListener(HandleHintButtonPressed);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (_eventBus != null)
            {
                _eventBus.Unsubscribe<PlayerMoveMadeEvent>(OnPlayerMoveMade);
                _eventBus.Unsubscribe<LevelCompletedEvent>(OnLevelCompleted);
                _eventBus.Unsubscribe<TileInteractionFeedbackEvent>(OnTileInteractionFeedback);
                _eventBus.Unsubscribe<GridInitializedEvent>(OnGridInitialized);
            }

            if (gameScreenView != null)
            {
                gameScreenView.OnPauseButtonClick.RemoveListener(HandlePauseButtonPressed);
                gameScreenView.OnHintButtonClick.RemoveListener(HandleHintButtonPressed);
            }
        }

        protected override void OnShow()
        {
            // Logic to execute when the screen starts to show
            // e.g., reset UI elements, start animations
            if (gameScreenView != null)
            {
                gameScreenView.UpdateMovesDisplay(0);
                gameScreenView.UpdateScoreDisplay(0);
                gameScreenView.UpdateTimerDisplay("00:00");
                // gameScreenView.UpdateGoalDisplay(...); // Needs LevelObjective data
            }
        }

        protected override void OnHide()
        {
            // Logic to execute when the screen starts to hide
        }

        protected override void OnUpdateView()
        {
            // Logic to update the view, perhaps called periodically or on specific events
        }
        
        private void OnGridInitialized(GridInitializedEvent eventData)
        {
            if (gameScreenView != null && gameScreenView.GridView != null)
            {
                // Assuming GridView has a method to take GridInitializedEvent or its data
                // gameScreenView.GridView.InitializeGrid(eventData.Dimensions, ...);
                Debug.Log($"GameScreenController: Grid Initialized - {eventData.Dimensions.Rows}x{eventData.Dimensions.Columns}");
            }
        }

        private void OnPlayerMoveMade(PlayerMoveMadeEvent eventData)
        {
            if (gameScreenView != null)
            {
                // Example: Update HUD elements based on eventData
                // gameScreenView.UpdateMovesDisplay(eventData.CurrentMoves);
                // gameScreenView.UpdateScoreDisplay(eventData.CurrentScore);
                Debug.Log($"Player move made. Success: {eventData.Success}, Points: {eventData.PointsEarned}");
            }
        }

        private void OnLevelCompleted(LevelCompletedEvent eventData)
        {
            Debug.Log($"Level completed! Score: {eventData.FinalScore}, Stars: {eventData.StarsAwarded}");
            // Example: Transition to a Level Complete screen via ScreenManager
            // ScreenManager.Instance.ShowScreen(ScreenType.LevelComplete, eventData);
        }

        private void OnTileInteractionFeedback(TileInteractionFeedbackEvent eventData)
        {
            // This event is primarily for VisualFeedbackController and AudioFeedbackController.
            // GameScreenController might react to specific feedback types if needed for UI state.
            Debug.Log($"Tile interaction feedback: {eventData.RequestedFeedback} at {eventData.Position}");
        }

        public void HandleTileTap(GridPosition pos)
        {
            Debug.Log($"Tile tapped at: {pos.Row}, {pos.Column}");
            var command = new ProcessPlayerMoveCommand
            {
                Type = ProcessPlayerMoveCommand.MoveType.Tap,
                Position1 = pos
            };
            _playerMoveCommandHandler.Handle(command);
        }

        public void HandleTileDrag(GridPosition start, GridPosition end)
        {
            Debug.Log($"Tile dragged from: {start.Row},{start.Column} to {end.Row},{end.Column}");
            var command = new ProcessPlayerMoveCommand
            {
                Type = ProcessPlayerMoveCommand.MoveType.Swap, // Assuming drag implies swap
                Position1 = start,
                Position2 = end
            };
            _playerMoveCommandHandler.Handle(command);
        }
        
        // For IGameInputReceiver (as per updated SDS)
        public void HandleTileSwap(GridPosition pos1, GridPosition pos2)
        {
            Debug.Log($"Tile swap requested between: {pos1.Row},{pos1.Column} and {pos2.Row},{pos2.Column}");
             var command = new ProcessPlayerMoveCommand
            {
                Type = ProcessPlayerMoveCommand.MoveType.Swap,
                Position1 = pos1,
                Position2 = pos2
            };
            _playerMoveCommandHandler.Handle(command);
        }


        private void HandlePauseButtonPressed()
        {
            Debug.Log("Pause button pressed");
            // Example: Show Pause Screen via ScreenManager or publish a PauseGameEvent
            // GlobalEventBus.Instance.Publish(new GamePausedEvent());
            // ScreenManager.Instance.PushScreen<PauseScreenController>();
        }

        private void HandleHintButtonPressed()
        {
            Debug.Log("Hint button pressed");
            // Example: Request a hint from a HintService
            // GlobalEventBus.Instance.Publish(new HintRequestedEvent());
        }
    }
}