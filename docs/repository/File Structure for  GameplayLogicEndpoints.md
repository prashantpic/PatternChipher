# Specification

# 1. Files

- **Path:** src/PatternCipher.Domain/PatternCipher.Domain.csproj  
**Description:** The .NET project file for the DomainCore library. It defines the project as a .NET Standard 2.1 class library, making it platform-agnostic. It will reference the SharedKernel repository.  
**Template:** C# Project File  
**Dependency Level:** 0  
**Name:** PatternCipher.Domain  
**Type:** Configuration  
**Relative Path:** PatternCipher.Domain.csproj  
**Repository Id:** REPO-PATT-002  
**Pattern Ids:**
    
    
**Members:**
    
    
**Methods:**
    
    
**Implemented Features:**
    
    - Project Configuration
    
**Requirement Ids:**
    
    
**Purpose:** Defines the project settings, framework, and dependencies for the core domain logic assembly.  
**Logic Description:** This file will be configured to target .NET Standard 2.1. It will include a ProjectReference to the SharedKernel project (REPO-PATT-012) to access shared data structures and interfaces.  
**Documentation:**
    
    - **Summary:** Project file for the PatternCipher.Domain assembly, containing core business logic decoupled from any specific platform or framework.
    
**Namespace:**   
**Metadata:**
    
    - **Category:** Build
    
- **Path:** src/PatternCipher.Domain/Puzzles/Aggregates/Puzzle.cs  
**Description:** Represents the Puzzle Aggregate Root. This is the central entity for a single gameplay instance, encapsulating the grid, the goal, and the solution. It is responsible for enforcing all game rules and maintaining a consistent state.  
**Template:** C# Class  
**Dependency Level:** 2  
**Name:** Puzzle  
**Type:** AggregateRoot  
**Relative Path:** Puzzles/Aggregates/Puzzle.cs  
**Repository Id:** REPO-PATT-002  
**Pattern Ids:**
    
    - AggregateRoot
    - DomainDriven
    
**Members:**
    
    - **Name:** Id  
**Type:** Guid  
**Attributes:** public|readonly  
    - **Name:** _grid  
**Type:** Grid  
**Attributes:** private|readonly  
    - **Name:** Goal  
**Type:** IPuzzleGoal  
**Attributes:** public|readonly  
    - **Name:** Solution  
**Type:** SolutionPath  
**Attributes:** public|readonly  
    - **Name:** MoveHistory  
**Type:** List<Move>  
**Attributes:** private|readonly  
    
**Methods:**
    
    - **Name:** Puzzle  
**Parameters:**
    
    - Guid id
    - Grid grid
    - IPuzzleGoal goal
    - SolutionPath solution
    
**Return Type:** void  
**Attributes:** public  
    - **Name:** ApplyMove  
**Parameters:**
    
    - Move move
    
**Return Type:** void  
**Attributes:** public  
    - **Name:** IsSolved  
**Parameters:**
    
    - IPuzzleEvaluator evaluator
    
**Return Type:** bool  
**Attributes:** public  
    - **Name:** GetGridState  
**Parameters:**
    
    
**Return Type:** Grid  
**Attributes:** public  
    
**Implemented Features:**
    
    - Puzzle State Management
    - Move Application
    - Goal Checking
    
**Requirement Ids:**
    
    - FR-B-001
    - FR-L-003
    
**Purpose:** To encapsulate the entire state and behavior of a single puzzle, acting as the primary transactional boundary for gameplay interactions.  
**Logic Description:** The constructor will initialize the puzzle with its grid, goal, and solution path. The ApplyMove method will first validate the move against the grid's rules (e.g., adjacency) and then apply it, updating the internal grid state and recording the move in history. The IsSolved method will delegate the evaluation logic to a strategy-based evaluator.  
**Documentation:**
    
    - **Summary:** The Puzzle aggregate root, which manages the game's core state including the tile grid and puzzle goal. It processes player moves and determines if the puzzle is solved.
    
**Namespace:** PatternCipher.Domain.Puzzles.Aggregates  
**Metadata:**
    
    - **Category:** BusinessLogic
    
- **Path:** src/PatternCipher.Domain/Puzzles/Entities/Grid.cs  
**Description:** An entity representing the game grid. It contains a collection of Tiles and manages their positions and state transitions. While part of the Puzzle aggregate, it's complex enough to be its own entity.  
**Template:** C# Class  
**Dependency Level:** 1  
**Name:** Grid  
**Type:** Entity  
**Relative Path:** Puzzles/Entities/Grid.cs  
**Repository Id:** REPO-PATT-002  
**Pattern Ids:**
    
    - Entity
    - DomainDriven
    
