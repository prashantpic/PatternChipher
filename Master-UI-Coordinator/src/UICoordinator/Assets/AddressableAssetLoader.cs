using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
using System;

// Assuming IAssetLoader interface exists
// namespace PatternCipher.UI.Coordinator.Assets { public interface IAssetLoader { /* methods */ } }

namespace PatternCipher.UI.Coordinator.Assets
{
    public class AddressableAssetLoader : IAssetLoader // Inferred from SDS
    {
        // NFR-P-001: Performance of asset loading is critical. Addressables handles this well.
        // Timeout logic can be added if UICoordinatorConfig provides timeouts.

        public async Task<T> LoadAssetAsync<T>(string key) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("AddressableAssetLoader: Asset key is null or empty.");
                return null;
            }

            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key);
            try
            {
                await handle.Task;
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    return handle.Result;
                }
                else
                {
                    Debug.LogError($"AddressableAssetLoader: Failed to load asset with key '{key}'. Error: {handle.OperationException}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"AddressableAssetLoader: Exception while loading asset '{key}'. Error: {ex.Message}");
                // Addressables.Release(handle); // Release if an exception occurs before success. This is tricky with await.
                // If handle.Task throws, it might not be necessary or safe to release here.
                // Generally, if LoadAssetAsync fails, the handle might not be valid for release or might auto-release.
                return null;
            }
            // Note: The caller is responsible for releasing the asset handle if T is not a GameObject instance
            // or if manual reference counting is used for non-GameObject assets.
            // For GameObjects loaded via InstantiateAsync, Addressables.ReleaseInstance is used.
        }

        public async Task<GameObject> LoadGameObjectAsync(string key, Transform parent = null, bool instantiateInWorldSpace = false)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("AddressableAssetLoader: GameObject key is null or empty.");
                return null;
            }
            
            AsyncOperationHandle<GameObject> handle;
            if (parent != null)
            {
                 handle = Addressables.InstantiateAsync(key, parent, instantiateInWorldSpace);
            }
            else
            {
                 handle = Addressables.InstantiateAsync(key);
            }

            try
            {
                await handle.Task;
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    return handle.Result;
                }
                else
                {
                    Debug.LogError($"AddressableAssetLoader: Failed to instantiate GameObject with key '{key}'. Error: {handle.OperationException}");
                    return null;
                }
            }
            catch(Exception ex)
            {
                Debug.LogError($"AddressableAssetLoader: Exception while instantiating GameObject '{key}'. Error: {ex.Message}");
                return null;
            }
        }

        public void ReleaseAsset<T>(T asset) where T : UnityEngine.Object
        {
            if (asset == null) return;

            if (asset is GameObject)
            {
                // For GameObjects instantiated via Addressables.InstantiateAsync,
                // Addressables.ReleaseInstance should be used.
                // However, this method signature is generic. If it was specifically
                // ReleaseGameObjectInstance(GameObject instance), it would be clearer.
                // If 'asset' is a prefab loaded by LoadAssetAsync<GameObject>, then Addressables.Release(asset) is correct.
                // This ambiguity suggests separate methods or clearer contracts.
                // For now, assuming this is for assets loaded with LoadAssetAsync, not InstantiateAsync.
                Debug.LogWarning("AddressableAssetLoader: ReleaseAsset<GameObject> called. If this GameObject was instantiated via Addressables, use ReleaseInstance instead.");
                Addressables.Release(asset);
            }
            else
            {
                Addressables.Release(asset);
            }
        }
        
        public bool ReleaseInstance(GameObject instance)
        {
            if (instance == null) return false;
            return Addressables.ReleaseInstance(instance);
        }

        // To release assets loaded via LoadAssetAsync (not GameObjects instantiated):
        public void ReleaseAssetHandle<T>(AsyncOperationHandle<T> handle) where T : UnityEngine.Object
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }
        }
        
        public void ReleaseAssetKey(string key)
        {
            // This is not directly supported by Addressables in a simple way.
            // Addressables releases based on the loaded asset object or its handle.
            // To implement this, one would need to keep track of loaded assets/handles by key.
            // For simplicity, this is omitted as it's not standard Addressables usage.
            Debug.LogWarning("AddressableAssetLoader: ReleaseAssetKey is not implemented due to Addressables API design.");
        }
    }
}