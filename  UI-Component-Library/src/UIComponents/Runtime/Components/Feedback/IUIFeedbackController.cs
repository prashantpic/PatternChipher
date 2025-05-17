using UnityEngine.UIElements; // Required for VisualElement
using PatternCipher.UI.Components.Feedback.Enums; // Assuming FeedbackEvent and other enums will be here

namespace PatternCipher.UI.Components.Feedback
{
    /// <summary>
    /// Defines the contract for the UI Feedback system.
    /// This interface allows other parts of the application to request various types of feedback
    /// (audio, visual, haptic) in a decoupled manner.
    /// Implements requirements: REQ-6-003 (Audio), REQ-6-007 (Visual), REQ-UIX-013 (Haptics Toggle, Reduced Motion indirectly).
    /// </summary>
    public interface IUIFeedbackController
    {
        /// <summary>
        /// Plays a predefined feedback sequence associated with a specific game event.
        /// </summary>
        /// <param name="gameEvent">The game event that triggers the feedback.</param>
        void PlayFeedbackForEvent(FeedbackEvent gameEvent);

        /// <summary>
        /// Plays a sound based on the provided audio feedback definition.
        /// </summary>
        /// <param name="soundDef">The ScriptableObject defining the sound to play.</param>
        void PlaySound(AudioFeedbackDefinition soundDef);

        /// <summary>
        /// Plays a visual effect on a target UI element based on the provided visual feedback definition.
        /// </summary>
        /// <param name="targetElement">The UI Toolkit VisualElement to apply the effect to.</param>
        /// <param name="visualDef">The ScriptableObject defining the visual effect.</param>
        void PlayVisualEffect(VisualElement targetElement, VisualFeedbackDefinition visualDef);

        /// <summary>
        /// Triggers haptic feedback based on the provided haptic feedback definition.
        /// </summary>
        /// <param name="hapticDef">The ScriptableObject defining the haptic feedback.</param>
        void TriggerHaptic(HapticFeedbackDefinition hapticDef);
    }
}