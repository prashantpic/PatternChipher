using UnityEngine;
using UnityEngine.UIElements;
using PatternCipher.UI.Accessibility; // For ITextStylable, IColorStylable

namespace PatternCipher.UI.Components.HUDView.Elements
{
    public class MoveCounterDisplay : MonoBehaviour, ITextStylable, IColorStylable
    {
        [Tooltip("Name of the Label element in UXML for displaying moves.")]
        public string movesLabelName = "MovesLabel";
        
        private Label _movesLabel;
        private VisualElement _rootVisualElement;

        public void Initialize(VisualElement hudRootElement)
        {
            _rootVisualElement = hudRootElement;
            if (_rootVisualElement == null)
            {
                Debug.LogError("MoveCounterDisplay: HUD Root Element is not provided.", this);
                return;
            }
            _movesLabel = _rootVisualElement.Q<Label>(movesLabelName);

            if (_movesLabel == null)
            {
                Debug.LogError($"MoveCounterDisplay: Label with name '{movesLabelName}' not found.", this);
            }
        }

        public void SetMoves(int currentMoves, int parMoves)
        {
            if (_movesLabel != null)
            {
                if (parMoves > 0)
                {
                    _movesLabel.text = $"Moves: {currentMoves}/{parMoves}";
                }
                else
                {
                    _movesLabel.text = $"Moves: {currentMoves}";
                }
            }
            else
            {
                Debug.LogWarning("MoveCounterDisplay: MovesLabel is null, cannot set moves text.", this);
            }
        }

        public void ApplyTextStyles(float textSizeMultiplier, ColorPaletteDefinition colorPalette)
        {
            if (_movesLabel != null)
            {
                // Similar to ScoreDisplay, ideally use USS variables or TextMeshProUtils.
                if (colorPalette != null && colorPalette.TryGetColor("HUDTextColor", out Color textColor))
                {
                    _movesLabel.style.color = textColor;
                }
            }
        }

        public void ApplyColorPalette(ColorPaletteDefinition colorPalette)
        {
            if (_movesLabel != null && colorPalette != null && colorPalette.TryGetColor("HUDTextColor", out Color textColor))
            {
                 _movesLabel.style.color = textColor;
            }
        }
    }
}