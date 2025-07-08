# Software Design Specification: LocalPersistenceEndpoints

## 1. Introduction

This document provides a detailed software design specification for the `LocalPersistenceEndpoints` repository, which is a component of the Pattern Cipher game client. The purpose of this repository is to provide a robust, reliable, and secure mechanism for persisting all local player data.

This infrastructure component is responsible for:
-   Saving and loading player progress and settings.
-   Ensuring data integrity and deterring casual tampering.
-   Handling data schema changes across application updates through a versioning and migration system.
-   Saving and restoring in-progress game state to recover from unexpected application interruptions.

This design adheres to the principles of Separation of Concerns and Dependency Inversion, ensuring the persistence logic is decoupled from the core application and is highly testable.

**Key Requirements:** `DM-001`, `DM-003`, `DM-004`, `NFR-R-002`, `NFR-SEC-001`, `NFR-R-004`.

## 2. Architectural Overview

The `LocalPersistenceEndpoints` repository follows a layered architecture using the **Repository Pattern** to abstract file system interactions. It provides a single entry point, `IPersistenceService`, which acts as a facade over several specialized components.

The internal data flow is as follows:
1.  **Service Layer (`PersistenceService`)**: The top-level orchestrator that coordinates the save/load process. It is the only component that the application layer interacts with directly.
2.  **Specialized Services Layer**: A set of services handling specific concerns:
    *   **Security (`IDataProtector`)**: Manages data obfuscation and checksum validation.
    *   **Migration (`IMigrationService`)**: Manages the evolution of the save data schema over time.
    *   **Serialization (`IJsonSerializer`)**: Handles the conversion between C# objects and JSON strings.
3.  **Repository Layer (`IFileRepository`)**: The lowest level, responsible for the actual reading from and writing to the device's file system.

All interactions between layers are done through interfaces, promoting loose coupling and enabling dependency injection for easier testing and maintenance.

---

## 3. Core Components and Interfaces

This section details the design of each class and interface within the repository.

### 3.1. Main Service Interface

#### `IPersistenceService.cs`

-   **Namespace**: `PatternCipher.Infrastructure.Persistence.Interfaces`
-   **Purpose**: Defines the public contract for all persistence operations. This is the primary interface used by the Application layer.
-   **Methods**:
    -   `Task<PlayerProfile> LoadPlayerProfileAsync()`: Asynchronously loads, migrates, and deserializes the main player profile from local storage. If no profile exists or if the data is corrupt and unrecoverable, it must return a new, default `PlayerProfile` object.
    -   `Task SavePlayerProfileAsync(PlayerProfile playerProfile)`: Asynchronously serializes, protects, and saves the provided `PlayerProfile` object to local storage.
    -   `bool HasPlayerProfile()`: Synchronously checks if a player profile save file exists.
    -   `void SaveInProgressState(GameStateInProgress state)`: Synchronously saves the state of an in-progress level for interruption recovery. This operation must be fast and is not expected to use the full protection/migration pipeline of the main profile save.
    -   `GameStateInProgress LoadInProgressState()`: Synchronously loads the in-progress level state. Returns `null` if no state exists.
    -   `void ClearInProgressState()`: Deletes the in-progress level state file.
    -   `bool HasInProgressState()`: Synchronously checks if an in-progress level state file exists.

### 3.2. Service Implementation

#### `PersistenceService.cs`

-   **Namespace**: `PatternCipher.Infrastructure.Persistence`
-   **Purpose**: Implements `IPersistenceService`. Orchestrates the complex process of loading and saving data by coordinating underlying services.
-   **Dependencies (injected via constructor)**:
    -   `IFileRepository _repository`
    -   `IDataProtector _dataProtector`
    -   `IJsonSerializer _serializer`
    -   `IMigrationService _migrationService`
-   **Private Constants**:
    -   `const string PLAYER_PROFILE_FILENAME = "player_profile.dat";`
    -   `const string IN_PROGRESS_STATE_FILENAME = "gamestate.tmp";`
