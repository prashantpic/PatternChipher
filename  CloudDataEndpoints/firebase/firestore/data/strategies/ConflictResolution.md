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