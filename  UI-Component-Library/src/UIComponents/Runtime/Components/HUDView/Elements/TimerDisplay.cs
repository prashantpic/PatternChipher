using UnityEngine;
using UnityEngine.UIElements;
using PatternCipher.UI.Accessibility; // For ITextStylable, IColorStylable

namespace PatternCipher.UI.Components.HUDView.Elements
{
    public class TimerDisplay : MonoBehaviour, ITextStylable, IColorStylable
    {
        [Tooltip("Name of the Label element in UXML for displaying the timer.")]
        public string timerLabelName = "TimerLabel";
        
        private Label _timerLabel;
        private VisualElement _rootVisualElement;

        public void Initialize(VisualElement hudRootElement)
        {
            _rootVisualElement = hudRootElement;
            if (_rootVisualElement == null)
            {
                Debug.LogError("TimerDisplay: HUD Root Element is not provided.", this);
                return;
            }
            _timerLabel = _rootVisualElement.Q<Label>(timerLabelName);

            if (_timerLabel == null)
            {
                Debug.LogError($"TimerDisplay: Label with name '{timerLabelName}' not found.", this);
            }
        }

        public void SetTime(float timeSeconds)
        {
            if (_timerLabel != null)
            {
                // Format time as M:SS or similar
                int minutes = Mathf.FloorToInt(timeSeconds / 60F);
                int seconds = Mathf.FloorToInt(timeSeconds - minutes * 60);
                _timerLabel.text = $"Time: {minutes:00}:{seconds:00}";
            }
            else
            {
                Debug.LogWarning("TimerDisplay: TimerLabel is null, cannot set time text.", this);
            }
        }

        public void ApplyTextStyles(float textSizeMultiplier, ColorPaletteDefinition colorPalette)
        {
            if (_timerLabel != null)
            {
                // Similar to ScoreDisplay, ideally use USS variables or TextMeshProUtils.
                // Simple direct scaling for demonstration:
                // if (_timerLabel.style.fontSize.value.value > 0) {
                //    _timerLabel.style.fontSize = _timerLabel.style.fontSize.value.value * textSizeMultiplier;
                // }
                if (colorPalette != null && colorPalette.TryGetColor("HUDTextColor", out Color textColor))
                {
                    _timerLabel.style.color = textColor;
                }
            }
        }

        public void ApplyColorPalette(ColorPaletteDefinition colorPalette)
        {
            if (_timerLabel != null && colorPalette != null && colorPalette.TryGetColor("HUDTextColor", out Color textColor))
            {
                 _timerLabel.style.color = textColor;
            }
        }
    }
}