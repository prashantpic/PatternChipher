using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using PatternCipher.Infrastructure.Persistence.Migrations.Scripts;

// Assuming a shared constants class
// using PatternCipher.Shared;

public static class SharedConstants
{
    public const int CURRENT_SCHEMA_VERSION = 2;
}

namespace PatternCipher.Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// Manages and executes the data migration process. It identifies the version
    /// of the loaded save data and applies a sequence of ordered migration scripts
    /// to bring it up to the current version.
    /// </summary>
    public class MigrationService : IMigrationService
    {
        private const string SCHEMA_VERSION_PROPERTY = "save_schema_version";
        private readonly IReadOnlyDictionary<int, IMigrationScript> _migrationScripts;

        /// <summary>
        /// Initializes a new instance of the MigrationService.
        /// </summary>
        /// <param name="scripts">An enumerable of all available migration script implementations, provided by dependency injection.</param>
        public MigrationService(IEnumerable<IMigrationScript> scripts)
        {
            // Order scripts by source version and store in a dictionary for fast lookup.
            // This also implicitly checks for duplicate source versions.
            _migrationScripts = scripts.OrderBy(s => s.SourceVersion)
                                       .ToDictionary(s => s.SourceVersion);
        }

        /// <inheritdoc/>
        public JObject MigrateToCurrentVersion(JObject saveData)
        {
            var currentVersion = GetSchemaVersion(saveData);

            if (currentVersion >= SharedConstants.CURRENT_SCHEMA_VERSION)
            {
                // Data is already at or newer than the current version. No migration needed.
                return saveData;
            }
            
            // Loop from the data's current version up to the application's target version.
            for (int versionToUpgrade = currentVersion; versionToUpgrade < SharedConstants.CURRENT_SCHEMA_VERSION; versionToUpgrade++)
            {
                if (_migrationScripts.TryGetValue(versionToUpgrade, out var script))
                {
                    // Found a script to upgrade from the current version, apply it.
                    script.Apply(saveData);
                    Console.WriteLine($"Successfully applied migration script for version {versionToUpgrade}.");
                }
                else
                {
                    // If a script is not found for an intermediate version, this indicates a gap in the migration path.
                    // Depending on policy, we could log and continue, but throwing is safer to prevent data corruption.
                    if (versionToUpgrade > currentVersion) // Only throw if we've already made progress
                    {
                        throw new InvalidOperationException($"Missing migration script for version {versionToUpgrade}. Cannot complete migration.");
                    }
                    // If we haven't even started, we just note that there's no script for this version.
                }
            }

            // After all migrations are applied, update the version number in the data.
            saveData[SCHEMA_VERSION_PROPERTY] = SharedConstants.CURRENT_SCHEMA_VERSION;

            return saveData;
        }

        private int GetSchemaVersion(JObject saveData)
        {
            var versionToken = saveData[SCHEMA_VERSION_PROPERTY];
            if (versionToken == null || versionToken.Type == JTokenType.Null)
            {
                // If the version property doesn't exist, assume it's the earliest version (e.g., 1 or 0).
                // Let's assume the first version that had this system was V1.
                return 1;
            }
            return versionToken.Value<int>();
        }
    }
}