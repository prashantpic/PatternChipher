using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PatternCipher.UI.Services
{
    /// <summary>
    /// Service for loading assets (icons, audio clips, prefabs, shaders) 
    /// via the Unity Addressables system. Implements IAddressableAssetService.
    /// Centralizes Addressable asset loading logic for use by UI components and other services.
    /// </summary>
    public class AddressableAssetService : IAddressableAssetService
    {
        // Keep track of loaded assets to release them properly.
        // Using object as key since AssetReference itself can be a key, or a string path.
        private readonly Dictionary<object, AsyncOperationHandle> _loadedAssetHandles = new Dictionary<object, AsyncOperationHandle>();
        private readonly Dictionary<object, int> _referenceCounts = new Dictionary<object, int>();


        public AddressableAssetService()
        {
            // Initialize Addressables if not already done.
            // Usually, Addressables initializes itself, but explicit initialization can be done here if needed.
            // Addressables.InitializeAsync();
        }

        /// <summary>
        /// Asynchronously loads an asset using its AssetReference.
        /// </summary>
        /// <typeparam name="T">The type of asset to load.</typeparam>
        /// <param name="assetReference">The AssetReference pointing to the asset.</param>
        /// <returns>A task that resolves to the loaded asset, or null if loading fails or reference is invalid.</returns>
        public async Task<T> LoadAssetAsync<T>(AssetReference assetReference) where T : Object
        {
            if (assetReference == null || !assetReference.RuntimeKeyIsValid())
            {
                Debug.LogError($"[AddressableAssetService] Invalid or null AssetReference provided.");
                return null;
            }

            object key = assetReference.RuntimeKey;
            if (_loadedAssetHandles.TryGetValue(key, out var existingHandle))
            {
                _referenceCounts[key]++;
                return existingHandle.Result as T;
            }

            AsyncOperationHandle<T> handle = assetReference.LoadAssetAsync<T>();
            _loadedAssetHandles[key] = handle;
            _referenceCounts[key] = 1;

            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                return handle.Result;
            }
            else
            {
                Debug.LogError($"[AddressableAssetService] Failed to load asset with key: {key}. Error: {handle.OperationException}");
                _loadedAssetHandles.Remove(key); // Clean up failed load
                _referenceCounts.Remove(key);
                Addressables.Release(handle); // Release the failed handle
                return null;
            }
        }

        /// <summary>
        /// Asynchronously loads an asset using its string key (address).
        /// </summary>
        /// <typeparam name="T">The type of asset to load.</typeparam>
        /// <param name="key">The string key (address) of the asset.</param>
        /// <returns>A task that resolves to the loaded asset, or null if loading fails or key is invalid.</returns>
        public async Task<T> LoadAssetAsync<T>(string key) where T : Object
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError($"[AddressableAssetService] Invalid or null key provided for asset loading.");
                return null;
            }

            if (_loadedAssetHandles.TryGetValue(key, out var existingHandle))
            {
                 _referenceCounts[key]++;
                return existingHandle.Result as T;
            }

            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key);
            _loadedAssetHandles[key] = handle;
            _referenceCounts[key] = 1;

            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                return handle.Result;
            }
            else
            {
                Debug.LogError($"[AddressableAssetService] Failed to load asset with key: {key}. Error: {handle.OperationException}");
                _loadedAssetHandles.Remove(key); // Clean up failed load
                _referenceCounts.Remove(key);
                Addressables.Release(handle); // Release the failed handle
                return null;
            }
        }

        /// <summary>
        /// Releases an asset instance or an asset loaded by its key/AssetReference.
        /// Decrements reference count and releases if count reaches zero.
        /// </summary>
        /// <param name="assetInstanceOrKey">
        /// The asset instance (Object) returned by LoadAssetAsync, 
        /// its AssetReference, or its string key.
        /// </param>
        public void ReleaseAsset(object assetInstanceOrKey)
        {
            if (assetInstanceOrKey == null)
            {
                Debug.LogWarning("[AddressableAssetService] Attempted to release a null asset or key.");
                return;
            }

            object keyToRelease = null;

            if (assetInstanceOrKey is AssetReference assetRef)
            {
                keyToRelease = assetRef.RuntimeKey;
            }
            else if (assetInstanceOrKey is string strKey)
            {
                keyToRelease = strKey;
            }
            else if (assetInstanceOrKey is Object) // Could be the actual asset instance
            {
                // Find the key associated with this instance
                foreach (var kvp in _loadedAssetHandles)
                {
                    if (kvp.Value.IsValid() && kvp.Value.Result == assetInstanceOrKey)
                    {
                        keyToRelease = kvp.Key;
                        break;
                    }
                }
                if (keyToRelease == null)
                {
                     Debug.LogWarning($"[AddressableAssetService] Attempted to release an asset instance that was not found in loaded handles: {assetInstanceOrKey.GetType().Name}. Direct release of instance is less reliable; prefer releasing by key or AssetReference.");
                     // Fallback: If it's a Unity Object and not found by key, it might be an AssetReference's asset that needs release.
                     // However, Addressables.Release(Object) is generally for instances directly.
                     // This path is tricky without knowing how the asset was loaded.
                     // For robust releasing, the key used for loading should be used for releasing.
                     // Addressables.Release(assetInstanceOrKey as Object); // Use with caution.
                     return;
                }
            }


            if (keyToRelease != null && _loadedAssetHandles.TryGetValue(keyToRelease, out AsyncOperationHandle handle))
            {
                if (_referenceCounts.TryGetValue(keyToRelease, out int count))
                {
                    _referenceCounts[keyToRelease] = count - 1;
                    if (_referenceCounts[keyToRelease] <= 0)
                    {
                        Addressables.Release(handle);
                        _loadedAssetHandles.Remove(keyToRelease);
                        _referenceCounts.Remove(keyToRelease);
                        // Debug.Log($"[AddressableAssetService] Asset released and removed: {keyToRelease}");
                    }
                    // else
                    // {
                    //    Debug.Log($"[AddressableAssetService] Decremented ref count for asset: {keyToRelease}. New count: {_referenceCounts[keyToRelease]}");
                    // }
                }
            }
            else
            {
                Debug.LogWarning($"[AddressableAssetService] Attempted to release an asset not found or already released: {keyToRelease ?? assetInstanceOrKey.ToString()}");
            }
        }

        /// <summary>
        /// Releases all assets loaded through this service.
        /// Typically called during scene transitions or application shutdown.
        /// </summary>
        public void ReleaseAllAssets()
        {
            foreach (var handle in _loadedAssetHandles.Values)
            {
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }
            }
            _loadedAssetHandles.Clear();
            _referenceCounts.Clear();
            Debug.Log("[AddressableAssetService] All tracked assets released.");
        }

        // Optional: Implement IDisposable if managing unmanaged resources directly,
        // or if Addressables requires specific cleanup on service destruction.
        // For now, relying on Addressables own management and ReleaseAllAssets.
    }
}