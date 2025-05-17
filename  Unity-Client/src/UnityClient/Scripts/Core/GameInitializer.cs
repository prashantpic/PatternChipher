using UnityEngine;
using System.Threading.Tasks;
using PatternCipher.Client.Infrastructure.Firebase; // Assuming FirebaseServiceInitializer is here
using PatternCipher.Client.Core.Events;
using PatternCipher.Client.Presentation.Screens; // Assuming ScreenManager is here
using PatternCipher.Client.Presentation.Input; // Assuming InputHandler is here
using PatternCipher.Client.DataPersistence.Services; // Assuming SaveLoadService and CloudSyncService are here

namespace PatternCipher.Client.Core
{
    public class GameInitializer : MonoBehaviour
    {
        public static GameInitializer Instance { get; private set; }

        // Assuming these service references will be set up, e.g., via DI or GetComponent
        // For simplicity, let's assume they might be components on the same GameObject or singletons themselves
        // Or they are passed in/found. The spec mentions responsibilities for initializing them.

        private FirebaseServiceInitializer _firebaseServiceInitializer;
        private ScreenManager _screenManager;
        private InputHandler _inputHandler;
        private SaveLoadService _saveLoadService;
        private CloudSyncService _cloudSyncService;
        private FirebaseRemoteConfigService _firebaseRemoteConfigService; // Assuming this service exists

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                // Initialize references if needed, e.g., GetComponent or find
                _firebaseServiceInitializer = GetComponent<FirebaseServiceInitializer>() ?? FindObjectOfType<FirebaseServiceInitializer>(); // Example
                _screenManager = GetComponent<ScreenManager>() ?? FindObjectOfType<ScreenManager>();
                _inputHandler = GetComponent<InputHandler>() ?? FindObjectOfType<InputHandler>();
                _saveLoadService = new SaveLoadService(); // Example: Or find/inject
                _cloudSyncService = new CloudSyncService(); // Example: Or find/inject
                _firebaseRemoteConfigService = new FirebaseRemoteConfigService(); // Example: Or find/inject

            }
            else
            {
                Destroy(gameObject);
            }
        }

        private async void Start() // Unity's Start can be async void
        {
            await InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            // Order of initialization based on responsibilities
            InitializeCoreSystems(); // GlobalEventBus is initialized here via its singleton nature

            if (_firebaseServiceInitializer != null) // Check if found/assigned
            {
                await InitializeFirebaseAsync();
            }
            else
            {
                Debug.LogError("FirebaseServiceInitializer not found.");
            }
            
            if (_firebaseRemoteConfigService != null) // Example: Load remote config after Firebase init
            {
                // await _firebaseRemoteConfigService.FetchAndActivateAsync(); // As per spec
            }


            await LoadPlayerDataAsync();
            
            TransitionToInitialScreen();
        }

        private async Task InitializeFirebaseAsync()
        {
            // Responsibility: "Initializes Firebase" via FirebaseServiceInitializer
            // The spec says "Calls Firebase SDK initialization." which is handled by FirebaseServiceInitializer
            if (_firebaseServiceInitializer != null)
            {
                // await _firebaseServiceInitializer.InitializeFirebaseAsync(); // As per spec on FirebaseServiceInitializer
                Debug.Log("Firebase Initialization would be called here via FirebaseServiceInitializer.");
            }
        }

        private void InitializeCoreSystems()
        {
            // Responsibility: "Initializes non-async core managers."
            // GlobalEventBus: Singleton, usually self-initializes or GameInitializer ensures it's ready.
            // ScreenManager: Assumed to be set up.
            // InputHandler: Assumed to be set up.
            // SaveLoadService: Instantiated or found.
            // CloudSyncService: Instantiated or found.
            Debug.Log("Core Systems (GlobalEventBus, ScreenManager, InputHandler, SaveLoadService, CloudSyncService) initialized.");
        }

        private async Task LoadPlayerDataAsync()
        {
            // Responsibility: "Loads player data from local and syncs with cloud."
            if (_saveLoadService != null)
            {
                // Example: await _saveLoadService.LoadPlayerProgressAsync();
                Debug.Log("Player Data Loading (local) would be called here.");
            }
            if (_cloudSyncService != null)
            {
                // Example: await _cloudSyncService.SyncAsync();
                Debug.Log("Player Data Sync (cloud) would be called here.");
            }
            await Task.CompletedTask; // Placeholder
        }

        private void TransitionToInitialScreen()
        {
            // Responsibility: "Decides and shows the initial screen using ScreenManager."
            if (_screenManager != null)
            {
                // Example: _screenManager.ShowOnlyScreen<MainMenuController>(); // Assuming MainMenuController exists
                Debug.Log("Transitioning to initial screen.");
            }
            else
            {
                Debug.LogError("ScreenManager not found for initial screen transition.");
            }
        }
    }
}