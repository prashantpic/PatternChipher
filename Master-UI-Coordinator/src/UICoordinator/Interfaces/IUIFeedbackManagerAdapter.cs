using UnityEngine; // For Vector3

namespace PatternCipher.UI.Coordinator.Interfaces
{
    /// <summary>
    /// Adapter interface for the UIFeedbackManager component from UI-Component-Library.
    /// Defines a contract for the UICoordinatorService to trigger UI feedback.
    /// </summary>
    public interface IUIFeedbackManagerAdapter
    {
        /// <summary>
        /// Plays a general UI feedback effect (e.g., visual animation, particle effect).
        /// </summary>
        /// <param name="feedbackKey">Identifier for the feedback to play.</param>
        /// <param name="position">Optional world position for positional feedback effects.</param>
        void PlayFeedback(string feedbackKey, Vector3? position);

        /// <summary>
        /// Plays a specific UI sound effect.
        /// </summary>
        /// <param name="soundEffectKey">Identifier for the sound effect to play.</param>
        void PlayUISound(string soundEffectKey);

        /// <summary>
        /// Triggers a haptic feedback effect.
        /// This method should respect the user's haptic feedback accessibility settings.
        /// </summary>
        /// <param name="hapticTypeKey">Identifier for the type of haptic feedback (e.g., "light", "medium", "success", "error").</param>
        void TriggerHaptic(string hapticTypeKey);
    }
}