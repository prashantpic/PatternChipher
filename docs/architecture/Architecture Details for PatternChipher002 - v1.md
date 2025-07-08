# Architecture Design Specification

# 1. Style
Hybrid


---

# 2. Patterns

## 2.1. Client-Server (BaaS)
The overall architecture is a client-server model where the client is a Unity-based mobile application and the server is a Backend as a Service (Firebase). The client handles all rendering, gameplay logic, and user interaction, while the BaaS provides scalable services like authentication, database, serverless functions, and configuration.

### 2.1.3. Benefits

- Rapid development of backend features by leveraging pre-built, managed services.
- High scalability and reliability for online features without managing server infrastructure.
- Separation of concerns between the core offline gameplay client and online services.
- Enables features like cloud save, leaderboards, and remote configuration.

### 2.1.4. Tradeoffs

- Dependency on a third-party provider (Google Firebase) and potential for vendor lock-in.
- Backend logic is constrained to the capabilities of the BaaS platform (e.g., Cloud Functions).
- Usage-based pricing can become expensive at a large scale.

### 2.1.5. Applicability

- **Scenarios:**
  
  - Mobile games requiring online features like leaderboards, achievements, and cloud synchronization.
  - Applications needing to dynamically balance gameplay post-launch via remote configuration.
  - Projects where minimizing backend development and operational overhead is a priority.
  

## 2.2. Layered Architecture
The client-side Unity application is structured into distinct layers: Presentation (UI), Application (Game Management), Domain (Core Rules), and Infrastructure (Services). This promotes a strong separation of concerns, making the application easier to develop, test, and maintain.

### 2.2.3. Benefits

- High maintainability and testability by isolating different aspects of the application.
- Clear separation between Unity-specific code (Presentation, Application) and pure C# game logic (Domain).
- Facilitates parallel development as teams can work on different layers simultaneously.
- Encapsulates external dependencies (like Firebase SDKs) in the Infrastructure layer, reducing their impact on core game logic.

### 2.2.4. Tradeoffs

- Can introduce some overhead and boilerplate code for communication between layers.
- Strict adherence to layer boundaries is required to maintain architectural integrity.

### 2.2.5. Applicability

- **Scenarios:**
  
  - Complex client applications like games with distinct UI, gameplay orchestration, core rules, and external service interactions.
  - Projects where long-term maintainability and the ability to swap out components (e.g., a different backend service) are important.
  

## 2.3. Model-View-Controller (MVC) / Model-View-Presenter (MVP)
Within the Presentation and Application layers, an MVC-like pattern is used. Domain objects act as the Model (data and rules). Unity GameObjects with 'View' scripts (e.g., TileView) represent the View. 'Controller' or 'Presenter' scripts (e.g., GameManager, UIManager) mediate between the Model and View, handling user input and updating the view based on model changes.

### 2.3.3. Benefits

- Decouples the visual representation from the underlying game state and logic.
- Improves the testability of UI logic by separating it into controllers/presenters.
- Allows for different views to be created for the same model data.

### 2.3.4. Applicability

- **Scenarios:**
  
  - Managing the complex UI and game state interactions required by the game's various screens and the interactive game grid.
  



---

# 3. Layers

## 3.1. Client: Presentation Layer
Handles all user-facing aspects of the game, including UI rendering, animations, visual effects, and capturing raw user input. This layer is built entirely with Unity's frameworks.

### 3.1.4. Technologystack
Unity UI (UGUI), TextMeshPro, DOTween, Unity Input System, Unity Particle System

### 3.1.5. Language
C#

### 3.1.6. Type
Presentation

### 3.1.7. Responsibilities

- Render all UI screens: Main Menu, Level Selection, Game Screen HUD, Pause Menu, Settings, etc. (REQ-UIX-001 to REQ-UIX-006).
- Display the game grid and tiles, including their symbols, states (locked, wildcard), and visual feedback (REQ-CGMI-001, REQ-CGMI-009, REQ-UIX-015).
- Handle raw touch input (taps, drags) and translate them into game actions (REQ-CGMI-002, REQ-UIX-018).
- Provide immediate visual feedback for all interactions: tile selection, swaps, taps, errors, and UI button presses (REQ-UIX-008).
- Execute 'juicy' animations and particle effects for a satisfying game feel (NFR-V-003, REQ-6-003).
- Adapt UI layout to various screen sizes and aspect ratios (REQ-UIX-014).

### 3.1.8. Components

- MainMenuScreenView
- LevelSelectScreenView
- GameScreenHUDView
- PauseMenuView
- SettingsScreenView
- LevelCompleteScreenView
- GridView
- TileView (Prefab)
- InputHandler

