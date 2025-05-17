/**
 * @file cloudsave.service.js
 * @summary Application service for managing cloud save data, including synchronization and conflict resolution logic.
 * @description Contains application service logic for cloud save synchronization, including data validation and conflict resolution strategies.
 * Handles CRUD operations for cloud save data in Firestore via `FirestoreService`.
 * Namespace: PatternCipher.Backend.UserData.Application
 * @version 1.0.0
 * @since 2024-07-16
 */

const firestoreService = require('../services/firestore.service.js'); // Assuming firestore.service.js exists
const config = require('../config/index.js'); // Assuming config/index.js exists for collection names
const logger = require('../core/logger'); // Assuming logger.js exists
const _ = require('lodash'); // Utility library

const { USER_CLOUD_SAVE_COLLECTION_NAME } = config;

/**
 * Saves user data to the cloud, handling potential conflicts.
 * REQ-PDP-006, REQ-9-006, REQ-9-007
 *
 * @async
 * @param {string} userId - The ID of the user.
 * @param {object} clientSaveData - The save data from the client. Conforms to CloudSaveSchema.
 * @param {number} clientTimestamp - The timestamp (milliseconds since epoch) from the client indicating when the data was last saved on the client.
 * @returns {Promise<{status: string, mergedData?: object, serverTimestamp: number | object}>}
 *          An object indicating the status ('saved', 'conflict', 'merged', 'error'),
 *          the resulting data, and the server timestamp of the operation.
 *          serverTimestamp will be a Firebase ServerTimestamp placeholder if a write occurred.
 */
async function saveUserData(userId, clientSaveData, clientTimestamp) {
    if (!userId || !clientSaveData || clientTimestamp == null) {
        logger.warn('Invalid parameters for saveUserData', { userId, clientSaveData, clientTimestamp });
        return { status: 'error', message: 'Invalid parameters.', serverTimestamp: Date.now() };
    }

    const docPath = `${USER_CLOUD_SAVE_COLLECTION_NAME}/${userId}`;

    try {
        const currentServerDoc = await firestoreService.getDocument(USER_CLOUD_SAVE_COLLECTION_NAME, userId);
        
        let dataToSave = clientSaveData;
        let status = 'saved'; // Default status

        if (currentServerDoc && currentServerDoc.data) {
            const serverData = currentServerDoc.data;
            // Ensure serverData.updated_at is a comparable timestamp.
            // Firestore server timestamps are objects; convert to milliseconds if needed for comparison,
            // or rely on Firestore's internal comparison if timestamps are directly comparable.
            // For simplicity, assuming `updated_at` is a Firestore Timestamp that `firestoreService` returns,
            // and it has a `toMillis()` method.
            const serverTimestampMillis = serverData.updated_at?.toMillis ? serverData.updated_at.toMillis() : (serverData.updated_at || 0);


            if (clientTimestamp < serverTimestampMillis) {
                // Conflict: Client data is older than server data
                logger.info(`Cloud save conflict detected for user ${userId}. Client ts: ${clientTimestamp}, Server ts: ${serverTimestampMillis}`, { clientSaveData, serverData });
                // Apply conflict resolution strategy. Default: Last Write Wins (LWW) based on server timestamp, so server data prevails.
                // Or, signal conflict to client and return server data.
                // For this implementation, we'll follow LWW: if client is older, server wins implicitly by not overwriting.
                // We can enhance `resolveConflict` or return server data.
                const resolvedData = resolveConflict(clientSaveData, serverData, 'server_wins_on_older_client_ts'); // Or 'last_write_wins' effectively
                
                if (!_.isEqual(resolvedData, serverData)) { // If resolution modified something (e.g. a merge attempt)
                    dataToSave = resolvedData;
                    status = 'merged'; // Or 'conflict_resolved_by_merge'
                } else {
                     // Client data is older, server data is more recent. Return server data.
                    logger.info(`Cloud save for user ${userId}: Server data is newer. Client data not saved.`, { userId });
                    return { status: 'conflict_server_newer', mergedData: serverData, serverTimestamp: serverTimestampMillis };
                }

            } else {
                // Client data is newer or same age, proceed to save client data (LWW).
                status = 'saved'; // Or 'updated'
            }
        }
        // `firestoreService.setDocument` or `updateDocument` should handle setting `updated_at` to serverTimestamp.
        // The `dataToSave` should not include `updated_at` from the client.
        const finalPayload = { ...dataToSave }; 
        // delete finalPayload.updated_at; // Ensure client doesn't try to set this reserved field

        await firestoreService.setDocument(USER_CLOUD_SAVE_COLLECTION_NAME, userId, finalPayload, { merge: true }); // Merge true to not overwrite fields not in payload
        const serverTime = firestoreService.getServerTimestamp(); // Conceptual: get the placeholder

        logger.info(`User data ${status} for ${userId}`, { userId });
        return { status, mergedData: finalPayload, serverTimestamp: serverTime };

    } catch (error) {
        logger.error('Error saving user data', error, { userId });
        return { status: 'error', message: error.message, serverTimestamp: Date.now() };
    }
}

