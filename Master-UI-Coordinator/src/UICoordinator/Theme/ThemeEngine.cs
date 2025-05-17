using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
// Assuming PatternCipher.UI.Coordinator.Core.UIEvents is defined
// Assuming PatternCipher.UI.Coordinator.Theme.ThemeDefinition, .AccessibilitySettings,
// .IThemeable, .IAccessibleUIElement, .AccessibilitySettingsManager, .ReducedMotionHandler are defined.

namespace PatternCipher.UI.Coordinator.Theme
{
    /// <summary>
    /// Manages theme consistency and propagates accessibility features across all UI components.
    /// It loads themes and accessibility settings and applies them to registered UI elements.
    /// </summary>
    public class ThemeEngine // Typically implements IThemeEngine
    {
        private ThemeDefinition _currentTheme;
        private AccessibilitySettings _currentAccessibilitySettings;

        private readonly List<IThemeable> _themeableElements;
        private readonly List<IAccessibleUIElement> _accessibleElements;

        private readonly AccessibilitySettingsManager _accessibilitySettingsManager;
        private readonly ReducedMotionHandler _reducedMotionHandler;
        
        private ThemeDefinition _defaultTheme; // To store the initial default theme

        public ThemeEngine(
            AccessibilitySettingsManager accessibilitySettingsManager,
            ReducedMotionHandler reducedMotionHandler)
        {
            _accessibilitySettingsManager = accessibilitySettingsManager;
            _reducedMotionHandler = reducedMotionHandler;

            _themeableElements = new List<IThemeable>();
            _accessibleElements = new List<IAccessibleUIElement>();
        }

        /// <summary>
        /// Initializes the ThemeEngine, loading initial settings and applying default theme.
        /// </summary>
        /// <param name="defaultTheme">The default theme definition to apply if none are loaded.</param>
        /// <param name="defaultAccessibilitySettings">Default accessibility settings if none are loaded.</param>
        public async Task InitializeAsync(ThemeDefinition defaultTheme, AccessibilitySettings defaultAccessibilitySettings)
        {
            _defaultTheme = defaultTheme;
            _currentAccessibilitySettings = await _accessibilitySettingsManager.LoadSettingsAsync();
            if (_currentAccessibilitySettings == null)
            {
                _currentAccessibilitySettings = defaultAccessibilitySettings ?? new AccessibilitySettings(); // Ensure not null
                // Optionally save these defaults if no settings were found
                // await _accessibilitySettingsManager.SaveSettingsAsync(_currentAccessibilitySettings);
            }

            // Apply initial theme (could be stored in UIState or use default)
            // For now, using the passed defaultTheme.
            // A more complex system might load the last used theme name from UIStateManager.
            _currentTheme = _defaultTheme; 

            ApplyCurrentThemeAndAccessibility();
        }

        public void ApplyCurrentThemeAndAccessibility()
        {
            if (_currentTheme != null)
            {
                ApplyThemeToAll(_currentTheme);
            }
            if (_currentAccessibilitySettings != null)
            {
                ApplyAccessibilityToAll(_currentAccessibilitySettings);
            }
        }

        /// <summary>
        /// Applies a new theme to all registered themeable elements.
        /// </summary>
        /// <param name="theme">The ThemeDefinition to apply.</param>
        public void ApplyTheme(ThemeDefinition theme)
        {
            if (theme == null)
            {
                Debug.LogWarning("ThemeEngine: ApplyTheme called with a null theme.");
                return;
            }
            _currentTheme = theme;
            ApplyThemeToAll(_currentTheme);
            Core.UIEvents.NotifyThemeUpdated(_currentTheme);
        }
        
        private void ApplyThemeToAll(ThemeDefinition theme)
        {
            // Iterate a copy in case collection is modified during iteration by an element
            foreach (var element in new List<IThemeable>(_themeableElements))
            {
                element.ApplyTheme(theme);
            }
        }


        /// <summary>
        /// Applies new accessibility settings to all registered accessible elements.
        /// Also updates systems like ReducedMotionHandler.
        /// </summary>
        /// <param name="settings">The AccessibilitySettings to apply.</param>
        public async Task ApplyAccessibilitySettingsAsync(AccessibilitySettings settings)
        {
            if (settings == null)
            {
                Debug.LogWarning("ThemeEngine: ApplyAccessibilitySettings called with null settings.");
                return;
            }
            _currentAccessibilitySettings = settings;
            
            ApplyAccessibilityToAll(_currentAccessibilitySettings);
            
            _reducedMotionHandler?.SetReducedMotion(_currentAccessibilitySettings.IsReducedMotionEnabled);
            
            await _accessibilitySettingsManager.SaveSettingsAsync(_currentAccessibilitySettings); // Persist changes
            Core.UIEvents.NotifyAccessibilitySettingChanged(_currentAccessibilitySettings);
        }

        private void ApplyAccessibilityToAll(AccessibilitySettings settings)
        {
             // Iterate a copy in case collection is modified during iteration
            foreach (var element in new List<IAccessibleUIElement>(_accessibleElements))
            {
                element.UpdateAccessibility(settings);
            }
        }


        public void RegisterThemeableElement(IThemeable element)
        {
            if (element != null && !_themeableElements.Contains(element))
            {
                _themeableElements.Add(element);
                if (_currentTheme != null) element.ApplyTheme(_currentTheme); // Apply current theme immediately
            }
        }

        public void UnregisterThemeableElement(IThemeable element)
        {
            _themeableElements.Remove(element);
        }

        public void RegisterAccessibleElement(IAccessibleUIElement element)
        {
            if (element != null && !_accessibleElements.Contains(element))
            {
                _accessibleElements.Add(element);
                if (_currentAccessibilitySettings != null) element.UpdateAccessibility(_currentAccessibilitySettings); // Apply current settings immediately
            }
        }

        public void UnregisterAccessibleElement(IAccessibleUIElement element)
        {
            _accessibleElements.Remove(element);
        }

        public ThemeDefinition GetCurrentTheme() => _currentTheme;
        public AccessibilitySettings GetCurrentAccessibilitySettings() => _currentAccessibilitySettings;

        public void Dispose()
        {
            _themeableElements.Clear();
            _accessibleElements.Clear();
        }
    }
}