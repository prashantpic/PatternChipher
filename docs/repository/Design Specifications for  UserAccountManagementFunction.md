# Software Design Specification (SDS) for UserAccountManagementFunction (REPO-PATT-011)

## 1. Introduction

This document provides the detailed software design specification for the `UserAccountManagementFunction` repository. This repository contains a serverless Firebase Cloud Function written in TypeScript, responsible for handling user data deletion requests. This function is a critical component for complying with data privacy regulations such as GDPR's "Right to be Forgotten" and CCPA's "Right to Delete".

**Primary Requirement:** `NFR-LC-002b`: Data Retention and Deletion Policy: ...processes for handling user-initiated data access and deletion requests... must be defined, documented, and implemented. This includes procedures for anonymizing or deleting user data from local storage, backend databases (Firebase)...

The function will securely delete all data associated with a specific user from the Firebase backend, which includes their Firebase Authentication record and all related documents in Cloud Firestore.

## 2. High-Level Architecture

The function follows a serverless, event-driven architecture triggered by an HTTPS callable endpoint. It is structured internally with a layered approach to separate concerns:

*   **Handler (Presentation):** The entry point of the function, responsible for handling the HTTPS request, validating the caller's authentication context, and parsing input.
*   **Service (Application/Business):** Orchestrates the deletion workflow, ensuring data is deleted in the correct order to maintain integrity.
*   **Infrastructure (Adapters):** Contains adapters that encapsulate direct interactions with external Firebase services (Authentication and Firestore), isolating the core logic from the SDK specifics.
*   **Shared/Utilities:** Common modules for logging, configuration, and data models (DTOs).

mermaid
graph TD
    ClientApp -- HTTPS Call --> A[Handler: userDeletion.handler.ts];
    A -- Invokes --> B[Service: accountDeletion.service.ts];
    B -- Calls --> C[Adapter: firestore.adapter.ts];
    B -- Calls --> D[Adapter: auth.adapter.ts];
    C -- Interacts with --> E((Cloud Firestore));
    D -- Interacts with --> F((Firebase Authentication));
    
    subgraph UserAccountManagementFunction
        A
        B
        C
        D
    end
    
    subgraph Firebase Backend
        E
        F
    end


---

## 3. Project Configuration

### 3.1. `package.json`

**Purpose:** Defines project metadata, dependencies, and scripts for automation.

**Implementation Details:**

json
{
  "name": "user-account-management-function",
  "version": "1.0.0",
  "description": "Firebase Cloud Function to handle user account deletion.",
  "main": "lib/index.js",
  "scripts": {
    "build": "tsc",
    "serve": "npm run build && firebase emulators:start --only functions",
    "shell": "npm run build && firebase functions:shell",
    "start": "npm run shell",
    "deploy": "firebase deploy --only functions:deleteUser",
    "logs": "firebase functions:log",
    "lint": "eslint --ext .js,.ts .",
    "test": "mocha --require ts-node/register 'test/**/*.test.ts' --exit"
  },
  "dependencies": {
    "firebase-admin": "^11.11.0",
    "firebase-functions": "^4.4.1"
  },
  "devDependencies": {
    "@types/chai": "^4.3.5",
    "@types/mocha": "^10.0.1",
    "@typescript-eslint/eslint-plugin": "^5.12.0",
    "@typescript-eslint/parser": "^5.12.0",
    "chai": "^4.3.8",
    "eslint": "^8.9.0",
    "eslint-config-google": "^0.14.0",
    "eslint-plugin-import": "^2.25.4",
    "mocha": "^10.2.0",
    "sinon": "^15.2.0",
    "ts-node": "^10.9.1",
    "typescript": "^4.9.0"
  },
  "engines": {
    "node": "18"
  },
  "private": true
}


### 3.2. `tsconfig.json`

**Purpose:** Configures the TypeScript compiler for the project.

**Implementation Details:**

json
{
  "compilerOptions": {
    "module": "commonjs",
    "noImplicitReturns": true,
    "noUnusedLocals": true,
    "outDir": "lib",
    "sourceMap": true,
    "strict": true,
    "target": "es2020",
    "esModuleInterop": true,
    "resolveJsonModule": true
  },
  "compileOnSave": true,
  "include": [
    "src",
    "test"
  ]
}


---

## 4. Function Implementation Details

### 4.1. `index.ts` (Root)

**Purpose:** The main entry point for deploying the Cloud Function.

**Implementation Details:** This file imports the handler and re-exports it for the Firebase CLI to discover and deploy.

typescript
import * as functions from "firebase-functions";
import { onDeleteUserRequest } from "./src/handlers/userDeletion.handler";

