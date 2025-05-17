namespace PatternCipher.UI.Accessibility.Enums
{
    /// <summary>
    /// Defines different color vision deficiency modes and other visual assistance profiles.
    /// Used by the AccessibilityController to apply appropriate themes and styles.
    /// Implements REQ-UIX-013.
    /// </summary>
    public enum ColorVisionDeficiencyMode
    {
        /// <summary>
        /// Standard color presentation.
        /// </summary>
        Normal,

        /// <summary>
        /// Optimized for Deuteranopia (red-green color blindness, green weak).
        /// </summary>
        Deuteranopia,

        /// <summary>
        /// Optimized for Protanopia (red-green color blindness, red weak).
        /// </summary>
        Protanopia,

        /// <summary>
        /// Optimized for Tritanopia (blue-yellow color blindness).
        /// </summary>
        Tritanopia,

        /// <summary>
        /// A high-contrast mode for improved general visibility.
        /// </summary>
        HighContrast
    }
}