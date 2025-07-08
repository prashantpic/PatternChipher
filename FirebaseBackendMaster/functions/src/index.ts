import * as admin from "firebase-admin";
import * as functions from "firebase-functions";

// Initialize the Firebase Admin SDK.
// This must be done once and is used by all functions.
admin.initializeApp();

// --- Import and re-export functions from feature modules ---

// Import functions related to leaderboards
import { submitScore } from "./leaderboards/submitScore";

// Import functions related to account management
import { cleanupUserData } from "./accounts/onUserDelete";

// Group and export functions for clear organization and deployment.
// This results in function names like 'leaderboards-submitScore'.
export const leaderboards = {
  submitScore,
};

export const accounts = {
  cleanupUserData,
};