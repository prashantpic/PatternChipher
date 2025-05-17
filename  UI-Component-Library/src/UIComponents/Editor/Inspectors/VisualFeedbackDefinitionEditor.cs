using UnityEditor;
using UnityEngine;

namespace PatternCipher.UI.Editor
{
    // This script requires:
    // 1. PatternCipher.UI.Components.Feedback.VisualFeedbackDefinition class to be defined.
    //    Assuming it's a ScriptableObject with fields like:
    //    VisualAnimationTypeEnum AnimationType;
    //    string DOTweenParams; // Serialized JSON or custom struct
    //    UnityEngine.AddressableAssets.AssetReferenceGameObject ParticleEffectAddress;
    //    float Duration;
    // 2. PatternCipher.UI.Components.Feedback.Enums.VisualAnimationTypeEnum enum to be defined.
    //    (e.g., ScalePunch, Fade, Move, Rotate, ParticleEffect)

    [CustomEditor(typeof(PatternCipher.UI.Components.Feedback.VisualFeedbackDefinition))]
    public class VisualFeedbackDefinitionEditor : UnityEditor.Editor
    {
        SerializedProperty _animationTypeProp;
        SerializedProperty _dotweenParamsProp;
        SerializedProperty _particleEffectAddressProp;
        SerializedProperty _durationProp;

        void OnEnable()
        {
            _animationTypeProp = serializedObject.FindProperty("AnimationType");
            _dotweenParamsProp = serializedObject.FindProperty("DOTweenParams"); // Consider a custom property drawer for complex params
            _particleEffectAddressProp = serializedObject.FindProperty("ParticleEffectAddress");
            _durationProp = serializedObject.FindProperty("Duration");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_animationTypeProp, new GUIContent("Animation Type"));

            PatternCipher.UI.Components.Feedback.Enums.VisualAnimationTypeEnum animationType = 
                (PatternCipher.UI.Components.Feedback.Enums.VisualAnimationTypeEnum)_animationTypeProp.enumValueIndex;

            switch (animationType)
            {
                case PatternCipher.UI.Components.Feedback.Enums.VisualAnimationTypeEnum.ParticleEffect:
                    EditorGUILayout.PropertyField(_particleEffectAddressProp, new GUIContent("Particle Effect (Addressable)"));
                    break;
                // Add cases for DOTween types if DOTweenParams needs specific UI
                // For ScalePunch, Fade, Move, Rotate:
                default: 
                    EditorGUILayout.PropertyField(_dotweenParamsProp, new GUIContent("DOTween Parameters (JSON/Custom)"));
                    EditorGUILayout.PropertyField(_durationProp, new GUIContent("Duration (seconds)"));
                    break;
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Define visual feedback. Respects Reduced Motion (REQ-6-007, REQ-UIX-013).", MessageType.Info);

            serializedObject.ApplyModifiedProperties();
        }
    }
}