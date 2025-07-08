# Specification

# 1. Files

- **Path:** Assets/PatternCipher/Client/PatternCipher.Client.asmdef  
**Description:** Assembly Definition file for the main client application layer. It defines the assembly's name and its dependencies on other assemblies within the project, such as the Domain, Presentation, and Infrastructure layers. This is crucial for enforcing the layered architecture and managing compilation boundaries.  
**Template:** Unity Assembly Definition  
**Dependency Level:** 3  
**Name:** PatternCipher.Client  
**Type:** Configuration  
**Relative Path:** ../PatternCipher.Client  
**Repository Id:** REPO-PATT-001  
**Pattern Ids:**
    
    - LayeredArchitecture
    
**Members:**
    
    
**Methods:**
    
    
**Implemented Features:**
    
    - Assembly Definition
    
**Requirement Ids:**
    
    - 2.6.1
    
**Purpose:** To define the compilation unit for the Application layer, ensuring proper separation of concerns and managing dependencies with other layers of the application.  
**Logic Description:** This file will be configured in the Unity Inspector. The 'references' section will list the assembly names for REPO-PATT-002 (Domain), REPO-PATT-003 (Presentation), REPO-PATT-004 (Persistence), and REPO-PATT-005 (FirebaseFacade). This enforces that this Application layer can access the public interfaces of the layers it orchestrates.  
**Documentation:**
    
    - **Summary:** Manages compilation and dependency references for the PatternCipher.Client assembly. This file is central to maintaining the architectural integrity of the layered system.
    
**Namespace:** PatternCipher.Client  
**Metadata:**
    
    - **Category:** Configuration
    
- **Path:** Assets/PatternCipher/Client/Scripts/Application/AppInitializer.cs  
**Description:** A MonoBehaviour that serves as the main entry point for the application. It runs in the initial scene and is responsible for bootstrapping all core services and managers, ensuring they are initialized in the correct order before the main menu is displayed.  
**Template:** C# MonoBehaviour  
**Dependency Level:** 2  
**Name:** AppInitializer  
**Type:** Bootstrap  
**Relative Path:** Scripts/Application  
**Repository Id:** REPO-PATT-001  
**Pattern Ids:**
    
    - Initialization
    
**Members:**
    
    - **Name:** _gameManagerPrefab  
**Type:** GameObject  
**Attributes:** private|SerializeField  
    
**Methods:**
    
    - **Name:** Awake  
**Parameters:**
    
    
**Return Type:** void  
**Attributes:** private  
    
**Implemented Features:**
    
    - Game Initialization
    
**Requirement Ids:**
    
    - 2.6.1
    
**Purpose:** To ensure a controlled and orderly startup sequence for the entire game, initializing all necessary systems before gameplay begins.  
**Logic Description:** In the Awake method, this script will instantiate the GameManager prefab if one is not already present in the scene. It will then initialize core services such as the PersistenceService, FirebaseServiceFacade, AudioManager, etc. After all services are ready, it will transition the game to the MainMenu state, which will trigger the loading of the main menu scene.  
**Documentation:**
    
    - **Summary:** The AppInitializer script is responsible for the initial setup of the game. It ensures all managers and services are instantiated and configured, providing a stable state from which the game can start.
    
**Namespace:** PatternCipher.Client.Application  
**Metadata:**
    
    - **Category:** ApplicationLogic
    
- **Path:** Assets/PatternCipher/Client/Scripts/Application/GameManager.cs  
**Description:** The central orchestrator of the game. This MonoBehaviour persists across all scenes and manages the main game state machine (e.g., MainMenu, InGame, Paused). It coordinates between different managers and services to control the overall game flow, responding to high-level player actions and system events.  
**Template:** C# MonoBehaviour  
**Dependency Level:** 3  
**Name:** GameManager  
**Type:** Manager  
**Relative Path:** Scripts/Application  
**Repository Id:** REPO-PATT-001  
**Pattern Ids:**
    
    - Singleton
    - StateMachine
    - FacadePattern
    
**Members:**
    
    - **Name:** Instance  
**Type:** GameManager  
**Attributes:** public|static  
    - **Name:** CurrentState  
**Type:** GameState  
**Attributes:** public|readonly|property  
    - **Name:** _levelManager  
**Type:** LevelManager  
**Attributes:** private|SerializeField  
    - **Name:** _uiService  
**Type:** IUIService  
**Attributes:** private  
    - **Name:** _persistenceService  
**Type:** IPersistenceService  
**Attributes:** private  
    
**Methods:**
    
    - **Name:** Awake  
**Parameters:**
    
    
**Return Type:** void  
**Attributes:** private  
    - **Name:** ChangeState  
**Parameters:**
    
    - GameState newState
    
**Return Type:** void  
**Attributes:** public  
    - **Name:** StartLevel  
**Parameters:**
    
    - LevelDefinition level
    
**Return Type:** void  
**Attributes:** public  
    - **Name:** PauseGame  
