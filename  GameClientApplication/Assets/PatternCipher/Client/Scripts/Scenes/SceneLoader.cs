using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PatternCipher.Client.Scenes
{
    /// <summary>
    /// Provides a centralized and robust way to manage scene transitions.
    /// It abstracts the complexity of Unity's SceneManager, handling asynchronous loading
    /// and providing hooks (via the GameEventSystem) to manage a loading screen
    /// for a smooth user experience.
    /// </summary>
    public class SceneLoader
    {
        /// <summary>
        /// Loads a scene asynchronously, preventing the UI from freezing.
        /// This method can be awaited to ensure code executes only after the scene is fully loaded.
        /// </summary>
        /// <param name="sceneId">The enum identifier of the scene to load.</param>
        /// <param name="mode">The scene loading mode (Single replaces all scenes, Additive adds the new one).</param>
        /// <param name="onLoaded">An optional callback to invoke after the scene has loaded and faded in.</param>
        public async Task LoadSceneAsync(SceneId sceneId, LoadSceneMode mode = LoadSceneMode.Single, Action onLoaded = null)
        {
            try
            {
                // Here, you would typically trigger a UI fade-out or show a loading screen.
                // This is done via the event system to keep the SceneLoader decoupled from the UI implementation.
                // GameEventSystem.Publish(new ShowLoadingScreenEvent());
                Debug.Log($"Starting to load scene: {sceneId}");

                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneId.ToString(), mode);

                // Wait until the asynchronous scene fully loads
                while (!asyncLoad.isDone)
                {
                    // You can get progress from asyncLoad.progress and update a loading bar here.
                    await Task.Yield(); // Yield control back to the main thread to avoid freezing.
                }

                Debug.Log($"Scene {sceneId} loaded successfully.");
                
                // Once the scene is loaded, hide the loading screen and/or fade in.
                // GameEventSystem.Publish(new HideLoadingScreenEvent());

                // Invoke the callback if one was provided.
                onLoaded?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load scene {sceneId}. Exception: {ex.Message}");
                // Handle the error gracefully, e.g., show an error message to the user.
            }
        }
    }
}