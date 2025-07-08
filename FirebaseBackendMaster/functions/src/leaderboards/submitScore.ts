import * as functions from "firebase-functions";
import * as admin from "firebase-admin";
import { ZodError } from "zod";

import { ensureAuthenticated } from "../shared/middleware/auth";
import { submitScoreSchema } from "./validators";
import { LeaderboardService } from "./service";

/**
 * An HTTPS Callable Cloud Function for submitting a player's score.
 * It serves as the secure entry point, orchestrating authentication,
 * validation, and the business logic service.
 */
export const submitScore = functions.https.onCall(async (data, context) => {
  // TODO: Implement rate limiting to prevent abuse.
  const leaderboardService = new LeaderboardService(admin.firestore());

  try {
    // 1. Ensure the user is authenticated.
    const uid = ensureAuthenticated(context);

    // 2. Validate the incoming data payload against the Zod schema.
    const validatedData = submitScoreSchema.parse(data);

    // 3. Call the service layer to handle the core business logic.
    await leaderboardService.submitUserScore(uid, validatedData);

    // 4. Return a success response.
    return { success: true, message: "Score submitted successfully." };
  } catch (error) {
    if (error instanceof ZodError) {
      // If validation fails, throw a specific 'invalid-argument' error.
      console.error("Validation Error:", error.errors);
      throw new functions.https.HttpsError(
        "invalid-argument",
        "The data provided is invalid.",
        error.errors
      );
    }

    if (error instanceof functions.https.HttpsError) {
      // Re-throw HttpsErrors (like from ensureAuthenticated).
      throw error;
    }
    
    // For all other errors, log them and throw a generic internal error.
    console.error("Internal Error in submitScore:", error);
    throw new functions.https.HttpsError(
      "internal",
      "An unexpected error occurred while submitting the score."
    );
  }
});