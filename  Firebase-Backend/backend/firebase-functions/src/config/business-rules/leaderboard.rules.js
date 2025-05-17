/**
 * @file leaderboard.rules.js
 * @summary Business rules (BR-LEAD-001) for leaderboard score validation. Allows dynamic adjustment of validation parameters.
 * @description Defines or loads specific business rules (BR-LEAD-001) for leaderboard score validation logic,
 * externalizing them from core validation code. Consumed by `score.validator.js`.
 * Namespace: PatternCipher.Backend.Config.BusinessRules
 * @version 1.0.0
 * @since 2024-07-16
 */

const logger = require('../../core/logger'); // Assuming logger.js exists

// Placeholder for rules. In a real application, these might be loaded from Firestore,
// a JSON file deployed with functions, or Firebase Remote Config.
// For simplicity, we'll hardcode some examples here.
const LEVEL_SPECIFIC_RULES = {
    level_1: {
        maxScore: 10000,
        minTime: 10, // seconds
        maxMoves: 50,
        parTime: 60, // seconds
        parMoves: 30,
    },
    level_2: {
        maxScore: 15000,
        minTime: 15,
        maxMoves: 70,
    },
    // ... more levels
};

const GLOBAL_RULES = {
    maxScore: 1000000, // Absolute maximum score possible in any level
    minTime: 1,         // Absolute minimum time for any level
    maxTime: 3600,      // Max time allowed (e.g., 1 hour)
    minMoves: 1,        // Absolute minimum moves
    // other global constraints
};

/**
 * Retrieves validation rules specific to a given level.
 * REQ-SRP-009, REQ-9-009, REQ-CPS-012
 *
 * @async
 * @param {string} levelId - The ID of the level for which to retrieve rules.
 * @returns {Promise<object>} An object containing the validation rules for the level.
 *                            Returns an empty object if no specific rules are found for the level.
 */
async function getValidationRulesForLevel(levelId) {
    // In a real scenario, this might involve an async call to Firestore or Remote Config.
    // For this example, we're returning from a hardcoded object.
    return new Promise((resolve) => {
        const rules = LEVEL_SPECIFIC_RULES[levelId];
        if (rules) {
            logger.debug(`Loaded validation rules for level ${levelId}`, { levelId, rules });
            resolve(rules);
        } else {
            logger.debug(`No specific validation rules found for level ${levelId}. Global rules may apply.`, { levelId });
            resolve({}); // Return empty object if no specific rules, caller can fallback to global
        }
    });
}

/**
 * Retrieves global validation rules applicable to all scores.
 * REQ-SRP-009, REQ-9-009, REQ-CPS-012
 *
 * @async
 * @returns {Promise<object>} An object containing the global validation rules.
 */
async function getGlobalValidationRules() {
    // Similar to above, this could be an async call.
    return new Promise((resolve) => {
        logger.debug('Loaded global validation rules', { rules: GLOBAL_RULES });
        resolve(GLOBAL_RULES);
    });
}

module.exports = {
    getValidationRulesForLevel,
    getGlobalValidationRules,
};