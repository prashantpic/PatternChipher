using UnityEngine;
using PatternCipher.Client.Core.Events;
using PatternCipher.Client.Domain.Events;
using PatternCipher.Client.Presentation.Feedback.Configuration; // For FeedbackConfigSO
using System.Collections.Generic;

namespace PatternCipher.Client.Presentation.Feedback
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioFeedbackController : MonoBehaviour
    {
        [SerializeField] private FeedbackConfigSO feedbackConfig;
        private AudioSource audioSource; // Used for one-shot sound effects

        // Optional: For more complex audio needs, multiple AudioSources could be used
        // [SerializeField] private AudioSource musicAudioSource;
        // [SerializeField] private List<AudioSource> sfxAudioSourcesPool; 
        // private int currentSfxSourceIndex = 0;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            audioSource.playOnAwake = false;

            // Initialize pool if using multiple sources
            // if (sfxAudioSourcesPool == null || sfxAudioSourcesPool.Count == 0)
            // {
            //     sfxAudioSourcesPool = new List<AudioSource>();
            //     for (int i = 0; i < 5; i++) // Example: pool of 5 audio sources for SFX
            //     {
            //         var pooledSource = gameObject.AddComponent<AudioSource>();
            //         pooledSource.playOnAwake = false;
            //         sfxAudioSourcesPool.Add(pooledSource);
            //     }
            // }
        }

        private void OnEnable()
        {
            GlobalEventBus.Instance?.Subscribe<TileInteractionFeedbackEvent>(HandleTileInteractionFeedback);
            GlobalEventBus.Instance?.Subscribe<LevelCompletedEvent>(HandleLevelCompletedFeedback);
            // Subscribe to other relevant events (e.g., ButtonClickEvent, ScreenTransitionEvent)
        }

        private void OnDisable()
        {
            GlobalEventBus.Instance?.Unsubscribe<TileInteractionFeedbackEvent>(HandleTileInteractionFeedback);
            GlobalEventBus.Instance?.Unsubscribe<LevelCompletedEvent>(HandleLevelCompletedFeedback);
        }

        private void HandleTileInteractionFeedback(TileInteractionFeedbackEvent eventData)
        {
            if (feedbackConfig == null) return;

            AudioClip clipToPlay = null;
            float volumeScale = 1.0f;

            switch (eventData.RequestedFeedback)
            {
                case FeedbackType.TapSuccess:
                    clipToPlay = feedbackConfig.TileTapSuccessSound;
                    volumeScale = feedbackConfig.TileTapSuccessVolume;
                    break;
                case FeedbackType.TapInvalid:
                    clipToPlay = feedbackConfig.TileTapInvalidSound;
                    volumeScale = feedbackConfig.TileTapInvalidVolume;
                    break;
                case FeedbackType.SwapSuccess: // Assuming swap implies a match or clear eventually
                    clipToPlay = feedbackConfig.TileSwapSound;
                     volumeScale = feedbackConfig.TileSwapVolume;
                    break;
                case FeedbackType.SwapInvalid:
                    clipToPlay = feedbackConfig.TileActionInvalidSound; // Or a dedicated swap invalid sound
                    volumeScale = feedbackConfig.TileActionInvalidVolume;
                    break;
                case FeedbackType.MatchFound:
                    clipToPlay = feedbackConfig.TileMatchSound;
                    volumeScale = feedbackConfig.TileMatchVolume;
                    // Could also vary pitch or select different sounds based on match size (eventData.MatchSize)
                    break;
                case FeedbackType.TileCleared:
                    clipToPlay = feedbackConfig.TileClearSound;
                    volumeScale = feedbackConfig.TileClearVolume;
                    break;
                // Add more cases as FeedbackType enum expands
            }

            PlaySFX(clipToPlay, volumeScale);
        }

        private void HandleLevelCompletedFeedback(LevelCompletedEvent eventData)
        {
            if (feedbackConfig == null) return;

            AudioClip clipToPlay = null;
            if (eventData.StarsAwarded > 0) // Simple win condition
            {
                clipToPlay = feedbackConfig.LevelCompleteSound;
            }
            else
            {
                clipToPlay = feedbackConfig.LevelFailSound;
            }
            PlaySFX(clipToPlay, feedbackConfig.LevelEndVolume);
        }

        public void PlaySFX(AudioClip clip, float volumeScale = 1.0f)
        {
            if (clip != null && audioSource != null)
            {
                audioSource.PlayOneShot(clip, volumeScale);
            }
            // else if (clip != null && sfxAudioSourcesPool != null && sfxAudioSourcesPool.Count > 0)
            // {
            //     sfxAudioSourcesPool[currentSfxSourceIndex].PlayOneShot(clip, volumeScale);
            //     currentSfxSourceIndex = (currentSfxSourceIndex + 1) % sfxAudioSourcesPool.Count;
            // }
        }
    }
}