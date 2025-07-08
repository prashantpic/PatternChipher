import { firestore } from "firebase-admin";
import { SubmitScorePayload } from "./validators";
import { LeaderboardEntry } from "../shared/types";
import { UserProfile } from "../shared/types/player";

/**
 * Encapsulates the core business logic for the leaderboard system.
 * It is decoupled from the function trigger and handles all Firestore interactions.
 */
export class LeaderboardService {
  constructor(private db: firestore.Firestore) {}

  /**
   * Processes and submits a user's score to the leaderboard.
   *
   * @param {string} userId The UID of the user submitting the score.
   * @param {SubmitScorePayload} scoreData The validated score submission data.
   * @returns {Promise<void>} A promise that resolves when the operation is complete.
   */
  public async submitUserScore(userId: string, scoreData: SubmitScorePayload): Promise<void> {
    const userProfileRef = this.db.collection("userProfiles").doc(userId);
    const userProfileDoc = await userProfileRef.get();

    if (!userProfileDoc.exists) {
      throw new Error(`User profile not found for userId: ${userId}`);
    }
    const displayName = (userProfileDoc.data() as UserProfile).displayName;

    // TODO: Implement plausibility check against a 'levels' collection.
    // For example, fetch a level config document:
    // const levelConfig = await this.db.collection('levels').doc(scoreData.levelId).get();
    // if (scoreData.score > levelConfig.data()?.maxPossibleScore) {
    //   throw new Error("Submitted score is higher than the maximum possible score.");
    // }

    const leaderboardsRef = this.db.collection("leaderboards");

    // Use a transaction to ensure we only set the score if it's a new high score.
    await this.db.runTransaction(async (transaction) => {
      const existingScoreQuery = leaderboardsRef
        .where("playerId", "==", userId)
        .where("levelId", "==", scoreData.levelId)
        .orderBy("score", "desc")
        .limit(1);

      const existingScoreSnapshot = await transaction.get(existingScoreQuery);

      if (!existingScoreSnapshot.empty) {
        const highestScore = existingScoreSnapshot.docs[0].data().score;
        if (scoreData.score <= highestScore) {
          // New score is not better, so we do nothing.
          console.log(`New score for user ${userId} on level ${scoreData.levelId} is not a high score. Ignoring.`);
          return;
        }
      }

      // If we are here, it's a new high score or the first score for this level.
      const newEntryRef = leaderboardsRef.doc(); // Create a new doc with a random ID.
      const newEntry: LeaderboardEntry = {
        playerId: userId,
        displayName: displayName,
        levelId: scoreData.levelId,
        score: scoreData.score,
        moves: scoreData.moves,
        timeInSeconds: scoreData.timeInSeconds,
        timestamp: firestore.FieldValue.serverTimestamp(),
      };

      transaction.set(newEntryRef, newEntry);
    });
  }
}