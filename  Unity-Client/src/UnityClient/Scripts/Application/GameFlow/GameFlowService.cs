using PatternCipher.Client.Core.Events;
using PatternCipher.Client.Domain.Events; // For LevelCompletedEvent
using PatternCipher.Client.Domain.ValueObjects; // For LevelGenerationParameters
using PatternCipher.Client.Presentation.Screens; // For ScreenManager
using System.Threading.Tasks; // For Task
using UnityEngine; // For Debug

namespace PatternCipher.Client.Application.GameFlow
{
    public enum GameState
    {
        Initializing,
        MainMenu,
        LevelLoading,
        Gameplay,
        Paused,
        LevelResults,
        Settings
    }

    public class GameFlowService
    {
        private readonly ScreenManager _screenManager;
        private readonly LevelLoadService _levelLoadService;
        private readonly GlobalEventBus _eventBus;
        // private readonly CloudSyncService _cloudSyncService; // Optional

        public GameState CurrentState { get; private set; }

        public GameFlowService(ScreenManager screenManager, LevelLoadService levelLoadService, GlobalEventBus eventBus /*, CloudSyncService cloudSyncService */)
        {
            _screenManager = screenManager;
            _levelLoadService = levelLoadService;
            _eventBus = eventBus;
            // _cloudSyncService = cloudSyncService;

            CurrentState = GameState.Initializing;
            _eventBus.Subscribe<LevelCompletedEvent>(HandleLevelCompleted);
        }

        public void InitializeGame()
        {
            // Called by GameInitializer after core systems are up
            GoToMainMenu();
        }

        public void GoToMainMenu()
        {
            CurrentState = GameState.MainMenu;
            // _screenManager.ShowOnlyScreen<MainMenuController>(); // Replace with actual MainMenuController type
            Debug.Log("GameFlow: Transitioning to Main Menu");
        }

        public async Task StartLevel(LevelGenerationParameters parameters)
        {
            if (CurrentState == GameState.LevelLoading) return;

            CurrentState = GameState.LevelLoading;
            // _screenManager.ShowScreen<LoadingScreenController>(); // Show a loading screen

            Debug.Log($"GameFlow: Starting level with params: {parameters}");
            bool success = await _levelLoadService.LoadLevelAsync(parameters);

            if (success)
            {
                CurrentState = GameState.Gameplay;
                // _screenManager.ShowOnlyScreen<GameScreenController>(); // Replace with actual GameScreenController type
                Debug.Log("GameFlow: Level loaded, transitioning to Gameplay");
                _eventBus.Publish(new GameStateChangedEvent(GameState.Gameplay));
            }
            else
            {
                Debug.LogError("GameFlow: Failed to load level.");
                // Handle error, perhaps return to main menu or show an error message
                GoToMainMenu();
                _eventBus.Publish(new GameStateChangedEvent(GameState.MainMenu));
            }
        }

        public void PauseGame()
        {
            if (CurrentState != GameState.Gameplay) return;
            CurrentState = GameState.Paused;
            // _screenManager.PushScreen<PauseMenuController>(); // Show pause menu
            Time.timeScale = 0f; // Example of pausing game time
            _eventBus.Publish(new GameStateChangedEvent(GameState.Paused));
            Debug.Log("GameFlow: Game Paused");
        }

        public void ResumeGame()
        {
            if (CurrentState != GameState.Paused) return;
            CurrentState = GameState.Gameplay;
            // _screenManager.PopScreen(); // Hide pause menu
            Time.timeScale = 1f; // Resume game time
            _eventBus.Publish(new GameStateChangedEvent(GameState.Gameplay));
            Debug.Log("GameFlow: Game Resumed");
        }

        private void HandleLevelCompleted(LevelCompletedEvent eventData)
        {
            EndLevel(eventData);
        }

        public void EndLevel(LevelCompletedEvent completionData)
        {
            if (CurrentState != GameState.Gameplay && CurrentState != GameState.Paused) // Can end from paused state too
            {
                Debug.LogWarning($"GameFlow: EndLevel called from unexpected state: {CurrentState}");
            }

            CurrentState = GameState.LevelResults;
            Time.timeScale = 1f; // Ensure time is resumed if paused

            // TODO: Save progress, sync with cloud, etc.
            // Example: _playerProgressService.RecordLevelCompletion(completionData);
            // Example: await _cloudSyncService.SyncAsync();

            // _screenManager.ShowOnlyScreen<LevelResultsScreenController>(completionData); // Pass data to results screen
            Debug.Log($"GameFlow: Level Ended. Score: {completionData.FinalScore}, Stars: {completionData.StarsAwarded}");
            _eventBus.Publish(new GameStateChangedEvent(GameState.LevelResults));
        }

        public void GoToSettings()
        {
            CurrentState = GameState.Settings;
            // _screenManager.PushScreen<SettingsScreenController>();
            _eventBus.Publish(new GameStateChangedEvent(GameState.Settings));
            Debug.Log("GameFlow: Transitioning to Settings");
        }
        
        // Unsubscribe when GameFlowService is disposed or game quits
        public void Teardown()
        {
            _eventBus.Unsubscribe<LevelCompletedEvent>(HandleLevelCompleted);
        }
    }

    // Example Application Event (Consider moving to a dedicated Events folder within Application layer)
    public class GameStateChangedEvent : GameEvent
    {
        public GameState NewState { get; }
        public GameStateChangedEvent(GameState newState)
        {
            NewState = newState;
        }
    }
}