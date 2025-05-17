using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening; // For DOTween Pro
using System.Threading.Tasks;
using PatternCipher.UI.Components.SymbolManagement; // Assuming SymbolDefinition is here
using PatternCipher.UI.Services.AssetResolution; // Assuming SymbolAssetResolver is here
using PatternCipher.UI.Accessibility; // For IColorStylable, ITextStylable
using PatternCipher.UI.Utilities; // Assuming UIAnimationUtils is here
using PatternCipher.UI.Components.GridView.Enums; // Assuming TileVisualState is here

namespace PatternCipher.UI.Components.GridView
{
    // REQ-UIX-013: Accessibility (Color, Text, Tap Target implicitly via USS)
    // REQ-UIX-015: Visual Clarity (Symbol display, Tile States)
    public class GridTileView : MonoBehaviour, IColorStylable, ITextStylable, IMotionControllable
    {
        public int GridX { get; private set; }
        public int GridY { get; private set; }

        private VisualElement _rootVisualElement;
        private Image _symbolImage;
        private Image _symbolShapeImage; // For shape distinctness
        private Image _symbolTextureOverlay; // For texture distinctness
        private Label _symbolLabel; // For accessibility or explicit text on symbol
        private VisualElement _stateOverlay; // For selected, locked, etc.

        private SymbolDefinition _currentSymbol;
        private SymbolAssetResolver _assetResolver;
        private TileVisualState _currentState = TileVisualState.Normal;

        private bool _isReducedMotionEnabled = false;

        // Constants for USS class names
        private const string USS_CLASS_BASE = "grid-tile";
        private const string USS_CLASS_SELECTED = "grid-tile--selected";
        private const string USS_CLASS_HIGHLIGHTED = "grid-tile--highlighted";
        private const string USS_CLASS_LOCKED = "grid-tile--locked";
        private const string USS_CLASS_OBSTACLE = "grid-tile--obstacle";
        private const string USS_CLASS_KEY = "grid-tile--key";
        
        // To be called by GridView after instantiation
        public void SetVisualElement(VisualElement visualElement)
        {
            _rootVisualElement = visualElement;
            if (_rootVisualElement == null)
            {
                Debug.LogError("GridTileView: RootVisualElement is null.", this);
                return;
            }

            _rootVisualElement.AddToClassList(USS_CLASS_BASE);

            // Query child elements from the UXML structure
            _symbolImage = _rootVisualElement.Q<Image>("SymbolIcon");
            _symbolShapeImage = _rootVisualElement.Q<Image>("SymbolShape");
            _symbolTextureOverlay = _rootVisualElement.Q<Image>("SymbolTexture");
            _symbolLabel = _rootVisualElement.Q<Label>("SymbolLabel"); // For accessibility text or if symbol has a character
            _stateOverlay = _rootVisualElement.Q<VisualElement>("StateOverlay");

            if (_symbolImage == null) Debug.LogWarning($"GridTileView at ({GridX},{GridY}): SymbolIcon Image not found in UXML.", this);
            if (_symbolLabel == null) Debug.LogWarning($"GridTileView at ({GridX},{GridY}): SymbolLabel Label not found in UXML.", this);
            // Add similar checks for shape and texture if they are critical for all tiles

            // Tap target size is primarily handled by USS on _rootVisualElement.
            // Add pointer event listeners
            _rootVisualElement.RegisterCallback<PointerDownEvent>(OnPointerDown);
            // Add other event listeners as needed (PointerUpEvent, ClickEvent)
        }

        public VisualElement GetVisualElement() => _rootVisualElement;


        public void Setup(int x, int y, SymbolAssetResolver assetResolver)
        {
            GridX = x;
            GridY = y;
            _assetResolver = assetResolver;
            gameObject.name = $"Tile_{x}_{y}";

            // Initial state (visuals updated by SetVisualState or SetSymbol)
        }

