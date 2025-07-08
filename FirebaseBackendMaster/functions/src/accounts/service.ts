import { firestore } from "firebase-admin";

/**
 * Encapsulates the business logic for user account management,
 * such as data cleanup upon account deletion.
 */
export class AccountService {
  constructor(private db: firestore.Firestore) {}

  /**
   * Deletes all data associated with a specific user ID from Firestore.
   * This is critical for GDPR 'right to be forgotten' compliance.
   *
   * @param {string} userId The UID of the user whose data should be deleted.
   * @returns {Promise<void>} A promise that resolves when all data has been deleted.
   */
  public async deleteAllDataForUser(userId: string): Promise<void> {
    const batch = this.db.batch();

    // 1. Delete the user's main profile document.
    const userProfileRef = this.db.collection("userProfiles").doc(userId);
    batch.delete(userProfileRef);

    // 2. Query and delete all leaderboard entries for the user.
    const leaderboardsRef = this.db.collection("leaderboards");
    const leaderboardQuery = leaderboardsRef.where("playerId", "==", userId);
    const leaderboardSnapshot = await leaderboardQuery.get();
    
    leaderboardSnapshot.docs.forEach((doc) => {
      batch.delete(doc.ref);
    });

    // 3. Query and delete all achievements for the user.
    const achievementsRef = this.db.collection(`userProfiles/${userId}/achievements`);
    const achievementsSnapshot = await achievementsRef.get();

    achievementsSnapshot.docs.forEach((doc) => {
        batch.delete(doc.ref);
    });

    // TODO: Add deletion logic for any other user-specific collections.

    // 4. Commit all delete operations in a single atomic batch.
    await batch.commit();
  }
}