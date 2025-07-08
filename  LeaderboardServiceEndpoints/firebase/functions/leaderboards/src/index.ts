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
        const data = doc.data();
        // Filter out timestamps that are outside the current window
        const submissions = (data?.timestamps || []).filter((ts: number) => ts > windowStart);
        
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