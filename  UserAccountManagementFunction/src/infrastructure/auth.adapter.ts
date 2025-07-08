import { getAuthInstance } from "./firebaseAdmin.client";
import * as logger from "../utils/logger";

/**
 * Deletes a user account from the Firebase Authentication service.
 * @param {string} uid - The UID of the user to delete.
 */
export const deleteAuthUser = async (uid: string): Promise<void> => {
  const auth = getAuthInstance();
  try {
    await auth.deleteUser(uid);
    logger.info(`Auth Adapter: Successfully deleted user ${uid} from Firebase Authentication.`);
  } catch (error: any) {
    // It's not a failure if the user is already gone. Log as a warning.
    if (error.code === 'auth/user-not-found') {
      logger.warn(`Auth Adapter: User ${uid} not found in Firebase Authentication. Assumed already deleted.`);
      return;
    }
    // For other errors, re-throw to be handled by the service layer.
    throw error;
  }
};