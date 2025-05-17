/**
 * @file firebaseAdminSetup.js
 * @summary Handles Firebase Admin SDK initialization, making it available throughout the backend functions.
 * @description Initializes and configures the Firebase Admin SDK for server-side use, providing a singleton instance.
 * This module provides a centrally initialized and configured instance of the Firebase Admin SDK,
 * necessary for server-side interactions with Firebase services like Firestore, Auth, etc.
 * Implements the Singleton pattern to ensure a single instance of the Firebase Admin app.
 * Namespace: PatternCipher.Backend.Core
 * @version 1.0.0
 * @since 2024-07-16
 */

const admin = require('firebase-admin');

// Initialize Firebase Admin SDK
// The SDK attempts to find service account credentials automatically if GOOGLE_APPLICATION_CREDENTIALS
// environment variable is set, or if running in a Google Cloud environment (like Cloud Functions).
// For local development, ensure the service account key JSON file path is set in GOOGLE_APPLICATION_CREDENTIALS.
if (!admin.apps.length) {
  try {
    admin.initializeApp();
    console.log('Firebase Admin SDK initialized successfully.');
  } catch (error) {
    console.error('Firebase Admin SDK initialization error:', error);
    // Optionally, re-throw or handle critical failure if SDK is essential for all operations
  }
}

/**
 * The initialized Firebase Admin SDK instance.
 * @type {admin.app.App}
 */
const adminInstance = admin.apps[0]; // Use the default app

/**
 * The Firestore database instance.
 * @type {admin.firestore.Firestore}
 */
const db = adminInstance ? adminInstance.firestore() : null;

if (db) {
  // Optional: Configure Firestore settings, e.g., timestampsInSnapshots
  // db.settings({ timestampsInSnapshots: true }); // Deprecated in newer versions, default behavior now
}

module.exports = {
  admin: adminInstance,
  db,
};