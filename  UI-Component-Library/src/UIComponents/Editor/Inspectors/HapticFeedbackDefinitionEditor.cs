using UnityEditor;
using UnityEngine;

namespace PatternCipher.UI.Editor
{
    // This script requires:
    // 1. PatternCipher.UI.Components.Feedback.HapticFeedbackDefinition class to be defined.
    //    Assuming it's a ScriptableObject with fields like:
    //    HapticTypeEnum HapticType;
    //    float Intensity;
    //    float Duration;
    // 2. PatternCipher.UI.Components.Feedback.Enums.HapticTypeEnum enum to be defined.
    //    (e.g., LightTap, MediumTap, HeavyTap, Success, Warning, Failure)

    [CustomEditor(typeof(PatternCipher.UI.Components.Feedback.HapticFeedbackDefinition))]
    public class HapticFeedbackDefinitionEditor : UnityEditor.Editor
    {
        SerializedProperty _hapticTypeProp;
        SerializedProperty _intensityProp;
        SerializedProperty _durationProp;

        void OnEnable()
        {
            _hapticTypeProp = serializedObject.FindProperty("HapticType");
            _intensityProp = serializedObject.FindProperty("Intensity");
            _durationProp = serializedObject.FindProperty("Duration");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_hapticTypeProp, new GUIContent("Haptic Type"));
            EditorGUILayout.Slider(_intensityProp, 0f, 1f, new GUIContent("Intensity"));
            EditorGUILayout.PropertyField(_durationProp, new GUIContent("Duration (seconds)"));
            
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Define haptic feedback properties. Actual implementation depends on target platform/plugin (REQ-UIX-013).", MessageType.Info);

            serializedObject.ApplyModifiedProperties();
        }
    }
}