**Members:**
    
    - **Name:** Rows  
**Type:** int  
**Attributes:** public|readonly  
    - **Name:** Columns  
**Type:** int  
**Attributes:** public|readonly  
    - **Name:** _tiles  
**Type:** Dictionary<GridPosition, Tile>  
**Attributes:** private|readonly  
    
**Methods:**
    
    - **Name:** Grid  
**Parameters:**
    
    - int rows
    - int columns
    - IEnumerable<Tile> initialTiles
    
**Return Type:** void  
**Attributes:** public  
    - **Name:** GetTileAt  
**Parameters:**
    
    - GridPosition position
    
**Return Type:** Tile  
**Attributes:** public  
    - **Name:** SwapTiles  
**Parameters:**
    
    - GridPosition pos1
    - GridPosition pos2
    
**Return Type:** void  
**Attributes:** public  
    - **Name:** IsMoveValid  
**Parameters:**
    
    - Move move
    
**Return Type:** bool  
**Attributes:** public  
    
**Implemented Features:**
    
    - Grid State Management
    - Tile Swapping Logic
    - Move Validation
    
**Requirement Ids:**
    
    - FR-L-001
    
**Purpose:** To model the game board, manage the collection of tiles, and handle low-level grid operations like swapping.  
**Logic Description:** The Grid class will hold tiles in a dictionary or 2D array for efficient access by position. The SwapTiles method will perform the position exchange between two tiles. The IsMoveValid method will check constraints, such as if the positions are adjacent and if the tiles are not locked.  
**Documentation:**
    
    - **Summary:** Manages the two-dimensional grid of tiles, providing methods to access, validate, and manipulate the tiles within the puzzle's constraints.
    
**Namespace:** PatternCipher.Domain.Puzzles.Entities  
**Metadata:**
    
    - **Category:** BusinessLogic
    
- **Path:** src/PatternCipher.Domain/Puzzles/ValueObjects/GridPosition.cs  
**Description:** An immutable value object representing a coordinate on the grid.  
**Template:** C# Record  
**Dependency Level:** 0  
**Name:** GridPosition  
**Type:** ValueObject  
**Relative Path:** Puzzles/ValueObjects/GridPosition.cs  
**Repository Id:** REPO-PATT-002  
**Pattern Ids:**
    
    - ValueObject
    
**Members:**
    
    - **Name:** Row  
**Type:** int  
**Attributes:** public|init  
    - **Name:** Column  
**Type:** int  
**Attributes:** public|init  
    
**Methods:**
    
    - **Name:** IsAdjacentTo  
**Parameters:**
    
    - GridPosition other
    
**Return Type:** bool  
**Attributes:** public  
    
**Implemented Features:**
    
    - Coordinate Representation
    - Adjacency Check
    
**Requirement Ids:**
    
    
**Purpose:** To provide a type-safe and immutable representation of a tile's position, preventing primitive obsession.  
**Logic Description:** Implemented as a C# record for value-based equality and immutability. The IsAdjacentTo method will contain the logic to check if another position is horizontally or vertically next to this one.  
**Documentation:**
    
    - **Summary:** A simple, immutable value object to represent a tile's (Row, Column) position on the grid.
    
**Namespace:** PatternCipher.Domain.Puzzles.ValueObjects  
**Metadata:**
    
    - **Category:** BusinessLogic
    
- **Path:** src/PatternCipher.Domain/Puzzles/ValueObjects/SolutionPath.cs  
**Description:** An immutable value object that encapsulates the known solution for a puzzle, including the sequence of moves and the optimal move count ('par').  
**Template:** C# Class  
**Dependency Level:** 1  
**Name:** SolutionPath  
**Type:** ValueObject  
**Relative Path:** Puzzles/ValueObjects/SolutionPath.cs  
**Repository Id:** REPO-PATT-002  
**Pattern Ids:**
    
    - ValueObject
    
**Members:**
    
    - **Name:** Moves  
**Type:** IReadOnlyList<Move>  
**Attributes:** public|readonly  
    - **Name:** Par  
**Type:** int  
**Attributes:** public|readonly  
    
**Methods:**
    
    
**Implemented Features:**
    
    - Solution Storage
    
**Requirement Ids:**
    
    - FR-L-001
    - FR-S-002
    
