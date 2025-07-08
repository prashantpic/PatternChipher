using UnityEngine;

namespace PatternCipher.Client.Application
{
    /// <summary>
    /// The AppInitializer script is responsible for the initial setup of the game.
    /// It is the primary bootstrap script for the entire application. It runs in the 
    /// initial "Bootstrap" scene and is responsible for instantiating the GameManager 
    /// prefab, which contains all persistent managers and services. This ensures a 
    /// controlled and orderly startup sequence for the entire game.
    /// </summary>
    public class AppInitializer : MonoBehaviour
    {
        /// <summary>
        /// A reference to the prefab containing the GameManager and other persistent managers.
        /// This must be assigned in the Unity Inspector.
        /// </summary>
        [SerializeField]
        [Tooltip("The prefab containing the GameManager and other core, persistent services.")]
        private GameObject _gameManagerPrefab;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// This method ensures that the core GameManager is initialized.
        /// </summary>
        private void Awake()
        {
            // Check if a GameManager instance already exists. This handles cases where
            // the bootstrap scene is reloaded, preventing duplicate GameManagers.
            if (GameManager.Instance != null)
            {
                return;
            }

            // The GameManager prefab is essential for the game to run.
            // If it's not assigned, we log a critical error and stop.
            if (_gameManagerPrefab == null)
            {
                Debug.LogError("FATAL: _gameManagerPrefab is not assigned in the AppInitializer script on the Bootstrap scene. The game cannot start.");
                // In a real build, you might want to display a user-friendly error and quit.
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #else
                Application.Quit();
                #endif
                return;
            }
            
            // Instantiate the persistent GameManager prefab. The Awake method of the
            // GameManager script on this prefab will then handle its own setup,
            // including the Singleton pattern and DontDestroyOnLoad.
            Instantiate(_gameManagerPrefab);
        }
    }
}