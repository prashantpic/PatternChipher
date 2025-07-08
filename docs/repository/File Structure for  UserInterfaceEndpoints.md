# Specification

# 1. Files

- **Path:** src/PatternCipher.Presentation/Scripts/Managers/UIManager.cs  
**Description:** Central singleton or service responsible for managing the lifecycle of all UI screens and popups. Handles screen transitions (fading in/out), manages the UI stack, and acts as a central hub for UI-related events. It is a key orchestrator for the entire presentation layer.  
**Template:** C# Script  
**Dependency Level:** 3  
**Name:** UIManager  
**Type:** Manager  
**Relative Path:** Scripts/Managers/UIManager.cs  
**Repository Id:** REPO-PATT-003  
**Pattern Ids:**
    
    - Singleton
    - ObserverPattern
    - Mediator
    
**Members:**
    
    - **Name:** instance  
**Type:** UIManager  
**Attributes:** public|static  
    - **Name:** screenPrefabs  
**Type:** List<GameObject>  
**Attributes:** private  
    - **Name:** screenStack  
**Type:** Stack<BaseScreen>  
**Attributes:** private  
    - **Name:** activeScreens  
**Type:** Dictionary<System.Type, BaseScreen>  
**Attributes:** private  
    
**Methods:**
    
    - **Name:** ShowScreen<T>  
**Parameters:**
    
    - object data = null
    
**Return Type:** void  
**Attributes:** public  
    - **Name:** HideScreen<T>  
**Parameters:**
    
    
**Return Type:** void  
**Attributes:** public  
    - **Name:** ShowPopup<T>  
**Parameters:**
    
    - object data = null
    
**Return Type:** void  
**Attributes:** public  
    - **Name:** HandleGameEvent  
**Parameters:**
    
    - GameEvent gameEvent
    
**Return Type:** void  
**Attributes:** private  
    
**Implemented Features:**
    
    - Screen Management
    - UI Navigation Flow
    
**Requirement Ids:**
    
    - FR-U-001
    - FR-U-002
    - FR-U-003
    - FR-U-004
    - FR-U-005
    - FR-U-006
    - FR-U-007
    
**Purpose:** Manages the instantiation, display, and hiding of all UI screens and popups, ensuring a consistent navigation flow throughout the application.  
**Logic Description:** On Awake, it initializes as a singleton. The ShowScreen method instantiates the requested screen prefab, passes data to it, and manages its display state, potentially hiding others. HideScreen deactivates or destroys a screen. It listens to game events to automatically show screens like LevelComplete or Pause.  
**Documentation:**
    
    - **Summary:** This component acts as the main controller for the entire UI system. It takes requests to show or hide specific screens (e.g., Main Menu, Settings) and manages the screen stack and transitions.
    
**Namespace:** PatternCipher.Presentation.Managers  
**Metadata:**
    
    - **Category:** UIController
    
- **Path:** src/PatternCipher.Presentation/Scripts/GameplayViews/GridView.cs  
**Description:** Manages the visual representation of the game grid. Responsible for instantiating, positioning, and managing all TileView prefabs based on the Grid model data received from the domain layer. It serves as the primary container for all in-game tile interactions.  
**Template:** C# Script  
**Dependency Level:** 2  
**Name:** GridView  
**Type:** View  
**Relative Path:** Scripts/GameplayViews/GridView.cs  
**Repository Id:** REPO-PATT-003  
**Pattern Ids:**
    
    - MVC
    
**Members:**
    
    - **Name:** tileViewPrefab  
**Type:** GameObject  
**Attributes:** private  
    - **Name:** tileViews  
**Type:** Dictionary<Vector2Int, TileView>  
**Attributes:** private  
    - **Name:** gridContainer  
**Type:** Transform  
**Attributes:** private  
    
**Methods:**
    
    - **Name:** CreateGrid  
**Parameters:**
    
    - GridData gridData
    
**Return Type:** void  
**Attributes:** public  
    - **Name:** UpdateTileView  
**Parameters:**
    
    - Vector2Int position
    - TileData tileData
    
**Return Type:** void  
**Attributes:** public  
    - **Name:** AnimateSwap  
**Parameters:**
    
    - Vector2Int posA
    - Vector2Int posB
    
