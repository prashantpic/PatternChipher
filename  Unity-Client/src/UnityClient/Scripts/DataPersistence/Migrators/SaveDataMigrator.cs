using UnityEngine;
using PatternCipher.Client.DataPersistence.Models; // For PlayerProfileData

namespace PatternCipher.Client.DataPersistence.Migrators
{
    public class SaveDataMigrator
    {
        // The target version this migrator aims to bring data to.
        // This should match PlayerProfileData.CurrentSchemaVersion
        private readonly int _currentTargetVersion;

        public SaveDataMigrator(int currentTargetSchemaVersion)
        {
            _currentTargetVersion = currentTargetSchemaVersion;
        }

        public PlayerProfileData Migrate(PlayerProfileData data)
        {
            if (data == null)
            {
                Debug.LogError("Cannot migrate null PlayerProfileData.");
                return null; 
            }

            if (data.SchemaVersion == _currentTargetVersion)
            {
                Debug.Log($"Data schema version {data.SchemaVersion} is current. No migration needed.");
                return data; // No migration needed
            }

            if (data.SchemaVersion > _currentTargetVersion)
            {
                Debug.LogError($"Data schema version {data.SchemaVersion} is newer than target version {_currentTargetVersion}. Cannot downgrade. Data might be corrupted or from a future version.");
                // Potentially offer to reset data or handle as an error.
                return data; // Or throw an exception / return null
            }
            
            Debug.Log($"Starting migration for data from version {data.SchemaVersion} to {_currentTargetVersion}.");

            // Iteratively apply migrations
            PlayerProfileData migratedData = data; // Start with the data to be migrated
            while (migratedData.SchemaVersion < _currentTargetVersion)
            {
                migratedData = ApplyNextMigrationStep(migratedData);
                if (migratedData == null) // A migration step failed
                {
                    Debug.LogError($"Migration failed at version {data.SchemaVersion}. Cannot proceed.");
                    return null; // Or return original data / throw
                }
            }
            
            Debug.Log($"Migration completed. Data is now at version {migratedData.SchemaVersion}.");
            return migratedData;
        }

        private PlayerProfileData ApplyNextMigrationStep(PlayerProfileData oldData)
        {
            // This switch will apply one migration step at a time.
            // Each case should increment oldData.SchemaVersion.
            switch (oldData.SchemaVersion)
            {
                case 1:
                    // Example: Migrate from version 1 to version 2
                    // PlayerProfileDataV2 newDataV2 = new PlayerProfileDataV2(oldData);
                    // newDataV2.SchemaVersion = 2;
                    // return newDataV2; // Assuming PlayerProfileDataV2 inherits or can be cast to PlayerProfileData for the next step
                    Debug.Log("Applying migration from v1 to v2...");
                    // Implement actual V1 to V2 transformation logic here.
                    // For example, if a field was renamed or a new default value added.
                    // oldData.SomeNewFieldInV2 = "default_value"; // If PlayerProfileData is mutable during migration
                    oldData.SchemaVersion = 2; // Crucially, update the version
                    return oldData; // Return the modified data

                case 2:
                    // Example: Migrate from version 2 to version 3
                    Debug.Log("Applying migration from v2 to v3...");
                    // Implement V2 to V3 transformation logic
                    oldData.SchemaVersion = 3;
                    return oldData;
                
                // Add more cases for each version jump
                // case N:
                //    return MigrateFromNToNPlus1(oldData);

                default:
                    Debug.LogError($"No migration path defined for schema version {oldData.SchemaVersion}.");
                    return null; // Or throw an exception
            }
        }

        // Example of a specific migration method (if logic is complex)
        // private PlayerProfileData MigrateFrom1To2(PlayerProfileDataV1 oldDataV1)
        // {
        //     PlayerProfileData currentDataFormat = new PlayerProfileData(); // Or target version DTO
        //     currentDataFormat.SchemaVersion = 2;
        //     currentDataFormat.UserId = oldDataV1.UserId;
        //     currentDataFormat.TotalStars = oldDataV1.TotalStars;
        //     // currentDataFormat.NewFieldInV2 = oldDataV1.OldFieldEquivalent ?? defaultValue;
        //     // ... map other fields, transform as needed
        //     return currentDataFormat;
        // }
    }
}