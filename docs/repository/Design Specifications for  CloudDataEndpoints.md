# Software Design Specification (SDS) for CloudDataEndpoints

## 1. Introduction

This document provides the detailed software design specification for the `CloudDataEndpoints` repository. This repository is not a traditional code repository; instead, it defines the configuration, schema, security, and policies for the Cloud Firestore database used by the Pattern Cipher game.

Its primary purpose is to support the optional **Cloud Save** feature, allowing players to synchronize their progress and settings across multiple devices. It also serves as the data store for other potential online features like leaderboards and achievements, though the initial focus is on cloud save.

The specifications below define the content of the configuration and documentation files that constitute this repository.

## 2. Firestore Collection Structure

The database will utilize a primary top-level collection to store all user-specific data.

- **`userProfiles`**: A collection where each document represents a single player. The document ID for each player's profile will be their unique Firebase Authentication UID. This structure ensures natural data partitioning and simplifies security rules.

## 3. Data Schema (`schemas/UserProfile.md`)

This section specifies the data model for a document within the `userProfiles` collection, as required by **DM-002**.

---
### UserProfile Schema (Version 1.0)

**Collection:** `userProfiles`
**Document ID:** `{userId}` (Firebase Authentication UID)

This document stores all cloud-synchronized data for a single user.

| Field Name | Data Type | Required | Description |
| :--- | :--- | :--- | :--- |
| `cloud_save_data_object_version`| `String` | Yes | The schema version of the `cloud_save_data_object`. Used for client-side data migration. e.g., "1.0". |
| `timestamp_of_last_cloud_sync` | `ServerTimestamp` | Yes | The server-side timestamp of the last successful write. Used as the source of truth for conflict resolution (**DM-006**). |
| `user_profile_schema_version` | `String` | Yes | The schema version of this top-level UserProfile document. e.g., "1.0". |
| `cloud_save_data_object` | `Map (Object)` | Yes | A map containing the player's actual progress and settings, mirroring the local save data (**DM-001**). |
| &nbsp;&nbsp;&nbsp;`level_completion_status` | `Map<String, Map>` | Yes | A map where the key is `levelId` and the value is a map containing `stars_earned` (INT) and `best_score` (INT). |
| &nbsp;&nbsp;&nbsp;`player_settings` | `Map` | Yes | A map containing all user-configurable settings. |
| &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;`bgm_volume` | `Number (0.0-1.0)`| Yes | Background music volume. |
| &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;`sfx_volume` | `Number (0.0-1.0)`| Yes | Sound effects volume. |
| &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;`accessibility_mode`| `String` | Yes | e.g., "default", "deuteranopia", "high_contrast". |
| &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;`analytics_opt_in_status`| `Boolean` | Yes | User's consent status for analytics collection. |
| &nbsp;&nbsp;&nbsp;`unlocked_features` | `Array<String>` | Yes | A list of identifiers for unlocked level packs or major features. |

---

## 4. Security Rules (`firestore.rules`)

This file implements the security model for the Firestore database as required by **NFR-SEC-004**. The rules enforce data isolation, ensuring users can only access their own data, and validate the schema of incoming data on write operations.

firestore-rules
rules_version = '2';
service cloud.firestore {
  match /databases/{database}/documents {

    // --- User Profiles Collection ---
    // Documents are keyed by the user's Firebase Auth UID.
    match /userProfiles/{userId} {

      // HELPER FUNCTION: Schema Validation for UserProfile v1.0
      function validateUserProfileV1(data) {
        return data.user_profile_schema_version == "1.0"
          && data.cloud_save_data_object_version is string
          && data.cloud_save_data_object is map
          && data.cloud_save_data_object.player_settings is map
          && data.cloud_save_data_object.level_completion_status is map
          && data.cloud_save_data_object.unlocked_features is list
          && request.time == data.timestamp_of_last_cloud_sync; // Enforce server timestamp
      }

      // --- PERMISSIONS ---

      // READ: A user can only read their own profile document.
      allow read: if request.auth != null && request.auth.uid == userId;

      // CREATE: A user can only create their own profile document.
      // The write must conform to the defined schema.
      allow create: if request.auth != null && request.auth.uid == userId
                      && validateUserProfileV1(request.resource.data);

      // UPDATE: A user can only update their own profile document.
      // The write must conform to the defined schema.
      allow update: if request.auth != null && request.auth.uid == userId
                      && validateUserProfileV1(request.resource.data);

      // DELETE: A user can only delete their own profile document.
      // This supports GDPR/CCPA data deletion requests.
      allow delete: if request.auth != null && request.auth.uid == userId;

      // DENY global list access to the collection.
      // This prevents users from discovering other user IDs.
      allow list: if false;
    }

    // --- Default Deny ---
    // Deny access to any other collections not explicitly defined.
    match /{document=**} {
      allow read, write: if false;
    }
  }
}


## 5. Database Indexes (`firestore.indexes.json`)

This file defines composite indexes for Firestore. Initially, no complex queries are required, so this file will contain a minimal placeholder structure. It establishes the pattern for future use if administrative or feature queries are needed.