// Expose the handler as a deployable cloud function named 'deleteUser'.
export const deleteUser = functions.https.onCall(onDeleteUserRequest);


### 4.2. `src/handlers/userDeletion.handler.ts`

**Purpose:** The HTTPS callable function handler. It serves as the controller, validating the request and delegating the core logic to the service layer.

**Implementation Details:**

typescript
import * as functions from "firebase-functions";
import { UserDeletionRequest } from "../models/userDeletion.dto";
import { deleteUserAccount } from "../services/accountDeletion.service";
import * as logger from "../utils/logger";

/**
 * Handles an HTTPS callable request to delete a user's account and all associated data.
 * @param {UserDeletionRequest} data - The request payload. Must contain the UID of the user to delete.
 * @param {functions.https.CallableContext} context - The context of the call, including authentication information.
 * @returns {Promise<{ success: boolean; message: string; }>} A success or error response.
 */
export const onDeleteUserRequest = async (
  data: UserDeletionRequest,
  context: functions.https.CallableContext
): Promise<{ success: boolean; message: string; }> => {
  // 1. Authentication Check: Ensure the caller is authenticated.
  if (!context.auth) {
    logger.warn("User deletion request from unauthenticated user.", { uidToDrop: data.uid });
    throw new functions.https.HttpsError(
      "unauthenticated",
      "You must be logged in to request account deletion."
    );
  }

  // 2. Authorization Check: Ensure the caller is either an admin or deleting themselves.
  const callerUid = context.auth.uid;
  const targetUid = data.uid;
  const isSelfDelete = callerUid === targetUid;
  // Note: Admin role check is omitted for simplicity but would be added here in a real app.
  // const isAdmin = context.auth.token.admin === true;

  if (!isSelfDelete) { // && !isAdmin) {
    logger.error("Unauthorized user deletion attempt.", { callerUid, targetUid });
    throw new functions.https.HttpsError(
      "permission-denied",
      "You do not have permission to delete this account."
    );
  }

  // 3. Input Validation
  if (!targetUid || typeof targetUid !== 'string') {
    throw new functions.https.HttpsError(
      "invalid-argument",
      "The function must be called with a 'uid' argument."
    );
  }

  logger.info(`Starting account deletion process for UID: ${targetUid}`, { callerUid });

  // 4. Delegate to Service Layer
  try {
    await deleteUserAccount(targetUid);
    logger.info(`Successfully deleted account for UID: ${targetUid}`);
    return { success: true, message: "Account successfully deleted." };
  } catch (error) {
    logger.error(`Failed to delete account for UID: ${targetUid}`, error as Error);
    // Throwing a generic error to the client to avoid leaking implementation details.
    throw new functions.https.HttpsError(
      "internal",
      "An unexpected error occurred while deleting the account. Please contact support."
    );
  }
};


### 4.3. `src/services/accountDeletion.service.ts`

**Purpose:** Orchestrates the multi-step deletion process, ensuring all user data is removed from every backend service in the correct order.

**Implementation Details:**

typescript
import * as authAdapter from "../infrastructure/auth.adapter";
import * as firestoreAdapter from "../infrastructure/firestore.adapter";
import * as logger from "../utils/logger";

/**
 * Orchestrates the complete deletion of a user account across all services.
 * The order of operations is critical: data is deleted from Firestore first
 * to prevent orphaned records if the final auth deletion fails.
 * @param {string} uid - The UID of the user to be deleted.
 */
export const deleteUserAccount = async (uid: string): Promise<void> => {
  if (!uid) {
    throw new Error("UID must be provided for account deletion.");
  }
  
  logger.info(`Service: Deleting all Firestore documents for user ${uid}.`);
  await firestoreAdapter.deleteUserDocuments(uid);

  logger.info(`Service: Deleting auth record for user ${uid}.`);
  await authAdapter.deleteAuthUser(uid);
};


### 4.4. `src/infrastructure/firestore.adapter.ts`

**Purpose:** Provides a dedicated interface for deleting all user-related documents from Cloud Firestore.

**Implementation Details:** This adapter will find and delete the user's main profile document and any other associated data, such as their leaderboard entries. It will use batched writes for efficiency.

typescript
import { getFirestoreInstance } from "./firebaseAdmin.client";
import * as logger from "../utils/logger";

/**
 * Deletes all documents related to a specific user from Cloud Firestore.
 * This includes their main user profile and any dependent data like leaderboard entries.
 * @param {string} uid - The UID of the user whose documents should be deleted.
 */