        public async void SetSymbol(SymbolDefinition symbolDef)
        {
            if (symbolDef == null)
            {
                ClearSymbol();
                return;
            }
            if (_assetResolver == null)
            {
                Debug.LogError($"GridTileView ({GridX},{GridY}): SymbolAssetResolver is not set. Cannot load symbol assets.", this);
                _symbolLabel?.SetText("ERR");
                return;
            }
            if (_rootVisualElement == null)
            {
                Debug.LogError($"GridTileView ({GridX},{GridY}): RootVisualElement is not set.", this);
                return;
            }

            _currentSymbol = symbolDef;

            // REQ-UIX-013.1, REQ-UIX-015: Symbol with multiple visual attributes
            // Base Color (can be applied to _rootVisualElement background or specific shape element)
            // For simplicity, let's assume the UXML background or a dedicated "ColorElement" handles this.
            // Here we primarily load the sprite/icon.
            // _rootVisualElement.style.backgroundColor = symbolDef.BaseColor; // Example

            if (_symbolImage != null)
            {
                if (symbolDef.IconSpriteAddress != null && symbolDef.IconSpriteAddress.RuntimeKeyIsValid())
                {
                    _symbolImage.sprite = await _assetResolver.ResolveSpriteAsync(symbolDef);
                    _symbolImage.style.unityBackgroundImageTintColor = symbolDef.BaseColor; // Apply color tint to icon
                    _symbolImage.style.display = DisplayStyle.Flex;
                }
                else
                {
                    _symbolImage.style.display = DisplayStyle.None;
                }
            }

            // Shape - could be another sprite or a procedural mesh/shader
            if (_symbolShapeImage != null)
            {
                if (symbolDef.ShapeSpriteAddress != null && symbolDef.ShapeSpriteAddress.RuntimeKeyIsValid()) // Assuming ShapeSpriteAddress in SymbolDefinition
                {
                     // _symbolShapeImage.sprite = await _assetResolver.ResolveSpriteAsync(symbolDef.ShapeSpriteAddress); // Hypothetical
                    _symbolShapeImage.style.unityBackgroundImageTintColor = symbolDef.BaseColor; // Or a distinct shape color
                    _symbolShapeImage.style.display = DisplayStyle.Flex;
                }
                else
                {
                    _symbolShapeImage.style.display = DisplayStyle.None;
                }
            }
            
            // Texture - could be an overlay texture
            if (_symbolTextureOverlay != null)
            {
                if (symbolDef.TextureAddress != null && symbolDef.TextureAddress.RuntimeKeyIsValid())
                {
                    _symbolTextureOverlay.sprite = await _assetResolver.ResolveTextureAsSpriteAsync(symbolDef); // Assuming a helper in resolver
                    _symbolTextureOverlay.style.display = DisplayStyle.Flex;
                }
                else
                {
                    _symbolTextureOverlay.style.display = DisplayStyle.None;
                }
            }


            if (_symbolLabel != null)
            {
                _symbolLabel.text = symbolDef.AccessibilityLabel; // REQ-UIX-013
                                                              // Or if symbolDef has a `Character: char` field
                                                              // _symbolLabel.text = symbolDef.Character.ToString();
                _symbolLabel.style.display = string.IsNullOrEmpty(symbolDef.AccessibilityLabel) ? DisplayStyle.None : DisplayStyle.Flex;
            }

            // Apply current visual state styling based on the new symbol (e.g. if symbol implies a state)
            ApplyCurrentVisualStateClasses();
        }

        public void ClearSymbol()
        {
            _currentSymbol = null;
            if (_symbolImage != null) _symbolImage.style.display = DisplayStyle.None;
            if (_symbolShapeImage != null) _symbolShapeImage.style.display = DisplayStyle.None;
            if (_symbolTextureOverlay != null) _symbolTextureOverlay.style.display = DisplayStyle.None;
            if (_symbolLabel != null) _symbolLabel.text = "";
        }

        public void SetVisualState(TileVisualState state, bool animate)
        {
            if (_currentState == state && !animate) return; // No change or no animation requested for same state
            
            _currentState = state;
            ApplyCurrentVisualStateClasses();

            if (animate && !_isReducedMotionEnabled)
            {
                AnimateStateChange(null);
            }
        }

        private void ApplyCurrentVisualStateClasses()
        {
            if (_rootVisualElement == null) return;

            _rootVisualElement.EnableInClassList(USS_CLASS_SELECTED, _currentState == TileVisualState.Selected);
            _rootVisualElement.EnableInClassList(USS_CLASS_HIGHLIGHTED, _currentState == TileVisualState.Highlighted);
            _rootVisualElement.EnableInClassList(USS_CLASS_LOCKED, _currentState == TileVisualState.Locked);
            _rootVisualElement.EnableInClassList(USS_CLASS_OBSTACLE, _currentState == TileVisualState.Obstacle);
            _rootVisualElement.EnableInClassList(USS_CLASS_KEY, _currentState == TileVisualState.Key);
            
            // Potentially update shader parameters on _stateOverlay or _rootVisualElement if using ShaderGraph for states
            if (_stateOverlay != null)
            {
                 // _stateOverlay.style.unityBackgroundImageTintColor = GetColorForState(_currentState); // Example
            }
        }