### 3.1.9. Dependencies

- **Layer Id:** client-application  
**Type:** Required  

## 3.2. Client: Application Layer
Acts as the orchestrator for the client application. It manages the game's lifecycle, state, and flow, connecting the user's actions from the Presentation layer with the core logic in the Domain layer.

### 3.2.4. Technologystack
Unity Engine

### 3.2.5. Language
C#

### 3.2.6. Type
ApplicationServices

### 3.2.7. Responsibilities

- Manage the overall game state (e.g., in menu, in-game, paused).
- Control the flow between different scenes/screens (e.g., from Main Menu to Level Select to Game Screen).
- Orchestrate level loading, starting, and ending sequences.
- Mediate communication between the UI and the Domain layer (e.g., passing a swap request from UI to the domain for validation).
- Manage game-wide systems like audio (AudioManager), tutorials (TutorialManager), and player progression (ProgressionManager).
- Trigger saving and loading of the player's profile via the Infrastructure layer.

### 3.2.8. Components

- GameManager (Singleton)
- SceneLoader
- LevelManager
- UIManager
- AudioManager
- TutorialManager
- ProgressionManager

### 3.2.9. Dependencies

- **Layer Id:** client-presentation  
**Type:** Required  
- **Layer Id:** client-domain  
**Type:** Required  
- **Layer Id:** client-infrastructure  
**Type:** Required  

## 3.3. Client: Domain Layer
The heart of the game. Contains all the core, platform-agnostic puzzle logic, rules, and data structures. This layer is composed of pure C# classes (POCOs) and has no dependency on the Unity Engine API.

### 3.3.4. Technologystack
.NET Standard 2.1

### 3.3.5. Language
C#

### 3.3.6. Type
BusinessLogic

### 3.3.7. Responsibilities

- Define the data structures for the game grid, tiles, and symbols (REQ-CGMI-001).
- Implement the logic for core mechanics: tile swapping and tapping (REQ-CGMI-002, REQ-CGMI-003).
- Define the behavior of all special tiles: Locked, Wildcard, Transformer, Obstacle, Key/Lock (REQ-CGMI-010 to REQ-CGMI-014).
- Implement the evaluation logic to check if the current grid state satisfies the level's objective (REQ-CGMI-005, REQ-APD-001 to REQ-APD-004).
- Calculate player scores, including base scores and bonuses (efficiency, speed, combo) (REQ-SRP-001 to REQ-SRP-004).
- Define the player's profile data model, including progress and settings.
- Encapsulate the algorithms for procedural content generation (REQ-PCGDS-001) and puzzle solving (REQ-PCGDS-002).

### 3.3.8. Components

- Grid (Data Model)
- Tile (Data Model)
- PlayerProfile (Data Model)
- MoveValidator
- GoalEvaluator (with strategies for DirectMatch, RuleBased, etc.)
- ScoringService
- SpecialTileBehavior (Base class and implementations)
- LevelGenerator
- PuzzleSolver (e.g., A* Search)

### 3.3.9. Dependencies


## 3.4. Client: Infrastructure Layer
Provides implementations for external-facing concerns, such as data persistence, communication with backend services, and interaction with native platform features.

### 3.4.4. Technologystack
Newtonsoft.Json, Unity Addressables, Firebase SDKs (Auth, Firestore, Remote Config, Analytics), Google Play Games SDK, Apple GameKit (via bridge)

### 3.4.5. Language
C#

### 3.4.6. Type
Infrastructure

### 3.4.7. Responsibilities

- Persist player data (progress, settings) to local storage (JSON files) and handle data integrity checks and migration (REQ-PDP-001, REQ-PDP-002, REQ-PDP-003).
- Implement crash/interruption state recovery (REQ-PDP-004).
- Communicate with the Firebase backend for all online features: Authentication, Cloud Save, Leaderboards, Achievements, Remote Config, and Analytics (REQ-9-001, REQ-10-007).
- Interact with native platform services (Game Center, Google Play Games) for achievements and leaderboards (REQ-EPS-001).
- Manage loading of game assets (sprites, prefabs, audio clips).
- Provide access to device-specific features like haptic feedback.

### 3.4.8. Components

- PersistenceService
- FirebaseService (Facade for all Firebase SDKs)
- PlatformLeaderboardService
- PlatformAchievementService
- AssetLoaderService
- HapticFeedbackService
- RemoteConfigService

### 3.4.9. Dependencies

- **Layer Id:** client-domain  
**Type:** Required  

## 3.5. Backend: Firebase Services
A collection of managed cloud services from Google's Firebase platform that provide the backend functionality for the game's online features.

### 3.5.4. Technologystack
Firebase Platform

