using System;
using UnityEngine; // For Rect
// Assuming these types are defined elsewhere as per the SDS and file definitions
// using PatternCipher.UI.Coordinator.Navigation;
// using PatternCipher.UI.Coordinator.Theme;

namespace PatternCipher.UI.Coordinator.Core
{
    /// <summary>
    /// Static class providing a centralized event hub for UI-wide events.
    /// Enables decoupled communication between UI components.
    /// </summary>
    public static class UIEvents
    {
        /// <summary>
        /// Event fired when a screen change is requested.
        /// Parameters: Target ScreenType, NavigationPayload.
        /// </summary>
        public static event Action<PatternCipher.UI.Coordinator.Navigation.ScreenType, PatternCipher.UI.Coordinator.Navigation.NavigationPayload> OnScreenChangeRequested;

        /// <summary>
        /// Event fired after a screen has been successfully changed.
        /// Parameters: New ScreenType, Previous ScreenType (can be a placeholder if no previous screen).
        /// </summary>
        public static event Action<PatternCipher.UI.Coordinator.Navigation.ScreenType, PatternCipher.UI.Coordinator.Navigation.ScreenType> OnScreenChanged;

        /// <summary>
        /// Event fired when the UI theme has been updated.
        /// Parameter: The new ThemeDefinition that was applied.
        /// </summary>
        public static event Action<PatternCipher.UI.Coordinator.Theme.ThemeDefinition> OnThemeUpdated;

        /// <summary>
        /// Event fired when an accessibility setting has changed.
        /// Parameter: The updated AccessibilitySettings.
        /// </summary>
        public static event Action<PatternCipher.UI.Coordinator.Theme.AccessibilitySettings> OnAccessibilitySettingChanged;

        /// <summary>
        /// Event fired when the safe area insets have changed.
        /// Parameter: The new safe area Rect.
        /// </summary>
        public static event Action<Rect> OnSafeAreaInsetsChanged;

        /// <summary>
        /// Event fired when the responsive layout has been recalculated for all relevant elements.
        /// </summary>
        public static event Action OnResponsiveLayoutRecalculated;

        // Invoker methods (optional, but good practice for encapsulation if used internally by coordinator)
        public static void RequestScreenChange(PatternCipher.UI.Coordinator.Navigation.ScreenType screenType, PatternCipher.UI.Coordinator.Navigation.NavigationPayload payload)
            => OnScreenChangeRequested?.Invoke(screenType, payload);

        public static void NotifyScreenChanged(PatternCipher.UI.Coordinator.Navigation.ScreenType newScreen, PatternCipher.UI.Coordinator.Navigation.ScreenType oldScreen)
            => OnScreenChanged?.Invoke(newScreen, oldScreen);

        public static void NotifyThemeUpdated(PatternCipher.UI.Coordinator.Theme.ThemeDefinition newTheme)
            => OnThemeUpdated?.Invoke(newTheme);

        public static void NotifyAccessibilitySettingChanged(PatternCipher.UI.Coordinator.Theme.AccessibilitySettings newSettings)
            => OnAccessibilitySettingChanged?.Invoke(newSettings);
        
        public static void NotifySafeAreaInsetsChanged(Rect newSafeArea)
            => OnSafeAreaInsetsChanged?.Invoke(newSafeArea);

        public static void NotifyResponsiveLayoutRecalculated()
            => OnResponsiveLayoutRecalculated?.Invoke();
    }
}