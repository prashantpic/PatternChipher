using UnityEditor;
using UnityEngine;

namespace PatternCipher.UI.Editor
{
    // This script requires the PatternCipher.UI.Components.Feedback.AudioFeedbackDefinition class to be defined.
    // Assuming PatternCipher.UI.Components.Feedback.AudioFeedbackDefinition is a ScriptableObject with fields like:
    // UnityEngine.AddressableAssets.AssetReferenceAudioClip AudioClipAddress;
    // float Volume;
    // float PitchMin;
    // float PitchMax;

    [CustomEditor(typeof(PatternCipher.UI.Components.Feedback.AudioFeedbackDefinition))]
    public class AudioFeedbackDefinitionEditor : UnityEditor.Editor
    {
        SerializedProperty _audioClipAddressProp;
        SerializedProperty _volumeProp;
        SerializedProperty _pitchMinProp;
        SerializedProperty _pitchMaxProp;

        void OnEnable()
        {
            _audioClipAddressProp = serializedObject.FindProperty("AudioClipAddress");
            _volumeProp = serializedObject.FindProperty("Volume");
            _pitchMinProp = serializedObject.FindProperty("PitchMin");
            _pitchMaxProp = serializedObject.FindProperty("PitchMax");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_audioClipAddressProp, new GUIContent("Audio Clip (Addressable)"));
            
            EditorGUILayout.Slider(_volumeProp, 0f, 1f, new GUIContent("Volume"));

            EditorGUILayout.LabelField("Pitch Variation", EditorStyles.boldLabel);
            float minPitch = _pitchMinProp.floatValue;
            float maxPitch = _pitchMaxProp.floatValue;

            EditorGUILayout.MinMaxSlider(new GUIContent("Pitch Range"), ref minPitch, ref maxPitch, 0.1f, 3f);
            _pitchMinProp.floatValue = minPitch;
            _pitchMaxProp.floatValue = maxPitch;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Min: {minPitch:F2}", GUILayout.Width(100));
            _pitchMinProp.floatValue = EditorGUILayout.FloatField(minPitch, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Max: {maxPitch:F2}", GUILayout.Width(100));
            _pitchMaxProp.floatValue = EditorGUILayout.FloatField(maxPitch, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();

            if (_pitchMinProp.floatValue > _pitchMaxProp.floatValue)
            {
                _pitchMinProp.floatValue = _pitchMaxProp.floatValue;
            }

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Define audio feedback properties (REQ-6-003).", MessageType.Info);

            serializedObject.ApplyModifiedProperties();
        }
    }
}