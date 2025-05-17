namespace PatternCipher.UI.Accessibility.Interfaces
{
    /// <summary>
    /// Defines a contract for UI elements or systems with animations 
    /// to respond to reduced motion settings from AccessibilityController (REQ-UIX-013.4).
    /// Implementers should reduce or disable non-essential animations when requested.
    /// </summary>
    public interface IMotionControllable
    {
        /// <summary>
        /// Sets the reduced motion preference for the element or system.
        /// </summary>
        /// <param name="reduceMotion">True if motion should be reduced, false otherwise.</param>
        void SetReducedMotion(bool reduceMotion);
    }
}