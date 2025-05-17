using DG.Tweening;

namespace PatternCipher.UI.Coordinator.Theme
{
    /// <summary>
    /// Handles enabling/disabling non-essential animations based on accessibility settings.
    /// Manages the state of non-essential UI animations based on the 'Reduced Motion' accessibility setting.
    /// </summary>
    public class ReducedMotionHandler
    {
        /// <summary>
        /// Gets a value indicating whether reduced motion is globally enabled.
        /// Other animation systems can check this flag.
        /// </summary>
        public static bool IsReducedMotionGloballyEnabled { get; private set; }

        /// <summary>
        /// Sets the reduced motion state.
        /// Can globally toggle DOTween animations or signal other custom animation systems.
        /// </summary>
        /// <param name="isEnabled">True to enable reduced motion, false to disable.</param>
        public void SetReducedMotion(bool isEnabled)
        {
            IsReducedMotionGloballyEnabled = isEnabled;

            // Example: Globally affect DOTween's time scale.
            // For more granular control, specific DOTween animation groups/tweens would need to check
            // IsReducedMotionGloballyEnabled or be controlled by a more sophisticated system.
            // Setting timeScale to 0 effectively pauses tweens, 1 is normal speed.
            // Adjust as needed for "reduced" vs "no" motion.
            if (isEnabled)
            {
                // Drastically reduce speed or pause non-essential animations.
                // This is a global setting; specific tweens might need individual handling
                // or to be assigned to an ID that can be targeted.
                // DOTween.timeScale = 0.1f; // Or 0.0f to completely stop.
            }
            else
            {
                // DOTween.timeScale = 1.0f;
            }

            // For DOTween, it's often better to manage tweens individually or by ID
            // rather than globally changing timeScale if only *some* animations are non-essential.
            // For example, a tween could be tagged with an ID: myTween.SetId("nonEssentialAnimation");
            // Then, if isEnabled: DOTween.Pause("nonEssentialAnimation"); else: DOTween.Play("nonEssentialAnimation");
            // Or, individual animation components would check IsReducedMotionGloballyEnabled.
            // The current implementation primarily sets the flag for other systems to query.
        }
    }
}