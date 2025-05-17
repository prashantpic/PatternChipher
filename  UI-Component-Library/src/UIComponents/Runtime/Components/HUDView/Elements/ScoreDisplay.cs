using UnityEngine;
using UnityEngine.UIElements;
using PatternCipher.UI.Accessibility; // For ITextStylable, IColorStylable
using PatternCipher.UI.Utilities; // For TextMeshProUtils, if HUD elements use TextMeshPro directly

namespace PatternCipher.UI.Components.HUDView.Elements
{
    public class ScoreDisplay : MonoBehaviour, ITextStylable, IColorStylable
    {
        [Tooltip("Name of the Label element in UXML for displaying the score.")]
        public string scoreLabelName = "ScoreLabel";
        
        private Label _scoreLabel;
        private VisualElement _rootVisualElement; // Parent HUDView's root or specific container for this element

        public void Initialize(VisualElement hudRootElement)
        {
            _rootVisualElement = hudRootElement;
            if (_rootVisualElement == null)
            {
                Debug.LogError("ScoreDisplay: HUD Root Element is not provided.", this);
                return;
            }
            _scoreLabel = _rootVisualElement.Q<Label>(scoreLabelName);

            if (_scoreLabel == null)
            {
                Debug.LogError($"ScoreDisplay: Label with name '{scoreLabelName}' not found.", this);
            }
        }

        public void SetScore(int newScore)
        {
            if (_scoreLabel != null)
            {
                _scoreLabel.text = $"Score: {newScore}"; // Or just newScore.ToString() depending on design
            }
            else
            {
                Debug.LogWarning("ScoreDisplay: ScoreLabel is null, cannot set score text.", this);
            }
        }

        public void ApplyTextStyles(float textSizeMultiplier, ColorPaletteDefinition colorPalette)
        {
            if (_scoreLabel != null)
            {
                // This is a simplified example. Ideally, use USS variables or TextMeshProUtils for robust scaling.
                // For UI Toolkit Label:
                // 1. Define base font size in USS (e.g., --hud-font-size).
                // 2. Apply multiplier by setting a custom USS property or changing class.
                // Or, if TextMeshPro is used via a custom VisualElement:
                // TextMeshProUtils.ApplyAccessibilityTextSettings(_scoreLabel.Q<TextMeshProUGUI>() or similar, textSizeMultiplier, colorPalette, "HUDText");
                
                // Simple direct scaling (less ideal but for demonstration):
                // if (_scoreLabel.style.fontSize.value.value > 0) { // Check if it has a concrete value
                //    _scoreLabel.style.fontSize = _scoreLabel.style.fontSize.value.value * textSizeMultiplier;
                // }

                if (colorPalette != null && colorPalette.TryGetColor("HUDTextColor", out Color textColor))
                {
                    _scoreLabel.style.color = textColor;
                }
            }
        }

        public void ApplyColorPalette(ColorPaletteDefinition colorPalette)
        {
            // Primarily for text color, handled by ApplyTextStyles.
            // If background or other elements need palette colors, apply here.
            // e.g., _rootVisualElement.Q<VisualElement>("ScoreBackground").style.backgroundColor = ...
            if (_scoreLabel != null && colorPalette != null && colorPalette.TryGetColor("HUDTextColor", out Color textColor))
            {
                 _scoreLabel.style.color = textColor;
            }
        }
    }
}