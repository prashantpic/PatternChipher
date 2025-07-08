# Specification

# 1. Files

- **Path:** src/PatternCipher.Infrastructure.Persistence/PatternCipher.Infrastructure.Persistence.csproj  
**Description:** The .NET project file for the persistence infrastructure layer. It defines dependencies on the shared kernel project (PatternCipher.Shared), Newtonsoft.Json for serialization, and other necessary .NET libraries.  
**Template:** C# Project File  
**Dependency Level:** 0  
**Name:** PatternCipher.Infrastructure.Persistence  
**Type:** Project  
**Relative Path:** .  
**Repository Id:** REPO-PATT-004  
**Pattern Ids:**
    
    
**Members:**
    
    
**Methods:**
    
    
**Implemented Features:**
    
    - Project Definition
    
**Requirement Ids:**
    
    
**Purpose:** Defines the project settings, dependencies, and framework targeting for the persistence library.  
**Logic Description:** This file will be configured to target .NET Standard 2.1 to ensure compatibility with Unity. It will list PackageReferences for Newtonsoft.Json and a ProjectReference to the SharedKernel project.  
**Documentation:**
    
    - **Summary:** Specifies the project build information and dependencies for the persistence layer, including third-party libraries like Newtonsoft.Json and internal dependencies like the SharedKernel.
    
**Namespace:**   
**Metadata:**
    
    - **Category:** Configuration
    
- **Path:** src/PatternCipher.Infrastructure.Persistence/Interfaces/IPersistenceService.cs  
**Description:** Defines the public contract for the persistence service. This is the primary interface the Application layer will use to save and load all player data, abstracting away the underlying implementation details of file I/O, serialization, and security.  
**Template:** C# Interface  
**Dependency Level:** 0  
**Name:** IPersistenceService  
**Type:** Interface  
**Relative Path:** Interfaces  
**Repository Id:** REPO-PATT-004  
**Pattern Ids:**
    
    - FacadePattern
    - DependencyInversion
    
**Members:**
    
    
**Methods:**
    
    - **Name:** LoadPlayerProfileAsync  
**Parameters:**
    
    
**Return Type:** Task<PlayerProfile>  
**Attributes:** public  
    - **Name:** SavePlayerProfileAsync  
**Parameters:**
    
    - PlayerProfile playerProfile
    
**Return Type:** Task  
**Attributes:** public  
    - **Name:** HasPlayerProfile  
**Parameters:**
    
    
**Return Type:** bool  
**Attributes:** public  
    - **Name:** SaveInProgressState  
**Parameters:**
    
    - GameStateInProgress state
    
**Return Type:** void  
**Attributes:** public  
    - **Name:** LoadInProgressState  
**Parameters:**
    
    
**Return Type:** GameStateInProgress  
**Attributes:** public  
    - **Name:** ClearInProgressState  
**Parameters:**
    
    
**Return Type:** void  
**Attributes:** public  
    - **Name:** HasInProgressState  
**Parameters:**
    
    
**Return Type:** bool  
**Attributes:** public  
    
**Implemented Features:**
    
    - Player Profile Persistence Contract
    - In-Progress State Persistence Contract
    
**Requirement Ids:**
    
    - NFR-R-002
    - NFR-R-004
    
**Purpose:** Provides a high-level abstraction for all data persistence operations, decoupling the application core from the persistence mechanism.  
**Logic Description:** This interface will define methods for loading and saving the main player profile asynchronously, and synchronous methods for handling the in-progress game state for interruption recovery.  
**Documentation:**
    
    - **Summary:** Defines the contract for the primary persistence service, which manages loading and saving the complete player profile and the temporary state for crash/interruption recovery.
    
**Namespace:** PatternCipher.Infrastructure.Persistence.Interfaces  
**Metadata:**
    
    - **Category:** Interface
    
- **Path:** src/PatternCipher.Infrastructure.Persistence/PersistenceService.cs  
**Description:** The concrete implementation of IPersistenceService. It orchestrates the entire save/load process, coordinating the data protector, serializer, and repository to handle player data securely and reliably.  
**Template:** C# Service Implementation  
**Dependency Level:** 2  
**Name:** PersistenceService  
**Type:** Service  
**Relative Path:** .  
**Repository Id:** REPO-PATT-004  
**Pattern Ids:**
    
    - FacadePattern
    
