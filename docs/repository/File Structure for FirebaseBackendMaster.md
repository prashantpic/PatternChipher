# Specification

# 1. Files

- **Path:** firebase.json  
**Description:** Core Firebase project configuration. Defines deployment targets for Cloud Functions, Firestore (rules and indexes), and Cloud Storage. Specifies rewrite rules for hosting and other project-level settings. Essential for orchestrating deployments from the monorepo.  
**Template:** Firebase Configuration Template  
**Dependency Level:** 0  
**Name:** firebase  
**Type:** Configuration  
**Relative Path:** ../  
**Repository Id:** REPO-PATT-MASTER-001  
**Pattern Ids:**
    
    - Serverless
    - Monorepo
    
**Members:**
    
    
**Methods:**
    
    
**Implemented Features:**
    
    - Firebase Deployment Configuration
    
**Requirement Ids:**
    
    - 2.6.2
    
**Purpose:** To configure the deployment of all Firebase services, including functions, Firestore rules, and indexes, ensuring consistent environments.  
**Logic Description:** This file contains JSON configuration. It will have a 'functions' key specifying the source directory ('functions') and runtime (e.g., nodejs18). It will also have 'firestore' and 'storage' keys pointing to their respective rules files. This file orchestrates what gets deployed when running 'firebase deploy'.  
**Documentation:**
    
    - **Summary:** Defines the deployment manifest for the Firebase project, linking source code and configuration files to their respective Firebase services.
    
**Namespace:**   
**Metadata:**
    
    - **Category:** DeploymentConfiguration
    
- **Path:** .firebaserc  
**Description:** Firebase project alias configuration. Maps local aliases like 'default', 'dev', and 'prod' to specific Firebase Project IDs. This allows developers to easily switch between different deployment environments without changing other configuration files.  
**Template:** Firebase Configuration Template  
**Dependency Level:** 0  
**Name:** .firebaserc  
**Type:** Configuration  
**Relative Path:** ../  
**Repository Id:** REPO-PATT-MASTER-001  
**Pattern Ids:**
    
    - Serverless
    
**Members:**
    
    
**Methods:**
    
    
**Implemented Features:**
    
    - Firebase Environment Mapping
    
**Requirement Ids:**
    
    - 2.6.2
    
**Purpose:** To map human-readable environment names to specific Firebase project IDs, facilitating multi-environment deployments.  
**Logic Description:** A simple JSON file with a 'projects' object. Each key in the object is an alias (e.g., 'default', 'production') and its value is the corresponding Firebase project ID. Commands like 'firebase use production' will utilize this mapping.  
**Documentation:**
    
    - **Summary:** Contains the mapping between local project aliases and the actual Firebase project identifiers, crucial for multi-environment CI/CD workflows.
    
**Namespace:**   
**Metadata:**
    
    - **Category:** DeploymentConfiguration
    
- **Path:** firestore.rules  
**Description:** Defines the security rules for the Cloud Firestore database. This is a critical security file that implements the principle of least privilege, specifying who can read, write, update, and delete documents in various collections. Addresses requirement NFR-SEC-004 directly.  
**Template:** Firebase Rules Template  
**Dependency Level:** 0  
**Name:** firestore.rules  
**Type:** SecurityConfiguration  
**Relative Path:** ../  
**Repository Id:** REPO-PATT-MASTER-001  
**Pattern Ids:**
    
    - Serverless
    - Security
    
**Members:**
    
    
**Methods:**
    
    
**Implemented Features:**
    
    - Database Access Control
    
**Requirement Ids:**
    
    - NFR-SEC-004
    
**Purpose:** To secure the Cloud Firestore database by defining granular access control rules for all collections and documents.  
**Logic Description:** This file uses Firebase's security rule syntax. It will define rules for collections like 'userProfiles', 'leaderboards', and 'achievements'. For example, it will specify that a user can only write to their own 'userProfiles/{userId}' document. Leaderboard writes will be restricted to server-side logic (Cloud Functions), while reads might be public. Default access will be denied.  
**Documentation:**
    
    - **Summary:** Specifies the security and data validation rules for the Cloud Firestore database, ensuring data integrity and authorization.
    
