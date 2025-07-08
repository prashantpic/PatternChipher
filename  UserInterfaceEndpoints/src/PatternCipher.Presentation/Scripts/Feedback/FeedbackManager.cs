using UnityEngine;
using PatternCipher.Presentation.Accessibility;
using PatternCipher.Application; // Placeholder

namespace PatternCipher.Presentation.Feedback
{
    /// <summary>
    /// Defines the types of feedback that can be played.
    /// </summary>
    public enum FeedbackType
    {
        TileSelect,
        TileSwap,
        InvalidMove,
        MatchSuccess,
        LevelComplete
    }

    /// <summary>
    /// This service acts as a central hub for triggering all 'juicy' feedback effects,
    /// both visual and auditory. This keeps the triggering logic separate from the
    /// effect implementation, improving maintainability and consistency.
    /// </summary>
    /// <remarks>
    /// Implements the Singleton and Facade patterns. It checks accessibility settings
    /// before playing potentially intense effects.
    /// </remarks>
    public class FeedbackManager : MonoBehaviour
    {
        public static FeedbackManager Instance { get; private set; }

        // Placeholder references to other feedback systems
        private AudioManager audioManager;
        // private VFXManager vfxManager;
        // private CameraShake cameraShaker;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            // Get instances of other managers
            audioManager = AudioManager.Instance;
            // vfxManager = VFXManager.Instance;
            // cameraShaker = CameraShake.Instance;
        }
        
        /// <summary>
        /// Plays a predefined feedback sequence based on the specified type.
        /// </summary>
        /// <param name="type">The type of feedback to play.</param>
        /// <param name="position">The world position for positional effects (like VFX).</param>
        public void PlayFeedback(FeedbackType type, Vector3 position = default)
        {
            switch (type)
            {
                case FeedbackType.TileSelect:
                    audioManager?.PlaySound(SoundType.TileSelect);
                    break;

                case FeedbackType.TileSwap:
                    audioManager?.PlaySound(SoundType.TileSwap);
                    break;
                
                case FeedbackType.InvalidMove:
                    audioManager?.PlaySound(SoundType.Error);
                    // The shake animation is handled on the TileView itself to keep it localized.
                    break;

                case FeedbackType.MatchSuccess:
                    audioManager?.PlaySound(SoundType.Match);
                    // vfxManager?.PlayEffect(VFXType.MatchSparks, position);
                    if (AccessibilityManager.Instance != null && !AccessibilityManager.Instance.IsReducedMotionEnabled)
                    {
                        // cameraShaker?.Shake(0.1f, 0.5f);
                    }
                    break;
                
                case FeedbackType.LevelComplete:
                    audioManager?.PlaySound(SoundType.Victory);
                    // vfxManager?.PlayEffect(VFXType.Fireworks, Vector3.zero);
                    break;
            }
        }
        
        /// <summary>
        /// A simplified method for playing non-positional feedback.
        /// </summary>
        public void PlayFeedback(FeedbackType type)
        {
            PlayFeedback(type, Vector3.zero);
        }

        // Placeholder for a public API that is more descriptive
        public void PlayLevelCompleteFeedback()
        {
            PlayFeedback(FeedbackType.LevelComplete);
        }

        public void PlayInvalidMoveFeedback()
        {
            PlayFeedback(FeedbackType.InvalidMove);
        }
    }
}