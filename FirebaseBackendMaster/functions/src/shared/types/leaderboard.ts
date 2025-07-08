import { firestore } from "firebase-admin";

/**
 * Represents a single entry in the `leaderboards` collection in Firestore.
 */
export interface LeaderboardEntry {
  playerId: string; // Corresponds to Firebase Auth UID
  displayName: string;
  score: number;
  levelId: string;
  timestamp: firestore.FieldValue; // Should be ServerValue.TIMESTAMP on creation
  moves?: number;
  timeInSeconds?: number;
}