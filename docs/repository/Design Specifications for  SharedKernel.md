# Software Design Specification for REPO-PATT-012: SharedKernel

## 1. Introduction

This document provides the detailed software design specification for the `SharedKernel` repository. This repository is a foundational class library for the Pattern Cipher game client. Its primary purpose is to define a set of shared, platform-agnostic data models, value objects, enums, and common base classes that constitute the core domain of the game.

By centralizing these core definitions, the `SharedKernel` ensures consistency, reduces code duplication, and promotes a clean, layered architecture where the core game logic is decoupled from the Unity engine's presentation and infrastructure concerns. This library targets `.NET Standard 2.1` to ensure full compatibility with the Unity project.

**Key Requirements Addressed:**
*   **NFR-M-001 (Code Modularity & Reusability):** This entire repository is a direct implementation of this requirement.
*   **DM-001 (Local Data Schema):** This repository defines the C# classes that model the local data schema for player progress and settings.

## 2. General Design Principles

*   **Platform Agnostic:** The code in this library **must not** have any dependencies on Unity-specific assemblies (e.g., `UnityEngine.dll`, `UnityEditor.dll`). All classes must be pure C#.
*   **Domain-Driven Design (DDD):** The design employs tactical DDD patterns.
    *   **Value Objects:** Used for domain concepts where equality is based on constituent values, not identity (e.g., `GridPosition`, `Symbol`). These objects should be immutable.
    *   **Models/Entities:** Used for domain concepts with a distinct identity and a lifecycle (e.g., `Tile`).
*   **Immutability:** Value objects and properties of models should be immutable (readonly or init-only setters) wherever possible to prevent unintended side effects and ensure a predictable state.
*   **Serialization:** Data models intended for persistence (e.g., `PlayerProfile`) are designed as Plain Old C# Objects (POCOs) to be easily serialized to and deserialized from JSON by the `LocalPersistenceEndpoints` repository. They will have parameterless constructors to support this.
*   **Clear Naming and Structure:** Namespaces and class names should be explicit and follow standard C# conventions to clearly communicate their purpose.

---

## 3. Detailed Module Specifications

### 3.1. Project Configuration

#### 3.1.1. `PatternCipher.Shared.csproj`
*   **Type:** C# Project File
*   **Purpose:** To configure the `.NET Standard 2.1` class library.
*   **Specification:**
    *   The file will be an XML-based `.csproj` file.
    *   It will specify `<TargetFramework>netstandard2.1</TargetFramework>`.
    *   It will define the root namespace as `PatternCipher.Shared`.
    *   No external NuGet package dependencies are required for this project.

### 3.2. Common Utilities

#### 3.2.1. `Common/ValueObject.cs`
*   **Type:** Abstract Class
*   **Purpose:** To provide a base implementation of the Value Object pattern.
*   **Specification:**
    csharp
    using System.Collections.Generic;
    using System.Linq;

    namespace PatternCipher.Shared.Common
    {
        /// <summary>
        /// Abstract base class for creating Value Objects, a core DDD tactical pattern.
        /// Value Objects are objects whose equality is based on their constituent values, not their identity.
        /// </summary>
        public abstract class ValueObject
        {
            /// <summary>
            /// Gets the components that are used for equality comparison.
            /// </summary>
            /// <returns>An enumerable of objects representing the equality components.</returns>
            protected abstract IEnumerable<object> GetEqualityComponents();

            public override bool Equals(object obj)
            {
                if (obj == null || obj.GetType() != GetType())
                {
                    return false;
                }

                var other = (ValueObject)obj;
                return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
            }

            public override int GetHashCode()
            {
                return GetEqualityComponents()
                    .Select(x => x != null ? x.GetHashCode() : 0)
                    .Aggregate(17, (current, next) => current * 23 + next);
            }

            public static bool operator ==(ValueObject a, ValueObject b)
            {
                if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                    return true;
                if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                    return false;
                return a.Equals(b);
            }

            public static bool operator !=(ValueObject a, ValueObject b)
            {
                return !(a == b);
            }
        }
    }
    

