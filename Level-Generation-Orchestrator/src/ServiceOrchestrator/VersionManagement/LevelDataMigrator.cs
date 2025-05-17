using PatternCipher.Services.Interfaces;
using PatternCipher.Services.Contracts;
using PatternCipher.Services.Exceptions;
using PatternCipher.Services.VersionManagement.Models; // For CurrentLevelData
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine; // For Debug.Log

namespace PatternCipher.Services.VersionManagement
{
    public class LevelDataMigrator : ILevelDataMigrator
    {
        private readonly Dictionary<(int fromVersion, int toVersion), Func<JObject, JObject>> _migrationFunctions = 
            new Dictionary<(int fromVersion, int toVersion), Func<JObject, JObject>>();

        public LevelDataMigrator()
        {
            // Example of registering a migration path
            // RegisterMigrationPath(1, 2, dataV1 => {
            //     JObject dataV2 = new JObject(dataV1); // Clone
            //     dataV2["newField"] = "defaultValue";
            //     if (dataV2["oldField"] != null) {
            //         dataV2["renamedField"] = dataV2["oldField"];
            //         dataV2.Remove("oldField");
            //     }
            //     return dataV2;
            // });
        }
        
        public void RegisterMigrationPath(int fromVersion, int toVersion, Func<object, object> migrationFunction)
        {
            // Adapt Func<object, object> to Func<JObject, JObject> for internal use
            // This assumes rawLevelData will be handled as JObject internally
            _migrationFunctions[(fromVersion, toVersion)] = rawObject =>
            {
                if (rawObject is JObject jObj)
                {
                    return migrationFunction(jObj) as JObject;
                }
                // Fallback if it's a string, try to parse
                if (rawObject is string jsonString)
                {
                    JObject parsedJObj = JObject.Parse(jsonString);
                    JObject resultJObj = migrationFunction(parsedJObj) as JObject;
                    return resultJObj;
                }
                throw new DataMigrationException($"Migration function for {fromVersion}->{toVersion} received unexpected data type: {rawObject?.GetType().Name}");
            };
             Debug.Log($"[LevelDataMigrator] Registered migration from v{fromVersion} to v{toVersion}.");
        }
        
        // Overload for direct JObject functions if preferred for internal registration
        public void RegisterMigrationPath(int fromVersion, int toVersion, Func<JObject, JObject> migrationFunction)
        {
            if (fromVersion >= toVersion)
            {
                throw new ArgumentException("FromVersion must be less than ToVersion.");
            }
            _migrationFunctions[(fromVersion, toVersion)] = migrationFunction;
            Debug.Log($"[LevelDataMigrator] Registered JObject migration from v{fromVersion} to v{toVersion}.");
        }


        public Task<GeneratedLevelData> MigrateToLatestAsync(object oldLevelData, int sourceVersion)
        {
            if (oldLevelData == null)
            {
                throw new ArgumentNullException(nameof(oldLevelData));
            }

            int targetVersion = CurrentLevelData.LatestVersion;
            Debug.Log($"[LevelDataMigrator] Attempting to migrate raw data from v{sourceVersion} to v{targetVersion}.");

            if (sourceVersion > targetVersion)
            {
                throw new DataMigrationException($"Source version {sourceVersion} is newer than target version {targetVersion}. Cannot downgrade.");
            }

            if (sourceVersion == targetVersion)
            {
                Debug.Log("[LevelDataMigrator] Source version is already latest. No migration needed for raw data.");
                // Repackage, as the contract demands a new GeneratedLevelData object
                return Task.FromResult(new GeneratedLevelData { RawLayoutData = oldLevelData, Version = sourceVersion });
            }

            JObject currentJObjectData;
            if (oldLevelData is JObject jObj)
            {
                currentJObjectData = jObj;
            }
            else if (oldLevelData is string jsonString)
            {
                try
                {
                    currentJObjectData = JObject.Parse(jsonString);
                }
                catch (JsonReaderException ex)
                {
                    throw new DataMigrationException("Failed to parse oldLevelData string as JSON.", ex);
                }
            }
            else
            {
                 // Attempt to serialize unknown object type to JObject via JSON
                try
                {
                    string tempJson = JsonConvert.SerializeObject(oldLevelData);
                    currentJObjectData = JObject.Parse(tempJson);
                    Debug.LogWarning($"[LevelDataMigrator] oldLevelData was of type {oldLevelData.GetType().Name}, converted to JObject via JSON serialization.");
                }
                catch (Exception ex)
                {
                    throw new DataMigrationException($"Unsupported oldLevelData type: {oldLevelData.GetType().Name}. Could not convert to JObject.", ex);
                }
            }


            int currentVersion = sourceVersion;
            while (currentVersion < targetVersion)
            {
                int nextVersion = currentVersion + 1;
                if (!_migrationFunctions.TryGetValue((currentVersion, nextVersion), out var migrationFunc))
                {
                    throw new DataMigrationException($"No migration path found from version {currentVersion} to {nextVersion}.");
                }
                
                Debug.Log($"[LevelDataMigrator] Migrating raw data from v{currentVersion} to v{nextVersion}.");
                try
                {
                    currentJObjectData = migrationFunc(currentJObjectData);
                }
                catch(Exception ex)
                {
                    throw new DataMigrationException($"Error during migration from v{currentVersion} to v{nextVersion}.", ex);
                }
                currentVersion = nextVersion;
            }
            
            Debug.Log($"[LevelDataMigrator] Raw data migration complete. Final version: {currentVersion}.");

            // The contract is to return a GeneratedLevelData.
            // We only have the migrated RawLayoutData and the new Version.
            // Other fields (LevelID, ParValue etc.) are not available to the migrator here.
            // The LevelGenerationService will take these and update its existing GeneratedLevelData object.
            var migrationResultShell = new GeneratedLevelData
            {
                RawLayoutData = currentJObjectData, // Store as JObject or back to string as needed
                Version = currentVersion
                // Other fields are default/null
            };

            return Task.FromResult(migrationResultShell);
        }
    }
}