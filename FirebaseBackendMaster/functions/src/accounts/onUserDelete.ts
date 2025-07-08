import * as functions from "firebase-functions";
import * as admin from "firebase-admin";
import { AccountService } from "./service";

/**
 * An Authentication-triggered Cloud Function that cleans up user data from
 * Firestore and other services after a user account is deleted from Firebase Auth.
 */
export const cleanupUserData = functions.auth.user().onDelete(async (user) => {
  const { uid } = user;
  console.log(`Starting cleanup process for deleted user: ${uid}`);

  const accountService = new AccountService(admin.firestore());

  try {
    await accountService.deleteAllDataForUser(uid);
    console.log(`Successfully cleaned up all data for user: ${uid}`);
  } catch (error) {
    console.error(`Error cleaning up data for user ${uid}:`, error);
    // You might want to add more robust error reporting here,
    // e.g., to a dedicated monitoring service.
  }
});