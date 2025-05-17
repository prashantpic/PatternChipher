using System.Threading.Tasks;
using UnityEngine; // For Object, GameObject, Transform

namespace PatternCipher.UI.Coordinator.Assets
{
    /// <summary>
    /// Interface for asynchronous loading of UI assets, typically using Unity's Addressables system.
    /// </summary>
    public interface IAssetLoader
    {
        /// <summary>
        /// Asynchronously loads an asset of a specified type using its key.
        /// </summary>
        /// <typeparam name="T">The type of asset to load (must be a UnityEngine.Object).</typeparam>
        /// <param name="key">The Addressable key or path of the asset.</param>
        /// <returns>A task that resolves to the loaded asset, or null if loading fails.</returns>
        Task<T> LoadAssetAsync<T>(string key) where T : Object;

        /// <summary>
        /// Asynchronously loads and instantiates a GameObject asset using its key.
        /// </summary>
        /// <param name="key">The Addressable key or path of the GameObject prefab.</param>
        /// <param name="parent">Optional parent Transform for the instantiated GameObject. If null, it will be instantiated at the root.</param>
        /// <returns>A task that resolves to the instantiated GameObject, or null if loading or instantiation fails.</returns>
        Task<GameObject> LoadGameObjectAsync(string key, Transform parent = null);

        /// <summary>
        /// Releases a previously loaded asset instance to free up memory.
        /// For GameObjects instantiated via LoadGameObjectAsync, this should handle Addressables.ReleaseInstance.
        /// For assets loaded via LoadAssetAsync, this should handle Addressables.Release.
        /// </summary>
        /// <param name="assetInstance">The asset instance (e.g., GameObject or ScriptableObject) to release.</param>
        void ReleaseAsset(object assetInstance);
    }
}