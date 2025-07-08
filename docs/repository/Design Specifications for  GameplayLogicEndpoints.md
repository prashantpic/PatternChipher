# Software Design Specification (SDS) for GameplayLogicEndpoints (REPO-PATT-002)

## 1. Introduction

### 1.1. Purpose and Scope

This document details the technical design for the `GameplayLogicEndpoints` repository (`PatternCipher.Domain` namespace). This repository represents the core domain logic of the Pattern Cipher game. It is designed as a platform-agnostic .NET Standard class library, completely decoupled from the Unity engine presentation layer.

The scope of this module includes:
-   Modeling the core game entities: `Puzzle`, `Grid`, `Tile`.
-   Defining and implementing the rules for all puzzle types (`Direct Match`, `Rule-Based`, etc.).
-   Procedurally generating unique, non-trivial, and guaranteed-solvable puzzles.
-   Validating puzzle solvability.
-   Calculating player scores based on performance metrics.

### 1.2. Technology Stack

-   **Language:** C#
-   **Framework:** .NET Standard 2.1
-   **Third-party Libraries:** None
-   **Testing Framework:** xUnit or NUnit

### 1.3. Architectural Principles

-   **Domain-Driven Design (DDD):** The design is centered around a rich domain model with aggregates, entities, and value objects that encapsulate the business logic of the puzzle game.
-   **Platform Agnostic:** The code in this repository will have no dependencies on Unity-specific APIs, ensuring the core logic is testable and potentially reusable outside of the Unity environment.
-   **Strategy Pattern:** Used extensively for evaluating puzzle goals and potentially for puzzle generation, allowing for easy extension with new puzzle types and rules without modifying existing code.
-   **Dependency on SharedKernel:** This project relies on `REPO-PATT-012` (`PatternCipher.Shared`) for common, cross-cutting data structures and interfaces like `Tile`, `Symbol`, and `IPuzzleGoal`.

---

## 2. Core Domain Model

The domain model is structured around the `Puzzle` aggregate, which ensures the consistency and validity of a single puzzle instance.

### 2.1. Puzzle Aggregate (`Puzzles/Aggregates/Puzzle.cs`)

The `Puzzle` class is the aggregate root, representing a complete, self-contained puzzle instance. It acts as the transactional boundary for all gameplay interactions.

**Members:**

| Name          | Type                     | Description                                                                 |
| :------------ | :----------------------- | :-------------------------------------------------------------------------- |
| `Id`          | `Guid`                   | The unique identifier for this puzzle instance.                             |
| `_grid`       | `Grid`                   | The internal state of the game board.                                       |
| `Goal`        | `IPuzzleGoal` (Shared)   | The objective of the puzzle (e.g., a target pattern, a set of rules).       |
| `Solution`    | `SolutionPath`           | The guaranteed solution path and 'par' move count for this puzzle.          |
| `_moveHistory` | `List<Move>` (Shared)   | A record of all valid moves applied to the puzzle during the session.       |

**Methods:**

-   `public Puzzle(Guid id, Grid grid, IPuzzleGoal goal, SolutionPath solution)`
    -   **Logic:** Constructor to initialize a new, valid puzzle instance. It validates that the input components are not null.

-   `public Result ApplyMove(Move move)`
    -   **Logic:**
        1.  Validates the move using `_grid.IsMoveValid(move)`.
        2.  If invalid, returns a `Result.Failure` with an error message (e.g., "Tiles not adjacent", "Tile is locked").
        3.  If valid, applies the move to the grid (e.g., `_grid.SwapTiles(...)` or `_grid.UpdateTileState(...)`).
        4.  Adds the move to the `_moveHistory`.
        5.  Returns `Result.Success`.

-   `public bool IsSolved(IGoalEvaluationStrategy evaluator)`
    -   **Requirement:** `FR-L-003`
    -   **Logic:** Delegates the evaluation to the provided strategy. `return evaluator.IsGoalMet(this);`. This decouples the puzzle from the specific win condition logic.

-   `public Grid GetGridState()`
    -   **Logic:** Returns a read-only view or a deep copy of the current grid to prevent external modification, ensuring the aggregate's integrity.

### 2.2. Grid Entity (`Puzzles/Entities/Grid.cs`)

The `Grid` class is a complex entity within the `Puzzle` aggregate. It manages the collection of tiles and their spatial relationships.

**Members:**

| Name      | Type                            | Description                                        |
| :-------- | :------------------------------ | :------------------------------------------------- |
| `Rows`    | `int`                           | The number of rows in the grid.                    |
| `Columns` | `int`                           | The number of columns in the grid.                 |
| `_tiles`  | `Dictionary<GridPosition, Tile>` | A dictionary for efficient tile lookup by position. |

**Methods:**

-   `public Grid(int rows, int columns, IEnumerable<Tile> initialTiles)`
    -   **Logic:** Initializes the grid with its dimensions and populates the `_tiles` dictionary. Validates that the number of tiles matches `rows * columns`.

