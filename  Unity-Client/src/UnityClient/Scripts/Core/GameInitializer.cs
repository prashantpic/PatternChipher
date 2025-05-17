using UnityEngine;
using System.Threading.Tasks;
using PatternCipher.Client.Core.Events;
using PatternCipher.Client.Presentation.Screens;
using PatternCipher.Client.Presentation.Input;
using PatternCipher.Client.DataPersistence.Services;
using PatternCipher.Client.Infrastructure.Firebase;

namespace PatternCipher.Client.Core
{
    public class GameInitializer : MonoBehaviour
    {
        public static GameInitializer Instance { get; private set; }

        [Header("Core Services")]
        [SerializeField] private FirebaseServiceInitializer firebaseServiceInitializer;
        [SerializeField] private FirebaseRemoteConfigService firebaseRemoteConfigService; // As per detailed SDS
        // GlobalEventBus is a static singleton, no need to serialize
        [SerializeField] private ScreenManager screenManager;
        [SerializeField] private InputHandler inputHandler; // Assuming InputHandler is a MonoBehaviour service
        [SerializeField] private SaveLoadService saveLoadService;
        [SerializeField] private CloudSyncService cloudSyncService;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeCoreSystems(); // Sync initializations
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private async void Start()
        {
            await InitializeAsyncSystems();
        }

        private void InitializeCoreSystems()
        {
            // Initialize non-async core managers and systems that don't have external dependencies yet
            // e.g., GlobalEventBus is accessed via its static Instance property
            if (screenManager == null) Debug.LogError("ScreenManager is not assigned in GameInitializer.");
            if (inputHandler == null) Debug.LogError("InputHandler is not assigned in GameInitializer.");
            if (saveLoadService == null) Debug.LogError("SaveLoadService is not assigned in GameInitializer.");
            if (cloudSyncService == null) Debug.LogError("CloudSyncService is not assigned in GameInitializer.");
            
            // Further synchronous initializations if needed
            Debug.Log("Core systems initialized (synchronous part).");
        }

        private async Task InitializeAsyncSystems()
        {
            if (firebaseServiceInitializer == null)
            {
                Debug.LogError("FirebaseServiceInitializer is not assigned.");
            }
            else
            {
                 await InitializeFirebaseAsync();
            }

            if (firebaseRemoteConfigService == null)
            {
                Debug.LogError("FirebaseRemoteConfigService is not assigned.");
            }
            else
            {
                // Assuming FetchAndActivateAsync is part of its initialization or called here
                // await firebaseRemoteConfigService.FetchAndActivateAsync(); 
                Debug.Log("Firebase Remote Config Loaded (Placeholder).");
            }
            
            await LoadPlayerDataAsync();
            
            TransitionToInitialScreen();
            Debug.Log("Async systems initialization complete.");
        }
        
        private async Task InitializeFirebaseAsync()
        {
            if (firebaseServiceInitializer != null)
            {
                await firebaseServiceInitializer.InitializeFirebaseAsync();
                Debug.Log("Firebase initialized.");
            }
            else
            {
                Debug.LogError("FirebaseServiceInitializer is not assigned in GameInitializer.");
            }
        }

        private async Task LoadPlayerDataAsync()
        {
            if (saveLoadService != null)
            {
                // Example: var_playerData = await saveLoadService.LoadAsync<PlayerProfileData>("playerProfile.dat");
                Debug.Log("Player data loaded (local placeholder).");
            }
            if (cloudSyncService != null)
            {
                // await cloudSyncService.SyncAsync();
                Debug.Log("Player data synced with cloud (placeholder).");
            }
        }

        private void TransitionToInitialScreen()
        {
            if (screenManager != null)
            {
                // Example: screenManager.ShowOnlyScreen<MainMenuController>(); // Or some other initial screen
                Debug.Log("Transitioning to initial screen (placeholder).");
            }
            else
            {
                Debug.LogError("ScreenManager not available to transition to initial screen.");
            }
        }
    }
}