using PatternCipher.Client.DataPersistence.Models; // For PlayerProfileData
using UnityEngine; // For Debug.Log

namespace PatternCipher.Client.DataPersistence.Migrators
{
    public class SaveDataMigrator
    {
        /// <summary>
        /// Migrates the PlayerProfileData from its current version to the targetVersion.
        /// </summary>
        /// <param name="data">The PlayerProfileData to migrate.</param>
        /// <param name="targetVersion">The desired schema version.</param>
        /// <returns>The migrated PlayerProfileData, or the original data if no migration was needed or possible.</returns>
        public PlayerProfileData Migrate(PlayerProfileData data, int targetVersion)
        {
            if (data == null)
            {
                Debug.LogError("Cannot migrate null PlayerProfileData.");
                return null;
            }

            if (data.SchemaVersion == targetVersion)
            {
                Debug.Log($"PlayerProfileData is already at target version {targetVersion}. No migration needed.");
                return data;
            }

            if (data.SchemaVersion > targetVersion)
            {
                // Downgrading is typically not supported or very complex.
                Debug.LogError($"Cannot downgrade PlayerProfileData from version {data.SchemaVersion} to {targetVersion}. " +
                               "This scenario is not supported. Returning original data.");
                return data;
            }

            Debug.Log($"Starting migration of PlayerProfileData from version {data.SchemaVersion} to {targetVersion}.");

            PlayerProfileData migratedData = data; // Start with a copy or the original reference

            // Iteratively apply migrations
            while (migratedData.SchemaVersion < targetVersion)
            {
                switch (migratedData.SchemaVersion)
                {
                    case 1:
                        migratedData = MigrateFromV1ToV2(migratedData);
                        break;
                    case 2:
                        migratedData = MigrateFromV2ToV3(migratedData);
                        break;
                    // Add more cases for each version step
                    // case N:
                    //     migratedData = MigrateFromVNToVNPlus1(migratedData);
                    //     break;
                    default:
                        Debug.LogError($"No migration path found for PlayerProfileData from version {migratedData.SchemaVersion}. " +
                                       "Stopping migration. Data might be in an intermediate state.");
                        return migratedData; // Return data in its current (partially migrated) state
                }

                if (migratedData == null) // A migration step failed critically
                {
                    Debug.LogError($"Migration from version {data.SchemaVersion} failed. Cannot proceed.");
                    return data; // Return original data or handle error appropriately
                }
            }
            
            Debug.Log($"Migration completed. PlayerProfileData is now at version {migratedData.SchemaVersion}.");
            return migratedData;
        }

        // --- Example Migration Methods ---
        // Each method handles migration from one specific version to the next.

        private PlayerProfileData MigrateFromV1ToV2(PlayerProfileData v1Data)
        {
            Debug.Log("Migrating PlayerProfileData from V1 to V2...");
            // Create a new V2 data object
            PlayerProfileData v2Data = new PlayerProfileData(); // Assuming default constructor or copy relevant fields
            
            // Copy common fields
            v2Data.UserId = v1Data.UserId;
            v2Data.TotalStars = v1Data.TotalStars;
            // ... copy other existing fields ...

            // Apply V1 to V2 changes:
            // Example: A new field 'LastPlayedLevelId' was added in V2, default to 0 or null.
            // v2Data.LastPlayedLevelId = 0; 
            
            // Example: A field 'PlayerName' was renamed to 'DisplayName'.
            // v2Data.DisplayName = v1Data.PlayerName; // Assuming PlayerName existed in V1 PlayerProfileData definition

            // Example: LevelRecords structure changed
            // if (v1Data.LevelScores != null) // Assuming old structure was LevelScores: Dictionary<int, int>
            // {
            //     v2Data.LevelRecords = new System.Collections.Generic.Dictionary<int, LevelRecord>();
            //     foreach(var kvp in v1Data.LevelScores)
            //     {
            //         v2Data.LevelRecords.Add(kvp.Key, new LevelRecord { Stars = 0, BestScore = kvp.Value }); // Default stars
            //     }
            // }


            v2Data.SchemaVersion = 2; // IMPORTANT: Update the schema version
            Debug.Log("Migration from V1 to V2 complete.");
            return v2Data;
        }

        private PlayerProfileData MigrateFromV2ToV3(PlayerProfileData v2Data)
        {
            Debug.Log("Migrating PlayerProfileData from V2 to V3...");
            PlayerProfileData v3Data = new PlayerProfileData();
            // Copy fields from v2Data...
            v3Data.UserId = v2Data.UserId;
            // ...

            // Apply V2 to V3 changes:
            // Example: Settings object was introduced.
            // v3Data.Settings = new GameSettings(); // Initialize with default settings
            // if (v2Data.HadSeparateVolumeField) v3Data.Settings.Volume = v2Data.OldVolumeField;


            v3Data.SchemaVersion = 3; // IMPORTANT
            Debug.Log("Migration from V2 to V3 complete.");
            return v3Data;
        }

        // Add more migration methods as needed:
        // private PlayerProfileData MigrateFromV3ToV4(PlayerProfileData v3Data) { /* ... */ }
    }
}