-   `public Tile GetTileAt(GridPosition position)`
    -   **Logic:** Returns the tile at the specified position. Throws an `ArgumentOutOfRangeException` if the position is invalid.

-   `public bool IsMoveValid(Move move)`
    -   **Logic:**
        1.  Checks if the positions in the move are within the grid bounds.
        2.  For a `SwapMove`, checks if the two positions are adjacent using `pos1.IsAdjacentTo(pos2)`.
        3.  Checks if the tiles involved in the move are interactive (e.g., not `IsLocked`).
        4.  Returns `true` if all checks pass, `false` otherwise.

-   `public void SwapTiles(GridPosition pos1, GridPosition pos2)`
    -   **Logic:**
        1.  Retrieves the tiles at `pos1` and `pos2`.
        2.  Updates their internal `Position` property.
        3.  Swaps their keys in the `_tiles` dictionary.

-   `public void UpdateTileState(GridPosition position, Symbol newSymbol)`
    -   **Logic:** Retrieves the tile at the given position and updates its symbol or other state properties.

### 2.3. Value Objects

-   **`GridPosition` (`Puzzles/ValueObjects/GridPosition.cs`)**
    -   **Type:** C# `record` for immutability and value semantics.
    -   **Members:** `public int Row { get; init; }`, `public int Column { get; init; }`.
    -   **Methods:**
        -   `public bool IsAdjacentTo(GridPosition other)`: Returns `true` if the Manhattan distance between the two points is exactly 1.

-   **`SolutionPath` (`Puzzles/ValueObjects/SolutionPath.cs`)**
    -   **Type:** Immutable C# `class`.
    -   **Requirement:** `FR-L-001`, `FR-S-002`
    -   **Members:** `public IReadOnlyList<Move> Moves { get; }`, `public int Par { get; }`.
    -   **Logic:** The constructor takes a list of moves. The `Par` property is set to `Moves.Count`.

---

## 3. Domain Services

### 3.1. Puzzle Generation Service (`Generation/Services/PuzzleGenerator.cs`)

This service is responsible for orchestrating the procedural generation of new puzzles.

**Interface:** `IPuzzleGenerator` (`Generation/Interfaces/IPuzzleGenerator.cs`)
csharp
public interface IPuzzleGenerator
{
    GenerationResult Generate(DifficultyProfile difficulty);
}


**Implementation:** `PuzzleGenerator`
-   **Dependencies:** `ISolvabilityValidator`, `IGenerationStrategy`.
-   **Method:** `public GenerationResult Generate(DifficultyProfile difficulty)`
    -   **Requirement:** `FR-L-001`, `FR-L-002`, `FR-L-006`, `NFR-R-003`, `FR-B-001`.
    -   **Logic:**
        1.  Selects a generation strategy (e.g., `ReverseShuffleStrategy`) based on the `difficulty.PuzzleType`.
        2.  The strategy creates a solved `goalGrid` based on the `difficulty` parameters.
        3.  The strategy then applies a number of random, valid *reverse* moves to the `goalGrid` to create the `initialGrid`. The number of moves is determined by the `difficulty` profile.
        4.  The sequence of forward moves that undoes the shuffling becomes the `idealSolutionPath`.
        5.  Creates a preliminary `Puzzle` instance with the `initialGrid`, `goal`, and `idealSolutionPath`.
        6.  Uses the `_solver.TryFindSolution(puzzle)` to validate that a solution exists from the generated state and to determine the true optimal 'par' value.
        7.  If the puzzle is unsolvable or the 'par' is below `difficulty.MinimumSolutionMoves`, the process repeats.
        8.  Returns a `GenerationResult` containing the final, validated `Puzzle` instance.

### 3.2. Solvability Validator Service (`Generation/Services/AStarSolver.cs`)

This service validates if a puzzle is solvable and finds an optimal solution path.

**Interface:** `ISolvabilityValidator` (`Generation/Interfaces/ISolvabilityValidator.cs`)
csharp
public interface ISolvabilityValidator
{
    // Tries to find a solution. Returns true if successful.
    // The found solution is returned via the out parameter.
    bool TryFindSolution(Puzzle puzzle, out SolutionPath solution);
}


**Implementation:** `AStarSolver`
-   **Requirement:** `NFR-R-003`.
-   **Logic:** Implements the A* pathfinding algorithm.
    -   **State:** A node in the search graph represents a full `Grid` state.
    -   **Neighbors:** A neighbor state is generated by applying one valid `Move` (e.g., a single swap) to the current state.
    -   **Heuristic (h(n)):** A function to estimate the cost to the goal. For a "Direct Match" puzzle, a good heuristic is the "Manhattan distance" of all mismatched tiles from their target positions. For rule-based puzzles, the heuristic could be the "number of violated rules".
    -   **Cost (g(n)):** The number of moves made to reach the current state.
    -   The algorithm will explore the state space, prioritizing nodes with the lowest `f(n) = g(n) + h(n)`.
    -   If the goal state is reached, it reconstructs the path of moves and returns it as the `SolutionPath`.

