using UnityEngine;
using DG.Tweening; // For Ease enum
using System.Collections.Generic;

namespace PatternCipher.Client.Presentation.Feedback.Configuration
{
    [CreateAssetMenu(fileName = "FeedbackConfig", menuName = "PatternCipher/Feedback Configuration", order = 1)]
    public class FeedbackConfigSO : ScriptableObject
    {
        [Header("Tile Interaction Feedback")]
        public TileFeedbackSettings tapSuccessFeedback;
        public TileFeedbackSettings tapInvalidFeedback;
        public TileFeedbackSettings swapSuccessFeedback;
        public TileFeedbackSettings swapInvalidFeedback;
        public TileFeedbackSettings matchFoundFeedback;
        public TileFeedbackSettings tileRemovedFeedback;

        [Header("Level Events Feedback")]
        public LevelEventFeedbackSettings levelCompleteWinFeedback;
        public LevelEventFeedbackSettings levelCompleteLoseFeedback;
        public LevelEventFeedbackSettings hintUsedFeedback;

        [Header("General UI Feedback")]
        public UIButtonFeedbackSettings defaultButtonPressFeedback;

        [Header("Screen Shake Parameters")]
        public ScreenShakeParams minorShake;
        public ScreenShakeParams majorShake;

        // Add other specific feedback configurations as needed
    }

    [System.Serializable]
    public class TileFeedbackSettings
    {
        [Header("Visuals")]
        public bool useAnimation = true;
        public float animationDuration = 0.3f;
        public Ease animationEase = Ease.OutQuad;
        public Vector3 animationScaleTo = new Vector3(1.1f, 1.1f, 1.1f);
        public GameObject particleEffectPrefab;

        [Header("Audio")]
        public AudioClip soundEffect;
        [Range(0f, 1f)] public float sfxVolume = 1.0f;

        [Header("Haptics (if applicable)")]
        public bool triggerHaptics = false;
        // public HapticType hapticType; // Define HapticType enum if used
    }

    [System.Serializable]
    public class LevelEventFeedbackSettings
    {
        [Header("Visuals")]
        public GameObject fullScreenParticleEffectPrefab;
        public float effectDuration = 1.5f;

        [Header("Audio")]
        public AudioClip soundEffect;
        [Range(0f, 1f)] public float sfxVolume = 1.0f;
        public AudioClip musicStinger; // Optional music change or stinger
    }

    [System.Serializable]
    public class UIButtonFeedbackSettings
    {
        [Header("Visuals - Animation")]
        public bool animateOnClick = true;
        public float animationDuration = 0.1f;
        public Ease animationEase = Ease.OutQuad;
        public Vector3 scaleOnClick = new Vector3(0.95f, 0.95f, 1f);

        [Header("Audio")]
        public AudioClip clickSound;
        [Range(0f, 1f)] public float sfxVolume = 0.8f;
    }
    
    [System.Serializable]
    public class ScreenShakeParams
    {
        public float duration = 0.2f;
        public float strength = 0.1f;
        public int vibrato = 10;
        public float randomness = 90f;
        public bool fadeOut = true;
    }
}