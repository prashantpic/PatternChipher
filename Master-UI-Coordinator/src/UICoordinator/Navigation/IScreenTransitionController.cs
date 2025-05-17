using System.Threading.Tasks;
// Assuming PatternCipher.UI.Coordinator.Interfaces.IUIView is defined elsewhere

namespace PatternCipher.UI.Coordinator.Navigation
{
    /// <summary>
    /// Interface for managing and executing animated visual transitions between UI screens.
    /// </summary>
    public interface IScreenTransitionController
    {
        /// <summary>
        /// Plays a visual transition between an outgoing and an incoming screen.
        /// </summary>
        /// <param name="screenIn">The IUIView instance that is transitioning in (becoming active).</param>
        /// <param name="screenOut">The IUIView instance that is transitioning out (becoming inactive). Can be null if there's no outgoing screen.</param>
        /// <param name="transitionTypeKey">A key or identifier for the type of transition to play (e.g., "Fade", "SlideLeft").</param>
        /// <returns>A task that completes when the transition animation is finished.</returns>
        Task PlayTransitionAsync(PatternCipher.UI.Coordinator.Interfaces.IUIView screenIn, PatternCipher.UI.Coordinator.Interfaces.IUIView screenOut, string transitionTypeKey);
    }
}