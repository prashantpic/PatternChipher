# Software Design Specification: FirebaseBackendMaster (REPO-PATT-MASTER-001)

## 1. Introduction

This document provides the detailed software design specification for the `FirebaseBackendMaster` repository. This repository functions as a monorepo for the entire serverless backend of the Pattern Cipher game, built on the Google Firebase platform.

It is responsible for:
-   Housing the source code for all Cloud Functions.
-   Defining and enforcing security rules for the database.
-   Managing backend project configuration and deployment.
-   Providing shared utilities, types, and middleware for all backend services.

The primary technologies used are **TypeScript**, **Node.js**, **Firebase Cloud Functions**, **Cloud Firestore**, and **Firebase Authentication**.

## 2. Backend Architecture & Core Principles

The backend follows a **Serverless, Event-Driven Architecture**. The code within the `functions` directory is structured to promote separation of concerns, maintainability, and security.

-   **Monorepo Structure**: All Cloud Functions and shared code reside within the `functions/src` directory. This simplifies dependency management and ensures a single source of truth for backend code.
-   **Separation of Concerns**: Each feature (e.g., leaderboards, accounts) is a separate module with a clear structure:
    -   **Handler (`*.ts`)**: The Cloud Function trigger entry point (e.g., HTTPS Callable, Auth Trigger). It is responsible for orchestrating the request flow, including authentication and validation.
    -   **Validator (`validators.ts`)**: Defines the data contract for incoming requests using the **Zod** library for robust schema validation.
    -   **Service (`service.ts`)**: Encapsulates the core business logic, interacting with Firestore and other services. It is completely decoupled from the function trigger context.
-   **Security First**:
    -   Firestore access is denied by default and explicitly granted via `firestore.rules`.
    -   Sensitive endpoints are protected by authentication middleware.
    -   Secrets and API keys are managed via Firebase environment configuration, not hardcoded.
-   **Type Safety**: The entire backend is written in TypeScript, leveraging interfaces and types defined in a shared module to ensure data consistency.
-   **Testability**: The architecture is designed to be highly testable, with unit tests for services and validators, and integration tests using the **Firebase Emulator Suite**.

## 3. Root Firebase Configuration

These files reside at the root of the repository and govern the entire Firebase project deployment.

### 3.1. `firebase.json`

This file is the deployment manifest for Firebase services.

-   **Purpose**: To define which Firebase services to deploy and where their configuration/source code is located.
-   **Specification**:
    json
    {
      "firestore": {
        "rules": "firestore.rules",
        "indexes": "firestore.indexes.json"
      },
      "functions": [
        {
          "source": "functions",
          "codebase": "default",
          "ignore": [
            "node_modules",
            ".git",
            "firebase-debug.log",
            "firebase-debug.*.log"
          ],
          "predeploy": [
            "npm --prefix \"$RESOURCE_DIR\" run lint",
            "npm --prefix \"$RESOURCE_DIR\" run build"
          ]
        }
      ],
      "storage": {
        "rules": "storage.rules" // Placeholder for future use
      },
      "emulators": {
        "auth": { "port": 9099 },
        "functions": { "port": 5001 },
        "firestore": { "port": 8080 },
        "ui": { "enabled": true },
        "singleProjectMode": true
      }
    }
    

### 3.2. `.firebaserc`

This file maps local aliases to remote Firebase Project IDs.

-   **Purpose**: To facilitate easy switching between development, staging, and production environments.
-   **Specification**:
    json
    {
      "projects": {
        "default": "pattern-cipher-dev", // Example dev project ID
        "staging": "pattern-cipher-staging", // Example staging project ID
        "production": "pattern-cipher-prod" // Example production project ID
      }
    }
    

### 3.3. `firestore.rules`

This file defines the security model for the Cloud Firestore database, directly implementing **NFR-SEC-004**.

-   **Purpose**: To enforce the principle of least privilege, ensuring users and services can only access data they are authorized to.
-   **Specification**:
    javascript
    rules_version = '2';
    service cloud.firestore {
      match /databases/{database}/documents {
        
        // Deny all access by default
        match /{document=**} {
          allow read, write: if false;
        }

        // User profiles: Users can read their own profile and create it.
        // They can only update their own profile. No one can delete a user profile except via admin SDK.
        match /userProfiles/{userId} {
          allow read, update: if request.auth != null && request.auth.uid == userId;
          allow create: if request.auth != null;
        }

        // Leaderboards: Any authenticated user can read leaderboards.
        // Writes are only allowed from server-side code (Cloud Functions), not from clients.
        match /leaderboards/{leaderboardId} {
          allow read: if request.auth != null;
          allow write: if false; // Only allow writes from Admin SDK
        }

        // Achievements: Users can only read their own achievement status.
        // Writes are only allowed from the server to prevent cheating.
        match /userProfiles/{userId}/achievements/{achievementId} {
          allow read: if request.auth != null && request.auth.uid == userId;
          allow write: if false; // Only allow writes from Admin SDK
        }
      }
    }
    

