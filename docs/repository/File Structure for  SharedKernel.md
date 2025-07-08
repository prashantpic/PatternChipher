# Specification

# 1. Files

- **Path:** src/PatternCipher.Shared/PatternCipher.Shared.csproj  
**Description:** The C# project file for the SharedKernel library. It defines the project's properties, target framework (.NET Standard 2.1), and any NuGet package dependencies. This file is essential for building the shared library that other client repositories will reference.  
**Template:** C# Project File  
**Dependency Level:** 0  
**Name:** PatternCipher.Shared.csproj  
**Type:** Configuration  
**Relative Path:** PatternCipher.Shared.csproj  
**Repository Id:** REPO-PATT-012  
**Pattern Ids:**
    
    
**Members:**
    
    
**Methods:**
    
    
**Implemented Features:**
    
    - Project Definition
    
**Requirement Ids:**
    
    - NFR-M-001
    
**Purpose:** Defines the .NET Standard library project, its target framework, and dependencies, ensuring it can be consumed by other .NET projects like the Unity client.  
**Logic Description:** This XML file configures the project as a .NET Standard 2.1 class library, making it compatible with the Unity game engine. It will list any third-party dependencies required by the shared kernel, although none are specified for this repository.  
**Documentation:**
    
    - **Summary:** Project configuration file for the PatternCipher.Shared library.
    
**Namespace:**   
**Metadata:**
    
    - **Category:** Build
    
- **Path:** src/PatternCipher.Shared/Common/ValueObject.cs  
**Description:** An abstract base class to facilitate the implementation of the Value Object pattern from Domain-Driven Design. It provides standard implementations for equality comparison based on the object's components, ensuring that two value objects are considered equal if their constituent parts are equal.  
**Template:** C# Abstract Class  
**Dependency Level:** 0  
**Name:** ValueObject  
**Type:** BaseClass  
**Relative Path:** Common/ValueObject.cs  
**Repository Id:** REPO-PATT-012  
**Pattern Ids:**
    
    - ValueObject
    
**Members:**
    
    
**Methods:**
    
    - **Name:** Equals  
**Parameters:**
    
    - object obj
    
**Return Type:** bool  
**Attributes:** public|override  
    - **Name:** GetHashCode  
**Parameters:**
    
    
**Return Type:** int  
**Attributes:** public|override  
    - **Name:** GetEqualityComponents  
**Parameters:**
    
    
**Return Type:** IEnumerable<object>  
**Attributes:** protected|abstract  
    
**Implemented Features:**
    
    - Value-based Equality
    
**Requirement Ids:**
    
    - NFR-M-001
    
**Purpose:** Provides a reusable, abstract base for creating domain objects where equality is based on constituent values rather than identity.  
**Logic Description:** The Equals method will compare the type of the objects and then iterate through the components returned by the abstract GetEqualityComponents method, comparing each one. The GetHashCode method will compute a hash code based on the combination of hash codes from all equality components. Derived classes must implement GetEqualityComponents.  
**Documentation:**
    
    - **Summary:** Abstract base class for creating Value Objects, a core DDD tactical pattern.
    
**Namespace:** PatternCipher.Shared.Common  
**Metadata:**
    
    - **Category:** BusinessLogic
    
- **Path:** src/PatternCipher.Shared/Gameplay/Grid/GridPosition.cs  
**Description:** Represents the X and Y coordinates of a tile on the game grid. Implemented as a Value Object to ensure that two positions are equal if their X and Y coordinates are the same. This provides a type-safe way to handle coordinates instead of using primitive tuples or arrays.  
**Template:** C# Record  
**Dependency Level:** 1  
**Name:** GridPosition  
**Type:** ValueObject  
**Relative Path:** Gameplay/Grid/GridPosition.cs  
**Repository Id:** REPO-PATT-012  
**Pattern Ids:**
    
    - ValueObject
    
**Members:**
    
    - **Name:** X  
**Type:** int  
**Attributes:** public|init  
    - **Name:** Y  
**Type:** int  
**Attributes:** public|init  
    
**Methods:**
    
    
**Implemented Features:**
    
    - Coordinate Representation
    
**Requirement Ids:**
    
    - NFR-M-001
    
**Purpose:** Provides a strongly-typed, immutable representation of a position on the game grid, ensuring correctness and clarity in code that deals with tile locations.  
**Logic Description:** This file will define a C# record struct named GridPosition. Records provide value-based equality semantics out of the box, making them perfect for implementing the Value Object pattern. It will have two integer properties, X and Y, representing the column and row.  
**Documentation:**
    
    - **Summary:** A value object representing a tile's position on the grid.
    