export const deleteUserDocuments = async (uid: string): Promise<void> => {
  const db = getFirestoreInstance();
  const batch = db.batch();

  // 1. Delete the main user profile document
  const userProfileRef = db.collection("UserProfiles").doc(uid);
  batch.delete(userProfileRef);
  logger.info(`Firestore Adapter: Queued deletion for UserProfile: ${userProfileRef.path}`);

  // 2. Query and delete related data (e.g., Leaderboard Entries)
  // This is an example. Add other collections as needed.
  const leaderboardEntriesRef = db.collection("LeaderboardEntries").where("userId", "==", uid);
  const leaderboardSnapshot = await leaderboardEntriesRef.get();
  if (!leaderboardSnapshot.empty) {
    leaderboardSnapshot.forEach(doc => {
      batch.delete(doc.ref);
      logger.info(`Firestore Adapter: Queued deletion for LeaderboardEntry: ${doc.ref.path}`);
    });
  }

  // Add queries for other collections like AchievementStatus if they exist
  // const achievementStatusRef = db.collection("AchievementStatuses").where("userId", "==", uid);
  // ...

  // 3. Commit the batched deletions
  await batch.commit();
  logger.info(`Firestore Adapter: Committed deletions for user ${uid}.`);
};


### 4.5. `src/infrastructure/auth.adapter.ts`

**Purpose:** Provides a dedicated interface for deleting a user from the Firebase Authentication service.

**Implementation Details:**

typescript
import { getAuthInstance } from "./firebaseAdmin.client";
import * as logger from "../utils/logger";

/**
 * Deletes a user account from the Firebase Authentication service.
 * @param {string} uid - The UID of the user to delete.
 */
export const deleteAuthUser = async (uid: string): Promise<void> => {
  const auth = getAuthInstance();
  try {
    await auth.deleteUser(uid);
    logger.info(`Auth Adapter: Successfully deleted user ${uid} from Firebase Authentication.`);
  } catch (error: any) {
    // It's not a failure if the user is already gone. Log as a warning.
    if (error.code === 'auth/user-not-found') {
      logger.warn(`Auth Adapter: User ${uid} not found in Firebase Authentication. Assumed already deleted.`);
      return;
    }
    // For other errors, re-throw to be handled by the service layer.
    throw error;
  }
};


### 4.6. `src/infrastructure/firebaseAdmin.client.ts`

**Purpose:** Initializes the Firebase Admin SDK using a singleton pattern.

**Implementation Details:**

typescript
import * as admin from "firebase-admin";

// Initialize Firebase Admin SDK only if it hasn't been already.
if (admin.apps.length === 0) {
  admin.initializeApp();
}

const firestore = admin.firestore();
const auth = admin.auth();

/**
 * Gets the singleton instance of the Firestore service.
 * @returns {admin.firestore.Firestore} The initialized Firestore instance.
 */
export const getFirestoreInstance = (): admin.firestore.Firestore => firestore;

/**
 * Gets the singleton instance of the Authentication service.
 * @returns {admin.auth.Auth} The initialized Auth instance.
 */
export const getAuthInstance = (): admin.auth.Auth => auth;


### 4.7. Models and Utilities

*   **`src/models/userDeletion.dto.ts`**:
    typescript
    /**
     * Data Transfer Object for a user deletion request.
     */
    export interface UserDeletionRequest {
      /**
       * The unique identifier (UID) of the user to be deleted.
       */
      readonly uid: string;
    }
    
*   **`src/utils/logger.ts`**:
    typescript
    import * as functionsLogger from "firebase-functions/logger";

    export const info = (message: string, metadata?: object): void => {
      functionsLogger.info(message, metadata);
    };
    
    export const warn = (message: string, metadata?: object): void => {
      functionsLogger.warn(message, metadata);
    };

    export const error = (message: string, error?: Error, metadata?: object): void => {
      const logData = { ...metadata, stack: error?.stack };
      functionsLogger.error(message, logData);
    };
    

## 5. Testing Strategy

*   **Unit Testing (Mocha/Chai/Sinon):**
    *   Test `accountDeletion.service` by mocking/stubbing the `firestore.adapter` and `auth.adapter` to verify the correct order of calls and parameter passing.
    *   Test `firestore.adapter` and `auth.adapter` by mocking the Firebase Admin SDK to verify they make the correct SDK calls.
    *   Test the `userDeletion.handler` by mocking the `accountDeletion.service` and providing mock `CallableContext` objects to verify authentication/authorization logic.
*   **Integration Testing (Firebase Emulators):**
    *   Deploy the function to the local Firebase Emulator Suite.
    *   Use an emulated callable function to trigger the entire flow.
    *   Pre-populate the emulated Auth and Firestore with test user data.
    *   Assert that after the function runs, the user and their data are successfully deleted from the emulators. Test edge cases like deleting a non-existent user.