/**
 * Loads user's cloud save data.
 * REQ-PDP-006, REQ-9-006
 *
 * @async
 * @param {string} userId - The ID of the user.
 * @returns {Promise<object | null>} The user's cloud save data, or null if not found or error.
 */
async function loadUserData(userId) {
    if (!userId) {
        logger.warn('Invalid userId for loadUserData');
        return null;
    }
    try {
        const doc = await firestoreService.getDocument(USER_CLOUD_SAVE_COLLECTION_NAME, userId);
        if (doc && doc.exists) {
            logger.info(`User data loaded for ${userId}`, { userId });
            return doc.data; // `data` should include the `updated_at` server timestamp
        }
        logger.info(`No cloud save data found for user ${userId}`, { userId });
        return null;
    } catch (error) {
        logger.error('Error loading user data', error, { userId });
        return null;
    }
}

/**
 * Resolves conflicts between local (client) and remote (server) data.
 * REQ-9-007
 * For initial implementation, it might be simple LWW based on server timestamp (handled in `saveUserData`)
 * or a specific strategy like "server wins".
 * More complex strategies (field-level merge) can be added here.
 *
 * @param {object} clientData - The data from the client.
 * @param {object} serverData - The data from the server.
 * @param {string} strategy - The conflict resolution strategy to apply (e.g., 'last_write_wins', 'server_wins', 'client_wins', 'merge_fields').
 * @returns {object} The resolved data.
 */
function resolveConflict(clientData, serverData, strategy) {
    logger.info('Resolving cloud save conflict', { strategy, clientDataKeys: Object.keys(clientData || {}), serverDataKeys: Object.keys(serverData || {}) });
    switch (strategy) {
        case 'server_wins':
        case 'server_wins_on_older_client_ts': // Used when client's timestamp is older
            return serverData;
        case 'client_wins':
            return clientData;
        case 'last_write_wins':
            // This strategy is typically determined by comparing timestamps *before* calling this function.
            // If called, it implies one is chosen based on a timestamp. For example, if serverData.updated_at > clientData.clientTimestamp
            // then serverData would be chosen. This function might just return one based on which one was deemed "later".
            // For now, let's assume if this is called, server data is preferred if timestamps are equal or server is newer.
             return serverData; // Or clientData if it's newer based on external timestamp check.

        case 'merge_fields': // Example of a more complex strategy
            // This requires a deep understanding of the data model.
            // Using lodash.merge for a deep merge, client data overwrites server data for shared keys.
            // More sophisticated merging might be needed for specific fields (e.g., arrays, counters).
            return _.merge({}, serverData, clientData);
        default:
            logger.warn(`Unknown conflict resolution strategy: ${strategy}. Defaulting to server_wins.`);
            return serverData; // Default to server data to be safe
    }
}

module.exports = {
    saveUserData,
    loadUserData,
    resolveConflict,
};