**Return Type:** IEnumerator  
**Attributes:** public  
    - **Name:** AnimateTileStateChange  
**Parameters:**
    
    - Vector2Int position
    - TileData newData
    
**Return Type:** void  
**Attributes:** public  
    
**Implemented Features:**
    
    - Grid Rendering
    - Tile Animation
    
**Requirement Ids:**
    
    - FR-U-003
    - FR-U-008
    
**Purpose:** Renders the interactive game board and its tiles, and orchestrates animations for tile swaps and state changes.  
**Logic Description:** The CreateGrid method clears any existing tiles and instantiates new TileView prefabs based on the provided data, arranging them in a grid layout. UpdateTileView finds a specific tile and updates its visual state (symbol, color, locked status). Animation methods use DOTween or Coroutines to visually represent tile movements and transformations.  
**Documentation:**
    
    - **Summary:** This view component is responsible for translating the abstract Grid model into a visible, interactive grid of tiles on the screen. It manages the lifecycle of TileView objects.
    
**Namespace:** PatternCipher.Presentation.GameplayViews  
**Metadata:**
    
    - **Category:** Presentation
    
- **Path:** src/PatternCipher.Presentation/Scripts/GameplayViews/TileView.cs  
**Description:** A script attached to the Tile prefab. It controls the visual appearance of a single tile, including its symbol, color, and state (e.g., selected, locked, wildcard). It also handles animations for selection, state changes, and feedback effects.  
**Template:** C# Script  
**Dependency Level:** 1  
**Name:** TileView  
**Type:** View  
**Relative Path:** Scripts/GameplayViews/TileView.cs  
**Repository Id:** REPO-PATT-003  
**Pattern Ids:**
    
    - MVC
    
**Members:**
    
    - **Name:** symbolImage  
**Type:** Image  
**Attributes:** private  
    - **Name:** background  
**Type:** Image  
**Attributes:** private  
    - **Name:** lockedOverlay  
**Type:** GameObject  
**Attributes:** private  
    - **Name:** selectionHighlight  
**Type:** GameObject  
**Attributes:** private  
    
**Methods:**
    
    - **Name:** Initialize  
**Parameters:**
    
    - TileData tileData
    
**Return Type:** void  
**Attributes:** public  
    - **Name:** SetSelected  
**Parameters:**
    
    - bool isSelected
    
**Return Type:** void  
**Attributes:** public  
    - **Name:** PlayStateChangeAnimation  
**Parameters:**
    
    
**Return Type:** void  
**Attributes:** public  
    - **Name:** OnPointerDown  
**Parameters:**
    
    - PointerEventData eventData
    
**Return Type:** void  
**Attributes:** public  
    
**Implemented Features:**
    
    - Tile Visuals
    - Tile Selection Feedback
    - Tile Input Handling
    
**Requirement Ids:**
    
    - FR-U-008
    - NFR-V-003
    - NFR-US-005
    
**Purpose:** Represents a single interactive tile on the game grid. It updates its visuals based on model data and communicates user input to its controller.  
**Logic Description:** The Initialize method sets the tile's initial symbol and state. SetSelected toggles a visual highlight. OnPointerDown captures tap events and notifies the InputManager. This component's RectTransform must adhere to minimum tap target sizes as per NFR-US-005. Animations use DOTween for satisfying 'juicy' effects.  
**Documentation:**
    
    - **Summary:** This component is the primary interactive element for the player. It visualizes tile data and captures user interactions like taps and drags.
    
**Namespace:** PatternCipher.Presentation.GameplayViews  
**Metadata:**
    
    - **Category:** Presentation
    
- **Path:** src/PatternCipher.Presentation/Scripts/Screens/BaseScreen.cs  
**Description:** An abstract base class for all UI screens (Main Menu, Settings, etc.). It provides common functionality for showing, hiding, and managing screen state, ensuring a consistent lifecycle for all views.  
**Template:** C# Script  
**Dependency Level:** 0  
**Name:** BaseScreen  
**Type:** View  
**Relative Path:** Scripts/Screens/BaseScreen.cs  
**Repository Id:** REPO-PATT-003  
**Pattern Ids:**
    
    - TemplateMethod
    
**Members:**
    
    
**Methods:**
    
    - **Name:** Show  
