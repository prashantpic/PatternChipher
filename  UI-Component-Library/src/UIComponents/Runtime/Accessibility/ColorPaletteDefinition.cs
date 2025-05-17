using UnityEngine;
using System.Collections.Generic;
using System; // Required for [Serializable]

namespace PatternCipher.UI.Accessibility
{
    /// <summary>
    /// Represents a named color, primarily for use within a ColorPaletteDefinition.
    /// </summary>
    [Serializable]
    public struct NamedColor
    {
        [Tooltip("Name used to reference this color (e.g., 'primary-background', 'accent-text').")]
        public string Name;
        [Tooltip("The actual color value.")]
        public Color Value;
    }

    /// <summary>
    /// ScriptableObject defining a color palette for UI theming or specific accessibility modes.
    /// Stores a set of named colors for UI elements, facilitating theming and colorblind-friendly options as per REQ-UIX-013.
    /// </summary>
    [CreateAssetMenu(fileName = "ColorPalette", menuName = "PatternCipher/UI/Accessibility/Color Palette")]
    public class ColorPaletteDefinition : ScriptableObject
    {
        [Tooltip("Descriptive name for this color palette (e.g., Default Theme, Deuteranopia Adapted).")]
        public string PaletteName = "DefaultPalette";

        [Tooltip("List of named colors defined in this palette.")]
        public List<NamedColor> Colors = new List<NamedColor>();

        /// <summary>
        /// Attempts to find a color by its name within the palette.
        /// </summary>
        /// <param name="colorName">The name of the color to find.</param>
        /// <param name="color">The output color if found.</param>
        /// <returns>True if the color was found, false otherwise.</returns>
        public bool TryGetColor(string colorName, out Color color)
        {
            foreach (var namedColor in Colors)
            {
                if (namedColor.Name.Equals(colorName, StringComparison.OrdinalIgnoreCase))
                {
                    color = namedColor.Value;
                    return true;
                }
            }
            color = Color.magenta; // Default fallback / error color
            Debug.LogWarning($"Color name '{colorName}' not found in palette '{PaletteName}'. Returning magenta.");
            return false;
        }
    }
}