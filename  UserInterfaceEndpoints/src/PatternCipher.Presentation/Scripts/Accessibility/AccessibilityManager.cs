using UnityEngine;
using System;

namespace PatternCipher.Presentation.Accessibility
{
    /// <summary>
    /// Defines the available colorblind modes.
    /// </summary>
    public enum ColorblindMode
    {
        None,
        Protanopia,
        Deuteranopia,
        Tritanopia
    }

    /// <summary>
    /// A service that manages and applies accessibility settings, such as colorblind modes
    /// and reduced motion, across the entire application. It acts as the single source
    /// of truth for these settings.
    /// </summary>
    /// <remarks>
    /// Implements the Singleton pattern. Persists settings using PlayerPrefs.
    /// Broadcasts an event when settings change so other components can react.
    /// </remarks>
    public class AccessibilityManager : MonoBehaviour
    {
        public static AccessibilityManager Instance { get; private set; }

        private const string COLORBLIND_MODE_KEY = "Accessibility.ColorblindMode";
        private const string REDUCED_MOTION_KEY = "Accessibility.ReducedMotion";

        /// <summary>
        /// Fired whenever an accessibility setting is changed.
        /// Components should subscribe to this to update their visuals or behavior.
        /// </summary>
        public event Action OnAccessibilitySettingsChanged;

        /// <summary>
        /// The currently active colorblind mode.
        /// </summary>
        public ColorblindMode CurrentColorblindMode { get; private set; }

        /// <summary>
        /// Whether reduced motion is enabled. Components should check this before
        /// playing intense animations like screen shake.
        /// </summary>
        public bool IsReducedMotionEnabled { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadSettings();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Loads the saved accessibility settings from PlayerPrefs.
        /// </summary>
        private void LoadSettings()
        {
            CurrentColorblindMode = (ColorblindMode)PlayerPrefs.GetInt(COLORBLIND_MODE_KEY, 0);
            IsReducedMotionEnabled = PlayerPrefs.GetInt(REDUCED_MOTION_KEY, 0) == 1;
        }

        /// <summary>
        /// Sets the new colorblind mode, saves it, and notifies listeners.
        /// </summary>
        /// <param name="mode">The new colorblind mode to apply.</param>
        public void SetColorblindMode(ColorblindMode mode)
        {
            if (CurrentColorblindMode == mode) return;

            CurrentColorblindMode = mode;
            PlayerPrefs.SetInt(COLORBLIND_MODE_KEY, (int)mode);
            PlayerPrefs.Save();

            OnAccessibilitySettingsChanged?.Invoke();
            Debug.Log($"Colorblind mode set to: {mode}");
        }

        /// <summary>
        /// Sets the reduced motion preference, saves it, and notifies listeners.
        /// </summary>
        /// <param name="isEnabled">True to enable reduced motion, false to disable.</param>
        public void SetReducedMotion(bool isEnabled)
        {
            if (IsReducedMotionEnabled == isEnabled) return;

            IsReducedMotionEnabled = isEnabled;
            PlayerPrefs.SetInt(REDUCED_MOTION_KEY, isEnabled ? 1 : 0);
            PlayerPrefs.Save();
            
            OnAccessibilitySettingsChanged?.Invoke();
            Debug.Log($"Reduced motion set to: {isEnabled}");
        }
    }
}