**Parameters:**
    
    - object data = null
    
**Return Type:** void  
**Attributes:** public  
    - **Name:** Hide  
**Parameters:**
    
    
**Return Type:** void  
**Attributes:** public  
    - **Name:** OnShow  
**Parameters:**
    
    - object data
    
**Return Type:** void  
**Attributes:** protected|virtual  
    - **Name:** OnHide  
**Parameters:**
    
    
**Return Type:** void  
**Attributes:** protected|virtual  
    
**Implemented Features:**
    
    - Screen Lifecycle Management
    
**Requirement Ids:**
    
    
**Purpose:** Provides a common interface and base functionality for all UI screens, standardizing how they are shown and hidden by the UIManager.  
**Logic Description:** Contains public Show and Hide methods that handle the activation/deactivation of the screen's GameObject and call the virtual OnShow/OnHide methods. Subclasses will override OnShow and OnHide to implement screen-specific setup and teardown logic.  
**Documentation:**
    
    - **Summary:** A foundational abstract class that all specific screen controllers inherit from. It defines a standard contract for screen presentation.
    
**Namespace:** PatternCipher.Presentation.Screens  
**Metadata:**
    
    - **Category:** UIController
    
- **Path:** src/PatternCipher.Presentation/Scripts/Screens/MainMenu/MainMenuScreen.cs  
**Description:** Controller for the Main Menu screen. Manages the buttons for 'Play Game', 'Settings', 'How to Play', and other optional features. It handles user clicks and navigates to the appropriate next screen via the UIManager.  
**Template:** C# Script  
**Dependency Level:** 1  
**Name:** MainMenuScreen  
**Type:** View  
**Relative Path:** Scripts/Screens/MainMenu/MainMenuScreen.cs  
**Repository Id:** REPO-PATT-003  
**Pattern Ids:**
    
    - MVC
    
**Members:**
    
    - **Name:** playButton  
**Type:** Button  
**Attributes:** private  
    - **Name:** settingsButton  
**Type:** Button  
**Attributes:** private  
    - **Name:** howToPlayButton  
**Type:** Button  
**Attributes:** private  
    - **Name:** exitButton  
**Type:** Button  
**Attributes:** private  
    
**Methods:**
    
    - **Name:** OnPlayButtonClicked  
**Parameters:**
    
    
**Return Type:** void  
**Attributes:** private  
    - **Name:** OnSettingsButtonClicked  
**Parameters:**
    
    
**Return Type:** void  
**Attributes:** private  
    - **Name:** OnHowToPlayButtonClicked  
**Parameters:**
    
    
**Return Type:** void  
**Attributes:** private  
    - **Name:** OnExitButtonClicked  
**Parameters:**
    
    
**Return Type:** void  
**Attributes:** private  
    
**Implemented Features:**
    
    - Main Menu Navigation
    
**Requirement Ids:**
    
    - FR-U-001
    
**Purpose:** Handles user interactions on the main menu, triggering navigation to other parts of the application.  
**Logic Description:** In its Awake or Start method, this script wires up OnClick listeners for each button. The handler methods then call the UIManager to show the corresponding screen (e.g., UIManager.Instance.ShowScreen<LevelSelectScreen>()). The Exit button will conditionally be shown based on the platform.  
**Documentation:**
    
    - **Summary:** The controller for the game's main entry point screen. It responds to player input to navigate to different game sections like level selection or settings.
    
**Namespace:** PatternCipher.Presentation.Screens.MainMenu  
**Metadata:**
    
    - **Category:** UIController
    
- **Path:** src/PatternCipher.Presentation/Scripts/Screens/LevelSelect/LevelSelectScreen.cs  
**Description:** Controller for the Level Selection screen. It dynamically populates a list or grid of available levels/packs, displays the player's progress (e.g., stars earned) for each, and handles the selection of a level to play.  
**Template:** C# Script  
**Dependency Level:** 1  
**Name:** LevelSelectScreen  
**Type:** View  
**Relative Path:** Scripts/Screens/LevelSelect/LevelSelectScreen.cs  
**Repository Id:** REPO-PATT-003  
**Pattern Ids:**
    
    - MVC
    
**Members:**
    
    - **Name:** levelItemPrefab  
