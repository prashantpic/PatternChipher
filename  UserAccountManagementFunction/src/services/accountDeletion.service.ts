import * as authAdapter from "../infrastructure/auth.adapter";
import * as firestoreAdapter from "../infrastructure/firestore.adapter";
import * as logger from "../utils/logger";

/**
 * Orchestrates the complete deletion of a user account across all services.
 * The order of operations is critical: data is deleted from Firestore first
 * to prevent orphaned records if the final auth deletion fails.
 * @param {string} uid - The UID of the user to be deleted.
 */
export const deleteUserAccount = async (uid: string): Promise<void> => {
  if (!uid) {
    throw new Error("UID must be provided for account deletion.");
  }
  
  logger.info(`Service: Deleting all Firestore documents for user ${uid}.`);
  await firestoreAdapter.deleteUserDocuments(uid);

  logger.info(`Service: Deleting auth record for user ${uid}.`);
  await authAdapter.deleteAuthUser(uid);
};