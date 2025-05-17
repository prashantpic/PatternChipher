namespace PatternCipher.UI.Coordinator.Theme
{
    /// <summary>
    /// Enumeration for different types of colorblind modes.
    /// Defines specific types of color vision deficiencies that the UI can adapt to.
    /// </summary>
    public enum ColorblindTypeEnum
    {
        /// <summary>
        /// No colorblind mode applied.
        /// </summary>
        None = 0,

        /// <summary>
        /// Deuteranopia (red-green color blindness, green weak).
        /// </summary>
        Deuteranopia = 1,

        /// <summary>
        /// Protanopia (red-green color blindness, red weak).
        /// </summary>
        Protanopia = 2,

        /// <summary>
        /// Tritanopia (blue-yellow color blindness).
        /// </summary>
        Tritanopia = 3
    }
}