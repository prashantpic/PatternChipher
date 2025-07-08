using Newtonsoft.Json.Linq;

namespace PatternCipher.Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// Defines the contract for the service responsible for migrating save data
    /// from older schema versions to the current one.
    /// </summary>
    public interface IMigrationService
    {
        /// <summary>
        /// Takes a JObject representation of save data, checks its version,
        /// and applies all necessary transformations in sequence to bring it up to the latest version.
        /// </summary>
        /// <param name="saveData">The save data as a JObject, potentially from an older schema version.</param>
        /// <returns>A JObject representing the save data after being migrated to the current schema version.</returns>
        JObject MigrateToCurrentVersion(JObject saveData);
    }
}