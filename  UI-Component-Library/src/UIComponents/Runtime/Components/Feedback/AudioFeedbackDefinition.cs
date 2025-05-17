using UnityEngine;
using UnityEngine.AddressableAssets; // Required for AssetReferenceAudioClip

namespace PatternCipher.UI.Components.Feedback
{
    /// <summary>
    /// ScriptableObject defining properties for an audio feedback event.
    /// This data container stores configurations for sound effects, including the audio clip,
    /// volume, and pitch variations, to be used by the UIFeedbackController (REQ-6-003).
    /// </summary>
    [CreateAssetMenu(fileName = "AudioFeedback", menuName = "PatternCipher/UI/Feedback/Audio Feedback Definition")]
    public class AudioFeedbackDefinition : ScriptableObject
    {
        [Tooltip("Addressable asset reference to the AudioClip to be played.")]
        public AssetReferenceAudioClip AudioClipAddress;

        [Tooltip("Volume at which the audio clip will be played. Range: 0.0 (silent) to 1.0 (full volume).")]
        [Range(0f, 1f)]
        public float Volume = 1.0f;

        [Tooltip("Minimum pitch for playback. Allows for slight variations if Min and Max are different.")]
        [Range(0.1f, 3f)]
        public float PitchMin = 1.0f;

        [Tooltip("Maximum pitch for playback. Allows for slight variations if Min and Max are different.")]
        [Range(0.1f, 3f)]
        public float PitchMax = 1.0f;

#if UNITY_EDITOR
        void OnValidate()
        {
            if (PitchMin > PitchMax)
            {
                PitchMin = PitchMax;
                Debug.LogWarning($"AudioFeedbackDefinition '{name}': PitchMin cannot be greater than PitchMax. Adjusted PitchMin to match PitchMax.");
            }
            if (PitchMin <= 0) PitchMin = 0.1f;
            if (PitchMax <= 0) PitchMax = 0.1f;

        }
#endif
    }
}