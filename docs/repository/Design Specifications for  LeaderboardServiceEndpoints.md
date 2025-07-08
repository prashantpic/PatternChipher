# Software Design Specification (SDS): LeaderboardServiceEndpoints

## 1. Introduction

This document provides a detailed software design specification for the `LeaderboardServiceEndpoints` repository (`REPO-PATT-008`). This repository contains the backend logic for the game's leaderboard system. Its primary responsibilities are to provide a secure endpoint for submitting player scores and to define the necessary database rules and indexes for securely reading leaderboard data.

The core component is a serverless Firebase Cloud Function written in TypeScript, which enforces data integrity and plausibility rules for all score submissions, preventing casual cheating and ensuring a fair competitive environment.

**Requirements Addressed:**
*   `FR-ONL-001`: Leaderboard system for high scores.
*   `FR-ONL-002`: Implies achievement data might be related, but the focus here is leaderboards.
*   `NFR-SEC-003`: Leaderboard Integrity (Server-side validation).
*   `BR-LEAD-001`: Business rules for "plausible" scores.
*   `NFR-BS-001`: Backend performance.
*   `NFR-BS-003`: Backend availability.
*   `NFR-SEC-004`: Security rules for Firestore.

## 2. System Overview & Architecture

This service follows a **Serverless, RPC-style architecture**. The client application does not interact directly with the leaderboard database for write operations. Instead, it makes a remote procedure call to a dedicated `https.onCall` Firebase Cloud Function.

**Data Flow for Score Submission:**
1.  **Client Application**: An authenticated player completes a level. The client invokes the `submitScore` function via the Firebase SDK.
2.  **Firebase Cloud Function (`submitScore`)**:
    *   Authenticates the request.
    *   Applies rate limiting.
    *   Validates the incoming data against a DTO.
    *   Performs business logic validation to ensure the score is plausible.
    *   If valid, constructs a leaderboard entry and persists it to Cloud Firestore.
3.  **Cloud Firestore**: The score is stored in the `leaderboards` collection. Firestore Security Rules prevent any direct writes from clients to this collection.

**Data Flow for Reading Leaderboards:**
1.  **Client Application**: The client directly queries the `leaderboards` collection in Cloud Firestore.
2.  **Cloud Firestore**: Firestore Security Rules allow public or authenticated read access. Composite indexes ensure these queries are fast and efficient.

This hybrid approach (secure writes via function, direct reads) provides security, scalability, and performance.

---

## 3. Configuration

### 3.1. `package.json`

This file manages all Node.js dependencies for the Cloud Functions.

json
{
  "name": "leaderboards-functions",
  "version": "1.0.0",
  "description": "Cloud Functions for the Pattern Cipher Leaderboard Service.",
  "main": "lib/index.js",
  "scripts": {
    "build": "tsc",
    "serve": "firebase emulators:start --only functions",
    "shell": "firebase functions:shell",
    "start": "npm run shell",
    "deploy": "firebase deploy --only functions",
    "logs": "firebase functions:log",
    "lint": "eslint --ext .js,.ts ."
  },
  "dependencies": {
    "firebase-admin": "^11.11.1",
    "firebase-functions": "^4.5.0",
    "joi": "^17.11.0" // For robust request data validation
  },
  "devDependencies": {
    "@typescript-eslint/eslint-plugin": "^6.13.1",
    "@typescript-eslint/parser": "^6.13.1",
    "eslint": "^8.55.0",
    "eslint-plugin-import": "^2.29.0",
    "typescript": "^5.3.2"
  }
}


### 3.2. `firestore.rules`

These rules secure the `leaderboards` collection. They are critical for `NFR-SEC-003` and `NFR-SEC-004`.


rules_version = '2';

service cloud.firestore {
  match /databases/{database}/documents {

    // Allow public read access to leaderboards.
    // This allows unauthenticated users to view leaderboards.
    // Can be changed to `allow read: if request.auth != null;` to restrict to logged-in users.
    match /leaderboards/{leaderboardId} {
      allow read: if true;

      // DENY all client-side write operations.
      // Scores can only be written by the backend via the Cloud Function,
      // which uses the Admin SDK and bypasses these rules.
      allow write: if false;
    }

    // Collection to manage rate limiting for score submissions.
    // Users can only create a document for themselves, and only update their own timestamp.
    match /scoreSubmissions/{userId} {
        allow read, update: if request.auth.uid == userId;
        allow create: if request.auth.uid == userId;
    }
  }
}


### 3.3. `firestore.indexes.json`

This file defines composite indexes needed for efficient leaderboard queries, addressing `FR-ONL-001` (filtering) and `NFR-BS-001` (performance).

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
    },
    {
      "collectionGroup": "leaderboards",
      "queryScope": "COLLECTION",
      "fields": [
        { "fieldPath": "levelId", "order": "ASCENDING" },
        { "fieldPath": "timeInSeconds", "order": "ASCENDING" }
      ]
    }
  ]
}


### 3.4. `src/config/constants.ts`