-   **Method Logic**:
    -   `LoadPlayerProfileAsync`:
        1.  Determine the full file path using `_repository` and `PLAYER_PROFILE_FILENAME`.
        2.  If `_repository.FileExists()` is false, `return new PlayerProfile();`.
        3.  Enter a `try-catch` block to handle exceptions like `SaveDataCorruptionException` or `JsonSerializationException`. On any exception, log the error and `return new PlayerProfile();`.
        4.  Read the raw, protected text from the file using `await _repository.ReadAllTextAsync()`.
        5.  Deserialize the raw text into a `SaveDataWrapper` object using `_serializer`.
        6.  Call `_dataProtector.Unprotect(wrapper.Payload)` to get the clean, inner JSON string. This step implicitly validates the checksum.
        7.  Use `Newtonsoft.Json.Linq.JObject.Parse()` on the clean JSON to prepare it for migration.
        8.  Call `_migrationService.MigrateToCurrentVersion()` on the `JObject`. This returns the migrated `JObject`.
        9.  Serialize the migrated `JObject` back to a string.
        10. Deserialize the final string into a `PlayerProfile` object using `_serializer`.
        11. Return the fully loaded and migrated `PlayerProfile`.
    -   `SavePlayerProfileAsync`:
        1.  Serialize the `PlayerProfile` object to a JSON string (`payloadJson`) using `_serializer`.
        2.  Protect the `payloadJson` using `_dataProtector.Protect()`. This returns an obfuscated string.
        3.  Create a `new SaveDataWrapper` instance, setting its `SchemaVersion` to the current version constant, its `Checksum` (from the protector, if separate), and its `Payload` to the protected string.
        4.  Serialize the `SaveDataWrapper` object into a final JSON string.
        5.  Write the final string to `PLAYER_PROFILE_FILENAME` using `await _repository.WriteAllTextAsync()`.
    -   `SaveInProgressState`: Performs a simple synchronous serialization and write of the `GameStateInProgress` object to `IN_PROGRESS_STATE_FILENAME`. Minimal protection may be applied if performance allows.
    -   Other methods will be straightforward implementations of their contracts.

### 3.3. File Repository

#### `IFileRepository.cs` & `LocalFileRepository.cs`

-   **Namespace**: `...Repositories`
-   **Purpose**: To abstract and implement direct file system I/O.
-   **`LocalFileRepository` Logic**:
    -   This class will use `System.IO.File` for its operations.
    -   All file paths will be constructed using `Path.Combine(Application.persistentDataPath, fileName)`. This ensures platform-agnostic pathing.
    -   Asynchronous methods (`ReadAllTextAsync`, `WriteAllTextAsync`) **must** use `Task.Run()` to wrap the synchronous `System.IO.File` calls to prevent blocking Unity's main thread.

### 3.4. Data Security

#### `IDataProtector.cs` & `SimpleXorDataProtector.cs`

-   **Namespace**: `...Security`
-   **Purpose**: To implement basic obfuscation and integrity checks on save data.
-   **`SimpleXorDataProtector` Logic**:
    -   **Private Key**: A `private readonly byte[] _secretKey` will be used for the XOR cipher. This key should not be a simple hardcoded string literal. It should be constructed at runtime from multiple parts or retrieved from a configuration that is not easily human-readable.
    -   **`Protect(string jsonData)`**:
        1.  Compute a SHA256 hash of `jsonData`.
        2.  Create a new string: `{hash}:{jsonData}`.
        3.  Apply a byte-by-byte XOR operation against the `_secretKey` (repeating the key as necessary).
        4.  Return the `Convert.ToBase64String()` of the resulting byte array.
    -   **`Unprotect(string protectedData)`**:
        1.  `Convert.FromBase64String(protectedData)` to get the XORed bytes.
        2.  Apply the same byte-by-byte XOR operation to reverse it.
        3.  Convert the result back to a string.
        4.  Split the string at the first `':'` character to separate the embedded hash from the original JSON data.
        5.  If splitting fails or results in fewer than 2 parts, throw `SaveDataCorruptionException`.
        6.  Re-compute the SHA256 hash of the extracted JSON data part.
        7.  Compare the re-computed hash with the embedded hash. If they do not match, throw `SaveDataCorruptionException`.
        8.  If they match, return the original JSON data string.

### 3.5. Data Migration

#### `IMigrationService.cs` & `MigrationService.cs`

-   **Namespace**: `...Migrations`
-   **Purpose**: To orchestrate the upgrade of old save data schemas to the current version.
-   **`MigrationService` Logic**:
    -   **Constructor**:
        1.  It will receive an `IEnumerable<IMigrationScript>` via dependency injection.
        2.  It will order this collection by `script.SourceVersion` and store it in a `private readonly` list or dictionary for fast lookup.
    -   **`MigrateToCurrentVersion(JObject saveData)`**:
        1.  Extract the `save_schema_version` from the `saveData` JObject. If not present, assume version 0 or 1.
        2.  Retrieve the `CURRENT_SCHEMA_VERSION` from a shared constant.
        3.  Loop from `currentVersion` up to `CURRENT_SCHEMA_VERSION - 1`.
        4.  In each iteration, find the `IMigrationScript` where `SourceVersion` matches the loop variable.
        5.  If a script is found, call its `Apply(saveData)` method.
        6.  If a script is not found for an intermediate version, this indicates a gap in the migration path. Log an error and throw an exception.
        7.  After the loop finishes, update the `save_schema_version` property in the `JObject` to `CURRENT_SCHEMA_VERSION`.
        8.  Return the modified `JObject`.