### 3.4. `firestore.indexes.json`

This file defines composite indexes required for complex queries, such as filtered and sorted leaderboards.

-   **Purpose**: To ensure efficient database queries for features like leaderboards.
-   **Specification**:
    json
    {
      "indexes": [
        {
          "collectionGroup": "leaderboards",
          "queryScope": "COLLECTION",
          "fields": [
            { "fieldPath": "levelId", "order": "ASCENDING" },
            { "fieldPath": "score", "order": "DESCENDING" }
          ]
        },
        {
          "collectionGroup": "leaderboards",
          "queryScope": "COLLECTION",
          "fields": [
            { "fieldPath": "levelId", "order": "ASCENDING" },
            { "fieldPath": "moves", "order": "ASCENDING" }
          ]
        }
      ],
      "fieldOverrides": []
    }
    

## 4. Cloud Functions Project (`functions/`)

### 4.1. Build & Dependency Configuration

-   **`package.json`**:
    -   **dependencies**: `firebase-admin`, `firebase-functions`, `zod`.
    -   **devDependencies**: `@typescript-eslint/eslint-plugin`, `@typescript-eslint/parser`, `eslint`, `eslint-plugin-import`, `firebase-functions-test`, `jest`, `ts-jest`, `typescript`.
    -   **scripts**:
        -   `lint`: "eslint --ext .js,.ts ."
        -   `build`: "tsc"
        -   `serve`: "npm run build && firebase emulators:start --only functions,firestore,auth"
        -   `test`: "jest --watch"
        -   `deploy`: "firebase deploy --only functions"
-   **`tsconfig.json`**:
    -   `compilerOptions`: `module: "commonjs"`, `noImplicitReturns: true`, `outDir: "lib"`, `sourceMap: true`, `strict: true`, `target: "es2020"`.
    -   `compileOnSave`: `true`.
    -   `include`: `["src"]`.

### 4.2. Shared Modules (`functions/src/shared/`)

#### 4.2.1. Types (`shared/types/`)

These files define the data contracts for the entire backend, fulfilling **DM-002** and **NFR-M-001**.

-   **`player.ts`**:
    typescript
    import { firestore } from "firebase-admin";

    // Represents the player's cloud save data object
    export interface CloudSaveData {
      schemaVersion: number;
      levelProgress: { [levelId: string]: { score: number; stars: number; } };
      settings: {
        soundVolume: number;
        musicVolume: number;
        // ... other settings
      };
    }

    // Represents the user's main profile document in Firestore
    export interface UserProfile {
      displayName: string;
      photoURL?: string;
      lastSeen: firestore.Timestamp;
      createdAt: firestore.Timestamp;
      cloudSave?: CloudSaveData;
    }
    
-   **`leaderboard.ts`**:
    typescript
    import { firestore } from "firebase-admin";

    export interface LeaderboardEntry {
      playerId: string; // Corresponds to Firebase Auth UID
      displayName: string;
      score: number;
      levelId: string;
      timestamp: firestore.FieldValue; // Will be ServerValue.TIMESTAMP
      moves?: number;
      timeInSeconds?: number;
    }
    
-   **`index.ts` (Barrel file)**:
    typescript
    export * from "./player";
    export * from "./leaderboard";
    

#### 4.2.2. Middleware (`shared/middleware/`)

-   **`auth.ts`**: Provides reusable authentication checks.
    -   **Function**: `ensureAuthenticated(context: functions.https.CallableContext): string`
    -   **Logic**:
        1.  Check if `context.auth` and `context.auth.uid` are present.
        2.  If not, throw a new `functions.https.HttpsError("unauthenticated", "The function must be called while authenticated.")`.
        3.  If authenticated, return the `context.auth.uid`.

### 4.3. Feature: Leaderboards (`functions/src/leaderboards/`)

This module handles all leaderboard functionality.

-   **`validators.ts`**: Defines the validation schema for score submissions (**BR-LEAD-001**).
    typescript
    import { z } from "zod";

    export const submitScoreSchema = z.object({
      levelId: z.string().min(1),
      score: z.number().int().positive(),
      moves: z.number().int().positive().optional(),
      timeInSeconds: z.number().positive().optional(),
    });

    export type SubmitScorePayload = z.infer<typeof submitScoreSchema>;
    

