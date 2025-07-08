# Architecture Design Specification

# 1. Components

- **Components:**
  
  ### .1. GameManager
  The central orchestrator of the application. Manages the overall game state (e.g., MainMenu, InGame, Paused), scene transitions, and coordinates between other high-level managers. Acts as a singleton for easy access.

  #### .1.4. Type
  Application

  #### .1.5. Dependencies
  
  - ui-manager-002
  - level-manager-003
  - audio-manager-004
  - persistence-service-009
  
  #### .1.6. Properties
  
  - **Pattern:** Singleton
  
  #### .1.7. Interfaces
  
  - IGameStateProvider
  
  #### .1.8. Technology
  Unity Engine (C#)

  #### .1.9. Resources
  
  - **Cpu:** Low
  - **Memory:** Low
  - **Storage:** N/A
  
  #### .1.10. Configuration
  
  - **Initial Scene:** MainMenu
  
  #### .1.11. Health Check
  None

  #### .1.12. Responsible Features
  
  - Game State Management
  - Application Lifecycle Control
  - Cross-Manager Coordination
  
  #### .1.13. Security
  
  - **Requires Authentication:** False
  - **Requires Authorization:** False
  
  ### .2. UIManager
  Manages all UI screens and elements. Handles screen transitions, pop-up dialogs, and acts as a mediator between the user-facing View components (Presentation) and the application logic.

  #### .2.4. Type
  Application

  #### .2.5. Dependencies
  
  - game-manager-001
  - grid-view-008
  - audio-manager-004
  
  #### .2.6. Properties
  
  
  #### .2.7. Interfaces
  
  - IUIManager
  
  #### .2.8. Technology
  Unity UI (UGUI), TextMeshPro, DOTween

  #### .2.9. Resources
  
  - **Cpu:** Low-Medium (during transitions)
  - **Memory:** Medium (holds references to UI prefabs)
  - **Storage:** N/A
  
  #### .2.10. Configuration
  
  - **Screen Prefabs:** List of all screen view prefabs
  - **Transition Animation Duration:** 0.3s
  
  #### .2.11. Health Check
  None

  #### .2.12. Responsible Features
  
  - REQ-UIX-001 (Main Menu)
  - REQ-UIX-002 (Level Selection)
  - REQ-UIX-003 (Game Screen)
  - REQ-UIX-004 (Pause Menu)
  - REQ-UIX-005 (Level Complete)
  - REQ-UIX-006 (Settings)
  - REQ-UIX-011 (UI/UX Learnability)
  - REQ-UIX-014 (Responsive UI)
  
  #### .2.13. Security
  
  - **Requires Authentication:** False
  - **Requires Authorization:** False
  
  ### .3. LevelManager
  Manages the lifecycle of a single puzzle level. Responsible for loading level data, initializing the domain models (GridModel), coordinating with the PuzzleLogicEngine for state evaluation, and tracking level-specific metrics like moves and time.

  #### .3.4. Type
  Application

  #### .3.5. Dependencies
  
  - procedural-content-generator-007
  - grid-model-005
  - puzzle-logic-engine-006
  - persistence-service-009
  - grid-view-008
  
  #### .3.6. Properties
  
  
  #### .3.7. Interfaces
  
  - ILevelManager
  
  #### .3.8. Technology
  Unity Engine (C#)

  #### .3.9. Resources
  
  - **Cpu:** Medium (during level generation/setup)
  - **Memory:** Low
  
  #### .3.10. Configuration
  
  
  #### .3.11. Health Check
  None

  #### .3.12. Responsible Features
  
  - REQ-CGMI-005 (Level Completion Evaluation)
  - REQ-CGMI-006 (Move Tracking)
  - REQ-CGMI-007 (Timer Mechanism)
  - REQ-PDP-004 (In-Progress State Recovery)
  
  #### .3.13. Security
  
  - **Requires Authentication:** False
  - **Requires Authorization:** False
  
  ### .4. AudioManager
  Manages the playback of all background music (BGM) and sound effects (SFX). Provides centralized control over volume, muting, and audio source pooling for performance.

  #### .4.4. Type
  Application

  #### .4.5. Dependencies
  
  - persistence-service-009
  
  #### .4.6. Properties
  
  - **Pattern:** Singleton
  
  #### .4.7. Interfaces
  
  - IAudioManager
  
  #### .4.8. Technology
  Unity Audio Mixer, C#

  #### .4.9. Resources
  
  - **Cpu:** Low
  - **Memory:** Low (excluding audio clip assets)
  
  #### .4.10. Configuration
  
  - **Bgm Volume Mixer Key:** BGMVolume
  - **Sfx Volume Mixer Key:** SFXVolume
  - **Sound Effect Pool Size:** 20
  
  #### .4.11. Health Check
  None

  #### .4.12. Responsible Features
  
  - REQ-GS-002 (Audio Control)
  - REQ-6-003 (SFX Feedback)
  - REQ-6-008 (BGM Playback)
  - REQ-6-009 (Audio Pooling)
  
  #### .4.13. Security
  
  - **Requires Authentication:** False
  - **Requires Authorization:** False
  
  ### .5. GridModel
  The core, non-visual data representation of the game grid. A pure C# class that contains the collection of Tile data models and methods to manipulate the grid state. Has no dependencies on Unity or other external frameworks.

  #### .5.4. Type
  Domain

  #### .5.5. Dependencies
  
  
  #### .5.6. Properties
  
  - **Purity:** Platform Agnostic (Pure C#)
  
  #### .5.7. Interfaces
  
  - IGrid
  
  #### .5.8. Technology
  .NET Standard 2.1 (C#)

  #### .5.9. Resources
  
  - **Cpu:** Low
  - **Memory:** Low-Medium (scales with grid size)
  
  #### .5.10. Configuration
  
  
  #### .5.11. Health Check
  None

  #### .5.12. Responsible Features
  
  - REQ-CGMI-001 (Game Grid Data Structure)
  - REQ-CGMI-010 to REQ-CGMI-014 (Special Tile State)
  
  #### .5.13. Security
  
  - **Requires Authentication:** False
  - **Requires Authorization:** False
  
  ### .6. PuzzleLogicEngine
  A stateless service within the Domain layer that encapsulates all core gameplay rules. It validates player moves, evaluates the current grid state against level objectives, and calculates scores. Uses strategy patterns for different puzzle types.

  #### .6.4. Type
  Domain

  #### .6.5. Dependencies
  
  - grid-model-005
  
  #### .6.6. Properties
  
  - **Purity:** Platform Agnostic (Pure C#)
  - **Pattern:** Strategy Pattern
  
  #### .6.7. Interfaces
  
  - IMoveValidator
  - IGoalEvaluator
  - IScoringCalculator
  
  #### .6.8. Technology
  .NET Standard 2.1 (C#)

  #### .6.9. Resources
  
  - **Cpu:** Low
  - **Memory:** Low
  
  #### .6.10. Configuration
  
  
  #### .6.11. Health Check
  None

  #### .6.12. Responsible Features
  
  - REQ-CGMI-002 (Swap Logic Validation)
  - REQ-CGMI-003 (Tap Logic Validation)
  - REQ-CGMI-005 (Level Completion Logic)
  - REQ-APD-001 to REQ-APD-004 (Puzzle Type Rules)
  - REQ-SRP-001 to REQ-SRP-004 (Scoring Logic)
  
  #### .6.13. Security
  
  - **Requires Authentication:** False
  - **Requires Authorization:** False
  
  ### .7. ProceduralContentGenerator
  Contains the algorithms for procedurally generating puzzle levels. This includes constructing an initial grid state, defining a goal, and using an integrated solver to guarantee solvability and determine the 'par' move count.

  #### .7.4. Type
  Domain

  #### .7.5. Dependencies
  
  
  #### .7.6. Properties
  
  - **Purity:** Platform Agnostic (Pure C#)
  - **Pattern:** Builder Pattern
  
  #### .7.7. Interfaces
  
  - ILevelGenerator
  
  #### .7.8. Technology
  .NET Standard 2.1 (C#)

  #### .7.9. Resources
  
  - **Cpu:** High (during generation)
  - **Memory:** Medium (during generation)
  
  #### .7.10. Configuration
  
  - **Difficulty Parameters:** JSON/ScriptableObject
  - **Solver Max Depth:** 30
  - **Solver Algorithm:** A*
  
  #### .7.11. Health Check
  None

  #### .7.12. Responsible Features
  
  - REQ-PCGDS-001 (Level Generation)
  - REQ-PCGDS-002 (Guaranteed Solvability)
  - REQ-PCGDS-003 (Difficulty Scaling)
  - REQ-PCGDS-004 (Puzzle Type Combination)
  - REQ-PCGDS-009 (Solver Integration)
  
  #### .7.13. Security
  
  - **Requires Authentication:** False
  - **Requires Authorization:** False
  
  ### .8. GridView
  The primary Presentation component for gameplay. Renders the visual representation of the GridModel, including all TileViews. Captures raw input, translates it into game actions for the LevelManager, and plays all tile-related animations and effects.

  #### .8.4. Type
  Presentation

  #### .8.5. Dependencies
  
  - ui-manager-002
  - level-manager-003
  
  #### .8.6. Properties
  
  
  #### .8.7. Interfaces
  
  - IGridView
  
  #### .8.8. Technology
  Unity Engine (C#), Unity Particle System

  #### .8.9. Resources
  
  - **Cpu:** Medium (many animating objects)
  - **Memory:** Medium (holds tile/effect assets)
  
  #### .8.10. Configuration
  
  - **Tile Prefab:** Reference to TileView prefab
  - **Swap Animation Speed:** 0.2s
  
  #### .8.11. Health Check
  None

  #### .8.12. Responsible Features
  
  - REQ-CGMI-001 (Grid Rendering)
  - REQ-CGMI-002 (Visual Swap Feedback)
  - REQ-CGMI-009 (Visual/Auditory Feedback)
  - REQ-UIX-008 ('Juicy' Feedback)
  - REQ-UIX-018 (Gesture Handling)
  
  #### .8.13. Security
  
  - **Requires Authentication:** False
  - **Requires Authorization:** False
  
  ### .9. PersistenceService
  Handles all local data persistence. Saves and loads the player's profile (progress, settings) to and from the device's local storage. Implements data integrity checks (checksums) and manages schema migration between versions.

  #### .9.4. Type
  Infrastructure

  #### .9.5. Dependencies
  
  
  #### .9.6. Properties
  
  
  #### .9.7. Interfaces
  
  - IPersistenceService
  
  #### .9.8. Technology
  C#, Newtonsoft.Json

  #### .9.9. Resources
  
  - **Cpu:** Low
  - **Memory:** Low
  - **Storage:** Low (writes small files to disk)
  
  #### .9.10. Configuration
  
  - **Save File Name:** playerProfile.dat
  - **Backup File Name:** playerProfile.bak
  
  #### .9.11. Health Check
  None

  #### .9.12. Responsible Features
  
  - REQ-PDP-001 (Local Persistence)
  - REQ-PDP-002 (Data Integrity)
  - REQ-PDP-003 (Data Migration)
  - REQ-GS-010 (Settings Persistence)
  
  #### .9.13. Security
  
  - **Requires Authentication:** False
  - **Requires Authorization:** False
  
  ### .10. FirebaseServiceFacade
  Acts as a unified interface (Facade) to all Firebase SDKs. Provides a simplified API for the Application layer to perform authentication, cloud save, remote config fetching, and analytics tracking without being directly coupled to the specifics of each Firebase SDK.

  #### .10.4. Type
  Infrastructure

  #### .10.5. Dependencies
  
  
  #### .10.6. Properties
  
  - **Pattern:** Facade
  
  #### .10.7. Interfaces
  
  - IAuthenticationService
  - ICloudSaveService
  - IRemoteConfigService
  - IAnalyticsService
  
  #### .10.8. Technology
  Firebase SDKs (Auth, Firestore, Remote Config, Analytics)

  #### .10.9. Resources
  
  - **Cpu:** Low
  - **Memory:** Low
  - **Network:** Variable
  
  #### .10.10. Configuration
  
  - **Remote Config Cache Expiration:** 12 hours
  
  #### .10.11. Health Check
  None

  #### .10.12. Responsible Features
  
  - REQ-9-001 (Authentication)
  - REQ-10-007 (Cloud Save)
  - REQ-8-006 (Remote Config)
  - REQ-8-001 (Analytics Collection)
  - REQ-9-002 (Leaderboard Client)
  - REQ-9-004 (Achievement Client)
  
  #### .10.13. Security
  
  - **Requires Authentication:** False
  - **Requires Authorization:** False
  
  ### .11. LeaderboardFunction
  A serverless cloud function responsible for handling leaderboard score submissions. It validates the submitted score for plausibility against game rules and level parameters to prevent cheating before writing the entry to the Firestore database.

  #### .11.4. Type
  Backend

  #### .11.5. Dependencies
  
  
  #### .11.6. Properties
  
  
  #### .11.7. Interfaces
  
  - HTTPS Callable Function
  
  #### .11.8. Technology
  Firebase Cloud Functions (TypeScript/Node.js), Cloud Firestore

  #### .11.9. Resources
  
  - **Cpu:** N/A (Managed)
  - **Memory:** N/A (Managed)
  
  #### .11.10. Configuration
  
  - **Max Score Delta:** 1.5x par score
  - **Min Time Seconds:** 1
  
  #### .11.11. Health Check
  
  - **Path:** N/A
  - **Interval:** 0
  - **Timeout:** 0
  
  #### .11.12. Responsible Features
  
  - REQ-SRP-009 (Server-Side Validation)
  - REQ-CPS-012 (Rate Limiting and Plausibility Checks)
  
  #### .11.13. Security
  
  - **Requires Authentication:** True
  - **Requires Authorization:** True
  - **Allowed Roles:**
    
    - AUTHENTICATED_PLAYER
    
  
  ### .12. UserAccountFunction
  A set of serverless cloud functions triggered by user account events. For example, an `onUserCreate` trigger initializes default data (player progress, settings) in Firestore for a new player account.

  #### .12.4. Type
  Backend

  #### .12.5. Dependencies
  
  
  #### .12.6. Properties
  
  
  #### .12.7. Interfaces
  
  - Authentication Trigger (onCreate)
  - Firestore Trigger (onDelete)
  
  #### .12.8. Technology
  Firebase Cloud Functions (TypeScript/Node.js), Cloud Firestore

  #### .12.9. Resources
  
  - **Cpu:** N/A (Managed)
  - **Memory:** N/A (Managed)
  
  #### .12.10. Configuration
  
  
  #### .12.11. Health Check
  
  - **Path:** N/A
  - **Interval:** 0
  - **Timeout:** 0
  
  #### .12.12. Responsible Features
  
  - User Data Initialization
  - REQ-CPS-006 (User Data Deletion Request Handling)
  
  #### .12.13. Security
  
  - **Requires Authentication:** True
  - **Requires Authorization:** True
  - **Allowed Roles:**
    
    - SERVICE_ACCOUNT
    
  
  
- **Configuration:**
  
  - **Environment:** Development/Production
  - **Logging Level:** INFO
  - **Client Version:** 1.0.0
  - **Backend Api Endpoint:** Firebase Project URL
  


---

# 2. Component_Relations

- **Architecture:**
  
  - **Components:**
    
    - **Id:** game-manager-001  
**Name:** GameManager  
**Description:** The central singleton component that orchestrates the overall game state, lifecycle, and flow. Manages transitions between main menu, level selection, and active gameplay.  
**Type:** ApplicationService  
**Dependencies:**
    
    - scene-loader-002
    - level-manager-003
    - ui-manager-004
    - audio-manager-005
    - persistence-service-015
    - progression-manager-007
    
**Properties:**
    
    - **Is Singleton:** true
    - **Is Persistent Across Scenes:** true
    
**Interfaces:**
    
    - IGameStateProvider
    
**Technology:** Unity Engine (C#)  
**Resources:**
    
    - **Cpu:** Minimal, orchestration logic
    - **Memory:** Minimal, holds state references
    
**Configuration:**
    
    - **Initial Scene:** MainMenu
    
**Health Check:**
    
    - **Path:** Logs critical state transition errors
    - **Interval:** 0
    - **Timeout:** 0
    
**Responsible Features:**
    
    - Game State Management
    - Lifecycle Control
    
**Security:**
    
    - **Requires Authentication:** False
    - **Requires Authorization:** False
    
    - **Id:** level-manager-003  
**Name:** LevelManager  
**Description:** Manages the lifecycle of a single puzzle level, including loading level data, initializing the grid model and view, and communicating with the GoalEvaluator to check for completion.  
**Type:** ApplicationService  
**Dependencies:**
    
    - level-generator-012
    - puzzle-solver-013
    - grid-model-008
    - grid-view-021
    - goal-evaluator-010
    - scoring-service-011
    - persistence-service-015
    
**Interfaces:**
    
    - ILevelLoader
    - ILevelState
    
**Technology:** Unity Engine (C#)  
**Resources:**
    
    - **Cpu:** Moderate during level generation
    - **Memory:** High, holds current level data
    
**Configuration:**
    
    - **Procedural Generation Parameters:** Loaded from RemoteConfig/ScriptableObjects
    
**Health Check:**
    
    - **Path:** Logs level generation and solvability check failures
    - **Interval:** 0
    - **Timeout:** 0
    
**Responsible Features:**
    
    - REQ-PCGDS-001
    - REQ-PCGDS-002
    - REQ-CGMI-005
    - REQ-PDP-004
    
**Security:**
    
    - **Requires Authentication:** False
    - **Requires Authorization:** False
    
    - **Id:** ui-manager-004  
**Name:** UIManager  
**Description:** Manages the state and transitions of all UI screens and elements in the Presentation Layer. Acts as a controller for showing/hiding menus and updating the HUD.  
**Type:** Controller  
**Dependencies:**
    
    - game-screen-controller-023
    - main-menu-controller-024
    - level-select-controller-025
    - settings-controller-026
    - pause-menu-controller-027
    - level-complete-controller-028
    
**Interfaces:**
    
    - IUIManager
    
**Technology:** Unity Engine (C#), Unity UI (UGUI)  
**Resources:**
    
    - **Cpu:** Minimal
    - **Memory:** Manages references to UI prefabs
    
**Configuration:**
    
    - **Ui Prefab References:** Assigned in Unity Editor
    
**Health Check:**
    
    - **Path:** Logs errors on missing UI references
    - **Interval:** 0
    - **Timeout:** 0
    
**Responsible Features:**
    
    - REQ-UIX-001
    - REQ-UIX-002
    - REQ-UIX-003
    - REQ-UIX-004
    - REQ-UIX-005
    - REQ-UIX-006
    
**Security:**
    
    - **Requires Authentication:** False
    - **Requires Authorization:** False
    
    - **Id:** audio-manager-005  
**Name:** AudioManager  
**Description:** A dedicated singleton service for controlling all audio playback, including background music (BGM) and sound effects (SFX). Manages volume levels and audio source pooling.  
**Type:** ApplicationService  
**Dependencies:**
    
    - asset-management-service-018
    
**Properties:**
    
    - **Is Singleton:** true
    
**Interfaces:**
    
    - IAudioManager
    
**Technology:** Unity Engine (C#), Unity AudioMixer  
**Resources:**
    
    - **Cpu:** Minimal
    - **Memory:** Holds references to loaded audio clips
    
**Configuration:**
    
    - **Sfx Volume:** Loaded from PlayerProfile
    - **Bgm Volume:** Loaded from PlayerProfile
    - **Audio Mixer Groups:** Configured in Unity Editor
    
**Health Check:**
    
    - **Path:** Logs missing audio clip errors
    - **Interval:** 0
    - **Timeout:** 0
    
**Responsible Features:**
    
    - REQ-6-003
    - REQ-6-008
    - REQ-GS-002
    
**Security:**
    
    - **Requires Authentication:** False
    - **Requires Authorization:** False
    
    - **Id:** input-controller-020  
**Name:** InputController  
**Description:** Captures and processes raw player input (taps, drags, swipes) from the Unity Input System and translates them into high-level game commands (e.g., SelectTile, AttemptSwap).  
**Type:** Controller  
**Dependencies:**
    
    - level-manager-003
    - move-validator-009
    
**Interfaces:**
    
    - IInputHandler
    
**Technology:** Unity Input System (C#)  
**Resources:**
    
    - **Cpu:** Low, event-driven
    - **Memory:** Minimal
    
**Configuration:**
    
    - **Input Action Asset:** Configured in Unity Editor
    
**Health Check:**
    
    - **Path:** N/A
    - **Interval:** 0
    - **Timeout:** 0
    
**Responsible Features:**
    
    - REQ-CGMI-002
    - REQ-CGMI-003
    - REQ-UIX-018
    
**Security:**
    
    - **Requires Authentication:** False
    - **Requires Authorization:** False
    
    - **Id:** grid-model-008  
**Name:** GridModel  
**Description:** A pure C# class representing the state of the game board. Contains a 2D array of TileModels and the logic for accessing and modifying them. Has no dependency on Unity.  
**Type:** DomainModel  
**Dependencies:**
    
    - tile-model-008b
    
**Interfaces:**
    
    
**Technology:** .NET Standard 2.1 (C#)  
**Resources:**
    
    - **Cpu:** N/A
    - **Memory:** Dependent on grid size (M x N)
    
**Configuration:**
    
    - **Dimensions:** Set per level
    
**Health Check:**
    
    - **Path:** N/A
    - **Interval:** 0
    - **Timeout:** 0
    
**Responsible Features:**
    
    - REQ-CGMI-001
    
**Security:**
    
    - **Requires Authentication:** False
    - **Requires Authorization:** False
    
    - **Id:** move-validator-009  
**Name:** MoveValidator  
**Description:** A pure C# domain service that validates player actions against game rules, such as checking if a tile swap is between adjacent tiles or if a tapped tile is interactive.  
**Type:** DomainService  
**Dependencies:**
    
    - grid-model-008
    - special-tile-logic-014
    
**Interfaces:**
    
    - IMoveValidator
    
**Technology:** .NET Standard 2.1 (C#)  
**Resources:**
    
    - **Cpu:** N/A
    - **Memory:** N/A
    
**Configuration:**
    
    
**Health Check:**
    
    - **Path:** N/A
    - **Interval:** 0
    - **Timeout:** 0
    
**Responsible Features:**
    
    - REQ-CGMI-002
    - REQ-CGMI-003
    - REQ-CGMI-010
    
**Security:**
    
    - **Requires Authentication:** False
    - **Requires Authorization:** False
    
    - **Id:** goal-evaluator-010  
**Name:** GoalEvaluator  
**Description:** A pure C# domain service that evaluates the current GridModel state against the level's goal configuration to determine if the level is complete. Uses a strategy pattern for different puzzle types.  
**Type:** DomainService  
**Dependencies:**
    
    - grid-model-008
    
**Properties:**
    
    - **Pattern:** Strategy
    
**Interfaces:**
    
    - IGoalEvaluator
    
**Technology:** .NET Standard 2.1 (C#)  
**Resources:**
    
    - **Cpu:** N/A
    - **Memory:** N/A
    
**Configuration:**
    
    - **Evaluation Strategy:** Set based on puzzle type
    
**Health Check:**
    
    - **Path:** N/A
    - **Interval:** 0
    - **Timeout:** 0
    
**Responsible Features:**
    
    - REQ-CGMI-005
    - REQ-APD-001
    - REQ-APD-002
    - REQ-APD-003
    - REQ-APD-004
    
**Security:**
    
    - **Requires Authentication:** False
    - **Requires Authorization:** False
    
    - **Id:** scoring-service-011  
**Name:** ScoringService  
**Description:** A pure C# domain service responsible for calculating the player's score for a completed level, including base score, efficiency bonus, speed bonus, and combo bonuses.  
**Type:** DomainService  
**Dependencies:**
    
    
**Interfaces:**
    
    - IScoringService
    
**Technology:** .NET Standard 2.1 (C#)  
**Resources:**
    
    - **Cpu:** N/A
    - **Memory:** N/A
    
**Configuration:**
    
    - **Scoring Formulas:** Loaded from RemoteConfig
    
**Health Check:**
    
    - **Path:** N/A
    - **Interval:** 0
    - **Timeout:** 0
    
**Responsible Features:**
    
    - REQ-SRP-001
    - REQ-SRP-002
    - REQ-SRP-003
    - REQ-SRP-004
    - REQ-SRP-005
    
**Security:**
    
    - **Requires Authentication:** False
    - **Requires Authorization:** False
    
    - **Id:** level-generator-012  
**Name:** LevelGenerator  
**Description:** A pure C# domain service that implements the procedural content generation (PCG) algorithms to create unique and solvable puzzle levels based on difficulty parameters.  
**Type:** DomainService  
**Dependencies:**
    
    - puzzle-solver-013
    
**Interfaces:**
    
    - ILevelGenerator
    
**Technology:** .NET Standard 2.1 (C#)  
**Resources:**
    
    - **Cpu:** High during generation
    - **Memory:** Moderate during generation
    
**Configuration:**
    
    - **Difficulty Parameters:** Loaded from RemoteConfig
    
**Health Check:**
    
    - **Path:** Logs generation failures or unsolvable states
    - **Interval:** 0
    - **Timeout:** 0
    
**Responsible Features:**
    
    - REQ-PCGDS-001
    - REQ-PCGDS-003
    - REQ-PCGDS-004
    - REQ-PCGDS-006
    
**Security:**
    
    - **Requires Authentication:** False
    - **Requires Authorization:** False
    
    - **Id:** puzzle-solver-013  
**Name:** PuzzleSolver  
**Description:** A pure C# domain service implementing a search algorithm (e.g., A* search) to verify the solvability of generated puzzles, determine the optimal solution path ('par' moves), and provide hints.  
**Type:** DomainService  
**Dependencies:**
    
    - grid-model-008
    
**Interfaces:**
    
    - IPuzzleSolver
    
**Technology:** .NET Standard 2.1 (C#)  
**Resources:**
    
    - **Cpu:** High during solving
    - **Memory:** High, can store many search nodes
    
**Configuration:**
    
    - **Search Heuristics:** Defined per puzzle type
    
**Health Check:**
    
    - **Path:** Logs solver timeouts or failures
    - **Interval:** 0
    - **Timeout:** 0
    
**Responsible Features:**
    
    - REQ-PCGDS-002
    - REQ-PCGDS-007
    - REQ-SRP-002
    
**Security:**
    
    - **Requires Authentication:** False
    - **Requires Authorization:** False
    
    - **Id:** persistence-service-015  
**Name:** PersistenceService  
**Description:** An infrastructure service that handles saving and loading of player data to/from the local device. Implements serialization, obfuscation, checksum validation, and data migration logic.  
**Type:** InfrastructureService  
**Dependencies:**
    
    - player-profile-model-008c
    
**Interfaces:**
    
    - IPersistenceService
    
**Technology:** C#, Newtonsoft.Json  
**Resources:**
    
    - **Cpu:** Low
    - **Memory:** Minimal
    
**Configuration:**
    
    - **Save File Path:** Application.persistentDataPath
    - **Encryption Key:** Stored securely, not in source code
    
**Health Check:**
    
    - **Path:** Logs data corruption or migration failures
    - **Interval:** 0
    - **Timeout:** 0
    
**Responsible Features:**
    
    - REQ-PDP-001
    - REQ-PDP-002
    - REQ-PDP-003
    - REQ-PDP-004
    - REQ-10-001
    
**Security:**
    
    - **Requires Authentication:** False
    - **Requires Authorization:** False
    - **Allowed Roles:**
      
      
    - **Notes:** Implements local data obfuscation and checksums to deter tampering.
    
    - **Id:** firebase-service-016  
**Name:** FirebaseService  
**Description:** An infrastructure facade that encapsulates all interactions with the Firebase backend, including Authentication, Firestore (for Cloud Save, Leaderboards, Achievements), and Analytics.  
**Type:** InfrastructureService  
**Dependencies:**
    
    - player-profile-model-008c
    
**Interfaces:**
    
    - IAuthService
    - ICloudSaveService
    - ILeaderboardService
    - IAchievementService
    - IAnalyticsService
    
**Technology:** Firebase SDK for Unity (Auth, Firestore, Analytics)  
**Resources:**
    
    - **Network:** Required for all operations
    
**Configuration:**
    
    - **Firebase Config:** google-services.json / GoogleService-Info.plist
    
**Health Check:**
    
    - **Path:** Logs Firebase API errors and authentication failures
    - **Interval:** 0
    - **Timeout:** 0
    
**Responsible Features:**
    
    - REQ-9-001
    - REQ-9-002
    - REQ-9-004
    - REQ-10-007
    - REQ-8-001
    
**Security:**
    
    - **Requires Authentication:** True
    - **Requires Authorization:** False
    - **Notes:** Handles all user authentication and secure communication with the backend.
    
    - **Id:** remote-config-service-017  
**Name:** RemoteConfigService  
**Description:** An infrastructure service responsible for fetching and caching dynamic game parameters from Firebase Remote Config. Provides these values to other services like LevelGenerator and ScoringService.  
**Type:** InfrastructureService  
**Dependencies:**
    
    
**Interfaces:**
    
    - IRemoteConfigProvider
    
**Technology:** Firebase SDK for Unity (Remote Config)  
**Resources:**
    
    - **Network:** Required on startup/refresh
    
**Configuration:**
    
    - **Default Config Values:** Stored locally for offline-first start
    - **Fetch Interval Seconds:** 3600
    
**Health Check:**
    
    - **Path:** Logs errors on fetching remote config
    - **Interval:** 0
    - **Timeout:** 0
    
**Responsible Features:**
    
    - REQ-8-006
    - REQ-PCGDS-007
    - REQ-8-008
    
**Security:**
    
    - **Requires Authentication:** False
    - **Requires Authorization:** False
    
    - **Id:** platform-integration-service-019  
**Name:** PlatformIntegrationService  
**Description:** An infrastructure service that provides a common interface for interacting with native platform features like Game Center (iOS) and Google Play Games Services (Android) for leaderboards and achievements.  
**Type:** InfrastructureService  
**Dependencies:**
    
    
**Interfaces:**
    
    - IPlatformLeaderboardService
    - IPlatformAchievementService
    
**Technology:** C#, Apple GameKit (via bridge), Google Play Games SDK  
**Resources:**
    
    - **Network:** Required for platform services
    
**Configuration:**
    
    - **Leaderboard Ids:** Platform-specific IDs
    - **Achievement Ids:** Platform-specific IDs
    
**Health Check:**
    
    - **Path:** Logs platform-specific API errors
    - **Interval:** 0
    - **Timeout:** 0
    
**Responsible Features:**
    
    - REQ-EPS-001
    - REQ-EPS-002
    - REQ-EPS-003
    
**Security:**
    
    - **Requires Authentication:** True
    - **Requires Authorization:** False
    - **Notes:** Relies on platform-native authentication (Game Center/Google Play login).
    
    - **Id:** grid-view-021  
**Name:** GridView  
**Description:** A presentation component that renders the game grid. It is responsible for instantiating and positioning TileView prefabs based on the GridModel.  
**Type:** View  
**Dependencies:**
    
    - tile-view-022
    - grid-model-008
    
**Interfaces:**
    
    
**Technology:** Unity Engine (C#), UGUI  
**Resources:**
    
    - **Cpu:** Minimal
    - **Memory:** Holds references to TileView instances
    
**Configuration:**
    
    
**Health Check:**
    
    - **Path:** N/A
    - **Interval:** 0
    - **Timeout:** 0
    
**Responsible Features:**
    
    - REQ-CGMI-001
    
**Security:**
    
    - **Requires Authentication:** False
    - **Requires Authorization:** False
    
    - **Id:** tile-view-022  
**Name:** TileView  
**Description:** A presentation component (prefab) representing a single tile on the grid. It displays the tile's symbol and visual state (selected, locked, etc.) and plays animations for swaps and state changes.  
**Type:** View  
**Dependencies:**
    
    
**Properties:**
    
    - **Is Prefab:** true
    
**Interfaces:**
    
    
**Technology:** Unity Engine (C#), DOTween, Unity Particle System  
**Resources:**
    
    - **Cpu:** Minimal
    - **Memory:** Minimal per instance
    
**Configuration:**
    
    - **Symbol Sprites:** Referenced in prefab
    - **State Visuals:** Referenced in prefab
    
**Health Check:**
    
    - **Path:** N/A
    - **Interval:** 0
    - **Timeout:** 0
    
**Responsible Features:**
    
    - REQ-CGMI-001
    - REQ-CGMI-009
    - REQ-UIX-008
    - REQ-UIX-015
    
**Security:**
    
    - **Requires Authentication:** False
    - **Requires Authorization:** False
    
    - **Id:** firebase-functions-030  
**Name:** Firebase Cloud Functions  
**Description:** Serverless backend logic executed in a managed Node.js environment. Used for operations that require server-side authority and security, like validating leaderboard scores.  
**Type:** BackendService  
**Dependencies:**
    
    - firebase-firestore-031
    
**Interfaces:**
    
    - HTTPS Callable Functions
    
**Technology:** Node.js/TypeScript, Firebase Admin SDK  
**Resources:**
    
    - **Cpu:** Serverless/Managed
    - **Memory:** Serverless/Managed
    - **Network:** Serverless/Managed
    
**Configuration:**
    
    - **Environment Variables:** For secrets and config
    - **Function Triggers:** HTTPS, Firestore
    
**Health Check:**
    
    - **Path:** Monitored via Google Cloud Logging & Monitoring
    - **Interval:** 0
    - **Timeout:** 0
    
**Responsible Features:**
    
    - REQ-9-009
    - REQ-SRP-009
    - REQ-CPS-012
    
**Security:**
    
    - **Requires Authentication:** True
    - **Requires Authorization:** True
    - **Notes:** Core of the server-side security model for validating client requests.
    
    - **Id:** firebase-firestore-031  
**Name:** Cloud Firestore  
**Description:** The NoSQL cloud database used to store all persistent online data, including player profiles for cloud save, leaderboard entries, and achievement status. Access is governed by Firestore Security Rules.  
**Type:** Database  
**Dependencies:**
    
    
**Interfaces:**
    
    - Firebase SDK API
    
**Technology:** Firebase Firestore  
**Resources:**
    
    - **Storage:** Serverless/Managed
    
**Configuration:**
    
    - **Firestore.Rules:** Defines security and access control
    - **Indexes:** Defined for complex queries
    
**Health Check:**
    
    - **Path:** Monitored via Firebase Console status dashboard
    - **Interval:** 0
    - **Timeout:** 0
    
**Responsible Features:**
    
    - REQ-10-008
    - REQ-9-002
    - REQ-9-004
    - REQ-9-010
    
**Security:**
    
    - **Requires Authentication:** True
    - **Requires Authorization:** True
    - **Notes:** Security is enforced by 'firestore.rules' which dictates read/write access based on user authentication.
    
    
  - **Configuration:**
    
    - **Environment:** Development/Production
    - **Logging Level:** INFO
    - **Client Architecture:** Layered (Presentation, Application, Domain, Infrastructure)
    - **Backend Architecture:** Serverless (Firebase BaaS)
    
  


---