json
{
  "indexes": [
    // {
    //   "collectionGroup": "userProfiles",
    //   "queryScope": "COLLECTION",
    //   "fields": [
    //     { "fieldPath": "some_future_field", "order": "ASCENDING" },
    //     { "fieldPath": "timestamp_of_last_cloud_sync", "order": "DESCENDING" }
    //   ]
    // }
  ],
  "fieldOverrides": []
}


## 6. Backup and Recovery Policy (`backups/backup_policy.md`)

This document outlines the strategy for data durability and disaster recovery, fulfilling requirement **NFR-BS-004**.

---
### Firestore Backup and Recovery Policy

**1. Policy Objective**
To ensure the durability and availability of all user data stored in Cloud Firestore for the Pattern Cipher application, and to establish clear objectives and procedures for data recovery in the event of a catastrophic data loss incident.

**2. Backup Strategy**
- The primary backup mechanism will be Cloud Firestore's built-in **Point-in-Time Recovery (PITR)** feature.
- PITR will be enabled for the production Firestore database, providing continuous backups and allowing for restoration to any microsecond within the last 7 days.

**3. Recovery Objectives**
- **Recovery Point Objective (RPO): 24 hours.** This is a worst-case scenario. With PITR, the RPO is effectively near-zero (minutes), but we set a 24-hour formal objective to account for detection and decision-making time.
- **Recovery Time Objective (RTO): 4 hours.** This is the target time from the moment a disaster is declared to the time the database is restored and operational.

**4. Recovery Procedure (High-Level)**
1.  **Incident Declaration:** The operations lead declares a disaster recovery event.
2.  **PITR Restore:** Using the Google Cloud Console or gcloud CLI, initiate a restore of the production database to a new database instance from a specific timestamp just prior to the data corruption event.
3.  **Validation:** Perform integrity checks on the restored data to ensure its validity.
4.  **Traffic Switch:** Update application configuration to point to the newly restored database instance.
5.  **Post-Mortem:** Conduct a post-mortem analysis to determine the root cause and prevent future occurrences.

**5. Validation**
- A simulated recovery drill will be conducted semi-annually to validate the documented procedure and confirm that the RTO can be met.

---

## 7. Cloud Save Strategy Documents

### 7.1. Conflict Resolution Strategy (`strategies/ConflictResolution.md`)

This document specifies the logic for handling data synchronization conflicts, as required by **DM-006**.

---
### Cloud Save Conflict Resolution Strategy

**1. Primary Strategy: Last Write Wins**
- The system will use a "Last Write Wins" strategy as the default conflict resolution mechanism.
- The authority for "last write" is the `timestamp_of_last_cloud_sync` field in the Firestore document, which is a **server-side timestamp**. Client-side timestamps are not trusted for this purpose.

**2. Client-Side Write Flow**
To prevent overwriting newer data, the client application MUST follow this sequence before writing to the cloud:
1.  Read the `timestamp_of_last_cloud_sync` from the user's cloud document.
2.  Compare it with the timestamp of the last *successful sync* known to the local client.
3.  **If the cloud timestamp is newer**, the client's local data is stale. The client MUST first download and apply the newer cloud data before attempting another write. This prevents overwriting progress made on another device.
4.  **If the cloud timestamp is not newer**, the client can proceed with the write operation.

**3. Initial Sync / New Device Conflict**
- When a user logs in on a new device or enables cloud save for the first time on a device with existing local progress, a potential conflict exists.
- **Flow:**
    1.  The client checks for existing data both locally and in the cloud.
    2.  If data exists in only one location (local or cloud), that data becomes the source of truth, and a sync is performed.
    3.  If data exists in both locations and they differ significantly (e.g., total stars differ by > 5, last played timestamp differs by > 1 hour), the user **MUST be prompted** to choose which save state to keep.
    4.  The prompt will display key metrics from both save states to help the user decide (e.g., "Local Save: 50 Stars, Last Played: 2 hours ago" vs. "Cloud Save: 75 Stars, Last Played: Yesterday").
    5.  The unchosen save state will be discarded, and the chosen state becomes the authoritative version on both local and cloud storage.

---

### 7.2. Synchronization Triggers (`strategies/SyncTriggers.md`)

This document defines the client-side events that trigger a cloud save, as required by **DM-005**.

---
### Cloud Save Synchronization Triggers

**Objective:** To ensure player data is synchronized in a timely manner, balancing data freshness with network and battery efficiency.

A cloud save sync should only be attempted if the user is authenticated, has cloud save enabled, and local data has changed since the last successful sync.

**1. Automatic Triggers**
- **Application Pause/Quit:** When the application is paused or is about to quit (e.g., `OnApplicationPause(true)` or `OnApplicationQuit()` in Unity). A debounce mechanism (e.g., 5-second cooldown) will prevent excessive syncs from rapid app switching.
- **Significant Progress:** Upon completion of a significant gameplay milestone. This includes:
    - Completing a level pack.
    - Unlocking a major new feature or puzzle type for the first time.
- **After Level Completion:** A sync will be triggered after a player successfully completes a level and returns to the level selection screen.

**2. Manual Trigger**
- A "Sync Now" button will be provided in the game's **Settings** menu.
- This allows the user to manually force a data synchronization at any time, providing them with explicit control and peace of mind before switching devices.
- The UI should provide feedback on the sync status (e.g., "Syncing...", "Last synced: Just now", "Sync failed").

---