using UnityEditor;
using UnityEngine;

namespace PatternCipher.UI.Editor
{
    // This script requires:
    // 1. PatternCipher.UI.Accessibility.AccessibilityProfile class to be defined.
    //    Assuming it's a ScriptableObject with fields like:
    //    string ProfileName;
    //    PatternCipher.UI.Accessibility.ColorPaletteDefinition BaseColorPalette;
    //    PatternCipher.UI.Accessibility.Enums.ColorVisionDeficiencyMode ColorVisionMode;
    //    PatternCipher.UI.Accessibility.ColorPaletteDefinition ColorBlindPaletteOverride; // Optional
    //    bool EnableReducedMotion;
    //    bool EnableHaptics;
    //    float TextSizeMultiplier;
    // 2. PatternCipher.UI.Accessibility.Enums.ColorVisionDeficiencyMode enum.
    // 3. PatternCipher.UI.Accessibility.ColorPaletteDefinition ScriptableObject class.

    [CustomEditor(typeof(PatternCipher.UI.Accessibility.AccessibilityProfile))]
    public class AccessibilityProfileEditor : UnityEditor.Editor
    {
        SerializedProperty _profileNameProp;
        SerializedProperty _baseColorPaletteProp;
        SerializedProperty _colorVisionModeProp;
        SerializedProperty _colorBlindPaletteOverrideProp;
        SerializedProperty _enableReducedMotionProp;
        SerializedProperty _enableHapticsProp;
        SerializedProperty _textSizeMultiplierProp;

        void OnEnable()
        {
            _profileNameProp = serializedObject.FindProperty("ProfileName");
            _baseColorPaletteProp = serializedObject.FindProperty("BaseColorPalette");
            _colorVisionModeProp = serializedObject.FindProperty("ColorVisionMode");
            _colorBlindPaletteOverrideProp = serializedObject.FindProperty("ColorBlindPaletteOverride");
            _enableReducedMotionProp = serializedObject.FindProperty("EnableReducedMotion");
            _enableHapticsProp = serializedObject.FindProperty("EnableHaptics");
            _textSizeMultiplierProp = serializedObject.FindProperty("TextSizeMultiplier");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_profileNameProp, new GUIContent("Profile Name"));
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Color Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_baseColorPaletteProp, new GUIContent("Base Color Palette"));
            EditorGUILayout.PropertyField(_colorVisionModeProp, new GUIContent("Color Vision Mode"));

            // Cast to the enum type to check if it's not 'Normal'
            PatternCipher.UI.Accessibility.Enums.ColorVisionDeficiencyMode currentMode =
                (PatternCipher.UI.Accessibility.Enums.ColorVisionDeficiencyMode)_colorVisionModeProp.enumValueIndex;

            // Assuming 'Normal' is the first enum value (index 0)
            // And HighContrast might be a special mode that uses its own specific palette not necessarily an override in this field.
            // The SDS describes specific palettes for Deuteranopia etc., so this override field is for those.
            if (currentMode != PatternCipher.UI.Accessibility.Enums.ColorVisionDeficiencyMode.Normal &&
                currentMode != PatternCipher.UI.Accessibility.Enums.ColorVisionDeficiencyMode.HighContrast) // Assuming HighContrast may use a specific theme/palette directly
            {
                EditorGUILayout.PropertyField(_colorBlindPaletteOverrideProp, new GUIContent("Mode-Specific Palette Override"));
                EditorGUILayout.HelpBox("Assign a palette specifically designed for the selected color vision deficiency mode.", MessageType.Info);
            }
            else if (currentMode == PatternCipher.UI.Accessibility.Enums.ColorVisionDeficiencyMode.HighContrast)
            {
                 EditorGUILayout.HelpBox("High Contrast mode typically uses a dedicated HighContrastPalette or USS theme.", MessageType.Info);
            }


            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Motion & Haptics", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_enableReducedMotionProp, new GUIContent("Enable Reduced Motion"));
            EditorGUILayout.PropertyField(_enableHapticsProp, new GUIContent("Enable Haptics"));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Text Settings", EditorStyles.boldLabel);
            EditorGUILayout.Slider(_textSizeMultiplierProp, 0.5f, 3.0f, new GUIContent("Text Size Multiplier"));
            
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Configure accessibility settings (REQ-UIX-013).", MessageType.Info);

            serializedObject.ApplyModifiedProperties();
        }
    }
}