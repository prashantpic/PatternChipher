using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
using PatternCipher.UI.Accessibility; // For IAccessibilitySettingsProvider
using PatternCipher.UI.Services.AssetLoading; // For IAddressableAssetService

namespace PatternCipher.UI.Components.Feedback
{
    public class ParticleEffectPlayer : MonoBehaviour
    {
        private IAddressableAssetService _addressableService;
        private IAccessibilitySettingsProvider _accessibilitySettings;

        public void Initialize(IAddressableAssetService assetService, IAccessibilitySettingsProvider accessibilitySettings)
        {
            _addressableService = assetService;
            _accessibilitySettings = accessibilitySettings;
        }

        public async Task PlayEffect(AssetReferenceGameObject particlePrefabRef, Vector3 position, Quaternion rotation)
        {
            if (_addressableService == null)
            {
                Debug.LogError("ParticleEffectPlayer: AddressableAssetService not initialized.", this);
                return;
            }
            if (_accessibilitySettings == null)
            {
                Debug.LogWarning("ParticleEffectPlayer: AccessibilitySettingsProvider not initialized. Cannot check reduced motion.", this);
            }

            bool reducedMotion = _accessibilitySettings?.CurrentAccessibilityProfile?.EnableReducedMotion ?? false;

            if (reducedMotion && particlePrefabRef.RuntimeKeyIsValid()) // REQ-UIX-013.4
            {
                // Optionally, log that particle effect was skipped due to reduced motion
                // Or play a very minimal, non-distracting version if available
                Debug.Log($"Particle effect '{particlePrefabRef.AssetGUID}' skipped due to reduced motion.");
                return;
            }

            if (!particlePrefabRef.RuntimeKeyIsValid())
            {
                Debug.LogWarning($"ParticleEffectPlayer: AssetReference for particle effect is not valid.", this);
                return;
            }
            
            GameObject prefab = null;
            try
            {
                prefab = await _addressableService.LoadAssetAsync<GameObject>(particlePrefabRef);
                if (prefab != null)
                {
                    GameObject instance = Instantiate(prefab, position, rotation);
                    ParticleSystem ps = instance.GetComponent<ParticleSystem>();
                    if (ps != null)
                    {
                        // Automatically destroy after duration (or use a pool)
                        // Ensure StopAction is set to Destroy or Disable on the ParticleSystem component
                        if (ps.main.stopAction != ParticleSystemStopAction.Destroy && ps.main.stopAction != ParticleSystemStopAction.Disable)
                        {
                            Debug.LogWarning($"ParticleEffectPlayer: ParticleSystem '{prefab.name}' StopAction is not Destroy or Disable. It might not self-release properly.", instance);
                            // Add a self-destruct script if necessary
                            Destroy(instance, ps.main.duration + ps.main.startLifetime.constantMax); 
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"ParticleEffectPlayer: Prefab '{prefab.name}' does not have a ParticleSystem component.", instance);
                        Destroy(instance, 5f); // Fallback destroy
                    }
                    // Do not release the prefab here if it's used multiple times rapidly, unless service handles ref counting.
                    // If service does not handle ref counting, Addressables.ReleaseInstance(instance) is needed when particle finishes.
                    // Or Addressables.Release(prefab) when no longer needed at all.
                    // For now, assume the service or a pooling system handles release.
                    // If simple instantiation, we might want to release the loaded asset if it's not cached by the service.
                    // _addressableService.ReleaseAsset(particlePrefabRef); // Or ReleaseAsset(prefab)
                    // This depends on IAddressableAssetService implementation details.
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"ParticleEffectPlayer: Failed to load or play particle effect '{particlePrefabRef.AssetGUID}': {ex.Message}", this);
                if (prefab != null)
                {
                    // If loading succeeded but instantiation failed, release the loaded asset
                    _addressableService.ReleaseAsset(particlePrefabRef);
                }
            }
        }
    }
}