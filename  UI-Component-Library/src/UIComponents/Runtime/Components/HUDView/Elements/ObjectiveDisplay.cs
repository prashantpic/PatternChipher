using UnityEngine;
using UnityEngine.UIElements;
using PatternCipher.UI.Accessibility; // For ITextStylable, IColorStylable

namespace PatternCipher.UI.Components.HUDView.Elements
{
    public class ObjectiveDisplay : MonoBehaviour, ITextStylable, IColorStylable
    {
        [Tooltip("Name of the Label element in UXML for objective text.")]
        public string objectiveTextLabelName = "ObjectiveTextLabel";
        [Tooltip("Name of the Image element in UXML for objective icon.")]
        public string objectiveIconImageName = "ObjectiveIconImage";

        private Label _objectiveTextLabel;
        private Image _objectiveIconImage;
        private VisualElement _rootVisualElement;

        public void Initialize(VisualElement hudRootElement)
        {
            _rootVisualElement = hudRootElement;
            if (_rootVisualElement == null)
            {
                Debug.LogError("ObjectiveDisplay: HUD Root Element is not provided.", this);
                return;
            }

            _objectiveTextLabel = _rootVisualElement.Q<Label>(objectiveTextLabelName);
            _objectiveIconImage = _rootVisualElement.Q<Image>(objectiveIconImageName);

            if (_objectiveTextLabel == null)
            {
                Debug.LogError($"ObjectiveDisplay: Label with name '{objectiveTextLabelName}' not found.", this);
            }
            if (_objectiveIconImage == null)
            {
                Debug.LogWarning($"ObjectiveDisplay: Image with name '{objectiveIconImageName}' not found.", this);
            }
        }

        public void SetObjective(string objectiveText, Sprite objectiveIcon)
        {
            if (_objectiveTextLabel != null)
            {
                _objectiveTextLabel.text = objectiveText;
                _objectiveTextLabel.style.display = string.IsNullOrEmpty(objectiveText) ? DisplayStyle.None : DisplayStyle.Flex;
            }
            else
            {
                Debug.LogWarning("ObjectiveDisplay: ObjectiveTextLabel is null.", this);
            }

            if (_objectiveIconImage != null)
            {
                _objectiveIconImage.sprite = objectiveIcon;
                _objectiveIconImage.style.display = (objectiveIcon == null) ? DisplayStyle.None : DisplayStyle.Flex;
            }
        }

        public void ApplyTextStyles(float textSizeMultiplier, ColorPaletteDefinition colorPalette)
        {
            if (_objectiveTextLabel != null)
            {
                // Similar to ScoreDisplay for text scaling.
                if (colorPalette != null && colorPalette.TryGetColor("HUDTextColor", out Color textColor))
                {
                    _objectiveTextLabel.style.color = textColor;
                }
            }
        }

        public void ApplyColorPalette(ColorPaletteDefinition colorPalette)
        {
            // For text color and potentially icon tint if needed
            if (_objectiveTextLabel != null && colorPalette != null && colorPalette.TryGetColor("HUDTextColor", out Color textColor))
            {
                 _objectiveTextLabel.style.color = textColor;
            }
            if (_objectiveIconImage != null && colorPalette != null && colorPalette.TryGetColor("HUDIconColor", out Color iconColor)) // Assuming an icon color in palette
            {
                 _objectiveIconImage.style.unityBackgroundImageTintColor = iconColor;
            }
        }
    }
}