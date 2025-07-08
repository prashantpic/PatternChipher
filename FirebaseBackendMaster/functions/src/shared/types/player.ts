import { firestore } from "firebase-admin";

/**
 * Represents the player's cloud save data object.
 * This structure is what gets synced between the client and the cloud.
 */
export interface CloudSaveData {
  schemaVersion: number;
  levelProgress: { [levelId: string]: { score: number; stars: number; } };
  settings: {
    soundVolume: number;
    musicVolume: number;
    // ... other user-configurable settings
  };
}

/**
 * Represents the user's main profile document in Firestore, stored under
 * the `userProfiles` collection.
 */
export interface UserProfile {
  displayName: string;
  photoURL?: string;
  lastSeen: firestore.Timestamp;
  createdAt: firestore.Timestamp;
  cloudSave?: CloudSaveData;
}