**Purpose:** To store the guaranteed solution path and the par move count generated alongside a puzzle, essential for the hint system and scoring.  
**Logic Description:** This class will be immutable. The constructor will take a list of moves, and the Par property will be calculated as the count of these moves. It ensures that the solution data is treated as a single, consistent value.  
**Documentation:**
    
    - **Summary:** Encapsulates the data for a puzzle's solution, including the list of moves and the par count for scoring.
    
**Namespace:** PatternCipher.Domain.Puzzles.ValueObjects  
**Metadata:**
    
    - **Category:** BusinessLogic
    
- **Path:** src/PatternCipher.Domain/Evaluation/Interfaces/IGoalEvaluationStrategy.cs  
**Description:** The interface for the Strategy pattern, defining a contract for different puzzle completion evaluation logics.  
**Template:** C# Interface  
**Dependency Level:** 2  
**Name:** IGoalEvaluationStrategy  
**Type:** Interface  
**Relative Path:** Evaluation/Interfaces/IGoalEvaluationStrategy.cs  
**Repository Id:** REPO-PATT-002  
**Pattern Ids:**
    
    - StrategyPattern
    
**Members:**
    
    - **Name:** PuzzleType  
**Type:** PuzzleType  
**Attributes:** public|get  
    
**Methods:**
    
    - **Name:** IsGoalMet  
**Parameters:**
    
    - Puzzle puzzle
    
**Return Type:** bool  
**Attributes:** public  
    
**Implemented Features:**
    
    - Puzzle Goal Evaluation Contract
    
**Requirement Ids:**
    
    - FR-L-003
    
**Purpose:** To decouple the puzzle evaluation logic from the puzzle itself, allowing different types of puzzles to be checked for completion using a common mechanism.  
**Logic Description:** This interface will define a single method, IsGoalMet, which takes the current puzzle state and returns true if the puzzle's specific goal has been achieved. The PuzzleType property allows a factory or service to select the correct strategy.  
**Documentation:**
    
    - **Summary:** A strategy interface for evaluating whether a puzzle's goal has been met. Different puzzle types will have their own concrete implementations.
    
**Namespace:** PatternCipher.Domain.Evaluation.Interfaces  
**Metadata:**
    
    - **Category:** BusinessLogic
    
- **Path:** src/PatternCipher.Domain/Evaluation/Strategies/DirectMatchGoalEvaluator.cs  
**Description:** Concrete implementation of the goal evaluation strategy for 'Direct Match' puzzles.  
**Template:** C# Class  
**Dependency Level:** 3  
**Name:** DirectMatchGoalEvaluator  
**Type:** Strategy  
**Relative Path:** Evaluation/Strategies/DirectMatchGoalEvaluator.cs  
**Repository Id:** REPO-PATT-002  
**Pattern Ids:**
    
    - StrategyPattern
    
**Members:**
    
    
**Methods:**
    
    - **Name:** IsGoalMet  
**Parameters:**
    
    - Puzzle puzzle
    
**Return Type:** bool  
**Attributes:** public  
    
**Implemented Features:**
    
    - Direct Match Puzzle Logic
    
**Requirement Ids:**
    
    - FR-L-003
    
**Purpose:** To implement the specific logic for checking if a grid of tiles exactly matches a target pattern.  
**Logic Description:** The IsGoalMet method will iterate through the puzzle's grid and compare each tile's symbol with the corresponding tile in the puzzle's target pattern. It returns true only if all tiles match perfectly.  
**Documentation:**
    
    - **Summary:** Implements IGoalEvaluationStrategy. Checks if the player's grid configuration exactly matches the target pattern defined in the puzzle's goal.
    
**Namespace:** PatternCipher.Domain.Evaluation.Strategies  
**Metadata:**
    
    - **Category:** BusinessLogic
    
- **Path:** src/PatternCipher.Domain/Evaluation/Strategies/RuleBasedGoalEvaluator.cs  
**Description:** Concrete implementation of the goal evaluation strategy for 'Rule-Based' puzzles.  
**Template:** C# Class  
**Dependency Level:** 3  
**Name:** RuleBasedGoalEvaluator  
**Type:** Strategy  
**Relative Path:** Evaluation/Strategies/RuleBasedGoalEvaluator.cs  
**Repository Id:** REPO-PATT-002  
**Pattern Ids:**
    
    - StrategyPattern
    - CompositePattern
    
**Members:**
    
    
**Methods:**
    
    - **Name:** IsGoalMet  
**Parameters:**
    
    - Puzzle puzzle
    
**Return Type:** bool  
**Attributes:** public  
    
**Implemented Features:**
    
    - Rule-Based Puzzle Logic
    
