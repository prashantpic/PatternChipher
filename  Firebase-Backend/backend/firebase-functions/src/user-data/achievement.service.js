/**
 * @file achievement.service.js
 * @summary Application service for managing player achievements. Handles progress updates and unlocking logic.
 * @description Contains application service logic for server-side achievement tracking, progress updates, and unlocking conditions.
 * Manages data in Firestore via `FirestoreService`.
 * Namespace: PatternCipher.Backend.UserData.Application
 * @version 1.0.0
 * @since 2024-07-16
 */

const firestoreService = require('../services/firestore.service.js'); // Assuming firestore.service.js exists
const config = require('../config/index.js'); // Assuming config/index.js exists for collection names
const logger = require('../core/logger'); // Assuming logger.js exists
// const { REPO_DATA_MODELS_SCHEMAS } = require('repo-data-models'); // Conceptual: Achievement definitions might come from here or config

const { ACHIEVEMENTS_COLLECTION_NAME, USER_PROFILE_COLLECTION_NAME } = config;

// Placeholder for achievement definitions. In a real app, this would be loaded from config or Firestore.
const ACHIEVEMENT_DEFINITIONS = {
    'first_win': { target: 1, description: 'Win your first game' },
    'ten_wins': { target: 10, description: 'Win 10 games' },
    'score_10000': { target: 1, type: 'value_ge', valueField: 'score', threshold: 10000, description: 'Achieve a score of 10,000 in a single game' },
    // ... other achievement definitions
};


/**
 * Updates a player's achievement progress and checks for unlocks.
 * REQ-9-004
 *
 * @async
 * @param {string} userId - The ID of the user.
 * @param {string} achievementId - The ID of the achievement to update.
 * @param {object} progressData - Data relevant to the achievement's progress.
 *                                e.g., { increment: 1 } for counter achievements,
 *                                or { value: 5000 } for value-based achievements.
 * @returns {Promise<{unlocked: boolean, updatedAchievement: object | null}>}
 *          An object indicating if the achievement was newly unlocked and the updated achievement state.
 *          Returns null for updatedAchievement if definition is not found or error.
 */
async function updatePlayerAchievement(userId, achievementId, progressData) {
    if (!userId || !achievementId || !progressData) {
        logger.warn('Invalid parameters for updatePlayerAchievement', { userId, achievementId, progressData });
        return { unlocked: false, updatedAchievement: null };
    }

    const achievementDef = ACHIEVEMENT_DEFINITIONS[achievementId];
    if (!achievementDef) {
        logger.warn(`Achievement definition not found for ID: ${achievementId}`, { userId });
        return { unlocked: false, updatedAchievement: null };
    }

    const docId = `${userId}_${achievementId}`; // Composite ID for user-specific achievement document
    let newlyUnlocked = false;

    try {
        let currentAchievement = await firestoreService.getDocument(ACHIEVEMENTS_COLLECTION_NAME, docId);
        let currentProgress = 0;
        let alreadyUnlocked = false;

        if (currentAchievement && currentAchievement.data) {
            currentProgress = currentAchievement.data.progress || 0;
            alreadyUnlocked = currentAchievement.data.unlocked || false;
        } else {
            // Initialize if it's the first time progress is reported for this achievement for this user
            currentAchievement = { data: { userId, achievementId, progress: 0, unlocked: false } };
        }
        
        if (alreadyUnlocked) {
            logger.info(`Achievement ${achievementId} already unlocked for user ${userId}. No update needed.`, { userId });
            return { unlocked: false, updatedAchievement: currentAchievement.data };
        }

        // Update progress based on definition and progressData
        if (achievementDef.type === 'value_ge' && progressData.value != null) {
            // For achievements like "reach X score", progressData.value is the new score.
            // We check if this new value meets the threshold.
             if (progressData.value >= achievementDef.threshold) {
                currentProgress = achievementDef.target; // Mark as complete
             } else {
                // For this type, progress might not be cumulative in the traditional sense,
                // but rather reflects the highest value achieved towards the goal.
                // Or, if the achievement is "do X Y times", then it's cumulative. This needs clarification in real reqs.
                // For now, assume "value_ge" is a one-shot condition.
                currentProgress = progressData.value; // Store the value that was checked
             }
        } else if (progressData.increment != null) { // Default to counter type
            currentProgress += progressData.increment;
        } else if (progressData.absolute != null) {
            currentProgress = progressData.absolute;
        }
        // Ensure progress doesn't exceed target if target is a max value
        if (achievementDef.target != null) {
            currentProgress = Math.min(currentProgress, achievementDef.target);
        }


        // Check for unlock
        if (achievementDef.target != null && currentProgress >= achievementDef.target) {
            newlyUnlocked = true;
            currentAchievement.data.unlocked = true;
            currentAchievement.data.unlockedTimestamp = firestoreService.getServerTimestamp(); // Server timestamp
        }
        currentAchievement.data.progress = currentProgress;
        currentAchievement.data.lastUpdated = firestoreService.getServerTimestamp();

        await firestoreService.setDocument(ACHIEVEMENTS_COLLECTION_NAME, docId, currentAchievement.data);

        if (newlyUnlocked) {
            logger.info(`Achievement ${achievementId} unlocked for user ${userId}!`, { userId, achievementId });
            // Potentially trigger other actions, e.g., update a counter on UserProfile
            // await firestoreService.updateDocument(USER_PROFILE_COLLECTION_NAME, userId, {
            //     unlockedAchievementsCount: firestoreService.getFieldValue().increment(1) // Firestore increment
            // });
        }
        logger.info(`Achievement ${achievementId} progress updated for user ${userId}. New progress: ${currentProgress}`, { userId });
        return { unlocked: newlyUnlocked, updatedAchievement: currentAchievement.data };

    } catch (error) {
        logger.error('Error updating player achievement', error, { userId, achievementId, progressData });
        return { unlocked: false, updatedAchievement: null };
    }
}

