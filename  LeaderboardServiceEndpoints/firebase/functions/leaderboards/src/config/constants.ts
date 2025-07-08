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