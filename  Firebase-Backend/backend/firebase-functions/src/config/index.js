/**
 * @file Application configuration module. Centralizes settings like database collection names and operational parameters.
 * @namespace PatternCipher.Backend.Config
 */

// It's good practice to load sensitive or environment-specific configurations from environment variables.
// Example: const logLevel = process.env.LOG_LEVEL || 'info';

const LEADERBOARD_COLLECTION_NAME = process.env.LEADERBOARD_COLLECTION_NAME || "leaderboards";
const USER_PROFILE_COLLECTION_NAME = process.env.USER_PROFILE_COLLECTION_NAME || "user_profiles";
const ACHIEVEMENTS_COLLECTION_NAME = process.env.ACHIEVEMENTS_COLLECTION_NAME || "user_achievements";
const USER_CLOUD_SAVE_COLLECTION_NAME = process.env.USER_CLOUD_SAVE_COLLECTION_NAME || "user_cloud_saves";
const RATE_LIMIT_COLLECTION_NAME = process.env.RATE_LIMIT_COLLECTION_NAME || "rate_limit_records";


const RATE_LIMIT_CONFIG = {
    submit_score: {
        windowSeconds: parseInt(process.env.RATE_LIMIT_SUBMIT_SCORE_WINDOW_SECONDS, 10) || 60, // 1 minute window
        maxRequests: parseInt(process.env.RATE_LIMIT_SUBMIT_SCORE_MAX_REQUESTS, 10) || 5,    // Max 5 requests per window
    },
    // Add other action types as needed
    // e.g., create_account, update_profile etc.
    default: {
        windowSeconds: parseInt(process.env.RATE_LIMIT_DEFAULT_WINDOW_SECONDS, 10) || 60,
        maxRequests: parseInt(process.env.RATE_LIMIT_DEFAULT_MAX_REQUESTS, 10) || 10,
    }
};

const LOG_LEVEL = process.env.LOG_LEVEL || "info"; // e.g., 'debug', 'info', 'warn', 'error'

module.exports = {
    /**
     * Firestore collection name for leaderboards.
     * @type {string}
     */
    LEADERBOARD_COLLECTION_NAME,

    /**
     * Firestore collection name for user profiles.
     * @type {string}
     */
    USER_PROFILE_COLLECTION_NAME,

    /**
     * Firestore collection name for user achievements.
     * @type {string}
     */
    ACHIEVEMENTS_COLLECTION_NAME,

    /**
     * Firestore collection name for user cloud save data.
     * @type {string}
     */
    USER_CLOUD_SAVE_COLLECTION_NAME,

    /**
     * Firestore collection name for rate limiting records.
     * @type {string}
     */
    RATE_LIMIT_COLLECTION_NAME,
    
    /**
     * Configuration for rate limiting different actions.
     * @type {object}
     */
    RATE_LIMIT_CONFIG,

    /**
     * Desired log level for the application.
     * @type {string}
     */
    LOG_LEVEL
};