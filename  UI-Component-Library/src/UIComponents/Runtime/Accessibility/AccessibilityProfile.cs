using UnityEngine;
using System.Collections.Generic;
using PatternCipher.UI.Accessibility.Enums; // Assuming Enums will be in this sub-namespace

namespace PatternCipher.UI.Accessibility
{
    /// <summary>
    /// ScriptableObject defining a complete set of accessibility configurations.
    /// Holds data for various accessibility settings (REQ-UIX-013) that can be applied globally or to specific UI contexts.
    /// </summary>
    [CreateAssetMenu(fileName = "AccessibilityProfile", menuName = "PatternCipher/UI/Accessibility/Accessibility Profile")]
    public class AccessibilityProfile : ScriptableObject
    {
        [Tooltip("Descriptive name for this accessibility profile (e.g., Default, High Contrast).")]
        public string ProfileName = "Default";

        [Tooltip("The base color palette to be used with this profile.")]
        public ColorPaletteDefinition BaseColorPalette;

        [Tooltip("The specific color vision deficiency mode this profile primarily targets or a general mode like 'Normal' or 'HighContrast'.")]
        public ColorVisionDeficiencyMode ColorVisionMode = ColorVisionDeficiencyMode.Normal;

        [Tooltip("An optional override color palette for specific color vision deficiency modes. If null, BaseColorPalette is used or modified programmatically.")]
        public ColorPaletteDefinition ColorBlindPaletteOverride;

        [Tooltip("Enable to reduce or disable non-essential UI animations and visual effects.")]
        public bool EnableReducedMotion = false;

        [Tooltip("Enable to allow haptic feedback for interactions.")]
        public bool EnableHaptics = true;

        [Tooltip("Multiplier for global text size. 1.0 is default size.")]
        [Range(0.5f, 3.0f)]
        public float TextSizeMultiplier = 1.0f;
    }
}