using System.Threading.Tasks;

namespace PatternCipher.UI.Coordinator.Theme
{
    /// <summary>
    /// Interface for the Theme Engine, defining contracts for UI theme and accessibility settings management.
    /// Defines the contract for managing and applying UI themes and accessibility settings consistently across the application.
    /// </summary>
    public interface IThemeEngine
    {
        /// <summary>
        /// Initializes the ThemeEngine, loading default theme and initial accessibility settings.
        /// </summary>
        /// <param name="defaultTheme">The default theme definition to apply.</param>
        /// <param name="initialAccessibilitySettings">The initial accessibility settings to load and apply.</param>
        Task InitializeAsync(ThemeDefinition defaultTheme, AccessibilitySettings initialAccessibilitySettings);

        /// <summary>
        /// Applies the specified theme to all registered themeable UI elements.
        /// </summary>
        /// <param name="theme">The theme definition to apply.</param>
        void ApplyTheme(ThemeDefinition theme);

        /// <summary>
        /// Gets the currently active theme definition.
        /// </summary>
        /// <returns>The current ThemeDefinition.</returns>
        ThemeDefinition GetCurrentTheme();

        /// <summary>
        /// Applies the specified accessibility settings to all registered accessible UI elements.
        /// </summary>
        /// <param name="settings">The accessibility settings to apply.</param>
        void ApplyAccessibilitySettings(AccessibilitySettings settings);

        /// <summary>
        /// Gets the currently active accessibility settings.
        /// </summary>
        /// <returns>The current AccessibilitySettings.</returns>
        AccessibilitySettings GetCurrentAccessibilitySettings();

        /// <summary>
        /// Registers a UI element that can be styled by themes.
        /// </summary>
        /// <param name="element">The themeable UI element to register.</param>
        void RegisterThemeableElement(IThemeable element);

        /// <summary>
        /// Unregisters a UI element from theme management.
        /// </summary>
        /// <param name="element">The themeable UI element to unregister.</param>
        void UnregisterThemeableElement(IThemeable element);

        /// <summary>
        /// Registers a UI element that adapts to accessibility settings.
        /// </summary>
        /// <param name="element">The accessible UI element to register.</param>
        void RegisterAccessibleElement(IAccessibleUIElement element);

        /// <summary>
        /// Unregisters a UI element from accessibility management.
        /// </summary>
        /// <param name="element">The accessible UI element to unregister.</param>
        void UnregisterAccessibleElement(IAccessibleUIElement element);
    }
}