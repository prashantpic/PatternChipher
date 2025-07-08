# Software Design Specification (SDS): UserInterfaceEndpoints (REPO-PATT-003)

## 1. Introduction

### 1.1 Purpose
This document provides a detailed technical specification for the `UserInterfaceEndpoints` repository (`REPO-PATT-003`). This repository constitutes the **Presentation Layer** of the Pattern Cipher game client. Its primary responsibilities are to render all user interfaces, handle raw player input, and provide all visual and auditory feedback required to create a responsive, accessible, and "juicy" user experience.

### 1.2 Scope
The scope of this repository includes the implementation of all UI screens, the in-game HUD, the visual representation of the game grid and tiles, and the management of all UI-related animations and feedback effects. It is dependent on the Application Layer (`GameManager`) for state management and the Domain Layer (`GameplayLogicEndpoints`) for data models.

### 1.3 Technologies
- **Engine:** Unity 2022 LTS
- **Language:** C# (.NET)
- **UI Framework:** Unity UI (UGUI) with TextMeshPro
- **Input System:** Unity Input System
- **Animation Library:** DOTween

---

## 2. System Architecture & Design Patterns

### 2.1 Architectural Approach
This repository adheres to a **Layered Architecture**, strictly acting as the Presentation Layer. It follows an **MVC (Model-View-Controller)** / **MVP (Model-View-Presenter)** hybrid pattern:
- **Model:** Plain C# data objects (e.g., `GridData`, `TileData`, `PlayerProfile`) provided by the Domain and Application layers. This layer does *not* define models.
- **View:** Unity `MonoBehaviour` components responsible for rendering data and capturing input (e.g., `TileView`, `MainMenuScreen`). Views are designed to be as "dumb" as possible.
- **Controller/Presenter:** Manager classes (e.g., `UIManager`, screen-specific controllers) that mediate between the Application Layer and the Views. They receive state updates and command the Views to update themselves.

### 2.2 Core Design Patterns
- **Observer Pattern:** UI components will update in response to game state changes by subscribing to C# events broadcast by Application Layer services (e.g., `GameManager.OnScoreUpdated`, `ProgressionManager.OnLevelUnlocked`). This decouples the UI from the game logic.
- **Singleton Pattern:** Used for global manager services like `UIManager`, `FeedbackManager`, and `AccessibilityManager` to provide easy, centralized access.
- **Prefab-Based Views:** All UI screens, list items, and game tiles will be implemented as Unity Prefabs, allowing for visual design in the editor and dynamic instantiation at runtime.
- **Facade Pattern:** The `FeedbackManager` will act as a facade, providing a simple, high-level API for triggering complex combinations of visual and auditory feedback.

---

## 3. Component Specifications

### 3.1 UI Management

#### 3.1.1 `UIManager.cs` (Manager)
- **Purpose:** Central singleton for managing the lifecycle and transitions of all UI screens and popups.
- **Responsibilities:**
    - Maintain a stack of active screens to handle navigation (e.g., back functionality).
    - Instantiate screen prefabs on demand.
    - Control screen transitions (e.g., fade-in/fade-out animations using DOTween and a CanvasGroup).
    - Act as a central point for showing globally relevant popups (e.g., "Confirm Reset" dialog).
    - Listen to high-level game events (e.g., `OnLevelCompleted`) to trigger screen changes.
- **Key Methods:**
    - `public static UIManager Instance { get; }`: Singleton instance.
    - `public void ShowScreen<T>(object data = null) where T : BaseScreen`: Instantiates and displays a screen of type T, pushing it onto the screen stack. Passes optional data for initialization.
    - `public void HideScreen<T>() where T : BaseScreen`: Hides and potentially destroys a screen of type T.
    - `public void HideCurrentScreen()`: Hides the screen at the top of the stack.
- **Dependencies:** `GameManager`, all `BaseScreen` implementations.

#### 3.1.2 `BaseScreen.cs` (Abstract View)
- **Purpose:** Abstract base class for all UI screens to ensure a consistent interface and lifecycle.
- **Responsibilities:**
    - Define the contract for showing and hiding screens.