### 3.5.5. Language
N/A (Configuration), TypeScript/Node.js (for Cloud Functions)

### 3.5.6. Type
ApplicationServices

### 3.5.7. Responsibilities

- Authenticate players using Anonymous, Google, or Apple sign-in methods (REQ-9-001).
- Store and retrieve player data (profiles, progress, settings) for the cloud save feature using Firestore (REQ-10-008).
- Store and manage leaderboard entries, enforcing access rules via Firestore Security Rules (REQ-9-002).
- Store and manage achievement definitions and player achievement status (REQ-9-004).
- Execute server-side logic, such as validating leaderboard score submissions, using Cloud Functions (REQ-9-009, REQ-CPS-012).
- Provide dynamic game parameters (difficulty, scoring rules) to the client via Remote Config (REQ-8-006).
- Collect, process, and display anonymized gameplay analytics (REQ-8-001).

### 3.5.8. Components

- Firebase Authentication
- Cloud Firestore (Database)
- Cloud Functions for Firebase
- Firebase Remote Config
- Firebase Analytics
- Firebase App Check



---

# 4. Quality Attributes

## 4.1. Maintainability
The system must be easy to modify, fix, and extend with new features or content without unintended side effects.

### 4.1.2. Priority
High

### 4.1.4. Tactics

- **Separation of Concerns:** The Layered Architecture isolates UI, game orchestration, core rules, and external services.
- **Modularity:** Features like procedural generation and special tile behaviors are encapsulated in dedicated components.
- **Information Hiding:** Interfaces are used to hide implementation details of services in the Infrastructure layer.
- **Externalized Configuration:** Game balancing parameters are managed in Firebase Remote Config and local ScriptableObjects, not hardcoded (REQ-8-008, REQ-PCGDS-007).

## 4.2. Security
Player data must be protected, and online features must be resilient to cheating and unauthorized access.

### 4.2.2. Priority
High

### 4.2.4. Tactics

- **Authentication:** Firebase Authentication provides secure player identification for all online features (REQ-9-001).
- **Authorization:** Firestore Security Rules enforce the principle of least privilege, ensuring users can only modify their own data (REQ-9-010).
- **Server-Side Validation:** Cloud Functions validate all leaderboard submissions to prevent fraudulent scores (REQ-9-009, REQ-CPS-012).
- **Secure Communication:** All client-server communication uses HTTPS, enforced by the Firebase SDKs (REQ-9-008).
- **Data Integrity:** Local save files use checksums and obfuscation to deter casual tampering (REQ-PDP-002, REQ-CPS-010).
- **Secret Management:** API keys and sensitive credentials are not stored in client code (REQ-9-011).

## 4.3. Reliability
The game must be stable, persist player data correctly, and handle interruptions gracefully.

### 4.3.2. Priority
High

### 4.3.4. Tactics

- **Fault Tolerance (Backend):** Leverage the high availability and automated backups of the Firebase platform (REQ-9-012, REQ-9-013).
- **Data Persistence & Migration:** A robust local save system with versioning and migration logic ensures player progress is not lost across updates (REQ-PDP-001, REQ-PDP-003).
- **State Recovery:** The system saves in-progress level state to recover from unexpected app terminations (REQ-PDP-004).
- **Conflict Resolution:** A defined strategy (e.g., last-write-wins via server timestamp) handles cloud save data conflicts (REQ-10-010).
- **Offline Functionality:** The core gameplay is fully functional offline, independent of backend service availability (REQ-9-012).

## 4.4. Performance
The game must be responsive, with smooth animations, and fast level load times, while being mindful of battery usage.

### 4.4.2. Priority
High

### 4.4.4. Tactics

- **Resource Management (Client):** Use of object pooling for frequently instantiated objects like tiles and particle effects. Asynchronous loading for assets.
- **Efficient Rendering (Client):** Optimized shaders and draw call batching.
- **Responsive Interaction:** UI feedback latency target of <100ms (NFR-P-001).
- **Efficient Data Queries (Backend):** Proper indexing on Firestore collections, as defined in the database design.
- **Caching (Backend):** Caching frequently accessed data like leaderboard tops and level definitions where applicable.

## 4.5. Scalability
The backend services must handle a growing number of players without degradation in performance.

### 4.5.2. Priority
High

### 4.5.4. Tactics

- **Managed Services:** The entire backend is built on Firebase, whose services (Firestore, Auth, Functions) are designed to scale automatically with user load.
- **Stateless Functions:** Cloud Functions are stateless, allowing the platform to scale instances horizontally as needed.
- **NoSQL Database:** Firestore is a horizontally scalable NoSQL database capable of handling large datasets and high traffic.



---