**Members:**
    
    - **Name:** _repository  
**Type:** IFileRepository  
**Attributes:** private|readonly  
    - **Name:** _dataProtector  
**Type:** IDataProtector  
**Attributes:** private|readonly  
    - **Name:** _serializer  
**Type:** IJsonSerializer  
**Attributes:** private|readonly  
    - **Name:** _migrationService  
**Type:** IMigrationService  
**Attributes:** private|readonly  
    
**Methods:**
    
    - **Name:** LoadPlayerProfileAsync  
**Parameters:**
    
    
**Return Type:** Task<PlayerProfile>  
**Attributes:** public|async  
    - **Name:** SavePlayerProfileAsync  
**Parameters:**
    
    - PlayerProfile playerProfile
    
**Return Type:** Task  
**Attributes:** public|async  
    
**Implemented Features:**
    
    - Player Profile Persistence Logic
    - In-Progress State Persistence Logic
    - Save/Load Orchestration
    
**Requirement Ids:**
    
    - NFR-R-002
    - NFR-R-004
    - NFR-SEC-001
    - DM-003
    - DM-004
    
**Purpose:** Implements the core logic for saving and loading player data, acting as a facade over the more granular persistence components.  
**Logic Description:** For loading, it reads the raw file via the repository, unprotects the data (which validates the checksum), runs it through the migration service, and finally deserializes it. For saving, it serializes the profile, wraps it with metadata, protects it (creating a checksum and obfuscating), and writes it via the repository. It handles exceptions like data corruption or migration failure.  
**Documentation:**
    
    - **Summary:** Orchestrates the process of loading, migrating, and saving player data. Implements IPersistenceService by coordinating the file repository, data protector, serializer, and migration services.
    
**Namespace:** PatternCipher.Infrastructure.Persistence  
**Metadata:**
    
    - **Category:** Service
    
- **Path:** src/PatternCipher.Infrastructure.Persistence/Repositories/IFileRepository.cs  
**Description:** An interface for abstracting the raw file I/O operations, making the PersistenceService independent of the specific file system API.  
**Template:** C# Interface  
**Dependency Level:** 1  
**Name:** IFileRepository  
**Type:** Interface  
**Relative Path:** Repositories  
**Repository Id:** REPO-PATT-004  
**Pattern Ids:**
    
    - RepositoryPattern
    
**Members:**
    
    
**Methods:**
    
    - **Name:** ReadAllTextAsync  
**Parameters:**
    
    - string filePath
    
**Return Type:** Task<string>  
**Attributes:** public  
    - **Name:** WriteAllTextAsync  
**Parameters:**
    
    - string filePath
    - string content
    
**Return Type:** Task  
**Attributes:** public  
    - **Name:** FileExists  
**Parameters:**
    
    - string filePath
    
**Return Type:** bool  
**Attributes:** public  
    - **Name:** DeleteFile  
**Parameters:**
    
    - string filePath
    
**Return Type:** void  
**Attributes:** public  
    
**Implemented Features:**
    
    - File I/O Abstraction
    
**Requirement Ids:**
    
    
**Purpose:** Defines a contract for reading from and writing to the local file system.  
**Logic Description:** Specifies methods for asynchronous file read/write operations and synchronous file existence checks.  
**Documentation:**
    
    - **Summary:** Provides an abstraction over direct file system access, enabling easier testing and potential platform-specific implementations.
    
**Namespace:** PatternCipher.Infrastructure.Persistence.Repositories  
**Metadata:**
    
    - **Category:** DataAccess
    
- **Path:** src/PatternCipher.Infrastructure.Persistence/Repositories/LocalFileRepository.cs  
**Description:** The concrete implementation of IFileRepository using standard .NET file system APIs and Unity's Application.persistentDataPath.  
**Template:** C# Repository Implementation  
**Dependency Level:** 2  
**Name:** LocalFileRepository  
**Type:** Repository  
**Relative Path:** Repositories  
**Repository Id:** REPO-PATT-004  
**Pattern Ids:**
    
    - RepositoryPattern
    
