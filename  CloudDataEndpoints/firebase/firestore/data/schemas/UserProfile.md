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