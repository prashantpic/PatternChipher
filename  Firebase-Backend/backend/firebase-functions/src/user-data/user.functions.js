const functions = require("firebase-functions");
const admin = require("../core/firebaseAdminSetup").admin; // Assuming firebaseAdminSetup.js exports admin
const logger =require("../core/logger");
const errorHandler = require("../core/errorHandler");
const cloudSaveService = require("./cloudsave.service"); // Assumed to exist
const achievementService = require("./achievement.service"); // Assumed to exist
const firestoreService = require("../services/firestore.service");
const config = require("../config/index");
const _ = require('lodash');


/**
 * @file Cloud Functions for managing user profile, achievements, and cloud save data.
 * @namespace PatternCipher.Backend.UserData
 */

/**
 * Synchronizes cloud save data for the authenticated user.
 * REQ-PDP-006: Cloud Save Data Management
 * REQ-9-006: Cloud Save Synchronization
 * @async
 * @function syncCloudSaveData
 * @param {object} cloudSavePayload - The cloud save data payload from the client.
 * @param {object.clientSaveData} cloudSavePayload.clientSaveData - The actual game data to save.
 * @param {number} cloudSavePayload.clientTimestamp - The timestamp from the client when the data was saved.
 * @param {functions.https.CallableContext} context - The context of the function call.
 * @returns {Promise<object>} A promise that resolves with the synchronization status and merged data.
 * @throws {functions.https.HttpsError} For authentication errors or internal issues.
 */
exports.syncCloudSaveData = functions.https.onCall(async (cloudSavePayload, context) => {
    logger.info("syncCloudSaveData function called", { userId: context.auth ? context.auth.uid : "anonymous" });

    if (!context.auth || !context.auth.uid) {
        logger.warn("syncCloudSaveData: Authentication required.");
        throw errorHandler.toHttpsError(new functions.https.HttpsError("unauthenticated", "Authentication required for cloud save."));
    }
    const userId = context.auth.uid;

    const { clientSaveData, clientTimestamp } = cloudSavePayload;
    if (!clientSaveData || typeof clientTimestamp !== 'number') {
        logger.warn("syncCloudSaveData: Invalid payload.", { userId, cloudSavePayload });
        throw errorHandler.toHttpsError(new functions.https.HttpsError("invalid-argument", "Invalid payload: clientSaveData and clientTimestamp are required."));
    }

    try {
        const result = await cloudSaveService.saveUserData(userId, clientSaveData, clientTimestamp);
        logger.info("syncCloudSaveData: Data synchronized successfully.", { userId, status: result.status });
        return { success: true, ...result };
    } catch (error) {
        logger.error("syncCloudSaveData: Error synchronizing data.", error, { userId });
        if (error instanceof functions.https.HttpsError) {
            throw error;
        }
        throw errorHandler.toHttpsError(new functions.https.HttpsError("internal", "An error occurred during cloud save synchronization."));
    }
});

/**
 * Updates achievement progress for the authenticated user.
 * REQ-9-004: Achievement Tracking
 * @async
 * @function updateAchievementProgress
 * @param {object} achievementData - Data for updating achievement progress.
 * @param {string} achievementData.achievementId - The ID of the achievement to update.
 * @param {object} achievementData.progressData - The progress data (e.g., { increment: 1 }).
 * @param {functions.https.CallableContext} context - The context of the function call.
 * @returns {Promise<object>} A promise that resolves with the updated achievement status.
 * @throws {functions.https.HttpsError} For authentication errors or internal issues.
 */
exports.updateAchievementProgress = functions.https.onCall(async (achievementData, context) => {
    logger.info("updateAchievementProgress function called", { userId: context.auth ? context.auth.uid : "anonymous" });

    if (!context.auth || !context.auth.uid) {
        logger.warn("updateAchievementProgress: Authentication required.");
        throw errorHandler.toHttpsError(new functions.https.HttpsError("unauthenticated", "Authentication required to update achievements."));
    }
    const userId = context.auth.uid;

    const { achievementId, progressData } = achievementData;
    if (!achievementId || !progressData) {
        logger.warn("updateAchievementProgress: Invalid payload.", { userId, achievementData });
        throw errorHandler.toHttpsError(new functions.https.HttpsError("invalid-argument", "Invalid payload: achievementId and progressData are required."));
    }

    try {
        const result = await achievementService.updatePlayerAchievement(userId, achievementId, progressData);
        logger.info("updateAchievementProgress: Achievement progress updated.", { userId, achievementId, unlocked: result.unlocked });
        return { success: true, ...result };
    } catch (error) {
        logger.error("updateAchievementProgress: Error updating achievement progress.", error, { userId, achievementId });
        if (error instanceof functions.https.HttpsError) {
            throw error;
        }
        throw errorHandler.toHttpsError(new functions.https.HttpsError("internal", "An error occurred while updating achievement progress."));
    }
});

