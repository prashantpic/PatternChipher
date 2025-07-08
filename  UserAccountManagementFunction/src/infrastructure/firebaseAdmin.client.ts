import * as admin from "firebase-admin";

// Initialize Firebase Admin SDK only if it hasn't been already.
if (admin.apps.length === 0) {
  admin.initializeApp();
}

const firestore = admin.firestore();
const auth = admin.auth();

/**
 * Gets the singleton instance of the Firestore service.
 * @returns {admin.firestore.Firestore} The initialized Firestore instance.
 */
export const getFirestoreInstance = (): admin.firestore.Firestore => firestore;

/**
 * Gets the singleton instance of the Authentication service.
 * @returns {admin.auth.Auth} The initialized Auth instance.
 */
export const getAuthInstance = (): admin.auth.Auth => auth;