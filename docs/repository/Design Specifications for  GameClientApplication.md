# Software Design Specification (SDS): GameClientApplication (REPO-PATT-001)

## 1. Introduction

This document provides the detailed software design for the `GameClientApplication` repository (`REPO-PATT-001`). This repository serves as the primary entry point and central orchestrator for the Pattern Cipher mobile game. Its core responsibilities include managing the application's lifecycle, controlling the main game state, orchestrating all other client-side services and managers, and bridging communication between the user interface (Presentation), core game rules (Domain), and external services (Infrastructure).

This repository implements the Application Layer of the client's Layered Architecture.

## 2. Architectural Design & Patterns

The `GameClientApplication` is designed using a combination of established architectural and design patterns to ensure maintainability, testability, and a clear separation of concerns.

-   **Layered Architecture**: This repository represents the Application Layer, acting as a mediator between the Presentation layer (`REPO-PATT-003`) and the Domain (`REPO-PATT-002`) and Infrastructure (`REPO-PATT-004`, `REPO-PATT-005`) layers.
-   **Event-Driven Architecture**: A global `GameEventSystem` is used to facilitate decoupled communication between various managers and systems. This minimizes direct dependencies and allows for flexible and scalable feature integration.
-   **Singleton Pattern**: The `GameManager` is implemented as a singleton to ensure a single, globally accessible point of control for the game state and lifecycle.
-   **State Machine Pattern**: The `GameManager` utilizes a state machine to manage the high-level flow of the game (e.g., `MainMenu`, `InGame`, `Paused`). This provides a structured and predictable way to handle transitions and associated logic.
-   **Service Locator / Dependency Injection**: Core services (like Persistence, UI, Audio) will be instantiated at startup by `AppInitializer` and accessed by the `GameManager` and other managers, likely through a simple service locator or direct references set up during initialization.

## 3. Core Components Detailed Design

### 3.1. Assembly Definition (`PatternCipher.Client.asmdef`)

-   **Purpose**: To define the `PatternCipher.Client` assembly, enforcing architectural boundaries.
-   **Type**: Unity Assembly Definition File.
-   **Configuration**:
    -   `name`: `PatternCipher.Client`
    -   `references`: This assembly will explicitly reference the following other assemblies:
        -   `PatternCipher.Domain` (from `REPO-PATT-002`)
        -   `PatternCipher.Presentation` (from `REPO-PATT-003`)
        -   `PatternCipher.Infrastructure.Persistence` (from `REPO-PATT-004`)
        -   `PatternCipher.Infrastructure.Firebase` (from `REPO-PATT-005`)
        -   `PatternCipher.Shared` (from `REPO-PATT-012`)
    -   `allowUnsafeCode`: `false`
    -   `overrideReferences`: `true`
    -   `precompiledReferences`: `[]`
    -   `autoReferenced`: `false`
    -   `defineConstraints`: `[]`

### 3.2. Application Initialization (`AppInitializer.cs`)

-   **Purpose**: The primary bootstrap script for the entire application. It ensures all core systems are initialized in the correct order before gameplay begins.
-   **Type**: `MonoBehaviour`
-   **Responsibilities**:
    -   Serve as the first-run script in the initial "Bootstrap" scene.
    -   Instantiate the persistent `GameManager` prefab.
    -   Initialize all core services (Persistence, Audio, UI, etc.).
    -   Transition the `GameManager` to its initial state (`MainMenu`).
-   **Class Definition**:
    csharp
    namespace PatternCipher.Client.Application
    {
        public class AppInitializer : MonoBehaviour
        {
            // Implementation details...
        }
    }
    
-   **Properties**:
    -   `[SerializeField] private GameObject _gameManagerPrefab;`: A reference to the prefab containing the `GameManager` and other persistent managers.
-   **Method Logic**:
    -   `private void Awake()`:
        1.  Check if a `GameManager` instance already exists. If so, do nothing (to handle returning to the bootstrap scene).
        2.  If not, instantiate `_gameManagerPrefab`. This will trigger the `Awake` method of the `GameManager` itself.
        3.  (Potentially) Initialize other core services that are not part of the `GameManager` prefab.
        4.  Load the Main Menu scene via the `SceneLoader`. This is often orchestrated by the `GameManager`'s initial state transition.

### 3.3. Central Orchestrator (`GameManager.cs`)

-   **Purpose**: The central singleton orchestrator for game state, lifecycle, and inter-service communication.
-   **Type**: `MonoBehaviour`
-   **Responsibilities**:
    -   Maintain its existence across all scene loads.
    -   Manage the main game state machine.
    -   Provide global access to its instance.
    -   Handle application lifecycle events (`OnApplicationPause`, `OnApplicationQuit`).
    -   Coordinate high-level game flow (e.g., starting a level, pausing, resuming).
-   **Class Definition**:
    csharp
    using PatternCipher.Client.Events;
    using PatternCipher.Client.Scenes;
    using PatternCipher.Shared.Models;
    
    namespace PatternCipher.Client.Application
    {
        public class GameManager : MonoBehaviour
        {
            // Implementation details...
        }
    }
    