**Members:**
    
    
**Methods:**
    
    - **Name:** ReadAllTextAsync  
**Parameters:**
    
    - string filePath
    
**Return Type:** Task<string>  
**Attributes:** public|async  
    - **Name:** WriteAllTextAsync  
**Parameters:**
    
    - string filePath
    - string content
    
**Return Type:** Task  
**Attributes:** public|async  
    
**Implemented Features:**
    
    - Local File I/O
    
**Requirement Ids:**
    
    - NFR-R-002
    
**Purpose:** Handles the actual reading and writing of string data to files on the device's persistent storage.  
**Logic Description:** Implements the IFileRepository interface. It will construct full file paths using Unity's Application.persistentDataPath and use System.IO.File methods for the read/write operations within a Task.Run to ensure they are off the main thread.  
**Documentation:**
    
    - **Summary:** A repository implementation that interacts directly with the local file system to persist and retrieve string-based data, such as serialized JSON.
    
**Namespace:** PatternCipher.Infrastructure.Persistence.Repositories  
**Metadata:**
    
    - **Category:** DataAccess
    
- **Path:** src/PatternCipher.Infrastructure.Persistence/Models/SaveDataWrapper.cs  
**Description:** A data model that wraps the actual player profile data with persistence-specific metadata, such as the schema version and a checksum for integrity.  
**Template:** C# POCO  
**Dependency Level:** 1  
**Name:** SaveDataWrapper  
**Type:** Model  
**Relative Path:** Models  
**Repository Id:** REPO-PATT-004  
**Pattern Ids:**
    
    
**Members:**
    
    - **Name:** SchemaVersion  
**Type:** int  
**Attributes:** public  
    - **Name:** Checksum  
**Type:** string  
**Attributes:** public  
    - **Name:** Payload  
**Type:** string  
**Attributes:** public  
    
**Methods:**
    
    
**Implemented Features:**
    
    - Save Data Encapsulation
    
**Requirement Ids:**
    
    - DM-003
    - NFR-SEC-001
    
**Purpose:** Encapsulates the core save data with necessary metadata for versioning and integrity checks.  
**Logic Description:** This is a Plain Old C# Object (POCO). The 'Payload' property will contain the obfuscated, serialized player profile JSON. This entire wrapper object is what gets serialized to a final JSON file and written to disk.  
**Documentation:**
    
    - **Summary:** Represents the structure of the save file on disk. It holds the schema version, a checksum for the payload, and the encrypted/obfuscated data payload itself.
    
**Namespace:** PatternCipher.Infrastructure.Persistence.Models  
**Metadata:**
    
    - **Category:** DataAccess
    
- **Path:** src/PatternCipher.Infrastructure.Persistence/Security/IDataProtector.cs  
**Description:** Defines the contract for protecting and unprotecting save data. This abstracts the specific methods of obfuscation and checksum validation.  
**Template:** C# Interface  
**Dependency Level:** 1  
**Name:** IDataProtector  
**Type:** Interface  
**Relative Path:** Security  
**Repository Id:** REPO-PATT-004  
**Pattern Ids:**
    
    
**Members:**
    
    
**Methods:**
    
    - **Name:** Protect  
**Parameters:**
    
    - string jsonData
    
**Return Type:** string  
**Attributes:** public  
    - **Name:** Unprotect  
**Parameters:**
    
    - string protectedData
    
**Return Type:** string  
**Attributes:** public  
    
**Implemented Features:**
    
    - Data Protection Contract
    
**Requirement Ids:**
    
    - NFR-SEC-001
    
**Purpose:** Abstracts the logic for applying and verifying security measures on save data.  
**Logic Description:** The Protect method takes clean JSON, computes a checksum, and obfuscates the result. The Unprotect method takes protected data, de-obfuscates it, verifies the checksum, and throws an exception if invalid, otherwise returns the clean JSON.  
**Documentation:**
    
    - **Summary:** Defines the interface for services that handle the security aspects of save data, including obfuscation/encryption and integrity validation via checksums.
    
**Namespace:** PatternCipher.Infrastructure.Persistence.Security  
**Metadata:**
    
    - **Category:** Security
    