**Parameters:**
    
    
**Return Type:** void  
**Attributes:** public  
    - **Name:** ResumeGame  
**Parameters:**
    
    
**Return Type:** void  
**Attributes:** public  
    - **Name:** OnApplicationPause  
**Parameters:**
    
    - bool pauseStatus
    
**Return Type:** void  
**Attributes:** private  
    - **Name:** OnApplicationQuit  
**Parameters:**
    
    
**Return Type:** void  
**Attributes:** private  
    
**Implemented Features:**
    
    - Game State Management
    - Application Lifecycle Handling
    - Service Orchestration
    
**Requirement Ids:**
    
    - 2.1
    - 2.6.1
    
**Purpose:** To act as the central nervous system of the game, managing high-level state, controlling scene flow, and coordinating the various subsystems (UI, Audio, Levels, Persistence).  
**Logic Description:** The GameManager will implement a singleton pattern to ensure a single instance exists. Its Awake method handles the singleton logic and DontDestroyOnLoad. The core logic is a state machine, managed by the ChangeState method. Each state change (e.g., to InGame) will trigger corresponding actions, like loading a new scene via the SceneLoader and commanding the LevelManager to start a level. It will also handle OnApplicationPause and OnApplicationQuit Unity messages to trigger data saving via the persistence service, ensuring the game functions correctly offline.  
**Documentation:**
    
    - **Summary:** Manages the overall game lifecycle and state. It coordinates all other managers to create a cohesive experience, handling transitions between menus, gameplay, and paused states, and managing data persistence during application lifecycle events.
    
**Namespace:** PatternCipher.Client.Application  
**Metadata:**
    
    - **Category:** ApplicationLogic
    
- **Path:** Assets/PatternCipher/Client/Scripts/Application/GameState.cs  
**Description:** A simple enumeration defining the possible high-level states of the application. This is used by the GameManager's state machine to control the game's flow and behavior.  
**Template:** C# Enum  
**Dependency Level:** 0  
**Name:** GameState  
**Type:** Enum  
**Relative Path:** Scripts/Application  
**Repository Id:** REPO-PATT-001  
**Pattern Ids:**
    
    
**Members:**
    
    - **Name:** Initializing  
**Type:** enum  
**Attributes:**   
    - **Name:** MainMenu  
**Type:** enum  
**Attributes:**   
    - **Name:** LevelSelection  
**Type:** enum  
**Attributes:**   
    - **Name:** InGame  
**Type:** enum  
**Attributes:**   
    - **Name:** Paused  
**Type:** enum  
**Attributes:**   
    - **Name:** LevelComplete  
**Type:** enum  
**Attributes:**   
    
**Methods:**
    
    
**Implemented Features:**
    
    - State Definition
    
**Requirement Ids:**
    
    - 2.6.1
    
**Purpose:** To provide a strongly-typed, clear definition of the game's possible states, preventing the use of magic strings and improving code readability and maintainability.  
**Logic Description:** This file contains a public enum named GameState. No other logic is required.  
**Documentation:**
    
    - **Summary:** Defines the finite states the game can be in, such as MainMenu, InGame, or Paused. This is used by the GameManager to control the overall application flow.
    
**Namespace:** PatternCipher.Client.Application  
**Metadata:**
    
    - **Category:** ApplicationLogic
    
- **Path:** Assets/PatternCipher/Client/Scripts/Scenes/SceneLoader.cs  
**Description:** A utility service responsible for handling all scene loading and unloading operations. It supports asynchronous loading to prevent the UI from freezing and can manage the display of a loading screen during transitions.  
**Template:** C# Service  
**Dependency Level:** 1  
**Name:** SceneLoader  
**Type:** Service  
**Relative Path:** Scripts/Scenes  
**Repository Id:** REPO-PATT-001  
**Pattern Ids:**
    
    
**Members:**
    
    
**Methods:**
    
    - **Name:** LoadSceneAsync  
**Parameters:**
    
    - SceneId sceneId
    - Action onLoaded
    
**Return Type:** void  
**Attributes:** public  
    
**Implemented Features:**
    
    - Asynchronous Scene Loading
    
**Requirement Ids:**
    
    - 2.6.1
    
**Purpose:** To abstract the complexity of Unity's SceneManager, providing a simple, asynchronous interface for transitioning between different parts of the game.  
**Logic Description:** The LoadSceneAsync method will use Unity's SceneManager.LoadSceneAsync function. It will activate a loading screen UI, perform the asynchronous load, and then deactivate the loading screen and invoke the onLoaded callback once the new scene is ready.  
**Documentation:**
    
    - **Summary:** Provides a centralized and robust way to manage scene transitions. It handles asynchronous loading and manages a loading screen to provide a smooth user experience.
    
**Namespace:** PatternCipher.Client.Scenes  
**Metadata:**
    
    - **Category:** ApplicationLogic
    
