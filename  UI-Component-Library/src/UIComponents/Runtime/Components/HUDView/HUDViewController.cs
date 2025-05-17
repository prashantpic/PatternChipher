using UnityEngine;
using UnityEngine.UIElements; // Required for UI Toolkit

namespace PatternCipher.UI.Components.HUDView
{
    // Assumes UXML structure with elements named:
    // ScoreLabel, MovesLabel, TimerLabel, ObjectiveTextLabel, ObjectiveIcon, HUDContainer
    // Also assumes child components like ScoreDisplay, TimerDisplay etc. are not separate C# controllers
    // but are UI Toolkit Labels or Images directly manipulated here for simplicity based on current file scope.
    // If ScoreDisplay etc. were separate components, HUDViewController would get references to them.

    // Dependencies:
    // - TextMeshPro (implicitly, if Labels are TextMeshPro based via UXML/USS)
    // - Sprite (for objective icon)

    public class HUDViewController : MonoBehaviour
    {
        [Tooltip("Assign the UIDocument component that hosts the HUD UXML.")]
        public UIDocument uiDocument;

        private VisualElement _hudContainer;
        private Label _scoreLabel;
        private Label _movesLabel;
        private Label _timerLabel;
        private Label _objectiveTextLabel;
        private Image _objectiveIcon; 
        // If using TextMeshPro specific elements from UXML, you might query for TextElement or a custom type if available.
        // For simplicity, UI Toolkit's Label and Image are used here.

        private bool _isInitialized = false;

        void OnEnable()
        {
            if (uiDocument == null)
            {
                Debug.LogError("[HUDViewController] UIDocument is not assigned.", this);
                this.enabled = false;
                return;
            }
            Initialize();
        }

        private void Initialize()
        {
            if (_isInitialized) return;

            var rootVisualElement = uiDocument.rootVisualElement;
            if (rootVisualElement == null)
            {
                Debug.LogError("[HUDViewController] RootVisualElement is null. Ensure UIDocument has a valid UXML asset.", this);
                this.enabled = false;
                return;
            }

            _hudContainer = rootVisualElement.Q<VisualElement>("HUDContainer"); // Assuming a container for show/hide
            _scoreLabel = rootVisualElement.Q<Label>("ScoreLabel");
            _movesLabel = rootVisualElement.Q<Label>("MovesLabel");
            _timerLabel = rootVisualElement.Q<Label>("TimerLabel");
            _objectiveTextLabel = rootVisualElement.Q<Label>("ObjectiveTextLabel");
            _objectiveIcon = rootVisualElement.Q<Image>("ObjectiveIcon");

            if (_hudContainer == null) Debug.LogWarning("[HUDViewController] HUDContainer not found in UXML.");
            if (_scoreLabel == null) Debug.LogWarning("[HUDViewController] ScoreLabel not found in UXML.");
            if (_movesLabel == null) Debug.LogWarning("[HUDViewController] MovesLabel not found in UXML.");
            if (_timerLabel == null) Debug.LogWarning("[HUDViewController] TimerLabel not found in UXML.");
            if (_objectiveTextLabel == null) Debug.LogWarning("[HUDViewController] ObjectiveTextLabel not found in UXML.");
            if (_objectiveIcon == null) Debug.LogWarning("[HUDViewController] ObjectiveIcon not found in UXML.");
            
            // Apply initial state, e.g., hide if not shown by default
            // Show(); // Or Hide(); depending on default state

            _isInitialized = true;
        }
        
        public void Show()
        {
            if (!_isInitialized) Initialize();
            if (_hudContainer != null)
            {
                _hudContainer.style.display = DisplayStyle.Flex;
            }
            else // Fallback if no main container
            {
                SetElementDisplay(_scoreLabel, true);
                SetElementDisplay(_movesLabel, true);
                SetElementDisplay(_timerLabel, true);
                SetElementDisplay(_objectiveTextLabel, true);
                SetElementDisplay(_objectiveIcon, true);
            }
        }

        public void Hide()
        {
            if (!_isInitialized) Initialize();
             if (_hudContainer != null)
            {
                _hudContainer.style.display = DisplayStyle.None;
            }
            else // Fallback if no main container
            {
                SetElementDisplay(_scoreLabel, false);
                SetElementDisplay(_movesLabel, false);
                SetElementDisplay(_timerLabel, false);
                SetElementDisplay(_objectiveTextLabel, false);
                SetElementDisplay(_objectiveIcon, false);
            }
        }

        private void SetElementDisplay(VisualElement element, bool show)
        {
            if (element != null)
            {
                element.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }

        public void UpdateScore(int score)
        {
            if (!_isInitialized) Initialize();
            if (_scoreLabel != null)
            {
                _scoreLabel.text = $"Score: {score}"; // TODO: Use localization keys
            }
        }

        public void UpdateMoves(int moves, int parMoves)
        {
            if (!_isInitialized) Initialize();
            if (_movesLabel != null)
            {
                _movesLabel.text = $"Moves: {moves} / {parMoves}"; // TODO: Use localization keys
            }
        }

        public void UpdateTimer(float timeRemaining)
        {
            if (!_isInitialized) Initialize();
            if (_timerLabel != null)
            {
                int minutes = Mathf.FloorToInt(timeRemaining / 60F);
                int seconds = Mathf.FloorToInt(timeRemaining - minutes * 60);
                _timerLabel.text = $"Time: {minutes:00}:{seconds:00}"; // TODO: Use localization keys
            }
        }

        public void DisplayObjective(string objectiveText, Sprite objectiveIconSprite)
        {
            if (!_isInitialized) Initialize();
            if (_objectiveTextLabel != null)
            {
                _objectiveTextLabel.text = objectiveText; // TODO: Use localization keys
                SetElementDisplay(_objectiveTextLabel, !string.IsNullOrEmpty(objectiveText));
            }
            if (_objectiveIcon != null)
            {
                _objectiveIcon.sprite = objectiveIconSprite;
                // Background can be used for sprites in UI Toolkit's Image element
                _objectiveIcon.style.backgroundImage = objectiveIconSprite != null ? new StyleBackground(objectiveIconSprite) : null;
                SetElementDisplay(_objectiveIcon, objectiveIconSprite != null);
            }
        }

        // Example of implementing ITextStylable for HUD elements if needed (not explicitly requested for HUDViewController itself but for its children)
        // This controller could propagate settings to its Label children.
        // public void ApplyTextStyles(float textSizeMultiplier, ColorPaletteDefinition colorPalette)
        // {
        //     if (!_isInitialized) Initialize();
        //     // Example for one label:
        //     // if (_scoreLabel != null && _scoreLabel.style.fontSize.keyword == StyleKeyword.Initial) 
        //     // {
        //     //    float baseSize = DefaultFontSize; // Define or get from USS var
        //     //    _scoreLabel.style.fontSize = baseSize * textSizeMultiplier;
        //     // }
        //     // if (colorPalette != null)
        //     // {
        //     //    _scoreLabel.style.color = colorPalette.GetColor("PrimaryTextColor");
        //     // }
        // }
    }
}