- **Path:** src/PatternCipher.Infrastructure.Persistence/Security/SimpleXorDataProtector.cs  
**Description:** A simple implementation of IDataProtector using a basic XOR cipher for obfuscation and a SHA256 hash for checksum validation.  
**Template:** C# Service Implementation  
**Dependency Level:** 2  
**Name:** SimpleXorDataProtector  
**Type:** Service  
**Relative Path:** Security  
**Repository Id:** REPO-PATT-004  
**Pattern Ids:**
    
    
**Members:**
    
    - **Name:** _secretKey  
**Type:** byte[]  
**Attributes:** private|readonly  
    
**Methods:**
    
    - **Name:** Protect  
**Parameters:**
    
    - string jsonData
    
**Return Type:** string  
**Attributes:** public  
    - **Name:** Unprotect  
**Parameters:**
    
    - string protectedData
    
**Return Type:** string  
**Attributes:** public  
    - **Name:** ComputeHash  
**Parameters:**
    
    - string data
    
**Return Type:** string  
**Attributes:** private  
    - **Name:** XorCipher  
**Parameters:**
    
    - string data
    
**Return Type:** string  
**Attributes:** private  
    
**Implemented Features:**
    
    - Data Obfuscation
    - Checksum Validation
    
**Requirement Ids:**
    
    - NFR-SEC-001
    
**Purpose:** Implements basic security measures to deter casual tampering with local save files.  
**Logic Description:** The Protect method first calculates a SHA256 hash of the input JSON, prepends it to the JSON, then applies a simple XOR cipher to the combined string, and finally Base64 encodes it. The Unprotect method reverses this process, throwing a SaveDataCorruptionException if the re-calculated hash does not match the embedded hash.  
**Documentation:**
    
    - **Summary:** Provides a concrete implementation for save data protection using SHA256 for integrity and a simple XOR cipher for obfuscation. This is not for high security but to prevent casual editing.
    
**Namespace:** PatternCipher.Infrastructure.Persistence.Security  
**Metadata:**
    
    - **Category:** Security
    
- **Path:** src/PatternCipher.Infrastructure.Persistence/Migrations/IMigrationService.cs  
**Description:** Defines the contract for the service responsible for migrating save data from older schema versions to the current one.  
**Template:** C# Interface  
**Dependency Level:** 1  
**Name:** IMigrationService  
**Type:** Interface  
**Relative Path:** Migrations  
**Repository Id:** REPO-PATT-004  
**Pattern Ids:**
    
    
**Members:**
    
    
**Methods:**
    
    - **Name:** MigrateToCurrentVersion  
**Parameters:**
    
    - JObject saveData
    
**Return Type:** JObject  
**Attributes:** public  
    
**Implemented Features:**
    
    - Data Migration Contract
    
**Requirement Ids:**
    
    - DM-004
    
**Purpose:** Abstracts the data migration process, allowing for a structured and extensible approach to handling save file updates.  
**Logic Description:** Defines a single method that takes a JSON representation of save data, checks its version, and applies necessary transformations to bring it up to the latest version.  
**Documentation:**
    
    - **Summary:** Provides the contract for a service that manages the step-by-step migration of save data across different schema versions.
    
**Namespace:** PatternCipher.Infrastructure.Persistence.Migrations  
**Metadata:**
    
    - **Category:** DataAccess
    
- **Path:** src/PatternCipher.Infrastructure.Persistence/Migrations/MigrationService.cs  
**Description:** The concrete implementation of IMigrationService. It discovers all available migration scripts and applies them sequentially to upgrade save data.  
**Template:** C# Service Implementation  
**Dependency Level:** 2  
**Name:** MigrationService  
**Type:** Service  
**Relative Path:** Migrations  
**Repository Id:** REPO-PATT-004  
**Pattern Ids:**
    
    - StrategyPattern
    
**Members:**
    
    - **Name:** _migrationScripts  
**Type:** IReadOnlyList<IMigrationScript>  
**Attributes:** private|readonly  
    
**Methods:**
    
    - **Name:** MigrateToCurrentVersion  
**Parameters:**
    
    - JObject saveData
    
