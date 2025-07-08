import * as functions from "firebase-functions";

/**
 * Middleware that checks if a user is authenticated in a Callable Function.
 * Throws an 'unauthenticated' error if the user is not signed in.
 *
 * @param {functions.https.CallableContext} context The context object from the callable function.
 * @returns {string} The authenticated user's UID.
 * @throws {functions.https.HttpsError} If the user is not authenticated.
 */
export const ensureAuthenticated = (context: functions.https.CallableContext): string => {
  // Check if the user is authenticated
  if (!context.auth || !context.auth.uid) {
    throw new functions.https.HttpsError(
      "unauthenticated",
      "The function must be called while authenticated."
    );
  }

  // Return the user's UID
  return context.auth.uid;
};