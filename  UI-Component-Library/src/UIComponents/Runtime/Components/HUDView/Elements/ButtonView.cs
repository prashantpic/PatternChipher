using UnityEngine;
using UnityEngine.UIElements;
using System;
using PatternCipher.UI.Accessibility; // For IColorStylable, ITextStylable, IMotionControllable
using PatternCipher.UI.Feedback; // For IUIFeedbackController (or direct FeedbackEvent enum)
using PatternCipher.UI.Feedback.Enums; // For FeedbackEvent

namespace PatternCipher.UI.Components.HUDView.Elements
{
    public class ButtonView : MonoBehaviour, IColorStylable, ITextStylable, IMotionControllable
    {
        [Tooltip("Name of the Button element in UXML.")]
        public string buttonName = "InteractiveButton";
        [Tooltip("Name of the Label element inside the Button for text.")]
        public string buttonLabelName = "ButtonLabel"; // Optional, if button has internal label
        [Tooltip("Name of the Image element inside the Button for icon.")]
        public string buttonIconName = "ButtonIcon"; // Optional, if button has internal icon

        private Button _button;
        private Label _buttonLabel;
        private Image _buttonIcon;
        private VisualElement _rootVisualElement; // The HUD root or a container this button belongs to

        private System.Action _onClickAction;
        private IUIFeedbackController _feedbackController; // Injected dependency
        private bool _isReducedMotion = false;

        public void Initialize(VisualElement hudRootElement, IUIFeedbackController feedbackController = null)
        {
            _rootVisualElement = hudRootElement;
            _feedbackController = feedbackController;

            if (_rootVisualElement == null)
            {
                Debug.LogError("ButtonView: HUD Root Element is not provided.", this);
                return;
            }

            _button = _rootVisualElement.Q<Button>(buttonName);
            if (_button == null)
            {
                Debug.LogError($"ButtonView: Button with name '{buttonName}' not found.", this);
                return;
            }

            _buttonLabel = _button.Q<Label>(buttonLabelName); // UI Toolkit Button often has its own text property.
                                                              // This would be for a custom structure.
                                                              // If using Button's built-in text: _button.text
            _buttonIcon = _button.Q<Image>(buttonIconName);

            _button.clicked += HandleClick;

            // REQ-UIX-013.3: Tap Target Size is primarily handled by USS styling for the Button.
            // Ensure ButtonView.uss sets min-width and min-height.
        }

        public void SetText(string text)
        {
            if (_button != null)
            {
                _button.text = text; // Standard way for UI Toolkit Button
            }
            if (_buttonLabel != null) // If using a separate label inside
            {
                _buttonLabel.text = text;
                _buttonLabel.style.display = string.IsNullOrEmpty(text) ? DisplayStyle.None : DisplayStyle.Flex;
            }
        }

        public void SetIcon(Sprite iconSprite)
        {
            if (_buttonIcon != null)
            {
                _buttonIcon.sprite = iconSprite;
                _buttonIcon.style.display = (iconSprite == null) ? DisplayStyle.None : DisplayStyle.Flex;
            }
        }

        public void OnClick(System.Action clickAction)
        {
            _onClickAction = clickAction;
        }

        private void HandleClick()
        {
            _onClickAction?.Invoke();

            // REQ-UIX-013 (FeedbackIntegration part)
            _feedbackController?.PlayFeedbackForEvent(FeedbackEvent.ButtonClick);
        }
        
        private void OnDestroy()
        {
            if (_button != null)
            {
                _button.clicked -= HandleClick;
            }
        }

        // --- IAccessibility Interfaces ---
        public void ApplyColorPalette(ColorPaletteDefinition colorPalette)
        {
            if (_button != null && colorPalette != null)
            {
                if (colorPalette.TryGetColor("ButtonBackgroundColor", out Color bgColor))
                {
                    _button.style.backgroundColor = bgColor;
                }
                if (colorPalette.TryGetColor("ButtonTextColor", out Color textColor)) // For button's direct text
                {
                    _button.style.color = textColor;
                }
                // For internal label/icon if they exist and need specific palette colors
                if (_buttonLabel != null && colorPalette.TryGetColor("ButtonLabelTextColor", out Color labelColor))
                {
                    _buttonLabel.style.color = labelColor;
                }
                if (_buttonIcon != null && colorPalette.TryGetColor("ButtonIconColor", out Color iconTintColor))
                {
                    _buttonIcon.style.unityBackgroundImageTintColor = iconTintColor;
                }
            }
        }

        public void ApplyTextStyles(float textSizeMultiplier, ColorPaletteDefinition colorPalette)
        {
            // For the button's built-in text:
            if (_button != null)
            {
                // Similar to ScoreDisplay, robust scaling requires USS variables or custom logic.
                // Example: _button.style.fontSize = baseButtonFontSize * textSizeMultiplier;
                if (colorPalette != null && colorPalette.TryGetColor("ButtonTextColor", out Color textColor))
                {
                     _button.style.color = textColor;
                }
            }
            // For internal label if used:
            if (_buttonLabel != null)
            {
                // _buttonLabel.style.fontSize = baseLabelFontSize * textSizeMultiplier;
                if (colorPalette != null && colorPalette.TryGetColor("ButtonLabelTextColor", out Color labelColor))
                {
                     _buttonLabel.style.color = labelColor;
                }
            }
        }

        public void SetReducedMotion(bool reduceMotion)
        {
            _isReducedMotion = reduceMotion;
            // If button has animations (e.g. on press), they should check _isReducedMotion.
        }
    }
}