-   **`submitScore.ts` (Handler)**: The HTTPS Callable function.
    -   **Function**: `export const submitScore = functions.https.onCall(async (data, context) => { ... });`
    -   **Logic**:
        1.  Invoke `ensureAuthenticated(context)` to get the `uid`.
        2.  Parse the incoming `data` using `submitScoreSchema.parse(data)`. Catch Zod errors and throw a `functions.https.HttpsError("invalid-argument", ...)`.
        3.  Instantiate `LeaderboardService`.
        4.  Call `leaderboardService.submitUserScore(uid, validatedData)`.
        5.  Return `{ success: true }` or handle errors from the service layer.
        6.  Implement rate limiting (e.g., using a Firestore-based rate limiter utility).

-   **`service.ts` (Service)**: The core business logic (**NFR-SEC-003**).
    -   **Class**: `LeaderboardService`
    -   **Constructor**: `constructor(private db: firestore.Firestore)`
    -   **Method**: `async submitUserScore(userId: string, scoreData: SubmitScorePayload): Promise<void>`
    -   **Logic**:
        1.  Fetch the user's profile from `/userProfiles/{userId}` to get their `displayName`.
        2.  Create a new `LeaderboardEntry` object.
        3.  Set the `timestamp` field to `firestore.FieldValue.serverTimestamp()`.
        4.  **Plausibility Check (BR-LEAD-001)**: Add a check here. E.g., query a (hypothetical) `levels` collection for the `maxPossibleScore` for the given `scoreData.levelId`. If `scoreData.score` exceeds it, throw an error.
        5.  Use a Firestore transaction to check if a higher score already exists for this `userId` and `levelId`.
        6.  If the new score is not higher, do nothing.
        7.  If it is a new high score (or no previous score exists), write the new `LeaderboardEntry` document to the `leaderboards` collection.

### 4.4. Feature: Account Management (`functions/src/accounts/`)

This module handles user data cleanup as required by **NFR-LC-002b**.

-   **`onUserDelete.ts` (Handler)**: The Auth-triggered function.
    -   **Function**: `export const cleanupUserData = functions.auth.user().onDelete(async (user) => { ... });`
    -   **Logic**:
        1.  Extract the `uid` from the `user` object.
        2.  Instantiate `AccountService`.
        3.  Call `accountService.deleteAllDataForUser(uid)`.
        4.  Log the result for auditing purposes.

-   **`service.ts` (Service)**: The core cleanup logic.
    -   **Class**: `AccountService`
    -   **Constructor**: `constructor(private db: firestore.Firestore)`
    -   **Method**: `async deleteAllDataForUser(userId: string): Promise<void>`
    -   **Logic**:
        1.  Create a Firestore batched write.
        2.  Add a delete operation for the user's profile document: `userProfiles/{userId}`.
        3.  Query the `leaderboards` collection for all entries where `playerId == userId`.
        4.  For each found leaderboard entry, add a delete operation to the batch.
        5.  Query any other collections containing user data (e.g., achievements) and add delete operations.
        6.  Commit the batched write.

### 4.5. Main Entry Point (`functions/src/index.ts`)

This file initializes the Admin SDK and exports all functions for deployment.

typescript
import * as admin from "firebase-admin";
import * as functions from "firebase-functions";

// Initialize Firebase Admin SDK
admin.initializeApp();

// Import and re-export functions from modules
import { submitScore } from "./leaderboards/submitScore";
import { cleanupUserData } from "./accounts/onUserDelete";

export const leaderboards = {
  submitScore,
};

export const accounts = {
  cleanupUserData,
};


## 5. Testing Strategy

-   **Unit Tests (Jest)**: Each `service.ts` and `validators.ts` file will have a corresponding `*.test.ts` file.
    -   Services will be tested by mocking the Firestore database interface.
    -   Validators will be tested with various valid and invalid data payloads.
-   **Integration Tests (Firebase Emulator Suite)**:
    -   Tests will be written to invoke the function handlers in the emulated environment.
    -   These tests will validate the end-to-end flow, including the interaction between functions, Firestore, and the enforcement of security rules.
    -   Scenarios to test include:
        -   Submitting a valid score as an authenticated user.
        -   Attempting to submit a score as an unauthenticated user (should fail).
        -   Attempting to submit a fraudulent/implausible score (should be rejected by server logic).
        -   Deleting a user from the Auth emulator and verifying their Firestore data is also deleted.