**Namespace:**   
**Metadata:**
    
    - **Category:** Security
    
- **Path:** firestore.indexes.json  
**Description:** Defines the composite indexes required for complex queries in Cloud Firestore that are not automatically created. This is essential for features like filtering and sorting leaderboards by multiple fields.  
**Template:** Firebase Configuration Template  
**Dependency Level:** 0  
**Name:** firestore.indexes  
**Type:** Configuration  
**Relative Path:** ../  
**Repository Id:** REPO-PATT-MASTER-001  
**Pattern Ids:**
    
    - Serverless
    
**Members:**
    
    
**Methods:**
    
    
**Implemented Features:**
    
    - Database Indexing
    
**Requirement Ids:**
    
    - FR-ONL-001
    
**Purpose:** To define composite indexes needed for performing complex queries against the Firestore database, such as sorted and filtered leaderboards.  
**Logic Description:** A JSON file containing an array of 'indexes'. Each index object specifies the 'collectionGroup', query 'scope', and an array of 'fields' to index, including the order (ascending or descending) for each field. This will be used for leaderboard queries combining score and timestamp.  
**Documentation:**
    
    - **Summary:** A declarative configuration for creating composite indexes in Firestore, enabling efficient execution of complex queries that would otherwise be impossible or slow.
    
**Namespace:**   
**Metadata:**
    
    - **Category:** DatabaseConfiguration
    
- **Path:** functions/package.json  
**Description:** The Node.js package manifest for the Cloud Functions. It lists all dependencies (like 'firebase-functions', 'firebase-admin', 'jest'), development dependencies, and defines scripts for building, testing, linting, and deploying the functions.  
**Template:** Node.js Package Template  
**Dependency Level:** 0  
**Name:** package  
**Type:** Configuration  
**Relative Path:** functions/  
**Repository Id:** REPO-PATT-MASTER-001  
**Pattern Ids:**
    
    - Serverless
    
**Members:**
    
    
**Methods:**
    
    
**Implemented Features:**
    
    - Dependency Management
    - Build Scripts
    
**Requirement Ids:**
    
    - 2.6.2
    
**Purpose:** To manage all dependencies and define build/test scripts for the TypeScript-based Firebase Cloud Functions.  
**Logic Description:** This JSON file will include a 'dependencies' section for production packages like firebase-functions and firebase-admin. The 'devDependencies' section will include typescript, jest, eslint, and their related plugins. The 'scripts' section will define commands like 'build' (tsc), 'serve' (firebase emulators:start), 'test' (jest), and 'deploy' (firebase deploy --only functions).  
**Documentation:**
    
    - **Summary:** Defines project metadata and dependencies for the Cloud Functions codebase, enabling reproducible builds and development environments.
    
**Namespace:**   
**Metadata:**
    
    - **Category:** BuildConfiguration
    
- **Path:** functions/tsconfig.json  
**Description:** TypeScript compiler configuration for the Cloud Functions project. It specifies the target ECMAScript version, module system, source directory, output directory, and other compiler options to ensure type safety and correct JavaScript generation.  
**Template:** TypeScript Configuration Template  
**Dependency Level:** 0  
**Name:** tsconfig  
**Type:** Configuration  
**Relative Path:** functions/  
**Repository Id:** REPO-PATT-MASTER-001  
**Pattern Ids:**
    
    - Serverless
    
**Members:**
    
    
**Methods:**
    
    
**Implemented Features:**
    
    - TypeScript Compilation Settings
    
**Requirement Ids:**
    
    - 2.6.2
    
**Purpose:** To configure the TypeScript compiler options for the server-side functions, ensuring code correctness and compatibility.  
**Logic Description:** This file will set 'compilerOptions' such as 'target': 'es2020', 'module': 'commonjs', 'strict': true, 'sourceMap': true, 'outDir': 'lib', and 'rootDir': 'src'. It will also include the 'src' directory to be compiled.  
**Documentation:**
    
    - **Summary:** Provides configuration settings for the TypeScript compiler, defining how .ts files are transpiled into JavaScript for execution in the Node.js environment.
    
**Namespace:**   
**Metadata:**
    
    - **Category:** BuildConfiguration
    