#### `IMigrationScript.cs` & `MigrationScript_V1_to_V2.cs`

-   **Namespace**: `...Migrations.Scripts`
-   **Purpose**: To define a single, atomic migration step.
-   **`IMigrationScript` Definition**:
    -   `int SourceVersion { get; }`: The schema version this script can upgrade *from*.
    -   `void Apply(JObject data)`: The method that performs the in-place modification of the JObject data.
-   **`MigrationScript_V1_to_V2` Example Logic**:
    -   `SourceVersion` will return `1`.
    -   `Apply(JObject data)` could, for example, rename a property:
        csharp
        // Example: Rename 'totalScore' to 'cumulativeScore'
        var scoreToken = data.SelectToken("playerStats.totalScore");
        if (scoreToken != null) {
            scoreToken.Parent.Add(new JProperty("cumulativeScore", scoreToken.Value));
            scoreToken.Parent.Remove();
        }
        

### 3.6. Supporting Components

#### `SaveDataWrapper.cs`

-   **Namespace**: `...Models`
-   **Purpose**: A simple data transfer object (DTO) that represents the on-disk file structure.
-   **Properties**:
    -   `public int SchemaVersion { get; set; }`: The schema version of the wrapped `Payload`.
    -   `public string Payload { get; set; }`: The Base64-encoded, XOR-obfuscated string containing the checksum and the serialized `PlayerProfile`.

#### `NewtonsoftJsonSerializer.cs`

-   **Namespace**: `...Serialization`
-   **Purpose**: A concrete implementation of `IJsonSerializer` using the `Newtonsoft.Json` library.
-   **Logic**: A thin wrapper around `JsonConvert.SerializeObject` and `JsonConvert.DeserializeObject<T>`.

#### `SaveDataCorruptionException.cs`

-   **Namespace**: `...Exceptions`
-   **Purpose**: A standard custom exception class inheriting from `System.Exception`. It will be thrown specifically when checksum validation fails during the `Unprotect` process, allowing for specific error handling in `PersistenceService`.

## 4. Data Flow Diagrams

### 4.1. Save Process

mermaid
sequenceDiagram
    participant App as Application Layer
    participant PS as PersistenceService
    participant S as IJsonSerializer
    participant DP as IDataProtector
    participant FR as IFileRepository

    App->>+PS: SavePlayerProfileAsync(playerProfile)
    PS->>+S: Serialize(playerProfile)
    S-->>-PS: playerProfileJson
    PS->>+DP: Protect(playerProfileJson)
    DP-->>-PS: protectedPayload
    PS->>PS: Create SaveDataWrapper
    PS->>+S: Serialize(saveDataWrapper)
    S-->>-PS: finalJson
    PS->>+FR: WriteAllTextAsync(path, finalJson)
    FR-->>-PS: Task
    PS-->>-App: Task


### 4.2. Load Process

mermaid
sequenceDiagram
    participant App as Application Layer
    participant PS as PersistenceService
    participant FR as IFileRepository
    participant S as IJsonSerializer
    participant DP as IDataProtector
    participant MS as IMigrationService

    App->>+PS: LoadPlayerProfileAsync()
    PS->>+FR: FileExists(path)
    FR-->>-PS: bool exists
    alt File does not exist
        PS-->>App: new PlayerProfile()
    else File exists
        PS->>+FR: ReadAllTextAsync(path)
        FR-->>-PS: finalJson
        PS->>+S: Deserialize<SaveDataWrapper>(finalJson)
        S-->>-PS: saveDataWrapper
        PS->>+DP: Unprotect(saveDataWrapper.Payload)
        alt Checksum Invalid
            DP-->>PS: throws SaveDataCorruptionException
            PS-->>App: new PlayerProfile()
        else Checksum Valid
            DP-->>-PS: playerProfileJson
            PS->>PS: Parse to JObject
            PS->>+MS: MigrateToCurrentVersion(jObject)
            MS-->>-PS: migratedJObject
            PS->>+S: Deserialize<PlayerProfile>(migratedJObject)
            S-->>-PS: playerProfile
            PS-->>-App: playerProfile
        end
    end

