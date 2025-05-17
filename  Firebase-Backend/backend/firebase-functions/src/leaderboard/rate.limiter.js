/**
 * @file rate.limiter.js
 * @summary Handles rate limiting for score submissions. Helps protect against spam and DoS attacks.
 * @description Implements rate limiting logic for score submissions to prevent spam, abuse, or excessive load on the backend.
 * Uses Firestore (via `FirestoreService`) to track recent submission timestamps or counts per user.
 * Namespace: PatternCipher.Backend.Leaderboard.Infrastructure
 * @version 1.0.0
 * @since 2024-07-16
 */

const firestoreService = require('../services/firestore.service.js'); // Assuming firestore.service.js exists
const config = require('../config/index.js'); // Assuming config/index.js exists for RATE_LIMIT_CONFIG
const logger = require('../core/logger'); // Assuming logger.js exists

const { RATE_LIMIT_CONFIG } = config;
const RATE_LIMIT_COLLECTION_NAME = 'rateLimitRecords'; // Define a collection name for rate limit records

/**
 * Checks if a user is rate-limited for a specific action.
 * REQ-9-009, REQ-CPS-012
 *
 * @async
 * @param {string} userId - The ID of the user performing the action.
 * @param {string} actionType - The type of action being performed (e.g., 'submit_score').
 * @returns {Promise<boolean>} True if the user is rate-limited, false otherwise.
 */
async function isRateLimited(userId, actionType) {
    const actionConfig = RATE_LIMIT_CONFIG[actionType];
    if (!actionConfig) {
        logger.warn(`No rate limit configuration found for actionType: ${actionType}`);
        return false; // No config means no rate limiting for this action
    }

    const { windowSeconds, maxRequests } = actionConfig;
    const now = Date.now();
    const windowStart = new Date(now - windowSeconds * 1000);

    try {
        const queryFilters = [
            { field: 'userId', operator: '==', value: userId },
            { field: 'actionType', operator: '==', value: actionType },
            { field: 'timestamp', operator: '>=', value: windowStart },
        ];
        
        // In a real scenario, firestoreService.queryCollection would handle Firestore Timestamps correctly.
        // For simplicity, assuming timestamp field stores JS Date objects or Firestore Timestamps
        // that can be compared with JS Date in the query.
        const recentRequests = await firestoreService.queryCollection(RATE_LIMIT_COLLECTION_NAME, queryFilters);

        if (recentRequests.length >= maxRequests) {
            logger.warn(`User ${userId} is rate-limited for action ${actionType}. Requests: ${recentRequests.length}, Max: ${maxRequests}`);
            return true;
        }
        return false;
    } catch (error) {
        logger.error('Error checking rate limit', error, { userId, actionType });
        // Fail open (don't block if there's an error checking) or fail closed depending on policy
        return false; // Defaulting to fail open for now
    }
}

/**
 * Records a request for rate-limiting purposes.
 * REQ-9-009, REQ-CPS-012
 *
 * @async
 * @param {string} userId - The ID of the user performing the action.
 * @param {string} actionType - The type of action being performed.
 * @returns {Promise<void>}
 */
async function recordRequest(userId, actionType) {
    const record = {
        userId,
        actionType,
        // timestamp will be set by firestore.service.js using serverTimestamp
    };
    // Generate a unique ID for the record or let Firestore do it.
    // Using a unique ID allows cleanup by TTL if desired.
    const recordId = `${userId}_${actionType}_${Date.now()}`; 
    try {
        await firestoreService.setDocument(RATE_LIMIT_COLLECTION_NAME, recordId, record);
        logger.info(`Recorded request for ${userId}, action: ${actionType}`);
    } catch (error) {
        logger.error('Error recording rate limit request', error, { userId, actionType });
        // Handle error: e.g., if recording fails, it might impact rate limiting accuracy.
    }
    // Old records can be cleaned up via Firestore TTL policies on the `timestamp` field.
}

module.exports = {
    isRateLimited,
    recordRequest,
};