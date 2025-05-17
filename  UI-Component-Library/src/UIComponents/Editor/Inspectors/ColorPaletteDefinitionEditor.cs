using UnityEditor;
using UnityEngine;

namespace PatternCipher.UI.Editor
{
    // This script requires:
    // 1. PatternCipher.UI.Accessibility.ColorPaletteDefinition class to be defined.
    //    Assuming it's a ScriptableObject with fields like:
    //    string PaletteName;
    //    System.Collections.Generic.List<PatternCipher.UI.Accessibility.NamedColor> Colors;
    // 2. PatternCipher.UI.Accessibility.NamedColor struct/class to be defined.
    //    (e.g., string Name; Color Value;)

    [CustomEditor(typeof(PatternCipher.UI.Accessibility.ColorPaletteDefinition))]
    public class ColorPaletteDefinitionEditor : UnityEditor.Editor
    {
        SerializedProperty _paletteNameProp;
        SerializedProperty _colorsProp;

        void OnEnable()
        {
            _paletteNameProp = serializedObject.FindProperty("PaletteName");
            _colorsProp = serializedObject.FindProperty("Colors");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_paletteNameProp, new GUIContent("Palette Name"));
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Colors", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_colorsProp, new GUIContent("Named Colors"), true); // 'true' enables editing children
            
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Define a color palette. Used for theming and accessibility (REQ-UIX-013).", MessageType.Info);

            serializedObject.ApplyModifiedProperties();
        }
    }
}