- **Path:** Assets/PatternCipher/Client/Scripts/Scenes/SceneId.cs  
**Description:** Defines an enumeration for all scenes in the game. This avoids the use of hardcoded strings for scene names, reducing the risk of typos and making the code more maintainable.  
**Template:** C# Enum  
**Dependency Level:** 0  
**Name:** SceneId  
**Type:** Enum  
**Relative Path:** Scripts/Scenes  
**Repository Id:** REPO-PATT-001  
**Pattern Ids:**
    
    
**Members:**
    
    - **Name:** Bootstrap  
**Type:** enum  
**Attributes:**   
    - **Name:** MainMenu  
**Type:** enum  
**Attributes:**   
    - **Name:** Game  
**Type:** enum  
**Attributes:**   
    
**Methods:**
    
    
**Implemented Features:**
    
    - Scene Name Abstraction
    
**Requirement Ids:**
    
    - 2.6.1
    
**Purpose:** To provide a type-safe way to reference Unity scenes, improving code quality and preventing runtime errors from misspelled scene names.  
**Logic Description:** This file contains a public enum named SceneId. Each enum member name should correspond exactly to a scene name in the Unity Build Settings.  
**Documentation:**
    
    - **Summary:** An enumeration that maps readable names to the actual scene files used in the game, used by the SceneLoader.
    
**Namespace:** PatternCipher.Client.Scenes  
**Metadata:**
    
    - **Category:** Configuration
    
- **Path:** Assets/PatternCipher/Client/Scripts/Events/GameEventSystem.cs  
**Description:** A centralized, static event bus for broadcasting and subscribing to game-wide events. This enables a decoupled, event-driven architecture where different systems can communicate without holding direct references to each other.  
**Template:** C# Static Class  
**Dependency Level:** 1  
**Name:** GameEventSystem  
**Type:** EventBus  
**Relative Path:** Scripts/Events  
**Repository Id:** REPO-PATT-001  
**Pattern Ids:**
    
    - ObserverPattern
    - EventBus
    
**Members:**
    
    
**Methods:**
    
    - **Name:** Subscribe<T>  
**Parameters:**
    
    - Action<T> listener
    
**Return Type:** void  
**Attributes:** public|static  
    - **Name:** Unsubscribe<T>  
**Parameters:**
    
    - Action<T> listener
    
**Return Type:** void  
**Attributes:** public|static  
    - **Name:** Publish<T>  
**Parameters:**
    
    - T event
    
**Return Type:** void  
**Attributes:** public|static  
    
**Implemented Features:**
    
    - Global Event Communication
    
**Requirement Ids:**
    
    - 2.6.1
    
**Purpose:** To provide a decoupled communication mechanism between various game managers and services, supporting an event-driven architecture.  
**Logic Description:** This class will use a dictionary to map event types (System.Type) to a list of delegates (Action<T>). The Subscribe method adds a listener to the dictionary, Unsubscribe removes it, and Publish iterates through all registered listeners for a given event type and invokes them. The implementation must be thread-safe if any off-main-thread operations are planned.  
**Documentation:**
    
    - **Summary:** A static event bus that allows different parts of the application to communicate by publishing and subscribing to events. This decouples components and improves maintainability.
    
**Namespace:** PatternCipher.Client.Events  
**Metadata:**
    
    - **Category:** ApplicationLogic
    
- **Path:** Assets/PatternCipher/Client/Scripts/Events/CoreGameEvents.cs  
**Description:** Defines the data structures for core game events that are broadcast through the GameEventSystem. Each event is a simple struct or class that carries relevant data.  
**Template:** C# Struct/Class  
**Dependency Level:** 0  
**Name:** CoreGameEvents  
**Type:** DataStructure  
**Relative Path:** Scripts/Events  
**Repository Id:** REPO-PATT-001  
**Pattern Ids:**
    
    - EventDriven
    
**Members:**
    
    
**Methods:**
    
    
**Implemented Features:**
    
    - Event Data Models
    
**Requirement Ids:**
    
    - 2.6.1
    
**Purpose:** To define the contracts for events used in the game's event-driven architecture, ensuring type safety and clear communication payloads.  
**Logic Description:** This file will contain multiple public struct or class definitions. For example: `public struct GameStateChangedEvent { public GameState NewState; }`, `public struct LevelCompletedEvent { public int Score; public int Stars; }`, `public struct PauseToggledEvent { public bool IsPaused; }`. These structures serve only as data containers.  
**Documentation:**
    
    - **Summary:** Contains the definitions for various event types used throughout the application. These are the data payloads that are sent through the GameEventSystem.
    
**Namespace:** PatternCipher.Client.Events  
**Metadata:**
    
    - **Category:** DataModel
    


---

# 2. Configuration

- **Feature Toggles:**
  
  - EnableCloudSave
  - EnableLeaderboards
  - EnableAchievements
  - EnableRewardedAds
  
- **Database Configs:**
  
  


---