- **Key Methods:**
    - `public virtual void Show(object data = null)`: Base implementation to activate the screen's GameObject and call `OnShow`.
    - `public virtual void Hide()`: Base implementation to deactivate the screen's GameObject and call `OnHide`.
    - `protected virtual void OnShow(object data)`: Abstract or virtual method for subclasses to override for setup logic (e.g., populating data).
    - `protected virtual void OnHide()`: Abstract or virtual method for subclasses to override for teardown logic.

### 3.2 Game Screens & Views

#### 3.2.1 `MainMenuScreen.cs` (View/Controller)
- **Purpose:** Controls the main menu UI.
- **Requirement Mapping:** `FR-U-001`.
- **Responsibilities:**
    - Handle OnClick events for "Play Game", "Settings", "How to Play", and "Exit" buttons.
    - The "Exit" button's visibility will be conditionally controlled (`#if UNITY_ANDROID ... #endif`).
- **Logic:**
    - `OnPlayButtonClicked()`: Calls `UIManager.Instance.ShowScreen<LevelSelectScreen>()`.
    - `OnSettingsButtonClicked()`: Calls `UIManager.Instance.ShowScreen<SettingsScreen>()`.
    - `OnHowToPlayButtonClicked()`: Calls `UIManager.Instance.ShowScreen<HowToPlayScreen>()`.
    - `OnExitButtonClicked()`: Calls `Application.Quit()`.

#### 3.2.2 `LevelSelectScreen.cs` (View/Controller)
- **Purpose:** Displays available levels and player progress.
- **Requirement Mapping:** `FR-U-002`.
- **Responsibilities:**
    - On `OnShow`, request level progression data from the `ProgressionManager`.
    - Dynamically instantiate and populate `LevelSelectItem` prefabs in a `ScrollRect`.
    - Each `LevelSelectItem` will display the level name/number, stars earned, and a locked/unlocked state.
    - Handle clicks on level items to start a level.
- **Logic:**
    - `OnLevelSelected(int levelId)`: Calls `GameManager.Instance.StartLevel(levelId)`.

#### 3.2.3 `GameScreenView.cs` (View)
- **Purpose:** Manages the in-game HUD.
- **Requirement Mapping:** `FR-U-003`.
- **Responsibilities:**
    - Hold and manage references to all HUD UI elements (`TextMeshProUGUI` for moves/timer, buttons, etc.).
    - Expose public methods to update the HUD elements.
    - Contain a reference to the `GridView` component.
- **Key Methods:**
    - `public void UpdateMoveCounter(int currentMoves, int parMoves)`: Updates the move counter text.
    - `public void UpdateTimer(float time)`: Updates the timer display.
    - `public void SetGoalDisplay(GoalViewModel goal)`: Updates the visual display for the level's goal.
- **Logic:** This class is a "dumb" view. It subscribes to events from `GameManager` (e.g., `OnMoveCountChanged`) and updates its text fields accordingly. Button clicks (Pause, Reset) raise events that `GameManager` listens to.

#### 3.2.4 `PauseScreen.cs` (View/Controller)
- **Purpose:** Manages the pause menu overlay.
- **Requirement Mapping:** `FR-U-004`.
- **Logic:**
    - On show, sets `Time.timeScale = 0`. On hide, sets `Time.timeScale = 1`.
    - `OnResumeButtonClicked()`: Calls `UIManager.Instance.HideCurrentScreen()`.
    - `OnRestartButtonClicked()`: Calls `GameManager.Instance.RestartLevel()`.
    - `OnSettingsButtonClicked()`: Calls `UIManager.Instance.ShowScreen<SettingsScreen>()`.
    - `OnMainMenuButtonClicked()`: Calls `GameManager.Instance.GoToMainMenu()`.

#### 3.2.5 `LevelCompleteScreen.cs` (View/Controller)
- **Purpose:** Displays level results with rewarding feedback.
- **Requirement Mapping:** `FR-U-005`, `NFR-V-003`.
- **Logic:**
    - `OnShow(object data)` receives a `LevelCompletionData` view model.
    - Populates score, moves, and time text fields.
    - Initiates a `DOTween.Sequence` to animate the star rating display, score counter, etc., in a "juicy" and rewarding fashion, coordinated with `FeedbackManager`.
    - Button clicks navigate to the next level, replay, or level selection.

