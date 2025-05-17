using UnityEngine.AddressableAssets;
using System.Threading.Tasks;
using UnityEngine; // Required for Object type constraint

namespace PatternCipher.UI.Services
{
    /// <summary>
    /// Defines the contract for a service that loads assets asynchronously
    /// using the Unity Addressables system.
    /// This interface allows for decoupling asset loading logic from its consumers.
    /// </summary>
    public interface IAddressableAssetService
    {
        /// <summary>
        /// Asynchronously loads an asset of type T using its AssetReference.
        /// </summary>
        /// <typeparam name="T">The type of the asset to load. Must be a Unity Object.</typeparam>
        /// <param name="assetReference">The AssetReference pointing to the asset.</param>
        /// <returns>A Task that, when completed, yields the loaded asset of type T, or null if loading fails.</returns>
        Task<T> LoadAssetAsync<T>(AssetReference assetReference) where T : Object;

        /// <summary>
        /// Asynchronously loads an asset of type T using its string key (address).
        /// </summary>
        /// <typeparam name="T">The type of the asset to load. Must be a Unity Object.</typeparam>
        /// <param name="key">The string key (address) of the asset.</param>
        /// <returns>A Task that, when completed, yields the loaded asset of type T, or null if loading fails.</returns>
        Task<T> LoadAssetAsync<T>(string key) where T : Object;

        /// <summary>
        /// Releases an asset that was previously loaded through this service.
        /// It's important to release assets when they are no longer needed to free up memory.
        /// </summary>
        /// <param name="assetInstanceOrKey">
        /// The asset instance (Object) that was returned by LoadAssetAsync,
        /// or the AssetReference or string key that was used to load it.
        /// Releasing by key or AssetReference is generally more robust for reference counting.
        /// </param>
        void ReleaseAsset(object assetInstanceOrKey);
    }
}