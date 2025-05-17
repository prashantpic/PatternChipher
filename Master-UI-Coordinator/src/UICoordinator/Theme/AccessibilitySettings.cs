using UnityEngine;

namespace PatternCipher.UI.Coordinator.Theme
{
    // Assuming ColorblindTypeEnum exists in PatternCipher.UI.Coordinator.Theme namespace
    // public enum ColorblindTypeEnum { Normal, Protanopia, Deuteranopia, Tritanopia }

    [CreateAssetMenu(fileName = "AccessibilitySettings", menuName = "PatternCipher/UI Coordinator/Accessibility Settings")]
    public class AccessibilitySettings : ScriptableObject
    {
        public ColorblindTypeEnum ColorblindMode = ColorblindTypeEnum.Normal;
        public bool HighContrastEnabled = false;
        public float BaseFontSizeMultiplier = 1.0f;
        public bool IsReducedMotionEnabled = false;
        public bool HapticFeedbackEnabled = true; // Default to true as per SDS NFRs/Implied
    }
}