**Return Type:** JObject  
**Attributes:** public  
    
**Implemented Features:**
    
    - Save Data Migration Logic
    
**Requirement Ids:**
    
    - DM-004
    
**Purpose:** Orchestrates the data migration process by applying a chain of version-specific migration scripts.  
**Logic Description:** This service is initialized with a collection of all IMigrationScript implementations. The `MigrateToCurrentVersion` method reads the `save_schema_version` from the JObject. It then filters and applies all relevant migration scripts in ascending order of their target version until the data reaches the current application schema version.  
**Documentation:**
    
    - **Summary:** Manages and executes the data migration process. It identifies the version of the loaded save data and applies a sequence of ordered migration scripts to bring it up to the current version.
    
**Namespace:** PatternCipher.Infrastructure.Persistence.Migrations  
**Metadata:**
    
    - **Category:** DataAccess
    
- **Path:** src/PatternCipher.Infrastructure.Persistence/Migrations/Scripts/IMigrationScript.cs  
**Description:** An interface representing a single, atomic migration step from one schema version to the next.  
**Template:** C# Interface  
**Dependency Level:** 1  
**Name:** IMigrationScript  
**Type:** Interface  
**Relative Path:** Migrations/Scripts  
**Repository Id:** REPO-PATT-004  
**Pattern Ids:**
    
    - StrategyPattern
    
**Members:**
    
    - **Name:** SourceVersion  
**Type:** int  
**Attributes:** public|property  
    
**Methods:**
    
    - **Name:** Apply  
**Parameters:**
    
    - JObject data
    
**Return Type:** void  
**Attributes:** public  
    
**Implemented Features:**
    
    - Migration Step Contract
    
**Requirement Ids:**
    
    - DM-004
    
**Purpose:** Defines the contract for a single, targeted migration script, making the migration process modular and testable.  
**Logic Description:** Each implementation will know its source version and contain the logic to transform a JObject from that version to the next. For example, a script with SourceVersion = 1 knows how to upgrade a v1 save to a v2 save.  
**Documentation:**
    
    - **Summary:** Represents a single migration operation, defining the source version it applies to and the transformation logic to upgrade the data.
    
**Namespace:** PatternCipher.Infrastructure.Persistence.Migrations.Scripts  
**Metadata:**
    
    - **Category:** DataAccess
    
- **Path:** src/PatternCipher.Infrastructure.Persistence/Migrations/Scripts/MigrationScript_V1_to_V2.cs  
**Description:** An example implementation of IMigrationScript that handles the migration from schema version 1 to version 2.  
**Template:** C# Class  
**Dependency Level:** 2  
**Name:** MigrationScript_V1_to_V2  
**Type:** MigrationScript  
**Relative Path:** Migrations/Scripts  
**Repository Id:** REPO-PATT-004  
**Pattern Ids:**
    
    - StrategyPattern
    
**Members:**
    
    - **Name:** SourceVersion  
**Type:** int  
**Attributes:** public|property  
    
**Methods:**
    
    - **Name:** Apply  
**Parameters:**
    
    - JObject data
    
**Return Type:** void  
**Attributes:** public  
    
**Implemented Features:**
    
    - V1 to V2 Migration
    
**Requirement Ids:**
    
    - DM-004
    
**Purpose:** Contains the specific logic for upgrading a version 1 save file to version 2.  
**Logic Description:** This class will implement the IMigrationScript interface. Its `SourceVersion` property will return 1. The `Apply` method will contain logic to manipulate the passed-in JObject, for example, by renaming a field, adding a new field with a default value, or restructuring a nested object.  
**Documentation:**
    
    - **Summary:** A concrete migration script that transforms a save data object from schema version 1 to schema version 2.
    
**Namespace:** PatternCipher.Infrastructure.Persistence.Migrations.Scripts  
**Metadata:**
    
    - **Category:** DataAccess
    
