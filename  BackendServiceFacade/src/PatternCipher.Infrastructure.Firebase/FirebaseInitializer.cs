using Firebase;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace PatternCipher.Infrastructure.Firebase
{
    /// <summary>
    /// Ensures the Firebase SDK is initialized safely and asynchronously at game startup.
    /// This script should be attached to a persistent GameObject in the initial scene.
    /// </summary>
    public class FirebaseInitializer : MonoBehaviour
    {
        private static TaskCompletionSource<bool> _initializationTcs;

        /// <summary>
        /// Asynchronously initializes Firebase. Subsequent calls will return the task of the initial call.
        /// This is the main entry point for other services to ensure Firebase is ready.
        /// </summary>
        /// <returns>A task that completes with 'true' if initialization is successful, and 'false' otherwise.</returns>
        public static Task<bool> InitializeAsync()
        {
            if (_initializationTcs == null)
            {
                // This case should ideally not be hit if the FirebaseInitializer MonoBehaviour is in the scene.
                // It's a fallback for editor scripts or contexts without the scene object.
                // A new GameObject is created to run the initialization coroutine.
                var go = new GameObject("FirebaseInitializer_Runtime");
                go.AddComponent<FirebaseInitializer>();
                DontDestroyOnLoad(go);
            }
            return _initializationTcs.Task;
        }

        private void Awake()
        {
            // Standard singleton pattern to ensure only one initializer exists.
            if (_initializationTcs != null)
            {
                Destroy(gameObject);
                return;
            }

            // Make this object persistent across scene loads.
            DontDestroyOnLoad(gameObject);
            _initializationTcs = new TaskCompletionSource<bool>();
            InitializeFirebaseAsync();
        }

        private async void InitializeFirebaseAsync()
        {
            try
            {
                var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
                if (dependencyStatus == DependencyStatus.Available)
                {
                    // Firebase is ready for use.
                    Debug.Log("Firebase SDK initialized successfully.");
                    _initializationTcs.SetResult(true);
                }
                else
                {
                    Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                    _initializationTcs.SetResult(false);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.LogError($"Firebase initialization failed with an exception.");
                _initializationTcs.SetResult(false);
            }
        }
    }
}