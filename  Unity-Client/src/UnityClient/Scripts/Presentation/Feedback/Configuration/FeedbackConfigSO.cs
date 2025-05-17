using UnityEngine;
using DG.Tweening; // Required for Ease
using System.Collections.Generic; // Required for List

namespace PatternCipher.Client.Presentation.Feedback.Configuration
{
    // Define placeholder for FeedbackType if not defined elsewhere yet
    // This would typically be in a shared enums file or Domain.Events
    public enum FeedbackType
    {
        TapSuccess,
        TapInvalid,
        SwapSuccess,
        SwapInvalid,
        MatchFound,
        TileRemoved,
        LevelCompleteWin,
        LevelCompleteLose
        // Add more feedback types as needed
    }

    [System.Serializable]
    public class VisualFeedbackSettings
    {
        public FeedbackType Type;
        public float AnimationDuration = 0.5f;
        public Ease AnimationEase = Ease.OutQuad;
        public GameObject ParticleEffectPrefab;
        public Vector3 ParticleOffset = Vector3.zero;
        // Screen Shake specific
        public bool UseScreenShake = false;
        public float ShakeDuration = 0.1f;
        public float ShakeStrength = 0.1f;
        public int ShakeVibrato = 10;
        public float ShakeRandomness = 90f;
    }

    [System.Serializable]
    public class AudioFeedbackSettings
    {
        public FeedbackType Type;
        public AudioClip SoundClip;
        [Range(0f, 1f)]
        public float Volume = 1.0f;
    }

    [CreateAssetMenu(fileName = "FeedbackConfig", menuName = "PatternCipher/Configuration/Feedback Config")]
    public class FeedbackConfigSO : ScriptableObject
    {
        [Header("Visual Feedback Configurations")]
        public List<VisualFeedbackSettings> VisualFeedbacks;

        [Header("Audio Feedback Configurations")]
        public List<AudioFeedbackSettings> AudioFeedbacks;

        // Helper methods to get specific feedback settings
        public VisualFeedbackSettings GetVisualFeedback(FeedbackType type)
        {
            return VisualFeedbacks.Find(vf => vf.Type == type);
        }

        public AudioFeedbackSettings GetAudioFeedback(FeedbackType type)
        {
            return AudioFeedbacks.Find(af => af.Type == type);
        }
    }
}