### 3.3. Scoring Service (`Scoring/Services/ScoringService.cs`)

A stateless service for calculating scores.

**Methods:**
-   `public int CalculateEfficiencyBonus(int par, int movesTaken)`
    -   **Requirement:** `FR-S-002`
    -   **Logic:**
        1.  If `movesTaken > par`, return 0.
        2.  Calculates the bonus based on a configurable formula.
        3.  Example formula: `(par - movesTaken) * pointsPerMoveSaved + flatBonusForPar`. The parameters (`pointsPerMoveSaved`, `flatBonusForPar`) would be part of a `ScoringProfile` object passed in or configured elsewhere.

---

## 4. Goal Evaluation (Strategy Pattern)

This system provides a flexible way to check for puzzle completion.

### 4.1. Evaluation Strategy Interface (`Evaluation/Interfaces/IGoalEvaluationStrategy.cs`)

csharp
public interface IGoalEvaluationStrategy
{
    PuzzleType PuzzleType { get; }
    bool IsGoalMet(Puzzle puzzle);
}


### 4.2. Concrete Strategies

-   **`DirectMatchGoalEvaluator` (`Evaluation/Strategies/DirectMatchGoalEvaluator.cs`)**
    -   **Requirement:** `FR-L-003` (Direct Match).
    -   **Logic:**
        1.  Casts the `puzzle.Goal` to `DirectMatchGoal`.
        2.  Iterates through every `GridPosition` on the `puzzle.Grid`.
        3.  For each position, compares the `Symbol` of the tile in the current grid with the `Symbol` of the tile in the `DirectMatchGoal.TargetGrid`.
        4.  If any symbol does not match, returns `false`.
        5.  If the loop completes, returns `true`.

-   **`RuleBasedGoalEvaluator` (`Evaluation/Strategies/RuleBasedGoalEvaluator.cs`)**
    -   **Requirement:** `FR-L-003` (Rule-Based).
    -   **Logic:**
        1.  Casts the `puzzle.Goal` to `RuleBasedGoal`.
        2.  Iterates through the `RuleBasedGoal.Rules` collection.
        3.  For each `IPuzzleRule` in the collection, it calls `rule.IsSatisfiedBy(puzzle.Grid)`.
        4.  If any rule returns `false`, the method immediately returns `false`.
        5.  If all rules return `true`, the method returns `true`.

### 4.3. Puzzle Rule Interface (Example)

(To be defined in the Shared Kernel, but relevant to the logic here)
csharp
// Example rule interface
public interface IPuzzleRule
{
    bool IsSatisfiedBy(Grid grid);
}

// Example concrete rule
public class NoThreeInARowRule : IPuzzleRule
{
    public bool IsSatisfiedBy(Grid grid)
    {
        // Logic to check all rows and columns for three consecutive identical symbols.
    }
}


---

## 5. Data Structures and Inputs

This domain relies on several key data structures, many of which are defined in the SharedKernel for reusability.

-   **`DifficultyProfile` (`Generation/ValueObjects/DifficultyProfile.cs`)**
    -   **Type:** C# `record`.
    -   **Purpose:** A data-transfer object (DTO) that encapsulates all parameters for generating a new puzzle, such as grid size, symbol count, puzzle type, and minimum solution complexity.
    -   **Requirement:** `FR-L-002`

-   **`GenerationResult` (Class)**
    -   **Purpose:** The output DTO from the `IPuzzleGenerator` service.
    -   **Members:** `public Puzzle Puzzle { get; }`, `public SolutionPath Solution { get; }`.
    -   **Logic:** A simple container to return the generated puzzle and its verified solution together.

---

## 6. Testing Strategy

-   **Unit Tests:**
    -   Test all Value Objects (`GridPosition`, `SolutionPath`, `DifficultyProfile`) for correct initialization and behavior (e.g., `GridPosition.IsAdjacentTo`).
    -   Test `Grid` entity methods: `GetTileAt` with valid/invalid positions, `SwapTiles`, and `IsMoveValid` with various scenarios (adjacent, non-adjacent, locked tiles).
    -   Test each `IGoalEvaluationStrategy` implementation with grids that should pass and fail.
    -   Test each `IPuzzleRule` implementation independently.
    -   Test `ScoringService` with different inputs for `par` and `movesTaken`.
-   **Integration Tests:**
    -   Test the `Puzzle` aggregate by applying a sequence of valid and invalid moves and checking its state.
    -   Test the `PuzzleGenerator` to ensure it produces a `GenerationResult` where the `Puzzle` is solvable and the `Solution` is valid and meets the `DifficultyProfile` constraints.
    -   Test the interaction between `PuzzleGenerator` and `AStarSolver` to ensure solvability is correctly determined and the 'par' value is reasonable.