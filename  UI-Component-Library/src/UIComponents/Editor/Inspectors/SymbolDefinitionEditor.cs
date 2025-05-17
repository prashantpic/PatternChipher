using UnityEditor;
using UnityEngine;

namespace PatternCipher.UI.Editor
{
    // This script requires the PatternCipher.UI.Components.SymbolDefinition class to be defined.
    // Assuming PatternCipher.UI.Components.SymbolDefinition is a ScriptableObject with fields like:
    // string SymbolId;
    // Color BaseColor;
    // UnityEngine.AddressableAssets.AssetReferenceSprite IconSpriteAddress;
    // string ShapeData; // Example: Could be enum or string
    // UnityEngine.AddressableAssets.AssetReferenceTexture2D TextureAddress;
    // string AccessibilityLabel;

    [CustomEditor(typeof(PatternCipher.UI.Components.SymbolDefinition))]
    public class SymbolDefinitionEditor : UnityEditor.Editor
    {
        SerializedProperty _symbolIdProp;
        SerializedProperty _baseColorProp;
        SerializedProperty _iconSpriteAddressProp;
        SerializedProperty _shapeDataProp; // Assuming this is a string or enum. Adjust if different.
        SerializedProperty _textureAddressProp;
        SerializedProperty _accessibilityLabelProp;

        void OnEnable()
        {
            _symbolIdProp = serializedObject.FindProperty("SymbolId");
            _baseColorProp = serializedObject.FindProperty("BaseColor");
            _iconSpriteAddressProp = serializedObject.FindProperty("IconSpriteAddress");
            _shapeDataProp = serializedObject.FindProperty("ShapeData");
            _textureAddressProp = serializedObject.FindProperty("TextureAddress");
            _accessibilityLabelProp = serializedObject.FindProperty("AccessibilityLabel");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_symbolIdProp, new GUIContent("Symbol ID"));
            EditorGUILayout.PropertyField(_accessibilityLabelProp, new GUIContent("Accessibility Label"));
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Visual Attributes", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_baseColorProp, new GUIContent("Base Color"));
            EditorGUILayout.PropertyField(_iconSpriteAddressProp, new GUIContent("Icon Sprite (Addressable)"));
            EditorGUILayout.PropertyField(_textureAddressProp, new GUIContent("Texture (Addressable)"));
            EditorGUILayout.PropertyField(_shapeDataProp, new GUIContent("Shape Data/Descriptor"));
            
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Ensure symbols are distinguishable by more than just color for accessibility (REQ-UIX-013, REQ-UIX-015).", MessageType.Info);

            serializedObject.ApplyModifiedProperties();
        }
    }
}