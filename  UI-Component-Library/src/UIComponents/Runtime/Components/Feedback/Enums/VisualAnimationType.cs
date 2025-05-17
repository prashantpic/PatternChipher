namespace PatternCipher.UI.Components.Feedback.Enums
{
    /// <summary>
    /// Defines the types of visual animations that can be specified in a VisualFeedbackDefinition.
    /// Used by the UIFeedbackController and UIAnimationUtils to trigger specific animations.
    /// Implements REQ-6-007.
    /// </summary>
    public enum VisualAnimationTypeEnum
    {
        /// <summary>
        /// No specific animation, might be used for placeholder or if particle effect is primary.
        /// </summary>
        None,

        /// <summary>
        /// A quick scaling animation, often used for emphasis (e.g., DoTween's PunchScale).
        /// </summary>
        ScalePunch,

        /// <summary>
        /// Fade in or out animation.
        /// </summary>
        Fade,

        /// <summary>
        /// Movement animation (e.g., shake, slide in/out).
        /// </summary>
        Move,

        /// <summary>
        /// Rotation animation.
        /// </summary>
        Rotate,

        /// <summary>
        /// Triggers a particle effect. The specific effect is defined by an Addressable reference.
        /// </summary>
        ParticleEffect,

        /// <summary>
        /// A color flash or change animation.
        /// </summary>
        ColorFlash,

        /// <summary>
        /// A shake animation, often used for warnings or invalid actions.
        /// </summary>
        Shake
    }
}