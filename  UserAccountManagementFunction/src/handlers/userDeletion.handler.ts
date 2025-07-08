import * as functions from "firebase-functions";
import { UserDeletionRequest } from "../models/userDeletion.dto";
import { deleteUserAccount } from "../services/accountDeletion.service";
import * as logger from "../utils/logger";

/**
 * Handles an HTTPS callable request to delete a user's account and all associated data.
 * @param {UserDeletionRequest} data - The request payload. Must contain the UID of the user to delete.
 * @param {functions.https.CallableContext} context - The context of the call, including authentication information.
 * @returns {Promise<{ success: boolean; message: string; }>} A success or error response.
 */
export const onDeleteUserRequest = async (
  data: UserDeletionRequest,
  context: functions.https.CallableContext
): Promise<{ success: boolean; message: string; }> => {
  // 1. Authentication Check: Ensure the caller is authenticated.
  if (!context.auth) {
    logger.warn("User deletion request from unauthenticated user.", { uidToDrop: data.uid });
    throw new functions.https.HttpsError(
      "unauthenticated",
      "You must be logged in to request account deletion."
    );
  }

  // 2. Authorization Check: Ensure the caller is either an admin or deleting themselves.
  const callerUid = context.auth.uid;
  const targetUid = data.uid;
  const isSelfDelete = callerUid === targetUid;
  // Note: Admin role check is omitted for simplicity but would be added here in a real app.
  // const isAdmin = context.auth.token.admin === true;

  if (!isSelfDelete) { // && !isAdmin) {
    logger.error("Unauthorized user deletion attempt.", { callerUid, targetUid });
    throw new functions.https.HttpsError(
      "permission-denied",
      "You do not have permission to delete this account."
    );
  }

  // 3. Input Validation
  if (!targetUid || typeof targetUid !== 'string') {
    throw new functions.https.HttpsError(
      "invalid-argument",
      "The function must be called with a 'uid' argument."
    );
  }

  logger.info(`Starting account deletion process for UID: ${targetUid}`, { callerUid });

  // 4. Delegate to Service Layer
  try {
    await deleteUserAccount(targetUid);
    logger.info(`Successfully deleted account for UID: ${targetUid}`);
    return { success: true, message: "Account successfully deleted." };
  } catch (error) {
    logger.error(`Failed to delete account for UID: ${targetUid}`, error as Error);
    // Throwing a generic error to the client to avoid leaking implementation details.
    throw new functions.https.HttpsError(
      "internal",
      "An unexpected error occurred while deleting the account. Please contact support."
    );
  }
};