/**
 * Retrieves all achievement statuses for a given player.
 * REQ-9-004
 *
 * @async
 * @param {string} userId - The ID of the user.
 * @returns {Promise<Array<object>>} An array of achievement status objects for the user.
 */
async function getPlayerAchievements(userId) {
    if (!userId) {
        logger.warn('Invalid userId for getPlayerAchievements');
        return [];
    }
    try {
        const queryFilters = [{ field: 'userId', operator: '==', value: userId }];
        const achievements = await firestoreService.queryCollection(ACHIEVEMENTS_COLLECTION_NAME, queryFilters);
        
        // Optionally, merge with definitions to provide full achievement details
        const detailedAchievements = achievements.map(achDoc => {
            const definition = ACHIEVEMENT_DEFINITIONS[achDoc.achievementId] || {};
            return { ...definition, ...achDoc }; // achDoc overrides definition for status fields
        });
        logger.info(`Fetched ${detailedAchievements.length} achievements for user ${userId}`, { userId });
        return detailedAchievements;
    } catch (error) {
        logger.error('Error fetching player achievements', error, { userId });
        return [];
    }
}

/**
 * Checks and unlocks achievements based on a general event context or current user data.
 * This is a more general function that might be called after significant game events.
 * REQ-9-004
 *
 * @async
 * @param {string} userId - The ID of the user.
 * @param {object} [eventContext] - Optional object containing data about the event that occurred
 *                                 (e.g., { eventType: 'level_complete', levelId: 'level5', score: 15000 }).
 * @returns {Promise<Array<string>>} A list of IDs of newly unlocked achievements.
 */
async function checkAndUnlockAchievements(userId, eventContext = {}) {
    if (!userId) {
        logger.warn('Invalid userId for checkAndUnlockAchievements');
        return [];
    }

    const newlyUnlockedIds = [];
    logger.info(`Checking achievements for user ${userId} with context:`, { userId, eventContext });

    // Example: Iterate through relevant achievement definitions and check conditions
    // This is highly dependent on how achievement conditions are defined and what eventContext contains.
    for (const achievementId in ACHIEVEMENT_DEFINITIONS) {
        const definition = ACHIEVEMENT_DEFINITIONS[achievementId];
        let shouldUpdate = false;
        let progressData = {};

        // Example: Check a score-based achievement if eventContext has score
        if (definition.type === 'value_ge' && eventContext.score != null && achievementId === 'score_10000') { // Example specific check
            if (eventContext.score >= definition.threshold) {
                progressData = { absolute: definition.target }; // Mark as complete
                shouldUpdate = true;
            }
        }

        // Example: Check win-based achievements if eventContext signifies a win
        if (eventContext.eventType === 'level_complete' && eventContext.win === true) {
            if (achievementId === 'first_win' || achievementId === 'ten_wins') { // These are counter based
                progressData = { increment: 1 };
                shouldUpdate = true;
            }
        }

        if (shouldUpdate) {
            try {
                // Before updating, get current state to avoid redundant updates or issues with already unlocked
                const docId = `${userId}_${achievementId}`;
                const currentAchievementDoc = await firestoreService.getDocument(ACHIEVEMENTS_COLLECTION_NAME, docId);
                if (currentAchievementDoc && currentAchievementDoc.data && currentAchievementDoc.data.unlocked) {
                    // Already unlocked, skip
                    continue;
                }

                const result = await updatePlayerAchievement(userId, achievementId, progressData);
                if (result.unlocked) {
                    newlyUnlockedIds.push(achievementId);
                }
            } catch (error) {
                logger.error(`Error during checkAndUnlock for achievement ${achievementId}`, error, { userId, eventContext });
            }
        }
    }

    if (newlyUnlockedIds.length > 0) {
        logger.info(`User ${userId} unlocked new achievements: ${newlyUnlockedIds.join(', ')}`, { userId, eventContext });
    }
    return newlyUnlockedIds;
}


module.exports = {
    updatePlayerAchievement,
    getPlayerAchievements,
    checkAndUnlockAchievements,
};