/**
 * Auth trigger fired when a new Firebase user is created.
 * Initializes a new user profile in Firestore.
 * @async
 * @function onUserCreateAuthTrigger
 * @param {admin.auth.UserRecord} user - The newly created user record.
 * @param {functions.EventContext} context - The event context.
 * @returns {Promise<void>}
 */
exports.onUserCreateAuthTrigger = functions.auth.user().onCreate(async (user, context) => {
    const userId = user.uid;
    const email = user.email;
    const displayName = user.displayName || `Player${userId.substring(0, 5)}`; // Default display name

    logger.info("onUserCreateAuthTrigger: New user created.", { userId, email, eventId: context.eventId });

    try {
        const userProfileData = {
            email: email,
            displayName: displayName,
            createdAt: admin.firestore.FieldValue.serverTimestamp(), // Handled by firestoreService
            lastLogin: admin.firestore.FieldValue.serverTimestamp(), // Handled by firestoreService
            // Initialize other default profile fields as per REPO-DATA-MODELS UserProfileCloudData
            gameProgress: {},
            settings: {},
            totalStars: 0, // Example from database design
        };

        await firestoreService.setDocument(config.USER_PROFILE_COLLECTION_NAME, userId, userProfileData);
        logger.info("onUserCreateAuthTrigger: User profile initialized.", { userId });

        // Optionally initialize achievements
        // await achievementService.initializeUserAchievements(userId);
        // logger.info("onUserCreateAuthTrigger: User achievements initialized.", { userId });

    } catch (error) {
        // Use handleBackgroundError for non-HTTPS functions
        errorHandler.handleBackgroundError(error, { userId, functionName: "onUserCreateAuthTrigger" });
    }
});

/**
 * Auth trigger fired when a Firebase user is deleted.
 * Cleans up associated user data from Firestore.
 * @async
 * @function onUserDeleteAuthTrigger
 * @param {admin.auth.UserRecord} user - The user record being deleted.
 * @param {functions.EventContext} context - The event context.
 * @returns {Promise<void>}
 */
exports.onUserDeleteAuthTrigger = functions.auth.user().onDelete(async (user, context) => {
    const userId = user.uid;
    logger.info("onUserDeleteAuthTrigger: User deletion event received.", { userId, eventId: context.eventId });

    // It's generally recommended to use a more robust solution for cascading deletes,
    // like the "Delete User Data" Firebase Extension, or a carefully managed batch process.
    // Direct recursive deletion in a function can be risky and timeout-prone for large data.
    // For this example, we'll show simple deletions.

    try {
        const db = admin.firestore();
        const batch = db.batch();

        // Delete user profile
        const userProfileRef = db.collection(config.USER_PROFILE_COLLECTION_NAME).doc(userId);
        batch.delete(userProfileRef);

        // Delete cloud save data
        const cloudSaveRef = db.collection(config.USER_CLOUD_SAVE_COLLECTION_NAME).doc(userId);
        batch.delete(cloudSaveRef);

        // Delete achievement data (assuming achievements are stored in a subcollection or top-level collection keyed by userId)
        // If achievements are in a top-level collection /users/{userId}/achievements/{achievementId}
        // or /achievements/{achievementId} with a userId field, querying and deleting is needed.
        // For simplicity, let's assume achievements are in USER_ACHIEVEMENTS_COLLECTION_NAME keyed by userId for user-specific docs
        // or a more complex structure requiring querying.
        // If achievements are per-user documents, a direct delete is possible.
        // Example:
        // const userAchievementsRef = db.collection(config.ACHIEVEMENTS_COLLECTION_NAME).doc(userId); // If structure is one doc per user
        // batch.delete(userAchievementsRef);

        // Query and delete leaderboard entries by this user (can be many)
        // This can be resource-intensive. Consider soft deletes or offline processing.
        const leaderboardEntriesQuery = db.collection(config.LEADERBOARD_COLLECTION_NAME).where("userId", "==", userId);
        const leaderboardSnapshots = await leaderboardEntriesQuery.get();
        leaderboardSnapshots.forEach(doc => batch.delete(doc.ref));


        // Example for deleting user's achievement progress if stored in a collection like /userAchievements/{userId}/progress/{achievementId}
        // This would require listing and deleting subcollections or documents within them.
        // A common pattern is storing achievements in a collection where docs are keyed by achievement ID
        // and contain a userId field, or in a subcollection under the user's profile.
        // For instance, if `ACHIEVEMENTS_COLLECTION_NAME` stores individual achievement progress documents:
        const achievementsQuery = db.collection(config.ACHIEVEMENTS_COLLECTION_NAME).where("userId", "==", userId);
        const achievementSnapshots = await achievementsQuery.get();
        achievementSnapshots.forEach(doc => batch.delete(doc.ref));


        await batch.commit();
        logger.info("onUserDeleteAuthTrigger: User data cleanup successful.", { userId });

    } catch (error) {
        errorHandler.handleBackgroundError(error, { userId, functionName: "onUserDeleteAuthTrigger" });
    }
});