Centralizes configuration values to adhere to `BR-LEAD-001`.

typescript
// src/config/constants.ts

export const FIRESTORE_COLLECTIONS = {
    LEADERBOARDS: 'leaderboards',
    SCORE_SUBMISSIONS: 'scoreSubmissions',
};

export const SCORE_VALIDATION_RULES = {
    MIN_SCORE: 0,
    MAX_SCORE: 9999999, // A reasonable upper limit
    MIN_MOVES: 1,
    MAX_MOVES: 1000,
    MIN_TIME_SECONDS: 1,
    MAX_TIME_SECONDS: 3600, // 1 hour
};

export const RATE_LIMIT_CONFIG = {
    WINDOW_SECONDS: 60, // 1 minute window
    MAX_SUBMISSIONS: 5, // Max 5 submissions per minute
};


---

## 4. Detailed Component Design

### 4.1. Domain Layer

#### 4.1.1. `domain/models/leaderboard-entry.model.ts`

Defines the canonical data structure for a leaderboard entry.

typescript
// src/domain/models/leaderboard-entry.model.ts
import { FieldValue } from 'firebase-admin/firestore';

export interface LeaderboardEntry {
    userId: string;
    userName: string;
    levelId: string;
    score: number;
    moves: number;
    timeInSeconds: number;
    submittedAt: FieldValue; // Will be a server timestamp
}


#### 4.1.2. `domain/services/score-validator.service.ts`

Encapsulates the business logic for score validation as per `BR-LEAD-001`.

typescript
// src/domain/services/score-validator.service.ts
import { SCORE_VALIDATION_RULES } from '../../config/constants';
import { SubmitScoreDto } from '../../application/dtos/submit-score.dto';

export class ScoreValidatorService {
    /**
     * Checks if a score submission is plausible based on defined business rules.
     * @param {SubmitScoreDto} submission - The score data to validate.
     * @returns {boolean} - True if the score is plausible, false otherwise.
     */
    public isPlausible(submission: SubmitScoreDto): boolean {
        const { score, moves, timeInSeconds } = submission;

        if (score < SCORE_VALIDATION_RULES.MIN_SCORE || score > SCORE_VALIDATION_RULES.MAX_SCORE) {
            return false;
        }
        if (moves < SCORE_VALIDATION_RULES.MIN_MOVES || moves > SCORE_VALIDATION_RULES.MAX_MOVES) {
            return false;
        }
        if (timeInSeconds < SCORE_VALIDATION_RULES.MIN_TIME_SECONDS || timeInSeconds > SCORE_VALIDATION_RULES.MAX_TIME_SECONDS) {
            return false;
        }

        // Future complex logic can be added here. For example:
        // const maxPossibleScore = calculateMaxScoreForLevel(submission.levelId, moves);
        // if (score > maxPossibleScore) { return false; }

        return true;
    }
}


### 4.2. Application Layer

#### 4.2.1. `application/dtos/submit-score.dto.ts`

Defines the API data contract for score submission requests.

typescript
// src/application/dtos/submit-score.dto.ts
import * as Joi from 'joi';

export interface SubmitScoreDto {
    levelId: string;
    score: number;
    moves: number;
    timeInSeconds: number;
}

// Joi schema for validation at the entry point
export const submitScoreSchema = Joi.object<SubmitScoreDto>({
    levelId: Joi.string().required(),
    score: Joi.number().integer().min(0).required(),
    moves: Joi.number().integer().min(1).required(),
    timeInSeconds: Joi.number().integer().min(1).required(),
});


#### 4.2.2. `application/leaderboard.service.ts`

Orchestrates the use case of submitting a new score.

typescript
// src/application/leaderboard.service.ts
import { ScoreValidatorService } from '../domain/services/score-validator.service';
import { LeaderboardRepository } from '../infrastructure/repositories/leaderboard.repository';
import { LeaderboardEntry } from '../domain/models/leaderboard-entry.model';
import { SubmitScoreDto } from './dtos/submit-score.dto';
import { FieldValue } from 'firebase-admin/firestore';
import { functions } from 'firebase-functions';

export class LeaderboardService {
    constructor(
        private readonly scoreValidator: ScoreValidatorService,
        private readonly leaderboardRepository: LeaderboardRepository,
    ) {}

    /**
     * Handles the full process of a score submission.
     * @param {SubmitScoreDto} submissionDto - The incoming data from the client.
     * @param {functions.https.CallableContext} authContext - The authentication context of the caller.
     * @throws {functions.https.HttpsError} - Throws specific errors for invalid data or business rule violations.
     */
    async handleScoreSubmission(
        submissionDto: SubmitScoreDto,
        authContext: functions.https.CallableContext
    ): Promise<void> {
        // 1. Validate plausibility (Business Rules)
        if (!this.scoreValidator.isPlausible(submissionDto)) {
            throw new functions.https.HttpsError('invalid-argument', 'The submitted score is not plausible.');
        }
        
        // 2. Construct Domain Model
        const newEntry: LeaderboardEntry = {
            ...submissionDto,
            userId: authContext.auth!.uid,
            userName: authContext.auth!.token.name || 'Anonymous Player',
            submittedAt: FieldValue.serverTimestamp(),
        };

        // 3. Persist to repository
        await this.leaderboardRepository.submitScore(newEntry);
    }
}


