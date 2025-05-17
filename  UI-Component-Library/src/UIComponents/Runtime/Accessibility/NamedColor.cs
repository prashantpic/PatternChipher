using UnityEngine;

namespace PatternCipher.UI.Accessibility
{
    /// <summary>
    /// Represents a color with an associated name for use in ColorPaletteDefinitions.
    /// Implements REQ-UIX-013.
    /// </summary>
    [System.Serializable]
    public struct NamedColor
    {
        /// <summary>
        /// The identifying name of the color (e.g., "primary-background", "accent-text").
        /// </summary>
        public string Name;

        /// <summary>
        /// The actual color value.
        /// </summary>
        public Color Value;
    }
}