**Requirement Ids:**
    
    - FR-L-003
    
**Purpose:** To implement the specific logic for checking if a grid satisfies a collection of logical rules.  
**Logic Description:** The IsGoalMet method will retrieve the list of IPuzzleRule objects from the puzzle's goal. It will then iterate through each rule, calling its IsSatisfiedBy method against the puzzle's grid. It returns true only if all rules are satisfied.  
**Documentation:**
    
    - **Summary:** Implements IGoalEvaluationStrategy. Checks if the player's grid configuration satisfies all logical rules defined in the puzzle's goal.
    
**Namespace:** PatternCipher.Domain.Evaluation.Strategies  
**Metadata:**
    
    - **Category:** BusinessLogic
    
- **Path:** src/PatternCipher.Domain/Generation/Interfaces/IPuzzleGenerator.cs  
**Description:** Defines the contract for the domain service responsible for generating new puzzles.  
**Template:** C# Interface  
**Dependency Level:** 1  
**Name:** IPuzzleGenerator  
**Type:** Interface  
**Relative Path:** Generation/Interfaces/IPuzzleGenerator.cs  
**Repository Id:** REPO-PATT-002  
**Pattern Ids:**
    
    
**Members:**
    
    
**Methods:**
    
    - **Name:** Generate  
**Parameters:**
    
    - DifficultyProfile difficulty
    
**Return Type:** GenerationResult  
**Attributes:** public  
    
**Implemented Features:**
    
    - Puzzle Generation Contract
    
**Requirement Ids:**
    
    - FR-L-001
    
**Purpose:** To provide a clean, high-level interface for creating puzzles, abstracting away the complexity of generation, solving, and difficulty tuning.  
**Logic Description:** The single Generate method will take a difficulty profile and is expected to return a complete, solvable puzzle along with its solution path.  
**Documentation:**
    
    - **Summary:** Interface for a service that creates new, solvable puzzles based on specified difficulty parameters.
    
**Namespace:** PatternCipher.Domain.Generation.Interfaces  
**Metadata:**
    
    - **Category:** BusinessLogic
    
- **Path:** src/PatternCipher.Domain/Generation/Services/PuzzleGenerator.cs  
**Description:** The primary implementation of the puzzle generator service. It uses a solver to guarantee solvability and can be configured with different strategies for creating varied puzzles.  
**Template:** C# Class  
**Dependency Level:** 3  
**Name:** PuzzleGenerator  
**Type:** DomainService  
**Relative Path:** Generation/Services/PuzzleGenerator.cs  
**Repository Id:** REPO-PATT-002  
**Pattern Ids:**
    
    - DomainService
    
**Members:**
    
    - **Name:** _solver  
**Type:** ISolvabilityValidator  
**Attributes:** private|readonly  
    - **Name:** _generationStrategy  
**Type:** IGenerationStrategy  
**Attributes:** private|readonly  
    
**Methods:**
    
    - **Name:** PuzzleGenerator  
**Parameters:**
    
    - ISolvabilityValidator solver
    - IGenerationStrategy generationStrategy
    
**Return Type:** void  
**Attributes:** public  
    - **Name:** Generate  
**Parameters:**
    
    - DifficultyProfile difficulty
    
**Return Type:** GenerationResult  
**Attributes:** public  
    
**Implemented Features:**
    
    - Procedural Puzzle Generation
    - Solvability Guarantee
    
**Requirement Ids:**
    
    - FR-L-001
    - FR-L-002
    - FR-L-006
    - NFR-R-003
    - FR-B-001
    
**Purpose:** To procedurally create unique, non-trivial, and guaranteed solvable puzzles according to difficulty parameters.  
**Logic Description:** The Generate method orchestrates the puzzle creation. It could work by generating a solved state (the goal) and then working backward a number of steps to create the initial puzzle state. After generating the initial state, it uses the ISolvabilityValidator to confirm a solution exists and to get the solution path and 'par' count. It will reject and regenerate puzzles that don't meet non-triviality constraints (e.g., solution path too short).  
**Documentation:**
    
    - **Summary:** A domain service that orchestrates the procedural generation of puzzles. It ensures each puzzle is solvable by using a solver and adheres to the specified difficulty.
    
**Namespace:** PatternCipher.Domain.Generation.Services  
**Metadata:**
    
    - **Category:** BusinessLogic
    