### 4.3. Infrastructure Layer

#### 4.3.1. `infrastructure/firebase.ts`

Initializes the Firebase Admin SDK singleton.

typescript
// src/infrastructure/firebase.ts
import * as admin from 'firebase-admin';

admin.initializeApp();

export const db = admin.firestore();
export const auth = admin.auth();


#### 4.3.2. `infrastructure/repositories/leaderboard.repository.ts`

Handles data persistence to Firestore.

typescript
// src/infrastructure/repositories/leaderboard.repository.ts
import { db } from '../firebase';
import { FIRESTORE_COLLECTIONS } from '../../config/constants';
import { LeaderboardEntry } from '../../domain/models/leaderboard-entry.model';
import { Firestore } from 'firebase-admin/firestore';

export class LeaderboardRepository {
    constructor(private readonly firestore: Firestore) {}

    /**
     * Writes a new leaderboard entry to the database.
     * @param {LeaderboardEntry} entry - The leaderboard entry to persist.
     */
    async submitScore(entry: LeaderboardEntry): Promise<void> {
        // We could set a specific document ID, but letting Firestore auto-generate it is fine.
        await this.firestore.collection(FIRESTORE_COLLECTIONS.LEADERBOARDS).add(entry);
    }
}


### 4.4. Presentation (Endpoint) Layer

#### 4.4.1. `src/index.ts`

The main entry point defining the `submitScore` HTTPS callable function.

typescript
// src/index.ts
import * as functions from 'firebase-functions';
import { db } from './infrastructure/firebase';
import { FIRESTORE_COLLECTIONS, RATE_LIMIT_CONFIG } from './config/constants';
import { submitScoreSchema, SubmitScoreDto } from './application/dtos/submit-score.dto';
import { LeaderboardService } from './application/leaderboard.service';
import { ScoreValidatorService } from './domain/services/score-validator.service';
import { LeaderboardRepository } from './infrastructure/repositories/leaderboard.repository';

// Instantiate services and repositories (Dependency Injection)
const scoreValidator = new ScoreValidatorService();
const leaderboardRepository = new LeaderboardRepository(db);
const leaderboardService = new LeaderboardService(scoreValidator, leaderboardRepository);

/**
 * Checks and enforces rate limiting for a user.
 * @param {string} userId - The user's UID.
 * @returns {Promise<void>} - Resolves if allowed, rejects with HttpsError if rate-limited.
 */
async function enforceRateLimit(userId: string): Promise<void> {
    const rateLimitRef = db.collection(FIRESTORE_COLLECTIONS.SCORE_SUBMISSIONS).doc(userId);
    const now = Date.now();
    const windowStart = now - RATE_LIMIT_CONFIG.WINDOW_SECONDS * 1000;

    await db.runTransaction(async (transaction) => {
        const doc = await transaction.get(rateLimitRef);
        const submissions = (doc.data()?.timestamps || []).filter((ts: number) => ts > windowStart);
        
        if (submissions.length >= RATE_LIMIT_CONFIG.MAX_SUBMISSIONS) {
            throw new functions.https.HttpsError('resource-exhausted', 'You are submitting scores too frequently. Please try again later.');
        }

        submissions.push(now);
        transaction.set(rateLimitRef, { timestamps: submissions }, { merge: true });
    });
}


export const submitScore = functions.https.onCall(async (data: SubmitScoreDto, context) => {
    // 1. Authentication Check
    if (!context.auth) {
        throw new functions.https.HttpsError('unauthenticated', 'The function must be called while authenticated.');
    }

    // 2. Data Validation (DTO Schema)
    const { error } = submitScoreSchema.validate(data);
    if (error) {
        throw new functions.https.HttpsError('invalid-argument', `Invalid request data: ${error.message}`);
    }

    try {
        // 3. Rate Limiting
        await enforceRateLimit(context.auth.uid);

        // 4. Delegate to Application Service
        await leaderboardService.handleScoreSubmission(data, context);

        return { success: true, message: 'Score submitted successfully.' };

    } catch (err) {
        if (err instanceof functions.https.HttpsError) {
            throw err; // Re-throw known HttpsError
        }
        // Log unexpected errors for monitoring
        functions.logger.error('Unexpected error in submitScore:', err);
        throw new functions.https.HttpsError('internal', 'An unexpected error occurred.');
    }
});


## 5. Testing Strategy

*   **Unit Tests**: Use a framework like Jest to test the `ScoreValidatorService` with various plausible and implausible inputs. Test repository and service classes with mock dependencies.
*   **Integration Tests**: Utilize the Firebase Emulator Suite. Write tests that call the `submitScore` function and assert that the correct data is written to the emulated Firestore database and that security rules and rate limiting behave as expected. Test error cases like unauthenticated calls and invalid data.