**Namespace:** PatternCipher.Shared.Gameplay.Grid  
**Metadata:**
    
    - **Category:** BusinessLogic
    
- **Path:** src/PatternCipher.Shared/Gameplay/Symbols/Symbol.cs  
**Description:** Represents a distinct symbol that can be displayed on a tile (e.g., color, geometric shape, icon). This class encapsulates the properties of a symbol, such as its unique identifier and potentially its visual asset key. It is a core concept for defining the game's visual vocabulary.  
**Template:** C# Class  
**Dependency Level:** 1  
**Name:** Symbol  
**Type:** ValueObject  
**Relative Path:** Gameplay/Symbols/Symbol.cs  
**Repository Id:** REPO-PATT-012  
**Pattern Ids:**
    
    - ValueObject
    
**Members:**
    
    - **Name:** Id  
**Type:** string  
**Attributes:** public|readonly  
    
**Methods:**
    
    - **Name:** Symbol  
**Parameters:**
    
    - string id
    
**Return Type:** void  
**Attributes:** public  
    
**Implemented Features:**
    
    - Symbol Definition
    
**Requirement Ids:**
    
    - NFR-M-001
    
**Purpose:** Defines the abstract concept of a tile symbol, allowing the game logic to operate on symbols independently of their visual representation.  
**Logic Description:** This will be implemented as a Value Object, likely inheriting from the abstract ValueObject base class. The primary component for equality will be its unique Id string. This ensures that two symbols are considered the same if and only if their IDs match.  
**Documentation:**
    
    - **Summary:** Represents a unique symbol that a tile can display.
    
**Namespace:** PatternCipher.Shared.Gameplay.Symbols  
**Metadata:**
    
    - **Category:** BusinessLogic
    
- **Path:** src/PatternCipher.Shared/Gameplay/Grid/Tile.cs  
**Description:** Represents a single tile within the game grid. This is a core entity in the gameplay domain. It holds its position, the symbol it displays, and its current state (e.g., Default, Locked). It is the primary interactive element from the player's perspective.  
**Template:** C# Class  
**Dependency Level:** 2  
**Name:** Tile  
**Type:** Model  
**Relative Path:** Gameplay/Grid/Tile.cs  
**Repository Id:** REPO-PATT-012  
**Pattern Ids:**
    
    
**Members:**
    
    - **Name:** Position  
**Type:** GridPosition  
**Attributes:** public  
    - **Name:** CurrentSymbol  
**Type:** Symbol  
**Attributes:** public  
    - **Name:** State  
**Type:** TileState  
**Attributes:** public  
    
**Methods:**
    
    - **Name:** Tile  
**Parameters:**
    
    - GridPosition position
    - Symbol initialSymbol
    
**Return Type:** void  
**Attributes:** public  
    - **Name:** ChangeSymbol  
**Parameters:**
    
    - Symbol newSymbol
    
**Return Type:** void  
**Attributes:** public  
    - **Name:** ChangeState  
**Parameters:**
    
    - TileState newState
    
**Return Type:** void  
**Attributes:** public  
    
**Implemented Features:**
    
    - Tile State Management
    
**Requirement Ids:**
    
    - DM-001
    
**Purpose:** To model a single, stateful tile on the game board, encapsulating its position, visual symbol, and interactive state.  
**Logic Description:** This class will represent the data and behavior of a tile. The constructor will initialize its position and symbol. Methods will allow changing the symbol and state, which will be called by the domain logic in the `GameplayLogicEndpoints` repository. The `State` will likely be an enum.  
**Documentation:**
    
    - **Summary:** Models a single tile on the game grid, holding its position, symbol, and state.
    
**Namespace:** PatternCipher.Shared.Gameplay.Grid  
**Metadata:**
    
    - **Category:** BusinessLogic
    
- **Path:** src/PatternCipher.Shared/Enums/TileState.cs  
**Description:** An enumeration defining the possible behavioral states of a tile. This provides a clear and constrained set of states, such as being a standard interactive tile, being locked, or acting as a wildcard. This is used by the Tile class and the game logic.  
**Template:** C# Enum  
**Dependency Level:** 1  
**Name:** TileState  
**Type:** Enum  
**Relative Path:** Enums/TileState.cs  
**Repository Id:** REPO-PATT-012  
**Pattern Ids:**
    
    
**Members:**
    
    - **Name:** Default  
