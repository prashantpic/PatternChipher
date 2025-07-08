import { getFirestoreInstance } from "./firebaseAdmin.client";
import * as logger from "../utils/logger";

/**
 * Deletes all documents related to a specific user from Cloud Firestore.
 * This includes their main user profile and any dependent data like leaderboard entries.
 * @param {string} uid - The UID of the user whose documents should be deleted.
 */
export const deleteUserDocuments = async (uid: string): Promise<void> => {
  const db = getFirestoreInstance();
  const batch = db.batch();

  // 1. Delete the main user profile document
  const userProfileRef = db.collection("UserProfiles").doc(uid);
  batch.delete(userProfileRef);
  logger.info(`Firestore Adapter: Queued deletion for UserProfile: ${userProfileRef.path}`);

  // 2. Query and delete related data (e.g., Leaderboard Entries)
  // This is an example. Add other collections as needed.
  const leaderboardEntriesRef = db.collection("LeaderboardEntries").where("userId", "==", uid);
  const leaderboardSnapshot = await leaderboardEntriesRef.get();
  if (!leaderboardSnapshot.empty) {
    leaderboardSnapshot.forEach(doc => {
      batch.delete(doc.ref);
      logger.info(`Firestore Adapter: Queued deletion for LeaderboardEntry: ${doc.ref.path}`);
    });
  }

  // Add queries for other collections like AchievementStatus if they exist
  // const achievementStatusRef = db.collection("AchievementStatuses").where("userId", "==", uid);
  // ...

  // 3. Commit the batched deletions
  await batch.commit();
  logger.info(`Firestore Adapter: Committed deletions for user ${uid}.`);
};