        public void AnimateSwap(GridTileView otherTile, System.Action onComplete)
        {
            if (_rootVisualElement == null || otherTile == null || otherTile.GetVisualElement() == null)
            {
                onComplete?.Invoke();
                return;
            }

            if (_isReducedMotionEnabled)
            {
                // Instant swap: Just update positions logically, visual update happens elsewhere or is skipped
                Vector3 myPos = _rootVisualElement.transform.position;
                _rootVisualElement.transform.position = otherTile.GetVisualElement().transform.position;
                otherTile.GetVisualElement().transform.position = myPos;
                onComplete?.Invoke();
                return;
            }
            
            // Requires UIAnimationUtils to be implemented
            // For now, a simple DOTween example assuming VisualElements can be animated this way (they can)
            Vector3 myOriginalPos = _rootVisualElement.transform.position;
            Vector3 otherOriginalPos = otherTile.GetVisualElement().transform.position;

            Sequence swapSequence = DOTween.Sequence();
            swapSequence.Append(_rootVisualElement.DOMove(otherOriginalPos, 0.3f).SetEase(Ease.OutQuad));
            swapSequence.Join(otherTile.GetVisualElement().DOMove(myOriginalPos, 0.3f).SetEase(Ease.OutQuad));
            swapSequence.OnComplete(() => onComplete?.Invoke());
            swapSequence.Play();
        }

        public void AnimateStateChange(System.Action onComplete)
        {
            if (_rootVisualElement == null)
            {
                onComplete?.Invoke();
                return;
            }

            if (_isReducedMotionEnabled)
            {
                onComplete?.Invoke(); // No animation if reduced motion
                return;
            }

            // Example: Punch scale animation for state change
            // Requires UIAnimationUtils or direct DOTween
            UIAnimationUtils.PunchScale(_rootVisualElement, 0.2f, 0.3f, _isReducedMotionEnabled)
                .OnComplete(() => onComplete?.Invoke());
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            // Communicate tap/click upwards, e.g., to GridView or a game manager
            // Or trigger UIFeedbackController directly for local feedback
            Debug.Log($"GridTileView ({GridX},{GridY}) tapped.");
            
            // Example: Trigger feedback
            // if (_uiFeedbackController != null)
            // {
            // _uiFeedbackController.PlayFeedbackForEvent(FeedbackEvent.TileTap);
            // }

            // Game logic would handle selection, etc. This view just reports interaction.
            // Potentially change state to "Selected" if logic allows
            // SetVisualState(TileVisualState.Selected, true); 
        }

        // --- IAccessibility Interfaces Implementation ---
        public void ApplyColorPalette(ColorPaletteDefinition colorPalette)
        {
            // This method might be more about setting USS variables if the palette drives them,
            // or directly applying colors if not handled by USS themes.
            // For now, assume USS themes handle most of it.
            // Individual symbol colors are part of SymbolDefinition.
            // This could be used to adjust borders, backgrounds if they are theme-dependent.
            if (_rootVisualElement != null && colorPalette != null)
            {
                // Example: Apply a border color from the palette
                // if (colorPalette.TryGetColor("TileBorder", out Color borderColor))
                // {
                // _rootVisualElement.style.borderTopColor = borderColor;
                // _rootVisualElement.style.borderBottomColor = borderColor;
                // ...
                // }
            }
            // Re-apply symbol if its appearance depends on the palette (e.g. if SymbolDefinition colors are palette keys)
            if(_currentSymbol != null) SetSymbol(_currentSymbol);
        }

        public void ApplyTextStyles(float textSizeMultiplier, ColorPaletteDefinition colorPalette)
        {
            // Apply to _symbolLabel if it's used for more than accessibility hints
            if (_symbolLabel != null)
            {
                 // Example: Get base font size from USS or a constant
                // float baseFontSize = 12f; 
                // _symbolLabel.style.fontSize = baseFontSize * textSizeMultiplier;
                // TextMeshProUtils.ApplyAccessibilityTextSettings(_symbolLabel, textSizeMultiplier, colorPalette, "SymbolText");
                // For UI Toolkit Label, direct style manipulation:
                Length currentFontSize = _symbolLabel.style.fontSize.value;
                if (currentFontSize.keyword == StyleKeyword.Initial || currentFontSize.keyword == StyleKeyword.Auto || currentFontSize.value.value > 0)
                {
                    // A more robust way would be to have base font sizes defined in USS variables
                    // and then apply multiplier. For simplicity, let's assume direct scaling if a value exists.
                    // Or, this could trigger adding/removing USS classes that define different font sizes.
                    // For now, this is a simplified placeholder.
                    // A common approach is to set a --font-scale variable on the root and use `calc(var(--base-font-size) * var(--font-scale))` in USS.
                }

                // Apply color if text color is part of the palette
                // if (colorPalette != null && colorPalette.TryGetColor("SymbolTextColor", out Color textColor))
                // {
                // _symbolLabel.style.color = textColor;
                // }
            }
        }

        public void SetReducedMotion(bool reduceMotion)
        {
            _isReducedMotionEnabled = reduceMotion;
        }
    }
}