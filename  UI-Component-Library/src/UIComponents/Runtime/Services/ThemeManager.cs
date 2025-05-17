using UnityEngine;
using UnityEngine.UIElements;

// Assuming these dependencies exist or will be created:
// - PatternCipher.UI.Accessibility.ColorPaletteDefinition (ScriptableObject C# class)
//   with a List<NamedColor> Colors field, where NamedColor has Name (string) and Value (Color).

namespace PatternCipher.UI.Services
{
    public class ThemeManager
    {
        /// <summary>
        /// Applies a theme to a root VisualElement. This could involve:
        /// 1. Adding/Removing specific USS StyleSheets.
        /// 2. Overriding USS custom properties (variables) based on the palette.
        /// </summary>
        /// <param name="rootElement">The root VisualElement to apply the theme to.</param>
        /// <param name="palette">The color palette to derive theme colors from.</param>
        /// <param name="themeStyleSheet">Optional: A specific StyleSheet to apply for this theme.</param>
        public void ApplyTheme(VisualElement rootElement, Accessibility.ColorPaletteDefinition palette, StyleSheet themeStyleSheet = null)
        {
            if (rootElement == null)
            {
                Debug.LogError("[ThemeManager] RootElement is null.");
                return;
            }

            // If a specific stylesheet is provided for the theme, apply it.
            // This might be a base theme or a full override theme like "HighContrast.uss".
            if (themeStyleSheet != null)
            {
                ApplyUSS(rootElement, themeStyleSheet); // This will add it. Consider removing others.
            }
            
            if (palette == null)
            {
                Debug.LogWarning("[ThemeManager] ColorPaletteDefinition is null. Cannot apply palette colors dynamically.");
                return;
            }

            // Apply colors from the palette as USS custom properties (variables)
            // This allows USS rules to use these variables, e.g., background-color: var(--primary-bg-color);
            if (palette.Colors != null)
            {
                foreach (var namedColor in palette.Colors)
                {
                    if (!string.IsNullOrEmpty(namedColor.Name))
                    {
                        // USS custom property names typically start with --
                        string varName = namedColor.Name.StartsWith("--") ? namedColor.Name : $"--{ToKebabCase(namedColor.Name)}";
                        rootElement.style.SetCustomProperty(varName, namedColor.Value);
                        // Debug.Log($"[ThemeManager] Set custom property {varName} to {namedColor.Value}");
                    }
                }
            }
        }

        /// <summary>
        /// Adds a StyleSheet to a VisualElement.
        /// </summary>
        /// <param name="rootElement">The VisualElement to add the StyleSheet to.</param>
        /// <param name="styleSheet">The StyleSheet to add.</param>
        public void ApplyUSS(VisualElement rootElement, StyleSheet styleSheet)
        {
            if (rootElement == null || styleSheet == null)
            {
                Debug.LogError("[ThemeManager] RootElement or StyleSheet is null.");
                return;
            }

            if (!rootElement.styleSheets.Contains(styleSheet))
            {
                rootElement.styleSheets.Add(styleSheet);
            }
        }
        
        /// <summary>
        /// Removes a StyleSheet from a VisualElement.
        /// </summary>
        public void RemoveUSS(VisualElement rootElement, StyleSheet styleSheet)
        {
            if (rootElement == null || styleSheet == null)
            {
                Debug.LogError("[ThemeManager] RootElement or StyleSheet is null.");
                return;
            }
            rootElement.styleSheets.Remove(styleSheet);
        }


        // Helper to convert CamelCase or PascalCase to kebab-case for CSS variable names
        private static string ToKebabCase(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            return System.Text.RegularExpressions.Regex.Replace(
                str,
                "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])",
                "-$1",
                System.Text.RegularExpressions.RegexOptions.Compiled)
                .Trim()
                .ToLower();
        }
    }
}