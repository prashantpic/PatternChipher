namespace PatternCipher.UI.Accessibility.Interfaces
{
    /// <summary>
    /// Defines a contract for UI elements to respond to color palette changes 
    /// from AccessibilityController (REQ-UIX-013.1).
    /// UI elements implementing this interface can have their colors dynamically updated
    /// to match the active accessibility color scheme.
    /// </summary>
    public interface IColorStylable
    {
        /// <summary>
        /// Applies a new color palette to the UI element.
        /// The element should update its relevant visual properties based on the colors defined in the palette.
        /// </summary>
        /// <param name="colorPalette">The color palette definition to apply.</param>
        void ApplyColorPalette(ColorPaletteDefinition colorPalette);
    }
}