#### 3.2.6 `SettingsScreen.cs` & `HowToPlayScreen.cs`
- **Purpose:** Implement the UI for settings and tutorials respectively.
- **Requirement Mapping:** `FR-U-006`, `FR-U-007`, `NFR-US-005`.
- **Logic:**
    - **Settings:** Reads current settings from services (`AudioManager`, `AccessibilityManager`) to initialize sliders/toggles. `onValueChanged` events on UI controls call the respective manager's setter methods.
    - **How To Play:** Dynamically populates a `ScrollRect` with tutorial section prefabs based on ScriptableObject data for easy content management.

### 3.3 Gameplay Grid Views

#### 3.3.1 `GridView.cs` (View)
- **Purpose:** Renders the interactive game board.
- **Responsibilities:**
    - On level start, receive `GridData` from the `GameManager`.
    - Instantiate `TileView` prefabs for each tile in the model.
    - Position `TileView` objects correctly within a `GridLayoutGroup`.
    - Orchestrate complex animations involving multiple tiles (e.g., swaps, row/column clears).
- **Key Methods:**
    - `public void CreateGrid(GridData gridData)`: Clears the existing grid and builds a new one.
    - `public void AnimateSwap(Vector2Int posA, Vector2Int posB, Action onComplete)`: Uses a `DOTween.Sequence` to smoothly animate the swapping of two `TileView` transforms. Invokes `onComplete` when finished.
    - `public TileView GetTileViewAt(Vector2Int position)`: Returns the `TileView` instance at a given coordinate.

#### 3.3.2 `TileView.cs` (View)
- **Purpose:** Visual representation of a single tile.
- **Requirement Mapping:** `FR-U-008`, `NFR-V-003`, `NFR-US-005`.
- **Responsibilities:**
    - Update its visual appearance (symbol sprite, background color, locked overlay) based on `TileData`.
    - Handle `IPointerDownHandler`, `IDragHandler`, etc. from the Unity Input System and raise C# events (e.g., `public static event Action<Vector2Int> OnTileTapped`).
    - Animate its own state changes (e.g., `DOTween.PunchScale()` on selection).
    - Ensure its `RectTransform` provides a large enough tap target (>44x44pts).
- **Key Methods:**
    - `public void Initialize(TileData data)`: Sets the initial state.
    - `public void UpdateVisuals(TileData data)`: Updates the visuals to match new data.
    - `public void AnimateSelection(bool isSelected)`: Animates the selection highlight.
    - `public void AnimateError()`: Plays a "shake" animation using `DOTween`.

### 3.4 Feedback & Accessibility

#### 3.4.1 `FeedbackManager.cs` (Manager)
- **Purpose:** Central hub for triggering "juicy" feedback effects.
- **Requirement Mapping:** `FR-U-008`, `NFR-V-003`.
- **Responsibilities:**
    - Provide a high-level API for feedback (e.g., `PlaySuccessFeedback()`).
    - Coordinate calls to `AudioManager` for SFX, a `VFXManager` for particles, and a `CameraShake` utility.
    - Check with `AccessibilityManager` before triggering intense visual effects.
- **Key Methods:**
    - `public void PlayFeedback(FeedbackType type, Vector3 position)`: A generic method to play a predefined feedback sequence.

#### 3.4.2 `AccessibilityManager.cs` (Manager)
- **Purpose:** Central point of control for all accessibility features.
- **Requirement Mapping:** `NFR-US-005`.
- **Responsibilities:**
    - Hold the state for `ColorblindMode` and `IsReducedMotionEnabled`.
    - Provide methods to change these settings, which will be called by `SettingsScreen`.
    - Broadcast events (e.g., `OnAccessibilitySettingsChanged`) when settings are updated, so other components can react.
- **Key Methods:**
    - `public void SetColorblindMode(ColorblindMode mode)`
    - `public void SetReducedMotion(bool isEnabled)`