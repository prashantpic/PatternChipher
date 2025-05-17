using System;

namespace PatternCipher.UI.Accessibility
{
    /// <summary>
    /// Defines a contract for classes that provide the current accessibility preferences (REQ-UIX-013).
    /// Allows decoupling from specific settings storage.
    /// </summary>
    public interface IAccessibilitySettingsProvider
    {
        /// <summary>
        /// Gets the currently active accessibility profile.
        /// </summary>
        AccessibilityProfile CurrentAccessibilityProfile { get; }

        /// <summary>
        /// Event invoked when accessibility settings change.
        /// Provides the new AccessibilityProfile.
        /// </summary>
        event Action<AccessibilityProfile> OnSettingsChanged;
    }
}