**Type:** GameObject  
**Attributes:** private  
    - **Name:** contentContainer  
**Type:** Transform  
**Attributes:** private  
    - **Name:** filterDropdown  
**Type:** TMP_Dropdown  
**Attributes:** private  
    
**Methods:**
    
    - **Name:** OnShow  
**Parameters:**
    
    - object data
    
**Return Type:** void  
**Attributes:** protected|override  
    - **Name:** PopulateLevels  
**Parameters:**
    
    
**Return Type:** void  
**Attributes:** private  
    - **Name:** OnLevelSelected  
**Parameters:**
    
    - LevelSelectInfoVM levelInfo
    
**Return Type:** void  
**Attributes:** public  
    
**Implemented Features:**
    
    - Level Listing
    - Progress Display
    
**Requirement Ids:**
    
    - FR-U-002
    
**Purpose:** Displays available levels and player progress, allowing the player to select which puzzle to play.  
**Logic Description:** OnShow, it fetches player progress and available level data from an application service. It then iterates through this data, instantiating `LevelSelectItem` prefabs and populating them with level information (name, stars earned, locked status). When a level item is clicked, OnLevelSelected is called, which then requests the Application layer to start that specific level.  
**Documentation:**
    
    - **Summary:** This screen acts as the hub for players to see their progression and choose a level. It dynamically generates the list of levels based on the player's save data.
    
**Namespace:** PatternCipher.Presentation.Screens.LevelSelect  
**Metadata:**
    
    - **Category:** UIController
    
- **Path:** src/PatternCipher.Presentation/Scripts/Screens/Game/GameScreenView.cs  
**Description:** The main controller for the in-game Heads-Up Display (HUD). It holds references to all on-screen gameplay UI elements like the move counter, timer, pause button, and the GridView itself. It updates these elements based on events from the game logic.  
**Template:** C# Script  
**Dependency Level:** 1  
**Name:** GameScreenView  
**Type:** View  
**Relative Path:** Scripts/Screens/Game/GameScreenView.cs  
**Repository Id:** REPO-PATT-003  
**Pattern Ids:**
    
    - MVC
    
**Members:**
    
    - **Name:** gridView  
**Type:** GridView  
**Attributes:** private  
    - **Name:** moveCounterText  
**Type:** TextMeshProUGUI  
**Attributes:** private  
    - **Name:** timerText  
**Type:** TextMeshProUGUI  
**Attributes:** private  
    - **Name:** goalDisplayToggle  
**Type:** Button  
**Attributes:** private  
    - **Name:** pauseButton  
**Type:** Button  
**Attributes:** private  
    
**Methods:**
    
    - **Name:** UpdateMoveCounter  
**Parameters:**
    
    - int moveCount
    
**Return Type:** void  
**Attributes:** public  
    - **Name:** UpdateTimer  
**Parameters:**
    
    - float time
    
**Return Type:** void  
**Attributes:** public  
    - **Name:** SetupLevel  
**Parameters:**
    
    - LevelData levelData
    
**Return Type:** void  
**Attributes:** public  
    - **Name:** OnPauseButtonClicked  
**Parameters:**
    
    
**Return Type:** void  
**Attributes:** private  
    
**Implemented Features:**
    
    - Game HUD Management
    
**Requirement Ids:**
    
    - FR-U-003
    
**Purpose:** Manages the display of all in-game UI elements, providing the player with real-time information about the current puzzle state.  
**Logic Description:** This script acts as the main view for the gameplay scene. It receives data updates (e.g., from a GameHUD_VM or through direct method calls from a controller) and applies them to its child UI components, such as updating the text for the move counter or timer. It also handles button clicks for pausing or resetting the level.  
**Documentation:**
    
    - **Summary:** Controller for the main game screen, responsible for displaying the grid, score, moves, timer, and other relevant gameplay information.
    
**Namespace:** PatternCipher.Presentation.Screens.Game  
**Metadata:**
    
    - **Category:** UIController
    
- **Path:** src/PatternCipher.Presentation/Scripts/Screens/Pause/PauseScreen.cs  
**Description:** Controller for the Pause Menu popup. It provides options to 'Resume', 'Restart', go to 'Settings', or return to the 'Main Menu'.  
**Template:** C# Script  
**Dependency Level:** 1  
**Name:** PauseScreen  
**Type:** View  
**Relative Path:** Scripts/Screens/Pause/PauseScreen.cs  
**Repository Id:** REPO-PATT-003  
**Pattern Ids:**
    
    - MVC
    
