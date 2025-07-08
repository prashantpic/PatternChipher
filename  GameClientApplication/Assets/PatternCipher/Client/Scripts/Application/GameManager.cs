using UnityEngine;
using PatternCipher.Client.Events;
using PatternCipher.Client.Scenes;
using PatternCipher.Shared.Models;
// The following using statements are placeholders for the actual service interfaces
// that will be defined in the Presentation and Infrastructure layers.
// using PatternCipher.Presentation.Services; 
// using PatternCipher.Infrastructure.Interfaces;

namespace PatternCipher.Client.Application
{
    // --- MOCK INTERFACES for compilation ---
    // These would be defined in their own assemblies (REPO-PATT-003, REPO-PATT-004, etc.)
    public interface IUIService { /* ... methods to control UI ... */ }
    public interface IPersistenceService 
    {
        void LoadProfile();
        void SaveProfile(object profileData);
    }
    public interface IBackendService { /* ... methods to interact with Firebase ... */ }
    public class LevelManager : MonoBehaviour 
    {
        public void StartLevel(LevelDefinition level)
        {
            Debug.Log($"LevelManager: Starting level '{level?.LevelId ?? "NULL"}'");
        }
    }
    // --- END MOCK INTERFACES ---


    /// <summary>
    /// Manages the overall game lifecycle and state. It coordinates all other managers
    /// to create a cohesive experience, handling transitions between menus, gameplay,
    /// and paused states, and managing data persistence during application lifecycle events.
    /// Implemented as a singleton, it provides a globally accessible point of control.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        /// <summary>
        /// The static singleton instance of the GameManager.
        /// Provides global access to the central orchestrator.
        /// </summary>
        public static GameManager Instance { get; private set; }

        /// <summary>
        /// Gets the current high-level state of the game.
        /// </summary>
        public GameState CurrentState { get; private set; }

        [Header("Component References")]
        [SerializeField]
        [Tooltip("Reference to the LevelManager component.")]
        private LevelManager _levelManager;

        // References to services from other layers.
        // In a full DI setup, these would be injected. Here, we'll locate them.
        private IUIService _uiService;
        private IPersistenceService _persistenceService;
        private IBackendService _backendService;
        private SceneLoader _sceneLoader;
        
        private LevelDefinition _currentLevelDefinition;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// It enforces the singleton pattern and initializes core systems.
        /// </summary>
        private void Awake()
        {
            // --- Singleton Pattern Implementation ---
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("Duplicate GameManager found. Destroying the new one.");
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // --- Initialization ---
            InitializeServices();

            // --- Initial State Transition ---
            // Load initial game data and transition to the first logical state.
            _persistenceService?.LoadProfile();
            ChangeState(GameState.MainMenu);
        }

        private void InitializeServices()
        {
            _sceneLoader = new SceneLoader();

            // Locate services on this persistent object. This is a simple form of service location.
            // A more robust system might use a dedicated ServiceLocator class or a DI framework like Zenject.
            _uiService = GetComponentInChildren<IUIService>();
            _persistenceService = GetComponentInChildren<IPersistenceService>();
            _backendService = GetComponentInChildren<IBackendService>();

            if (_levelManager == null) Debug.LogError("GameManager: LevelManager is not assigned!");
            //if (_uiService == null) Debug.LogError("GameManager: IUIService implementation not found!");
            //if (_persistenceService == null) Debug.LogError("GameManager: IPersistenceService implementation not found!");
            //if (_backendService == null) Debug.LogError("GameManager: IBackendService implementation not found!");
        }

        /// <summary>
        /// Transitions the game to a new state, handling exit and entry logic.
        /// </summary>
        /// <param name="newState">The state to transition to.</param>
        public void ChangeState(GameState newState)
        {
            if (newState == CurrentState)
            {
                return;
            }

            Debug.Log($"Game State changing from {CurrentState} to {newState}");

            OnExitState(CurrentState);
            CurrentState = newState;
            OnEnterState(newState);

            GameEventSystem.Publish(new GameStateChangedEvent { NewState = CurrentState });
        }

        /// <summary>
        /// Logic to execute when entering a new state.
        /// </summary>
        private void OnEnterState(GameState state)
        {
            switch (state)
            {
                case GameState.Initializing:
                    // Handled by Awake
                    break;
                case GameState.MainMenu:
                    _sceneLoader.LoadSceneAsync(SceneId.MainMenu);
                    break;
                case GameState.LevelSelection:
                    // Potentially load a different scene or show a specific UI panel
                    break;
                case GameState.InGame:
                    _levelManager.StartLevel(_currentLevelDefinition);
                    GameEventSystem.Publish(new LevelStartedEvent { Level = _currentLevelDefinition });
                    break;
                case GameState.Paused:
                    Time.timeScale = 0f;
                    // _uiService.ShowPauseMenu(); // Example interaction
                    break;
                case GameState.LevelComplete:
                    // _uiService.ShowLevelCompleteScreen(); // Example interaction
                    break;
            }
        }

        /// <summary>
        /// Logic to execute when exiting the current state.
        /// </summary>
        private void OnExitState(GameState state)
        {
            switch (state)
            {
                case GameState.Paused:
                    Time.timeScale = 1f;
                    // _uiService.HidePauseMenu(); // Example interaction
                    break;
                // Add cleanup logic for other states if necessary
            }
        }

        #region Public API for UI and other systems

        /// <summary>
        /// Initiates the start of a specific level. Called by UI.
        /// </summary>
        /// <param name="level">The definition of the level to start.</param>
        public void StartLevel(LevelDefinition level)
        {
            if (level == null)
            {
                Debug.LogError("Cannot start a null level.");
                return;
            }
            _currentLevelDefinition = level;
            // Load the game scene first, the InGame state will handle starting the level logic
            _sceneLoader.LoadSceneAsync(SceneId.Game).ContinueWith(task => {
                if (task.IsCompletedSuccessfully)
                {
                    ChangeState(GameState.InGame);
                }
            });
        }

        /// <summary>
        /// Pauses the game if it's currently in progress.
        /// </summary>
        public void PauseGame()
        {
            if (CurrentState == GameState.InGame)
            {
                ChangeState(GameState.Paused);
            }
        }

        /// <summary>
        /// Resumes the game from a paused state.
        /// </summary>
        public void ResumeGame()
        {
            if (CurrentState == GameState.Paused)
            {
                ChangeState(GameState.InGame);
            }
        }

        #endregion

        #region Application Lifecycle Handlers

        /// <summary>
        /// Called by Unity when the application is paused or resumed.
        /// </summary>
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                Debug.Log("Application is pausing. Requesting game save.");
                // This is a critical point to save player progress.
                _persistenceService?.SaveProfile(null); // Pass actual profile data
                GameEventSystem.Publish(new SaveGameRequestEvent());
            }
        }

        /// <summary>
        /// Called by Unity when the application is about to quit.
        /// </summary>
        private void OnApplicationQuit()
        {
            Debug.Log("Application is quitting. Requesting final game save.");
            // Ensure data is saved before the application closes.
            _persistenceService?.SaveProfile(null); // Pass actual profile data
            GameEventSystem.Publish(new SaveGameRequestEvent());
        }
        
        #endregion
    }
}