- **Path:** src/PatternCipher.Domain/Generation/Interfaces/ISolvabilityValidator.cs  
**Description:** Defines a contract for a service that can determine if a puzzle is solvable and find a solution path.  
**Template:** C# Interface  
**Dependency Level:** 2  
**Name:** ISolvabilityValidator  
**Type:** Interface  
**Relative Path:** Generation/Interfaces/ISolvabilityValidator.cs  
**Repository Id:** REPO-PATT-002  
**Pattern Ids:**
    
    
**Members:**
    
    
**Methods:**
    
    - **Name:** TryFindSolution  
**Parameters:**
    
    - Puzzle puzzle
    
**Return Type:** bool  
**Attributes:** public  
    
**Implemented Features:**
    
    - Solvability Check Contract
    
**Requirement Ids:**
    
    - NFR-R-003
    
**Purpose:** To abstract the algorithm used for puzzle solving, allowing different solver implementations to be used.  
**Logic Description:** This interface defines a method to attempt finding a solution for a given puzzle state. It will likely have an out parameter or a result object that contains the SolutionPath if one is found.  
**Documentation:**
    
    - **Summary:** Interface for a component that validates if a puzzle is solvable from its current state and can return a valid solution path.
    
**Namespace:** PatternCipher.Domain.Generation.Interfaces  
**Metadata:**
    
    - **Category:** BusinessLogic
    
- **Path:** src/PatternCipher.Domain/Generation/ValueObjects/DifficultyProfile.cs  
**Description:** An immutable value object containing all parameters that define a puzzle's difficulty.  
**Template:** C# Record  
**Dependency Level:** 1  
**Name:** DifficultyProfile  
**Type:** ValueObject  
**Relative Path:** Generation/ValueObjects/DifficultyProfile.cs  
**Repository Id:** REPO-PATT-002  
**Pattern Ids:**
    
    - ValueObject
    
**Members:**
    
    - **Name:** GridWidth  
**Type:** int  
**Attributes:** public|init  
    - **Name:** GridHeight  
**Type:** int  
**Attributes:** public|init  
    - **Name:** UniqueSymbolCount  
**Type:** int  
**Attributes:** public|init  
    - **Name:** PuzzleType  
**Type:** PuzzleType  
**Attributes:** public|init  
    - **Name:** MinimumSolutionMoves  
**Type:** int  
**Attributes:** public|init  
    
**Methods:**
    
    
**Implemented Features:**
    
    - Difficulty Parameterization
    
**Requirement Ids:**
    
    - FR-L-002
    
**Purpose:** To provide a structured and type-safe way to pass difficulty settings to the puzzle generator.  
**Logic Description:** This will be a simple data container, implemented as a C# record for immutability and value-based equality. It aggregates all factors that contribute to difficulty into a single, cohesive object.  
**Documentation:**
    
    - **Summary:** A value object that encapsulates all parameters used to control the difficulty of procedurally generated puzzles.
    
**Namespace:** PatternCipher.Domain.Generation.ValueObjects  
**Metadata:**
    
    - **Category:** BusinessLogic
    
- **Path:** src/PatternCipher.Domain/Scoring/Services/ScoringService.cs  
**Description:** A domain service that calculates a player's score for a completed level.  
**Template:** C# Class  
**Dependency Level:** 3  
**Name:** ScoringService  
**Type:** DomainService  
**Relative Path:** Scoring/Services/ScoringService.cs  
**Repository Id:** REPO-PATT-002  
**Pattern Ids:**
    
    - DomainService
    
**Members:**
    
    
**Methods:**
    
    - **Name:** CalculateEfficiencyBonus  
**Parameters:**
    
    - Puzzle puzzle
    - int movesTaken
    
**Return Type:** int  
**Attributes:** public  
    
**Implemented Features:**
    
    - Efficiency Bonus Calculation
    
**Requirement Ids:**
    
    - FR-S-002
    
**Purpose:** To encapsulate the business logic for score calculations, specifically the efficiency bonus.  
**Logic Description:** The CalculateEfficiencyBonus method will compare the `movesTaken` with the `puzzle.Solution.Par` value. If the moves taken are less than or equal to par, it will calculate a bonus score based on a formula (e.g., (Par - MovesTaken) * PointsPerMoveSaved). The formula itself would be a business rule, possibly configurable.  
**Documentation:**
    
    - **Summary:** A stateless domain service responsible for calculating player scores, including bonuses like the efficiency bonus for completing a puzzle under par.
    
**Namespace:** PatternCipher.Domain.Scoring.Services  
**Metadata:**
    
    - **Category:** BusinessLogic
    


---

# 2. Configuration

- **Feature Toggles:**
  
  
- **Database Configs:**
  
  


---