**Type:**   
**Attributes:**   
    - **Name:** Locked  
**Type:**   
**Attributes:**   
    - **Name:** Wildcard  
**Type:**   
**Attributes:**   
    - **Name:** Obstacle  
**Type:**   
**Attributes:**   
    
**Methods:**
    
    
**Implemented Features:**
    
    - Tile State Definition
    
**Requirement Ids:**
    
    - NFR-M-001
    
**Purpose:** To provide a strongly-typed and finite set of possible states for a game tile, preventing invalid state assignments and improving code readability.  
**Logic Description:** This will be a simple C# enum declaration. Each member represents a distinct state a tile can be in, which dictates its appearance and interactive behavior within the game's rules engine.  
**Documentation:**
    
    - **Summary:** Defines the possible states for a Tile, such as Default, Locked, or Wildcard.
    
**Namespace:** PatternCipher.Shared.Enums  
**Metadata:**
    
    - **Category:** BusinessLogic
    
- **Path:** src/PatternCipher.Shared/Gameplay/Grid/Grid.cs  
**Description:** Represents the entire game grid. It is a container for a two-dimensional collection of Tile objects. This class provides the primary interface for the domain logic to interact with the game board, such as retrieving tiles at specific positions or getting all tiles.  
**Template:** C# Class  
**Dependency Level:** 3  
**Name:** Grid  
**Type:** Model  
**Relative Path:** Gameplay/Grid/Grid.cs  
**Repository Id:** REPO-PATT-012  
**Pattern Ids:**
    
    
**Members:**
    
    - **Name:** Width  
**Type:** int  
**Attributes:** public|readonly  
    - **Name:** Height  
**Type:** int  
**Attributes:** public|readonly  
    - **Name:** _tiles  
**Type:** Dictionary<GridPosition, Tile>  
**Attributes:** private|readonly  
    
**Methods:**
    
    - **Name:** Grid  
**Parameters:**
    
    - int width
    - int height
    - IEnumerable<Tile> initialTiles
    
**Return Type:** void  
**Attributes:** public  
    - **Name:** GetTileAt  
**Parameters:**
    
    - GridPosition position
    
**Return Type:** Tile  
**Attributes:** public  
    - **Name:** GetAllTiles  
**Parameters:**
    
    
**Return Type:** IEnumerable<Tile>  
**Attributes:** public  
    - **Name:** SwapTiles  
**Parameters:**
    
    - GridPosition pos1
    - GridPosition pos2
    
**Return Type:** void  
**Attributes:** public  
    
**Implemented Features:**
    
    - Grid Data Structure
    - Tile Access and Manipulation
    
**Requirement Ids:**
    
    - NFR-M-001
    
**Purpose:** To model the game board as a cohesive unit, managing the dimensions and the collection of tiles, and providing safe access methods.  
**Logic Description:** The Grid class will hold its dimensions and a dictionary or 2D array of Tile objects. The constructor will initialize the grid with a given set of tiles. It will have methods to get a specific tile by its GridPosition and to get all tiles. A SwapTiles method will handle the logic of swapping two tiles' positions within its internal collection.  
**Documentation:**
    
    - **Summary:** A data model representing the game board, containing a collection of tiles and its dimensions.
    
**Namespace:** PatternCipher.Shared.Gameplay.Grid  
**Metadata:**
    
    - **Category:** BusinessLogic
    
- **Path:** src/PatternCipher.Shared/Player/PlayerSettings.cs  
**Description:** A data model that encapsulates all user-configurable settings. This includes audio volumes, accessibility options like colorblind mode, and other preferences. This class will be serialized as part of the PlayerProfile.  
**Template:** C# Class  
**Dependency Level:** 1  
**Name:** PlayerSettings  
**Type:** Model  
**Relative Path:** Player/PlayerSettings.cs  
**Repository Id:** REPO-PATT-012  
**Pattern Ids:**
    
    
**Members:**
    
    - **Name:** MasterVolume  
**Type:** float  
**Attributes:** public  
    - **Name:** MusicVolume  
**Type:** float  
**Attributes:** public  
    - **Name:** SfxVolume  
**Type:** float  
**Attributes:** public  
    - **Name:** IsMusicMuted  
**Type:** bool  
**Attributes:** public  
    - **Name:** IsSfxMuted  
**Type:** bool  
**Attributes:** public  
    - **Name:** ColorblindMode  
**Type:** string  
**Attributes:** public  
    - **Name:** AnalyticsConsent  
**Type:** bool  
**Attributes:** public  
    