- **Path:** functions/src/shared/types/index.ts  
**Description:** Barrel file for the shared types module. It exports all type definitions from this directory for easy and clean importing in other parts of the backend application.  
**Template:** TypeScript Barrel File Template  
**Dependency Level:** 1  
**Name:** index  
**Type:** Types  
**Relative Path:** functions/src/shared/types/  
**Repository Id:** REPO-PATT-MASTER-001  
**Pattern Ids:**
    
    
**Members:**
    
    
**Methods:**
    
    
**Implemented Features:**
    
    - Type Aggregation
    
**Requirement Ids:**
    
    - NFR-M-001
    
**Purpose:** To provide a single, consistent entry point for accessing all shared data types and interfaces within the backend.  
**Logic Description:** This file will consist of export statements. For example: 'export * from './player';', 'export * from './leaderboard';', 'export * from './common';'. This simplifies imports in service and handler files.  
**Documentation:**
    
    - **Summary:** Aggregates and re-exports all type definitions from the 'types' module, simplifying dependency management and improving code organization.
    
**Namespace:** PatternCipher.Backend.Shared.Types  
**Metadata:**
    
    - **Category:** DataModel
    
- **Path:** functions/src/shared/types/player.ts  
**Description:** Defines the TypeScript interfaces and types related to the player and their data, aligning with the backend data schema (DM-002). This includes UserProfile and CloudSaveData structures.  
**Template:** TypeScript Type Definition Template  
**Dependency Level:** 0  
**Name:** player  
**Type:** Types  
**Relative Path:** functions/src/shared/types/  
**Repository Id:** REPO-PATT-MASTER-001  
**Pattern Ids:**
    
    
**Members:**
    
    
**Methods:**
    
    
**Implemented Features:**
    
    - Player Data Models
    
**Requirement Ids:**
    
    - DM-002
    
**Purpose:** To provide strongly-typed data structures for player-related information, ensuring data consistency across the backend.  
**Logic Description:** This file will define interfaces such as 'UserProfile' with fields like 'displayName', 'photoURL', 'createdAt', and 'updatedAt'. It will also define the 'CloudSaveData' interface, matching the structure of the data to be synced, including fields like 'levelProgress', 'settings', and 'schemaVersion'.  
**Documentation:**
    
    - **Summary:** Contains TypeScript type definitions for player-centric data models, such as user profiles and cloud save objects.
    
**Namespace:** PatternCipher.Backend.Shared.Types  
**Metadata:**
    
    - **Category:** DataModel
    
- **Path:** functions/src/shared/types/leaderboard.ts  
**Description:** Defines the TypeScript interface for a single leaderboard entry, aligning with the backend data schema (DM-002). This ensures consistency when writing to and reading from the leaderboards collection in Firestore.  
**Template:** TypeScript Type Definition Template  
**Dependency Level:** 0  
**Name:** leaderboard  
**Type:** Types  
**Relative Path:** functions/src/shared/types/  
**Repository Id:** REPO-PATT-MASTER-001  
**Pattern Ids:**
    
    
**Members:**
    
    
**Methods:**
    
    
**Implemented Features:**
    
    - Leaderboard Data Model
    
**Requirement Ids:**
    
    - DM-002
    
**Purpose:** To provide a strongly-typed data structure for leaderboard entries, ensuring data integrity.  
**Logic Description:** This file will define the 'LeaderboardEntry' interface. It will contain fields such as 'playerId', 'displayName', 'score', 'levelId', 'timestamp' (which will be a Firebase ServerValue.TIMESTAMP), and potentially 'moves' or 'timeInSeconds'.  
**Documentation:**
    
    - **Summary:** Contains the TypeScript type definition for a leaderboard entry, modeling the data stored in the leaderboards collection.
    
**Namespace:** PatternCipher.Backend.Shared.Types  
**Metadata:**
    
    - **Category:** DataModel
    