### 3.3. Gameplay Domain Models

#### 3.3.1. `Enums/TileState.cs`
*   **Type:** Enum
*   **Purpose:** Defines the finite set of behavioral states for a `Tile`.
*   **Specification:**
    csharp
    namespace PatternCipher.Shared.Enums
    {
        /// <summary>
        /// Defines the possible behavioral states for a Tile.
        /// </summary>
        public enum TileState
        {
            /// <summary>Standard interactive tile.</summary>
            Default,
            /// <summary>Tile cannot be moved or changed directly.</summary>
            Locked,
            /// <summary>Tile can match any symbol or color.</summary>
            Wildcard,
            /// <summary>Tile that blocks interaction or movement.</summary>
            Obstacle
        }
    }
    

#### 3.3.2. `Gameplay/Grid/GridPosition.cs`
*   **Type:** Record Struct (Value Object)
*   **Purpose:** An immutable, strongly-typed representation of a 2D coordinate on the grid.
*   **Specification:**
    csharp
    namespace PatternCipher.Shared.Gameplay.Grid
    {
        /// <summary>
        /// A value object representing a tile's position on the grid (column X, row Y).
        /// Implemented as a readonly record struct for immutability and value-based equality.
        /// </summary>
        /// <param name="X">The horizontal coordinate (column).</param>
        /// <param name="Y">The vertical coordinate (row).</param>
        public readonly record struct GridPosition(int X, int Y);
    }
    

#### 3.3.3. `Gameplay/Symbols/Symbol.cs`
*   **Type:** Class (Value Object)
*   **Purpose:** To define a unique, logical symbol, separate from its visual representation.
*   **Specification:**
    csharp
    using PatternCipher.Shared.Common;
    using System;
    using System.Collections.Generic;

    namespace PatternCipher.Shared.Gameplay.Symbols
    {
        /// <summary>
        /// Represents a unique symbol that a tile can display.
        /// Implemented as a Value Object where equality is based on the symbol's unique ID.
        /// </summary>
        public sealed class Symbol : ValueObject
        {
            public string Id { get; }

            public Symbol(string id)
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    throw new ArgumentException("Symbol ID cannot be null or whitespace.", nameof(id));
                }
                Id = id;
            }
            
            protected override IEnumerable<object> GetEqualityComponents()
            {
                yield return Id;
            }
        }
    }
    

#### 3.3.4. `Gameplay/Grid/Tile.cs`
*   **Type:** Class (Model)
*   **Purpose:** Represents a single, stateful tile on the game grid.
*   **Specification:**
    csharp
    using PatternCipher.Shared.Enums;
    using PatternCipher.Shared.Gameplay.Symbols;
    
    namespace PatternCipher.Shared.Gameplay.Grid
    {
        /// <summary>
        /// Models a single tile on the game grid, holding its position, symbol, and state.
        /// </summary>
        public class Tile
        {
            public GridPosition Position { get; private set; }
            public Symbol CurrentSymbol { get; private set; }
            public TileState State { get; private set; }

            public Tile(GridPosition position, Symbol initialSymbol, TileState initialState = TileState.Default)
            {
                Position = position;
                CurrentSymbol = initialSymbol;
                State = initialState;
            }

            public void ChangeSymbol(Symbol newSymbol)
            {
                if (State == TileState.Locked) return; // Or throw exception as per domain rules
                CurrentSymbol = newSymbol;
            }

            public void ChangeState(TileState newState)
            {
                State = newState;
            }
            
            // Internal method to be called by a higher-level grid or board service.
            internal void UpdatePosition(GridPosition newPosition)
            {
                Position = newPosition;
            }
        }
    }
    