**Methods:**
    
    
**Implemented Features:**
    
    - Settings Data Model
    
**Requirement Ids:**
    
    - DM-001
    
**Purpose:** To provide a structured container for all player-specific settings, making it easy to save, load, and apply them across the application.  
**Logic Description:** This will be a plain C# class (POCO) with public properties for each setting. It is designed for serialization, so it will contain simple data types. Default values for a new player will be set in its constructor.  
**Documentation:**
    
    - **Summary:** Data model for storing all user-configurable game settings.
    
**Namespace:** PatternCipher.Shared.Player  
**Metadata:**
    
    - **Category:** DataAccess
    
- **Path:** src/PatternCipher.Shared/Player/LevelCompletionStatus.cs  
**Description:** A data model to track the player's progress and performance on a specific level. It stores whether the level has been completed, the best score achieved, and the number of stars earned. A collection of these objects will be part of the PlayerProfile.  
**Template:** C# Class  
**Dependency Level:** 1  
**Name:** LevelCompletionStatus  
**Type:** Model  
**Relative Path:** Player/LevelCompletionStatus.cs  
**Repository Id:** REPO-PATT-012  
**Pattern Ids:**
    
    
**Members:**
    
    - **Name:** LevelId  
**Type:** string  
**Attributes:** public  
    - **Name:** IsCompleted  
**Type:** bool  
**Attributes:** public  
    - **Name:** StarsEarned  
**Type:** int  
**Attributes:** public  
    - **Name:** BestScore  
**Type:** int  
**Attributes:** public  
    - **Name:** BestTimeInSeconds  
**Type:** int  
**Attributes:** public  
    
**Methods:**
    
    
**Implemented Features:**
    
    - Per-Level Progress Tracking
    
**Requirement Ids:**
    
    - DM-001
    
**Purpose:** To model the player's achievements on a per-level basis, enabling features like star displays on the level selection screen and progress gating.  
**Logic Description:** This is a simple data-holding class (POCO) designed for serialization. It contains properties to store all relevant metrics of a player's performance for a single game level. The LevelId will be the key to associate this status with a specific level definition.  
**Documentation:**
    
    - **Summary:** Stores the completion status and best performance for a single level.
    
**Namespace:** PatternCipher.Shared.Player  
**Metadata:**
    
    - **Category:** DataAccess
    
- **Path:** src/PatternCipher.Shared/Player/PlayerProfile.cs  
**Description:** The root data model for all locally persisted player data, as defined in requirement DM-001. This class aggregates player progress, settings, and other persistent information into a single serializable object. It is the main contract for the LocalPersistenceEndpoints repository.  
**Template:** C# Class  
**Dependency Level:** 2  
**Name:** PlayerProfile  
**Type:** Model  
**Relative Path:** Player/PlayerProfile.cs  
**Repository Id:** REPO-PATT-012  
**Pattern Ids:**
    
    
**Members:**
    
    - **Name:** PlayerId  
**Type:** string  
**Attributes:** public  
    - **Name:** SaveSchemaVersion  
**Type:** string  
**Attributes:** public  
    - **Name:** Settings  
**Type:** PlayerSettings  
**Attributes:** public  
    - **Name:** LevelStatuses  
**Type:** Dictionary<string, LevelCompletionStatus>  
**Attributes:** public  
    - **Name:** TimestampOfLastSave  
**Type:** DateTime  
**Attributes:** public  
    - **Name:** TimestampOfFirstAppOpen  
**Type:** DateTime  
**Attributes:** public  
    - **Name:** AppVersionAtLastSave  
**Type:** string  
**Attributes:** public  
    
**Methods:**
    
    
**Implemented Features:**
    
    - Local Save Data Schema
    
**Requirement Ids:**
    
    - DM-001
    
**Purpose:** To define the complete, versioned structure of all player data that needs to be saved locally, ensuring data integrity and facilitating future migrations.  
**Logic Description:** This class serves as the root object for JSON serialization. It holds properties for all major pieces of player data, including a unique player ID, the version of the save schema, an instance of PlayerSettings, and a dictionary of LevelCompletionStatus keyed by level ID. This structure makes it easy to manage and evolve the player's save file over time.  
**Documentation:**
    
    - **Summary:** Defines the root structure for a player's entire local save data.
    
**Namespace:** PatternCipher.Shared.Player  
**Metadata:**
    
    - **Category:** DataAccess
    


---

# 2. Configuration

- **Feature Toggles:**
  
  
- **Database Configs:**
  
  


---

