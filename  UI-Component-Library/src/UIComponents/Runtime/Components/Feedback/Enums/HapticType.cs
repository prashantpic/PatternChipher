namespace PatternCipher.UI.Components.Feedback.Enums
{
    /// <summary>
    /// Defines standardized types of haptic feedback patterns.
    /// These are abstract types that would be mapped to platform-specific haptic APIs.
    /// Implements REQ-UIX-013 (specifically REQ-UIX-013.5 Haptics Toggle).
    /// </summary>
    public enum HapticTypeEnum
    {
        /// <summary>
        /// A light, brief tap. Suitable for minor confirmations or selections.
        /// </summary>
        LightTap,

        /// <summary>
        /// A tap with medium intensity. Suitable for standard interactions.
        /// </summary>
        MediumTap,

        /// <summary>
        /// A stronger, more distinct tap. Suitable for significant events or confirmations.
        /// </summary>
        HeavyTap,

        /// <summary>
        /// A haptic pattern indicating success or a positive outcome.
        /// </summary>
        Success,

        /// <summary>
        /// A haptic pattern indicating a warning or a need for attention.
        /// </summary>
        Warning,

        /// <summary>
        /// A haptic pattern indicating an error or a negative outcome.
        /// </summary>
        Failure,

        /// <summary>
        /// A short, sharp haptic, often used for selection changes in lists or grids.
        /// </summary>
        SelectionChange,

        /// <summary>
        /// A double tap haptic pattern.
        /// </summary>
        DoubleTap
    }
}