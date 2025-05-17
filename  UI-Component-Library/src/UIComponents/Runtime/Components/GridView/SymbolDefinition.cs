using UnityEngine;
using UnityEngine.AddressableAssets; // Required for AssetReferenceSprite and AssetReferenceTexture2D

namespace PatternCipher.UI.Components.GridView
{
    /// <summary>
    /// ScriptableObject defining the properties of a unique symbol used in the game grid.
    /// This data container ensures symbols are distinguishable (REQ-UIX-013.1) through various visual attributes
    /// like color, shape, icon, and texture, contributing to overall visual clarity (REQ-UIX-015).
    /// </summary>
    [CreateAssetMenu(fileName = "SymbolDefinition", menuName = "PatternCipher/UI/Gameplay/Symbol Definition")]
    public class SymbolDefinition : ScriptableObject
    {
        [Tooltip("Unique identifier for this symbol (e.g., 'circle_red', 'square_blue_striped').")]
        public string SymbolId;

        [Tooltip("The primary base color associated with this symbol.")]
        public Color BaseColor = Color.white;

        [Tooltip("Addressable asset reference to the main icon sprite for this symbol.")]
        public AssetReferenceSprite IconSpriteAddress;

        [Tooltip("Data defining the symbol's shape, could be a key for procedural generation or a descriptive tag (e.g., 'circle', 'triangle'). This helps distinguish symbols beyond color alone.")]
        public string ShapeData;

        [Tooltip("Addressable asset reference to an optional texture/pattern overlay for this symbol. Adds another layer of visual distinction.")]
        public AssetReferenceTexture2D TextureAddress;

        [Tooltip("Accessibility label for this symbol, used for screen readers or alternative text descriptions (e.g., 'Red Circle with Stripes').")]
        public string AccessibilityLabel;

#if UNITY_EDITOR
        void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(SymbolId) && !string.IsNullOrWhiteSpace(name))
            {
                SymbolId = name.Replace(" ", "_").ToLowerInvariant();
            }
            if (string.IsNullOrWhiteSpace(AccessibilityLabel) && !string.IsNullOrWhiteSpace(SymbolId))
            {
                // Attempt to generate a basic accessibility label from SymbolId
                AccessibilityLabel = System.Text.RegularExpressions.Regex.Replace(SymbolId, "_", " ");
                AccessibilityLabel = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(AccessibilityLabel);
            }
        }
#endif
    }
}