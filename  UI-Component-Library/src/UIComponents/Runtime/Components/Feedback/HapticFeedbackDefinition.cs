using UnityEngine;
using PatternCipher.UI.Components.Feedback.Enums; // Assuming HapticTypeEnum will be here

namespace PatternCipher.UI.Components.Feedback
{
    /// <summary>
    /// ScriptableObject defining properties for a haptic feedback event.
    /// This data container stores configurations for haptic patterns, intensity, and duration,
    /// supporting the haptics toggle feature from accessibility settings (REQ-UIX-013.5).
    /// Note: Actual haptic capabilities depend on the platform and any haptic plugins used.
    /// </summary>
    [CreateAssetMenu(fileName = "HapticFeedback", menuName = "PatternCipher/UI/Feedback/Haptic Feedback Definition")]
    public class HapticFeedbackDefinition : ScriptableObject
    {
        [Tooltip("The type or pattern of haptic feedback to trigger (e.g., LightTap, Success, Failure).")]
        public HapticTypeEnum HapticType = HapticTypeEnum.LightTap;

        [Tooltip("Intensity of the haptic feedback. Range and effect depend on the haptic engine. Typically 0.0 to 1.0.")]
        [Range(0f, 1f)]
        public float Intensity = 0.5f;

        [Tooltip("Duration of the haptic feedback in seconds, if controllable by the haptic engine.")]
        public float Duration = 0.1f; // Sensible default for a light tap
    }
}