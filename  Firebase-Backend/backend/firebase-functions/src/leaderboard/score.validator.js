/**
 * @file score.validator.js
 * @summary Validates leaderboard score submissions according to defined game and business rules.
 * @description Contains core domain logic for validating submitted scores against game rules, level parameters, and business rules (BR-LEAD-001).
 * This is a critical component for maintaining fair leaderboards.
 * Namespace: PatternCipher.Backend.Leaderboard.Domain
 * @version 1.0.0
 * @since 2024-07-16
 */

const leaderboardRules = require('../config/business-rules/leaderboard.rules.js');
const logger = require('../core/logger'); // Assuming logger.js exists as per SDS
// const { REPO_DATA_MODELS_SCHEMAS } = require('repo-data-models'); // Conceptual import

/**
 * Validates score data against predefined rules and context.
 * Checks for required fields, data types, plausibility, and business rules.
 * REQ-SRP-009, REQ-9-009, REQ-CPS-012
 *
 * @async
 * @param {object} scoreData - The score data submitted by the client. Expected to conform to LeaderboardEntry schema.
 *                             Example: { score: 1000, levelId: "level1", time: 120, moves: 10, /* ...other fields */ }
 * @param {string} userId - The ID of the user submitting the score.
 * @param {object} [levelContext] - Optional additional context about the level (e.g., pre-fetched level data).
 * @returns {Promise<{isValid: boolean, errors: Array<string>, validatedScore?: object}>}
 *          An object indicating if the score is valid, an array of error messages if not,
 *          and the validated/cleaned score data if valid.
 */
async function validateScoreData(scoreData, userId, levelContext = {}) {
    const errors = [];
    let validatedScore = { ...scoreData }; // Start with a copy

    // 1. Basic Sanity Checks (Required fields, types)
    if (scoreData.score == null || typeof scoreData.score !== 'number' || scoreData.score < 0) {
        errors.push('Invalid or missing score value.');
    }
    if (!scoreData.levelId || typeof scoreData.levelId !== 'string') {
        errors.push('Invalid or missing levelId.');
    }
    // Add more checks for time, moves, etc. based on REPO-DATA-MODELS.LeaderboardEntry
    if (scoreData.time != null && (typeof scoreData.time !== 'number' || scoreData.time < 0)) {
        errors.push('Invalid time value.');
    }
    if (scoreData.moves != null && (typeof scoreData.moves !== 'number' || scoreData.moves < 0)) {
        errors.push('Invalid moves value.');
    }

    if (errors.length > 0) {
        logger.warn('Basic score validation failed', { userId, scoreData, errors });
        return { isValid: false, errors };
    }

    // 2. Load Business Rules
    let rules;
    try {
        if (scoreData.levelId) {
            rules = await leaderboardRules.getValidationRulesForLevel(scoreData.levelId);
        }
        if (!rules || Object.keys(rules).length === 0) { // Fallback to global if level-specific not found or empty
            rules = await leaderboardRules.getGlobalValidationRules();
        }
    } catch (error) {
        logger.error('Failed to load leaderboard validation rules', error, { userId, levelId: scoreData.levelId });
        errors.push('Internal error: Could not load validation rules.');
        return { isValid: false, errors };
    }

    // 3. Apply Business Rules (BR-LEAD-001)
    if (rules) {
        if (rules.maxScore != null && scoreData.score > rules.maxScore) {
            errors.push(`Score ${scoreData.score} exceeds maximum allowed score of ${rules.maxScore}.`);
        }
        if (rules.minTime != null && scoreData.time != null && scoreData.time < rules.minTime) {
            errors.push(`Time ${scoreData.time}s is less than minimum allowed time of ${rules.minTime}s.`);
        }
        if (rules.maxMoves != null && scoreData.moves != null && scoreData.moves > rules.maxMoves) {
            errors.push(`Moves ${scoreData.moves} exceed maximum allowed moves of ${rules.maxMoves}.`);
        }
        // Add more rule checks based on what `leaderboard.rules.js` provides
    }

    // 4. Plausibility Checks (e.g., against historical data - simplified for now)
    // This section would typically involve more complex logic, potentially querying user's past scores.
    // For now, we'll keep it simple.
    if (scoreData.score > 9999999) { // Arbitrary high score check
        errors.push('Score seems implausibly high.');
    }

    // 5. Prepare validated score (e.g., ensure all necessary fields from schema are present, default if needed)
    // validatedScore.timestamp = admin.firestore.FieldValue.serverTimestamp(); // This should be set by FirestoreService or calling function
    validatedScore.userId = userId; // Ensure userId is part of the validated score object

    if (errors.length > 0) {
        logger.warn('Score validation failed', { userId, scoreData, rules, errors, levelContext });
        return { isValid: false, errors };
    }

    logger.info('Score validated successfully', { userId, validatedScore, levelContext });
    return { isValid: true, errors: [], validatedScore };
}

module.exports = {
    validateScoreData,
};