- **Path:** functions/src/shared/utils/config.ts  
**Description:** A utility for managing and accessing environment configuration and secrets. It safely loads environment variables set in the Firebase environment, preventing secrets from being hardcoded.  
**Template:** TypeScript Utility Template  
**Dependency Level:** 0  
**Name:** config  
**Type:** Utility  
**Relative Path:** functions/src/shared/utils/  
**Repository Id:** REPO-PATT-MASTER-001  
**Pattern Ids:**
    
    
**Members:**
    
    
**Methods:**
    
    
**Implemented Features:**
    
    - Configuration Management
    
**Requirement Ids:**
    
    - NFR-SEC-005
    
**Purpose:** To provide a centralized and secure way to access environment variables and application configuration within Cloud Functions.  
**Logic Description:** This file will export a configuration object that is populated using 'functions.config()'. It will provide typed access to configuration values, for example, 'config.sentry.dsn' or 'config.some_api.key'. This ensures that secrets are managed through Firebase's environment configuration and not in the source code.  
**Documentation:**
    
    - **Summary:** Provides a typed, centralized interface for accessing environment variables and other configuration data for the backend services.
    
**Namespace:** PatternCipher.Backend.Shared.Utils  
**Metadata:**
    
    - **Category:** Configuration
    
- **Path:** functions/src/shared/middleware/auth.ts  
**Description:** Defines middleware for protecting HTTPS callable functions. It checks the request context to ensure a valid, authenticated user is making the call, throwing an 'unauthenticated' error if not. This enforces security on sensitive endpoints.  
**Template:** TypeScript Middleware Template  
**Dependency Level:** 1  
**Name:** auth  
**Type:** Middleware  
**Relative Path:** functions/src/shared/middleware/  
**Repository Id:** REPO-PATT-MASTER-001  
**Pattern Ids:**
    
    - MiddlewarePattern
    
**Members:**
    
    
**Methods:**
    
    - **Name:** ensureAuthenticated  
**Parameters:**
    
    - context: functions.https.CallableContext
    
**Return Type:** string  
**Attributes:** export const  
    
**Implemented Features:**
    
    - Authentication Check
    
**Requirement Ids:**
    
    - NFR-SEC-004
    
**Purpose:** To provide a reusable function that ensures an HTTPS callable function is invoked only by an authenticated user.  
**Logic Description:** The 'ensureAuthenticated' function will take the 'context' object from a callable function. It will check if 'context.auth' and 'context.auth.uid' exist. If they don't, it will throw an 'https.HttpsError' with the code 'unauthenticated'. If they do, it will return the UID for use in the main function logic.  
**Documentation:**
    
    - **Summary:** An authentication middleware for callable Cloud Functions that verifies the presence of a valid user token in the request context.
    
**Namespace:** PatternCipher.Backend.Shared.Middleware  
**Metadata:**
    
    - **Category:** Security
    
- **Path:** functions/src/leaderboards/submitScore.ts  
**Description:** The main HTTPS callable Cloud Function for submitting a player's score to a leaderboard. It acts as the secure entry point, orchestrating authentication checks, input validation, and calling the service layer to process the submission.  
**Template:** Firebase Function Template  
**Dependency Level:** 3  
**Name:** submitScore  
**Type:** FunctionHandler  
**Relative Path:** functions/src/leaderboards/  
**Repository Id:** REPO-PATT-MASTER-001  
**Pattern Ids:**
    
    - Serverless
    
**Members:**
    
    
**Methods:**
    
    - **Name:** submitScoreHandler  
**Parameters:**
    
    - data: any
    - context: functions.https.CallableContext
    
**Return Type:** Promise<any>  
**Attributes:** export const  
    
**Implemented Features:**
    
    - Leaderboard Score Submission
    
**Requirement Ids:**
    
    - FR-ONL-001
    - NFR-SEC-003
    
**Purpose:** To provide a secure, server-validated endpoint for clients to submit scores to the leaderboard.  
**Logic Description:** This file exports a Cloud Function using 'functions.https.onCall'. The handler function will first call 'ensureAuthenticated' middleware. Then, it will validate the input 'data' against the 'submitScoreValidator' schema. If validation passes, it will call the 'LeaderboardService.submit' method with the validated data and the user's UID. It will handle any errors thrown and return a success or error response.  
**Documentation:**
    
    - **Summary:** The HTTPS callable Cloud Function that handles incoming requests to submit a new score. It validates input and user authentication before passing the request to the leaderboard service.
    
