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