namespace PatternCipher.UI.Accessibility.Interfaces
{
    /// <summary>
    /// Defines a contract for UI elements to respond to text styling changes 
    /// from AccessibilityController (REQ-UIX-013.2).
    /// This includes adjustments to text size and potentially color based on the active accessibility profile.
    /// </summary>
    public interface ITextStylable
    {
        /// <summary>
        /// Applies text styles based on the provided accessibility settings.
        /// </summary>
        /// <param name="textSizeMultiplier">The multiplier to apply to the base text size.</param>
        /// <param name="colorPalette">The current color palette, which may influence text color.</param>
        void ApplyTextStyles(float textSizeMultiplier, ColorPaletteDefinition colorPalette);
    }
}