**Namespace:** PatternCipher.Backend.Leaderboards  
**Metadata:**
    
    - **Category:** BusinessLogic
    
- **Path:** functions/src/leaderboards/service.ts  
**Description:** Contains the core business logic for the leaderboard system. This service is responsible for interacting with the Firestore database, applying game rules (e.g., only storing a user's best score), and constructing the data to be saved.  
**Template:** TypeScript Service Template  
**Dependency Level:** 2  
**Name:** service  
**Type:** Service  
**Relative Path:** functions/src/leaderboards/  
**Repository Id:** REPO-PATT-MASTER-001  
**Pattern Ids:**
    
    - ServiceLayer
    
**Members:**
    
    - **Name:** db  
**Type:** Firestore  
**Attributes:** private  
    
**Methods:**
    
    - **Name:** submit  
**Parameters:**
    
    - userId: string
    - scoreData: ValidatedScoreData
    
**Return Type:** Promise<void>  
**Attributes:** public async  
    
**Implemented Features:**
    
    - Leaderboard Business Logic
    
**Requirement Ids:**
    
    - FR-ONL-001
    - NFR-SEC-003
    
**Purpose:** To encapsulate the business logic for processing and persisting leaderboard scores.  
**Logic Description:** The 'LeaderboardService' class will be initialized with a Firestore instance. The 'submit' method will fetch the user's display name, potentially check their existing score for the given level, and then write a new 'LeaderboardEntry' document to the 'leaderboards' collection. It will use a Firestore transaction to ensure atomicity if it needs to read-then-write.  
**Documentation:**
    
    - **Summary:** A service class that contains the core logic for managing leaderboard data, including score submission and retrieval logic.
    
**Namespace:** PatternCipher.Backend.Leaderboards  
**Metadata:**
    
    - **Category:** BusinessLogic
    
- **Path:** functions/src/leaderboards/validators.ts  
**Description:** Defines the data validation schema for the 'submitScore' function payload. It uses a library like Zod or Joi to specify the expected shape, data types, and constraints (e.g., score must be a positive integer) of the incoming data, directly implementing BR-LEAD-001.  
**Template:** TypeScript Validator Template  
**Dependency Level:** 1  
**Name:** validators  
**Type:** Validator  
**Relative Path:** functions/src/leaderboards/  
**Repository Id:** REPO-PATT-MASTER-001  
**Pattern Ids:**
    
    - SpecificationPattern
    
**Members:**
    
    
**Methods:**
    
    
**Implemented Features:**
    
    - Score Submission Validation
    
**Requirement Ids:**
    
    - NFR-SEC-003
    - BR-LEAD-001
    
**Purpose:** To define and enforce the contract for data submitted to the leaderboard endpoints.  
**Logic Description:** This file will export a schema object, for instance, 'submitScoreSchema'. Using a library like Zod, it will define that the payload must be an object with properties like 'levelId' (string, non-empty), 'score' (number, integer, greater than 0), 'moves' (number, integer, optional), etc. This schema is then used by the handler to parse and validate the request data.  
**Documentation:**
    
    - **Summary:** Contains data validation schemas for leaderboard-related operations, ensuring incoming data conforms to the expected structure and constraints.
    
**Namespace:** PatternCipher.Backend.Leaderboards  
**Metadata:**
    
    - **Category:** Validation
    
- **Path:** functions/src/accounts/onUserDelete.ts  
**Description:** An Auth-triggered Cloud Function that automatically runs when a user account is deleted from Firebase Authentication. Its purpose is to clean up all associated user data from other Firebase services (like Firestore and Storage) to comply with data privacy regulations (GDPR's 'right to be forgotten').  
**Template:** Firebase Function Template  
**Dependency Level:** 3  
**Name:** onUserDelete  
**Type:** FunctionHandler  
**Relative Path:** functions/src/accounts/  
**Repository Id:** REPO-PATT-MASTER-001  
**Pattern Ids:**
    
    - Serverless
    - EventDriven
    
**Members:**
    
    
**Methods:**
    
    - **Name:** cleanupUserAccount  
**Parameters:**
    
    - user: admin.auth.UserRecord
    
**Return Type:** Promise<any>  
**Attributes:** export const  
    
**Implemented Features:**
    
    - User Data Deletion
    
**Requirement Ids:**
    
    - NFR-LC-002b
    
**Purpose:** To automate the deletion of a user's data across all backend services when their authentication record is removed.  
**Logic Description:** This file exports a Cloud Function using 'functions.auth.user().onDelete()'. The handler receives the deleted user record. It will then call the 'AccountService.deleteAllUserData' method, passing the user's UID. This ensures that when a user is deleted, their corresponding profile, save games, and other records in Firestore are also programmatically and atomically deleted.  
**Documentation:**
    
    - **Summary:** An authentication-triggered Cloud Function that performs cleanup of user-related data in Firestore after a user is deleted from Firebase Authentication.
    
**Namespace:** PatternCipher.Backend.Accounts  
**Metadata:**
    
    - **Category:** BusinessLogic
    
- **Path:** functions/src/accounts/service.ts  
**Description:** Contains the business logic for user account management, specifically the process of deleting all data related to a given user ID. It encapsulates the queries and delete operations across multiple Firestore collections.  
**Template:** TypeScript Service Template  
**Dependency Level:** 2  
**Name:** service  
**Type:** Service  
**Relative Path:** functions/src/accounts/  
**Repository Id:** REPO-PATT-MASTER-001  
**Pattern Ids:**
    
    - ServiceLayer
    
**Members:**
    
    - **Name:** db  
**Type:** Firestore  
**Attributes:** private  
    
**Methods:**
    
    - **Name:** deleteAllUserData  
**Parameters:**
    
    - userId: string
    
**Return Type:** Promise<void>  
**Attributes:** public async  
    
**Implemented Features:**
    
    - User Data Cleanup Logic
    
**Requirement Ids:**
    
    - NFR-LC-002b
    
**Purpose:** To encapsulate the logic for completely removing a user's data from the backend systems.  
**Logic Description:** The 'AccountService' class's 'deleteAllUserData' method will take a 'userId'. It will then perform batched delete operations on Firestore. This includes deleting the document in the 'userProfiles' collection with the matching ID, and querying and deleting any documents in other collections (like 'leaderboards') where the 'playerId' matches the given 'userId'.  
**Documentation:**
    
    - **Summary:** A service class containing the core logic for managing user accounts, including data deletion procedures required for privacy compliance.
    
**Namespace:** PatternCipher.Backend.Accounts  
**Metadata:**
    
    - **Category:** BusinessLogic
    
- **Path:** functions/src/index.ts  
**Description:** The main entry point for all Cloud Functions. This file imports the function handlers from the various modules (leaderboards, accounts, etc.) and exports them under a single object. Firebase uses this file to discover and deploy all backend functions.  
**Template:** Firebase Index Template  
**Dependency Level:** 4  
**Name:** index  
**Type:** Main  
**Relative Path:** functions/src/  
**Repository Id:** REPO-PATT-MASTER-001  
**Pattern Ids:**
    
    - Serverless
    
**Members:**
    
    
**Methods:**
    
    
**Implemented Features:**
    
    - Function Aggregation
    
**Requirement Ids:**
    
    - 2.6.2
    
**Purpose:** To act as the root entry point for deploying all Cloud Functions within the project.  
**Logic Description:** This file will initialize the Firebase Admin SDK using 'admin.initializeApp()'. It will then import functions from other modules, like 'import { submitScoreHandler } from './leaderboards/submitScore';'. Finally, it will export them, potentially grouping them, e.g., 'export const leaderboards = { submitScore: submitScoreHandler };'.  
**Documentation:**
    
    - **Summary:** The primary entry point for the Firebase Cloud Functions deployment. It aggregates and exports all defined function triggers.
    
**Namespace:** PatternCipher.Backend  
**Metadata:**
    
    - **Category:** Application
    


---

# 2. Configuration

- **Feature Toggles:**
  
  - enableLeaderboardSubmission
  - enableAccountDeletionFunction
  
- **Database Configs:**
  
  - FIRESTORE_PROJECT_ID
  - FIRESTORE_EMULATOR_HOST
  


---

