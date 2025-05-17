using PatternCipher.UI.Coordinator.Interfaces; // For IUIFeedbackManagerAdapter
using PatternCipher.UI.Coordinator.Theme;     // For IThemeEngine (to check HapticFeedbackEnabled)
using PatternCipher.UI;                       // For UIFeedbackManager (from UI-Component-Library)
using System;
using UnityEngine;                            // For Vector3, Debug.Log

namespace PatternCipher.UI.Coordinator.Adapters
{
    /// <summary>
    /// Concrete adapter for the UIFeedbackManager component from REPO-UI-COMPONENTS.
    /// Implements IUIFeedbackManagerAdapter to interact with the actual UIFeedbackManager.
    /// Translates calls from UICoordinatorService to trigger various UI feedbacks,
    /// respecting accessibility settings like HapticFeedbackEnabled.
    /// </summary>
    public class UIFeedbackManagerAdapter : IUIFeedbackManagerAdapter
    {
        private readonly PatternCipher.UI.UIFeedbackManager _uiFeedbackManagerInstance;
        private readonly IThemeEngine _themeEngine;

        public UIFeedbackManagerAdapter(
            PatternCipher.UI.UIFeedbackManager uiFeedbackManagerInstance,
            IThemeEngine themeEngine)
        {
            _uiFeedbackManagerInstance = uiFeedbackManagerInstance ?? throw new ArgumentNullException(nameof(uiFeedbackManagerInstance));
            _themeEngine = themeEngine ?? throw new ArgumentNullException(nameof(themeEngine));
        }

        public void PlayFeedback(string feedbackKey, Vector3? position = null)
        {
            if (_uiFeedbackManagerInstance == null) return;

            // Assuming _uiFeedbackManagerInstance has a method like PlayVisualFeedback(string key, Vector3? pos)
            // _uiFeedbackManagerInstance.PlayVisualFeedback(feedbackKey, position);
            Debug.Log($"[UIFeedbackManagerAdapter] PlayFeedback: '{feedbackKey}' at {position}. Implementation depends on UIFeedbackManager API.");
        }

        public void PlayUISound(string soundKey)
        {
            if (_uiFeedbackManagerInstance == null) return;

            // Assuming _uiFeedbackManagerInstance has a method like PlayAudioFeedback(string key)
            // _uiFeedbackManagerInstance.PlayAudioFeedback(soundKey);
            Debug.Log($"[UIFeedbackManagerAdapter] PlayUISound: '{soundKey}'. Implementation depends on UIFeedbackManager API.");
        }

        public void TriggerHaptic(string hapticKey)
        {
            if (_uiFeedbackManagerInstance == null) return;

            bool hapticsEnabled = _themeEngine.CurrentAccessibilitySettings?.IsHapticFeedbackEnabled ?? false;

            if (hapticsEnabled)
            {
                // Assuming _uiFeedbackManagerInstance has a method like TriggerHapticFeedback(string key)
                // _uiFeedbackManagerInstance.TriggerHapticFeedback(hapticKey);
                Debug.Log($"[UIFeedbackManagerAdapter] TriggerHaptic: '{hapticKey}'. Haptics enabled. Implementation depends on UIFeedbackManager API.");
            }
            else
            {
                Debug.Log($"[UIFeedbackManagerAdapter] TriggerHaptic: '{hapticKey}'. Haptics disabled by accessibility settings.");
            }
        }
    }
}