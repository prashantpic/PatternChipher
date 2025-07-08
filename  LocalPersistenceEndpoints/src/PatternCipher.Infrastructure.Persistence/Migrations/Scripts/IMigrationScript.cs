using Newtonsoft.Json.Linq;

namespace PatternCipher.Infrastructure.Persistence.Migrations.Scripts
{
    /// <summary>
    /// An interface representing a single, atomic migration step from one schema version to the next.
    /// </summary>
    public interface IMigrationScript
    {
        /// <summary>
        /// Gets the schema version this script can upgrade *from*.
        /// For example, a script with SourceVersion = 1 knows how to upgrade a v1 save to a v2 save.
        /// </summary>
        int SourceVersion { get; }

        /// <summary>
        /// Performs the in-place modification of the JObject data to upgrade it to the next version.
        /// </summary>
        /// <param name="data">The JObject representation of the save data to be modified.</param>
        void Apply(JObject data);
    }
}