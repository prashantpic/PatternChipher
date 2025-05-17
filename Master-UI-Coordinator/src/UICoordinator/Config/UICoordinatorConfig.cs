using UnityEngine;
using PatternCipher.UI.Coordinator.Layout;
using PatternCipher.UI.Coordinator.Theme;

namespace PatternCipher.UI.Coordinator.Config
{
    /// <summary>
    /// ScriptableObject for general settings of the UI coordinator.
    /// Holds configuration settings for the UI Coordinator, such as default screen transition types,
    /// asset loading parameters, default theme, and initial layout profiles.
    /// </summary>
    [CreateAssetMenu(fileName = "UICoordinatorConfig", menuName = "PatternCipher/UI Coordinator/Config", order = 1)]
    public class UICoordinatorConfig : ScriptableObject
    {
        [Header("Navigation & Transitions")]
        [Tooltip("Default key identifying the screen transition animation/type to use.")]
        public string DefaultTransitionTypeKey = "DefaultFade";

        [Header("Asset Loading")]
        [Tooltip("Timeout in seconds for loading UI assets via Addressables.")]
        public float AssetLoadingTimeoutSeconds = 10.0f;

        [Header("Defaults")]
        [Tooltip("The default responsive profile to be used by the LayoutManager on initialization.")]
        public ResponsiveProfile DefaultResponsiveProfile;

        [Tooltip("The default theme definition to be applied by the ThemeEngine on initialization.")]
        public ThemeDefinition DefaultTheme;

        [Tooltip("The initial accessibility settings to be loaded and applied by the ThemeEngine on initialization.")]
        public AccessibilitySettings InitialAccessibilitySettings;
    }
}