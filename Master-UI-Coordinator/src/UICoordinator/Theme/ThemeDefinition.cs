using UnityEngine;
using TMPro;

namespace PatternCipher.UI.Coordinator.Theme
{
    [CreateAssetMenu(fileName = "ThemeDefinition", menuName = "PatternCipher/UI Coordinator/Theme Definition")]
    public class ThemeDefinition : ScriptableObject
    {
        public string ThemeName = "Default";
        public Color PrimaryColor = Color.blue;
        public Color SecondaryColor = Color.cyan;
        public Color AccentColor = Color.yellow;
        public Color BackgroundColor = Color.white;
        public Color TextColor = Color.black;
        public TMP_FontAsset DefaultFont;
    }
}