**Members:**
    
    - **Name:** resumeButton  
**Type:** Button  
**Attributes:** private  
    - **Name:** restartButton  
**Type:** Button  
**Attributes:** private  
    - **Name:** settingsButton  
**Type:** Button  
**Attributes:** private  
    - **Name:** mainMenuButton  
**Type:** Button  
**Attributes:** private  
    
**Methods:**
    
    - **Name:** OnResumeButtonClicked  
**Parameters:**
    
    
**Return Type:** void  
**Attributes:** private  
    - **Name:** OnRestartButtonClicked  
**Parameters:**
    
    
**Return Type:** void  
**Attributes:** private  
    
**Implemented Features:**
    
    - Pause Menu Logic
    
**Requirement Ids:**
    
    - FR-U-004
    
**Purpose:** Handles user interactions within the pause menu, allowing the player to control the game session.  
**Logic Description:** Inherits from BaseScreen. Button listeners are wired up to call the appropriate methods on the Application layer's GameManager or UIManager, such as resuming the game, restarting the current level, or navigating to another screen. It ensures the game time scale is set to 0 when shown and restored when hidden.  
**Documentation:**
    
    - **Summary:** Manages the UI and logic for the in-game pause menu, providing navigation and game state control options to the player.
    
**Namespace:** PatternCipher.Presentation.Screens.Pause  
**Metadata:**
    
    - **Category:** UIController
    
- **Path:** src/PatternCipher.Presentation/Scripts/Screens/LevelComplete/LevelCompleteScreen.cs  
**Description:** Controller for the Level Complete screen. It displays the final score, stars earned, moves, and time. It also provides navigation buttons for 'Next Level', 'Replay', and 'Level Selection'. Features rewarding animations and sound.  
**Template:** C# Script  
**Dependency Level:** 1  
**Name:** LevelCompleteScreen  
**Type:** View  
**Relative Path:** Scripts/Screens/LevelComplete/LevelCompleteScreen.cs  
**Repository Id:** REPO-PATT-003  
**Pattern Ids:**
    
    - MVC
    
**Members:**
    
    - **Name:** scoreText  
**Type:** TextMeshProUGUI  
**Attributes:** private  
    - **Name:** starRatingDisplay  
**Type:** StarRatingDisplay  
**Attributes:** private  
    - **Name:** nextLevelButton  
**Type:** Button  
**Attributes:** private  
    - **Name:** replayButton  
**Type:** Button  
**Attributes:** private  
    
**Methods:**
    
    - **Name:** OnShow  
**Parameters:**
    
    - object data
    
**Return Type:** void  
**Attributes:** protected|override  
    - **Name:** PlayVictoryAnimation  
**Parameters:**
    
    
**Return Type:** IEnumerator  
**Attributes:** private  
    - **Name:** OnNextLevelClicked  
**Parameters:**
    
    
**Return Type:** void  
**Attributes:** private  
    
**Implemented Features:**
    
    - Post-Level Summary
    - Victory Feedback
    
**Requirement Ids:**
    
    - FR-U-005
    - FR-U-008
    - NFR-V-003
    
**Purpose:** Presents the player with their performance results upon completing a level and provides options for what to do next.  
**Logic Description:** When shown, it receives a `LevelCompleteStatsVM` object. It populates its UI elements with this data. It then triggers a sequence of 'juicy' animations, like stars filling up one by one, score counting up, accompanied by sound effects from the FeedbackManager. Button handlers navigate the user to the next level or back to the level select screen.  
**Documentation:**
    
    - **Summary:** The view controller for the screen that appears after a player successfully completes a level, showing their stats and providing rewarding feedback.
    
**Namespace:** PatternCipher.Presentation.Screens.LevelComplete  
**Metadata:**
    
    - **Category:** UIController
    