-   **Properties**:
    -   `public static GameManager Instance { get; private set; }`: The static singleton instance.
    -   `public GameState CurrentState { get; private set; }`: The current state of the game state machine.
    -   `[SerializeField] private LevelManager _levelManager;`: Reference to the `LevelManager` component.
    -   `private IUIService _uiService;`: Reference to the UI Service (from Presentation layer).
    -   `private IPersistenceService _persistenceService;`: Reference to the Persistence Service (from Infrastructure layer).
    -   `private IBackendService _backendService;`: Reference to the Backend Service Facade (from Infrastructure layer).
    -   `private SceneLoader _sceneLoader;`: Reference to the Scene Loader utility.
-   **Method Logic**:
    -   `private void Awake()`:
        1.  Implement the singleton pattern: Check if `Instance` is null. If not, destroy `gameObject`.
        2.  If `Instance` is null, set `Instance = this;` and call `DontDestroyOnLoad(gameObject);`.
        3.  Locate and assign references to core services (`_persistenceService`, `_uiService`, `_backendService`, etc.).
        4.  Load player data using `_persistenceService.LoadProfile()`.
        5.  Change state to the initial state: `ChangeState(GameState.MainMenu);`.
    -   `public void ChangeState(GameState newState)`:
        1.  If `newState == CurrentState`, return.
        2.  Call `OnExitState(CurrentState)`.
        3.  Set `CurrentState = newState;`.
        4.  Call `OnEnterState(newState)`.
        5.  Publish a `GameStateChangedEvent` via the `GameEventSystem`.
    -   `private void OnEnterState(GameState state)`: A `switch` statement that executes logic based on the new state.
        -   `GameState.MainMenu`: `_sceneLoader.LoadSceneAsync(SceneId.MainMenu, ...);`
        -   `GameState.InGame`: `_sceneLoader.LoadSceneAsync(SceneId.Game, ...);`
        -   Other states will manage UI visibility or game logic (e.g., `Time.timeScale = 0` for `Paused`).
    -   `private void OnExitState(GameState state)`: Logic to clean up from the previous state.
    -   `public void StartLevel(LevelDefinition level)`: Called by UI to start a game. Sets the current level data and calls `ChangeState(GameState.InGame)`.
    -   `public void PauseGame()`: Calls `ChangeState(GameState.Paused)`.
    -   `public void ResumeGame()`: Calls `ChangeState(GameState.InGame)`.
    -   `private void OnApplicationPause(bool pauseStatus)`: If `pauseStatus` is true, trigger a save operation: `_persistenceService.SaveProfile(playerProfile);`.
    -   `private void OnApplicationQuit()`: Trigger a final save operation.

### 3.4. State Definitions (`GameState.cs` & `SceneId.cs`)

-   **Purpose**: To provide strongly-typed, maintainable enumerations for game states and scenes.
-   **Type**: `enum`
-   **Definitions**:
    -   `public enum GameState { Initializing, MainMenu, LevelSelection, InGame, Paused, LevelComplete }`
    -   `public enum SceneId { Bootstrap, MainMenu, Game }`

### 3.5. Scene Management (`SceneLoader.cs`)

-   **Purpose**: A dedicated utility for handling scene transitions.
-   **Type**: Standard C# class (can be a `MonoBehaviour` if it needs to run coroutines for animations).
-   **Responsibilities**:
    -   Load scenes asynchronously.
    -   Show/hide a loading screen UI.
    -   Provide callbacks upon load completion.
-   **Method Logic**:
    -   `public async Task LoadSceneAsync(SceneId sceneId, LoadSceneMode mode = LoadSceneMode.Single)`:
        1.  Trigger a "Fade Out" or "Show Loading Screen" event/animation.
        2.  Use `SceneManager.LoadSceneAsync(sceneId.ToString(), mode)`.
        3.  `await` the async operation.
        4.  Trigger a "Fade In" or "Hide Loading Screen" event/animation.
        5.  This method allows other systems to `await` the scene transition.

### 3.6. Event System (`GameEventSystem.cs` & `CoreGameEvents.cs`)

-   **Purpose**: To enable decoupled communication across the application.
-   **`GameEventSystem.cs` Type**: Static C# class.
-   **`CoreGameEvents.cs` Type**: C# file containing multiple `struct` definitions.
-   **Responsibilities**:
    -   Provide static methods to `Subscribe`, `Unsubscribe`, and `Publish` generic events.
    -   Define the data contracts (payloads) for all major game events.
-   **`GameEventSystem.cs` Method Logic**:
    -   `private static Dictionary<Type, Delegate> _eventListeners = new Dictionary<Type, Delegate>();`
    -   `public static void Subscribe<T>(Action<T> listener)`: Adds the `listener` delegate to the dictionary entry for type `T`. Handles dictionary key creation and delegate combining (`+=`).
    -   `public static void Unsubscribe<T>(Action<T> listener)`: Removes the `listener` delegate. Handles delegate removal (`-=`) and cleaning up empty dictionary entries.
    -   `public static void Publish<T>(T eventData)`: Retrieves the delegate for type `T` and invokes it with `eventData`.
-   **`CoreGameEvents.cs` Example Definitions**:
    csharp
    namespace PatternCipher.Client.Events
    {
        public struct GameStateChangedEvent { public GameState NewState; }
        public struct LevelStartedEvent { public LevelDefinition Level; }
        public struct LevelCompletedEvent { public LevelResult Result; } // LevelResult contains score, stars, etc.
        public struct SaveGameRequestEvent { }
    }
    