- **Path:** src/PatternCipher.Infrastructure.Persistence/Exceptions/SaveDataCorruptionException.cs  
**Description:** Custom exception thrown by the data protector when a save file's checksum does not match its content, indicating tampering or corruption.  
**Template:** C# Exception  
**Dependency Level:** 1  
**Name:** SaveDataCorruptionException  
**Type:** Exception  
**Relative Path:** Exceptions  
**Repository Id:** REPO-PATT-004  
**Pattern Ids:**
    
    
**Members:**
    
    
**Methods:**
    
    - **Name:** SaveDataCorruptionException  
**Parameters:**
    
    - string message
    - Exception innerException
    
**Return Type:**   
**Attributes:** public  
    
**Implemented Features:**
    
    - Custom Error Handling
    
**Requirement Ids:**
    
    - NFR-SEC-001
    - DM-004
    
**Purpose:** Provides a specific exception type for handling cases of data integrity failure during the load process.  
**Logic Description:** A standard custom exception class inheriting from `System.Exception` to allow for specific catch blocks in the `PersistenceService`.  
**Documentation:**
    
    - **Summary:** A custom exception used to signal that locally persisted data has failed an integrity check (e.g., checksum mismatch).
    
**Namespace:** PatternCipher.Infrastructure.Persistence.Exceptions  
**Metadata:**
    
    - **Category:** ErrorHandling
    
- **Path:** src/PatternCipher.Infrastructure.Persistence/Serialization/IJsonSerializer.cs  
**Description:** An interface abstracting the JSON serialization/deserialization functionality.  
**Template:** C# Interface  
**Dependency Level:** 1  
**Name:** IJsonSerializer  
**Type:** Interface  
**Relative Path:** Serialization  
**Repository Id:** REPO-PATT-004  
**Pattern Ids:**
    
    
**Members:**
    
    
**Methods:**
    
    - **Name:** Serialize  
**Parameters:**
    
    - object obj
    
**Return Type:** string  
**Attributes:** public  
    - **Name:** Deserialize<T>  
**Parameters:**
    
    - string json
    
**Return Type:** T  
**Attributes:** public  
    
**Implemented Features:**
    
    - Serialization Abstraction
    
**Requirement Ids:**
    
    - NFR-R-002
    
**Purpose:** Decouples the persistence logic from a specific JSON library.  
**Logic Description:** Defines generic methods for serializing an object to a JSON string and deserializing a JSON string to a specific type.  
**Documentation:**
    
    - **Summary:** Provides a contract for JSON serialization and deserialization, allowing the underlying library (e.g., Newtonsoft.Json) to be swapped out if needed.
    
**Namespace:** PatternCipher.Infrastructure.Persistence.Serialization  
**Metadata:**
    
    - **Category:** Utility
    
- **Path:** src/PatternCipher.Infrastructure.Persistence/Serialization/NewtonsoftJsonSerializer.cs  
**Description:** A concrete implementation of IJsonSerializer using the Newtonsoft.Json library.  
**Template:** C# Service Implementation  
**Dependency Level:** 2  
**Name:** NewtonsoftJsonSerializer  
**Type:** Service  
**Relative Path:** Serialization  
**Repository Id:** REPO-PATT-004  
**Pattern Ids:**
    
    
**Members:**
    
    
**Methods:**
    
    - **Name:** Serialize  
**Parameters:**
    
    - object obj
    
**Return Type:** string  
**Attributes:** public  
    - **Name:** Deserialize<T>  
**Parameters:**
    
    - string json
    
**Return Type:** T  
**Attributes:** public  
    
**Implemented Features:**
    
    - JSON Serialization
    
**Requirement Ids:**
    
    - NFR-R-002
    
**Purpose:** Provides JSON serialization and deserialization using the Newtonsoft.Json library.  
**Logic Description:** Implements the IJsonSerializer interface by calling the corresponding static methods `JsonConvert.SerializeObject` and `JsonConvert.DeserializeObject<T>` from the Newtonsoft.Json library.  
**Documentation:**
    
    - **Summary:** A concrete serializer implementation that leverages the powerful and widely-used Newtonsoft.Json library for object-to-JSON conversion.
    
**Namespace:** PatternCipher.Infrastructure.Persistence.Serialization  
**Metadata:**
    
    - **Category:** Utility
    


---

# 2. Configuration

- **Feature Toggles:**
  
  
- **Database Configs:**
  
  


---