- **Path:** src/PatternCipher.Presentation/Scripts/Screens/Settings/SettingsScreen.cs  
**Description:** Controller for the Settings screen. Manages UI elements for various game settings, such as volume sliders for music and SFX, and toggles for accessibility features like colorblind mode.  
**Template:** C# Script  
**Dependency Level:** 1  
**Name:** SettingsScreen  
**Type:** View  
**Relative Path:** Scripts/Screens/Settings/SettingsScreen.cs  
**Repository Id:** REPO-PATT-003  
**Pattern Ids:**
    
    - MVC
    
**Members:**
    
    - **Name:** musicVolumeSlider  
**Type:** Slider  
**Attributes:** private  
    - **Name:** sfxVolumeSlider  
**Type:** Slider  
**Attributes:** private  
    - **Name:** colorblindModeDropdown  
**Type:** TMP_Dropdown  
**Attributes:** private  
    - **Name:** resetProgressButton  
**Type:** Button  
**Attributes:** private  
    
**Methods:**
    
    - **Name:** OnShow  
**Parameters:**
    
    - object data
    
**Return Type:** void  
**Attributes:** protected|override  
    - **Name:** OnMusicVolumeChanged  
**Parameters:**
    
    - float value
    
**Return Type:** void  
**Attributes:** private  
    - **Name:** OnColorblindModeChanged  
**Parameters:**
    
    - int index
    
**Return Type:** void  
**Attributes:** private  
    - **Name:** OnResetProgressClicked  
**Parameters:**
    
    
**Return Type:** void  
**Attributes:** private  
    
**Implemented Features:**
    
    - Game Settings Configuration
    
**Requirement Ids:**
    
    - FR-U-006
    - NFR-US-005
    
**Purpose:** Allows the player to configure game settings such as audio volume and accessibility options.  
**Logic Description:** OnShow, it populates the UI controls with the current values from a settings service. It then adds listeners to each control (e.g., `onValueChanged`). When a value changes, the corresponding handler is called, which in turn notifies the relevant service (e.g., AudioManager, AccessibilityManager) of the new setting, which is then persisted.  
**Documentation:**
    
    - **Summary:** Manages the user interface for all player-configurable game settings. It reads current settings to populate the view and saves changes made by the user.
    
**Namespace:** PatternCipher.Presentation.Screens.Settings  
**Metadata:**
    
    - **Category:** UIController
    
- **Path:** src/PatternCipher.Presentation/Scripts/Screens/HowToPlay/HowToPlayScreen.cs  
**Description:** Controller for the 'How to Play' or Tutorial reference screen. Displays static information, diagrams, or short animations explaining the various game mechanics, puzzle types, and special tiles.  
**Template:** C# Script  
**Dependency Level:** 1  
**Name:** HowToPlayScreen  
**Type:** View  
**Relative Path:** Scripts/Screens/HowToPlay/HowToPlayScreen.cs  
**Repository Id:** REPO-PATT-003  
**Pattern Ids:**
    
    - MVC
    
**Members:**
    
    - **Name:** contentArea  
**Type:** ScrollRect  
**Attributes:** private  
    - **Name:** tutorialSectionPrefab  
**Type:** GameObject  
**Attributes:** private  
    
**Methods:**
    
    - **Name:** OnShow  
**Parameters:**
    
    - object data
    
**Return Type:** void  
**Attributes:** protected|override  
    - **Name:** PopulateTutorialSections  
**Parameters:**
    
    
**Return Type:** void  
**Attributes:** private  
    
**Implemented Features:**
    
    - Tutorial Information Display
    
**Requirement Ids:**
    
    - FR-U-007
    
**Purpose:** Provides a static, referenceable guide to game mechanics for players.  
**Logic Description:** This screen dynamically populates a scrollable area with prefab sections for each game mechanic. The data for these sections (text, images, animation references) is loaded from ScriptableObjects or a configuration file, allowing for easy updates to the tutorial content without changing code.  
**Documentation:**
    
    - **Summary:** The controller for the screen that provides players with instructions and reference material on how to play the game.
    
**Namespace:** PatternCipher.Presentation.Screens.HowToPlay  
**Metadata:**
    
    - **Category:** UIController
    
