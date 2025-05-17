using UnityEngine;
using System.Threading.Tasks;

// Assuming these dependencies exist or will be created:
// - PatternCipher.UI.Services.IAddressableAssetService (Interface)
// - PatternCipher.UI.Components.SymbolDefinition (ScriptableObject C# class)
//   with fields like IconSpriteAddress (AssetReferenceSprite) and TextureAddress (AssetReferenceTexture2D)

namespace PatternCipher.UI.Services
{
    public class SymbolAssetResolver
    {
        private readonly IAddressableAssetService _addressableAssetService;

        public SymbolAssetResolver(IAddressableAssetService addressableAssetService)
        {
            _addressableAssetService = addressableAssetService ?? throw new System.ArgumentNullException(nameof(addressableAssetService));
        }

        public async Task<Sprite> ResolveSpriteAsync(Components.SymbolDefinition symbolDef)
        {
            if (symbolDef == null)
            {
                Debug.LogError("[SymbolAssetResolver] SymbolDefinition is null.");
                return null;
            }

            if (symbolDef.IconSpriteAddress == null || !symbolDef.IconSpriteAddress.RuntimeKeyIsValid())
            {
                // Debug.LogWarning($"[SymbolAssetResolver] IconSpriteAddress is null or invalid for SymbolId: {symbolDef.SymbolId}");
                return null; 
            }
            
            if (!_addressableAssetService.IsValid())
            {
                Debug.LogError("[SymbolAssetResolver] AddressableAssetService is not valid/initialized.");
                return null;
            }

            try
            {
                return await _addressableAssetService.LoadAssetAsync<Sprite>(symbolDef.IconSpriteAddress);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SymbolAssetResolver] Failed to load sprite for SymbolId {symbolDef.SymbolId}: {e.Message}");
                return null;
            }
        }

        public async Task<Texture2D> ResolveTextureAsync(Components.SymbolDefinition symbolDef)
        {
            if (symbolDef == null)
            {
                Debug.LogError("[SymbolAssetResolver] SymbolDefinition is null.");
                return null;
            }

            if (symbolDef.TextureAddress == null || !symbolDef.TextureAddress.RuntimeKeyIsValid())
            {
                // Debug.LogWarning($"[SymbolAssetResolver] TextureAddress is null or invalid for SymbolId: {symbolDef.SymbolId}");
                return null;
            }

            if (!_addressableAssetService.IsValid())
            {
                Debug.LogError("[SymbolAssetResolver] AddressableAssetService is not valid/initialized.");
                return null;
            }

            try
            {
                return await _addressableAssetService.LoadAssetAsync<Texture2D>(symbolDef.TextureAddress);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SymbolAssetResolver] Failed to load texture for SymbolId {symbolDef.SymbolId}: {e.Message}");
                return null;
            }
        }
    }
}