#### 3.3.5. `Gameplay/Grid/Grid.cs`
*   **Type:** Class (Model)
*   **Purpose:** Represents the entire game board as a container of tiles.
*   **Specification:**
    csharp
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    namespace PatternCipher.Shared.Gameplay.Grid
    {
        /// <summary>
        /// A data model representing the game board, containing a collection of tiles and its dimensions.
        /// Provides a clean interface for querying the state of the grid.
        /// </summary>
        public class Grid
        {
            public int Width { get; }
            public int Height { get; }
            private readonly IReadOnlyDictionary<GridPosition, Tile> _tiles;

            public Grid(int width, int height, IEnumerable<Tile> initialTiles)
            {
                Width = width;
                Height = height;
                _tiles = new ReadOnlyDictionary<GridPosition, Tile>(initialTiles.ToDictionary(t => t.Position));
            }

            public Tile GetTileAt(GridPosition position)
            {
                return _tiles.TryGetValue(position, out var tile) ? tile : null;
            }
            
            public bool TryGetTileAt(GridPosition position, out Tile tile)
            {
                return _tiles.TryGetValue(position, out tile);
            }

            public IEnumerable<Tile> GetAllTiles()
            {
                return _tiles.Values;
            }
        }
    }
    

### 3.4. Player Data Models (for Persistence)

These classes are designed as POCOs for easy JSON serialization. They directly implement the schema defined in requirement `DM-001`.

#### 3.4.1. `Player/PlayerSettings.cs`
*   **Type:** Class (Model)
*   **Purpose:** Data container for all user-configurable settings.
*   **Specification:**
    csharp
    namespace PatternCipher.Shared.Player
    {
        /// <summary>
        /// Data model for storing all user-configurable game settings.
        /// </summary>
        public class PlayerSettings
        {
            public float MusicVolume { get; set; } = 0.8f;
            public float SfxVolume { get; set; } = 1.0f;
            public bool IsMusicMuted { get; set; } = false;
            public bool IsSfxMuted { get; set; } = false;
            public string ColorblindMode { get; set; } = "None"; // e.g., "None", "Deuteranopia"
            public bool AnalyticsConsent { get; set; } = true; // Default state may depend on region
            public bool ReducedMotion { get; set; } = false;
            public bool HapticsEnabled { get; set; } = true;
            public string Language { get; set; } = "en";
        }
    }
    

#### 3.4.2. `Player/LevelCompletionStatus.cs`
*   **Type:** Class (Model)
*   **Purpose:** Tracks player performance for a single level.
*   **Specification:**
    csharp
    namespace PatternCipher.Shared.Player
    {
        /// <summary>
        /// Stores the completion status and best performance for a single level.
        /// </summary>
        public class LevelCompletionStatus
        {
            public string LevelId { get; set; }
            public bool IsCompleted { get; set; } = false;
            public int StarsEarned { get; set; } = 0;
            public int BestScore { get; set; } = 0;
            public int BestTimeInSeconds { get; set; } = -1; // -1 indicates not set
        }
    }
    

#### 3.4.3. `Player/PlayerProfile.cs`
*   **Type:** Class (Model)
*   **Purpose:** The root object for the player's local save file.
*   **Specification:**
    csharp
    using System;
    using System.Collections.Generic;

    namespace PatternCipher.Shared.Player
    {
        /// <summary>
        /// Defines the root structure for a player's entire local save data.
        /// This is the primary object to be serialized/deserialized for persistence.
        /// </summary>
        public class PlayerProfile
        {
            public string PlayerId { get; set; }
            public string SaveSchemaVersion { get; set; }
            public PlayerSettings Settings { get; set; }
            public Dictionary<string, LevelCompletionStatus> LevelStatuses { get; set; }
            public DateTime TimestampOfLastSave { get; set; }
            public DateTime TimestampOfFirstAppOpen { get; set; }
            public string AppVersionAtLastSave { get; set; }

            // Parameterless constructor for deserialization
            public PlayerProfile()
            {
                Settings = new PlayerSettings();
                LevelStatuses = new Dictionary<string, LevelCompletionStatus>();
            }
        }
    }
    