- **Path:** src/PatternCipher.Presentation/Scripts/Feedback/FeedbackManager.cs  
**Description:** A centralized service for triggering visual and audio feedback. Other components call this manager to play a specific sound or trigger a visual effect, ensuring feedback is consistent and can be globally controlled (e.g., by the 'Reduced Motion' setting).  
**Template:** C# Script  
**Dependency Level:** 1  
**Name:** FeedbackManager  
**Type:** Manager  
**Relative Path:** Scripts/Feedback/FeedbackManager.cs  
**Repository Id:** REPO-PATT-003  
**Pattern Ids:**
    
    - Singleton
    
**Members:**
    
    - **Name:** vfxPlayer  
**Type:** VFXPlayer  
**Attributes:** private  
    - **Name:** cameraShaker  
**Type:** CameraShake  
**Attributes:** private  
    - **Name:** audioManager  
**Type:** AudioManager  
**Attributes:** private  
    
**Methods:**
    
    - **Name:** PlayTileSwapFeedback  
**Parameters:**
    
    
**Return Type:** void  
**Attributes:** public  
    - **Name:** PlayInvalidMoveFeedback  
**Parameters:**
    
    - Transform tileTransform
    
**Return Type:** void  
**Attributes:** public  
    - **Name:** PlayLevelCompleteFeedback  
**Parameters:**
    
    
**Return Type:** void  
**Attributes:** public  
    
**Implemented Features:**
    
    - Centralized Game Feel
    - Juiciness Effects
    
**Requirement Ids:**
    
    - FR-U-008
    - NFR-V-003
    
**Purpose:** Decouples feedback effects (sound, particles, animation) from the game logic that triggers them, improving maintainability and consistency.  
**Logic Description:** Provides a public API of contextual feedback methods (e.g., PlayTileSwapFeedback). Internally, it calls the appropriate subsystems (AudioManager for sound, VFXPlayer for particles, UIAnimator for animations) to execute the feedback. It checks the 'Reduced Motion' setting before triggering significant visual effects like screen shake.  
**Documentation:**
    
    - **Summary:** This service acts as a central hub for triggering all 'juicy' feedback effects, both visual and auditory. This keeps the triggering logic separate from the effect implementation.
    
**Namespace:** PatternCipher.Presentation.Feedback  
**Metadata:**
    
    - **Category:** UIController
    
- **Path:** src/PatternCipher.Presentation/Scripts/Accessibility/AccessibilityManager.cs  
**Description:** Manages all accessibility-related features. This includes applying colorblind filters or palettes, adjusting text sizes, and controlling the 'Reduced Motion' setting across the application.  
**Template:** C# Script  
**Dependency Level:** 1  
**Name:** AccessibilityManager  
**Type:** Manager  
**Relative Path:** Scripts/Accessibility/AccessibilityManager.cs  
**Repository Id:** REPO-PATT-003  
**Pattern Ids:**
    
    - Singleton
    - ObserverPattern
    
**Members:**
    
    - **Name:** currentColorblindMode  
**Type:** ColorblindMode  
**Attributes:** private  
    - **Name:** isReducedMotionEnabled  
**Type:** bool  
**Attributes:** public|readonly  
    
**Methods:**
    
    - **Name:** SetColorblindMode  
**Parameters:**
    
    - ColorblindMode mode
    
**Return Type:** void  
**Attributes:** public  
    - **Name:** SetReducedMotion  
**Parameters:**
    
    - bool isEnabled
    
**Return Type:** void  
**Attributes:** public  
    
**Implemented Features:**
    
    - Colorblind Mode Management
    - Reduced Motion Control
    
**Requirement Ids:**
    
    - NFR-US-005
    
**Purpose:** Provides a central point of control for all accessibility features, applying user preferences throughout the UI.  
**Logic Description:** This manager holds the current state of accessibility settings. When a setting is changed (e.g., via the SettingsScreen), it broadcasts an event or uses a public property that other components (like FeedbackManager or TileView) can query to adjust their behavior accordingly. For color modes, it might swap out a global color palette asset.  
**Documentation:**
    
    - **Summary:** A service that manages and applies accessibility settings, such as colorblind modes and reduced motion, across the entire application.
    
**Namespace:** PatternCipher.Presentation.Accessibility  
**Metadata:**
    
    - **Category:** UIController
    


---

# 2. Configuration

- **Feature Toggles:**
  
  - EnableLeaderboards
  - EnableAchievements
  - EnableUndoButton
  - EnableHintSystem
  
- **Database Configs:**
  
  


---

