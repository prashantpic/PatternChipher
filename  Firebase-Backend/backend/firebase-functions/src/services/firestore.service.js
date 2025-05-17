const { admin, db } = require("../core/firebaseAdminSetup"); // Assumes firebaseAdminSetup exports db
const logger = require("../core/logger");
const _ = require('lodash');

/**
 * @file Helper service for Firestore operations. Encapsulates common database interaction patterns.
 * @namespace PatternCipher.Backend.Infrastructure.Firestore
 * REQ-9-015: Firestore Data Access Abstraction
 */

/**
 * Retrieves a single document from Firestore.
 * @async
 * @function getDocument
 * @param {string} collectionPath - The path to the Firestore collection.
 * @param {string} documentId - The ID of the document to retrieve.
 * @returns {Promise<object|null>} A promise that resolves with the document data if found, otherwise null.
 */
const getDocument = async (collectionPath, documentId) => {
    logger.debug(`getDocument called`, { collectionPath, documentId });
    try {
        const docRef = db.collection(collectionPath).doc(documentId);
        const docSnap = await docRef.get();
        if (docSnap.exists) {
            logger.debug(`Document found`, { collectionPath, documentId, data: docSnap.data() });
            return { id: docSnap.id, ...docSnap.data() };
        } else {
            logger.debug(`Document not found`, { collectionPath, documentId });
            return null;
        }
    } catch (error) {
        logger.error(`Error getting document ${collectionPath}/${documentId}`, error);
        throw error; // Re-throw to be handled by the caller
    }
};

/**
 * Sets (creates or overwrites) a document in Firestore.
 * Automatically adds `created_at` and `updated_at` server timestamps.
 * @async
 * @function setDocument
 * @param {string} collectionPath - The path to the Firestore collection.
 * @param {string} documentId - The ID of the document to set.
 * @param {object} data - The data to set in the document.
 * @param {object} [options] - Firestore set options (e.g., { merge: true }).
 * @returns {Promise<void>}
 */
const setDocument = async (collectionPath, documentId, data, options = {}) => {
    logger.debug(`setDocument called`, { collectionPath, documentId, data: !_.isEmpty(data) ? 'data provided' : 'no data', options });
    try {
        const docRef = db.collection(collectionPath).doc(documentId);
        const timestamp = admin.firestore.FieldValue.serverTimestamp();
        
        let dataWithTimestamps = {
            ...data,
            updated_at: timestamp,
        };

        // Add created_at only if it's not a merge operation that might preserve an existing created_at
        // Or if it's a full overwrite, then created_at should be set.
        // A simple way: if not merging, or merging but it's effectively a create.
        // For simplicity, we'll add created_at if it's not a merge or if merging with intent to create if not exists.
        // If options.merge is true, we might want to preserve existing created_at.
        // A common pattern: set created_at only on true creation.
        // Firestore's set with merge won't add created_at if it's not in `data`.
        // If we want to ensure created_at on first write:
        if (!options.merge || options.mergeFields) { // if full overwrite or specific fields merge (not deep merge)
            const currentDoc = await docRef.get();
            if (!currentDoc.exists) {
                dataWithTimestamps.created_at = timestamp;
            }
        } else if (options.merge && _.isEmpty(data)) {
            // If merging an empty object, it's usually to just update timestamp.
            // Ensure created_at is not accidentally set here.
        } else if (!options.merge) { // Full overwrite
             dataWithTimestamps.created_at = timestamp;
        }


        await docRef.set(dataWithTimestamps, options);
        logger.debug(`Document set successfully`, { collectionPath, documentId });
    } catch (error) {
        logger.error(`Error setting document ${collectionPath}/${documentId}`, error);
        throw error;
    }
};

/**
 * Updates specific fields in a document in Firestore.
 * Automatically updates the `updated_at` server timestamp.
 * @async
 * @function updateDocument
 * @param {string} collectionPath - The path to the Firestore collection.
 * @param {string} documentId - The ID of the document to update.
 * @param {object} data - An object containing the fields to update.
 * @returns {Promise<void>}
 */
const updateDocument = async (collectionPath, documentId, data) => {
    logger.debug(`updateDocument called`, { collectionPath, documentId, data: !_.isEmpty(data) ? 'data provided' : 'no data' });
    if (_.isEmpty(data)) {
        logger.warn(`updateDocument called with empty data for ${collectionPath}/${documentId}. Only timestamp will be updated if document exists.`);
        // Firestore update with empty data throws error, but we can ensure updated_at is touched if needed.
        // For now, let's assume non-empty data or let Firestore handle the error.
    }
    try {
        const docRef = db.collection(collectionPath).doc(documentId);
        const dataWithTimestamp = {
            ...data,
            updated_at: admin.firestore.FieldValue.serverTimestamp(),
        };
        await docRef.update(dataWithTimestamp);
        logger.debug(`Document updated successfully`, { collectionPath, documentId });
    } catch (error) {
        logger.error(`Error updating document ${collectionPath}/${documentId}`, error);
        throw error;
    }
};

/**
 * Deletes a document from Firestore.
 * @async
 * @function deleteDocument
 * @param {string} collectionPath - The path to the Firestore collection.
 * @param {string} documentId - The ID of the document to delete.
 * @returns {Promise<void>}
 */
const deleteDocument = async (collectionPath, documentId) => {
    logger.debug(`deleteDocument called`, { collectionPath, documentId });
    try {
        const docRef = db.collection(collectionPath).doc(documentId);
        await docRef.delete();
        logger.debug(`Document deleted successfully`, { collectionPath, documentId });
    } catch (error) {
        logger.error(`Error deleting document ${collectionPath}/${documentId}`, error);
        throw error;
    }
};

/**
 * Queries a collection in Firestore based on specified filters, ordering, and limit.
 * @async
 * @function queryCollection
 * @param {string} collectionPath - The path to the Firestore collection.
 * @param {Array<object>} [queryFilters=[]] - An array of filter objects (e.g., { field: 'levelId', operator: '==', value: 'level_1' }).
 * @param {Array<object>} [orderBy=[]] - An array of order objects (e.g., { field: 'score', direction: 'desc' }).
 * @param {number} [limitVal] - The maximum number of documents to return.
 * @returns {Promise<Array<object>>} A promise that resolves with an array of document data.
 */
const queryCollection = async (collectionPath, queryFilters = [], orderBy = [], limitVal) => {
    logger.debug(`queryCollection called`, { collectionPath, queryFilters, orderBy, limitVal });
    try {
        let query = db.collection(collectionPath);

        queryFilters.forEach(filter => {
            if (filter.field && filter.operator && filter.value !== undefined) {
                query = query.where(filter.field, filter.operator, filter.value);
            } else {
                logger.warn('Invalid filter object:', filter);
            }
        });

        orderBy.forEach(order => {
            if (order.field) {
                query = query.orderBy(order.field, order.direction || 'asc');
            } else {
                logger.warn('Invalid orderBy object:', order);
            }
        });

        if (limitVal && typeof limitVal === 'number' && limitVal > 0) {
            query = query.limit(limitVal);
        }

        const snapshot = await query.get();
        const documents = [];
        snapshot.forEach(doc => {
            documents.push({ id: doc.id, ...doc.data() });
        });

        logger.debug(`Query executed successfully, ${documents.length} documents found`, { collectionPath });
        return documents;
    } catch (error) {
        logger.error(`Error querying collection ${collectionPath}`, error);
        throw error;
    }
};

module.exports = {
    getDocument,
    setDocument,
    updateDocument,
    deleteDocument,
    queryCollection,
};