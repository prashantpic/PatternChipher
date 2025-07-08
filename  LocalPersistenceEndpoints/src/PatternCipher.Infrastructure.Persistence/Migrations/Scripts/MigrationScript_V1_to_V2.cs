using Newtonsoft.Json.Linq;

namespace PatternCipher.Infrastructure.Persistence.Migrations.Scripts
{
    /// <summary>
    /// A concrete migration script that transforms a save data object from schema version 1 to schema version 2.
    /// </summary>
    public class MigrationScript_V1_to_V2 : IMigrationScript
    {
        /// <summary>
        /// This script upgrades data *from* version 1.
        /// </summary>
        public int SourceVersion => 1;

        /// <summary>
        /// Applies the migration logic to upgrade data from v1 to v2.
        /// Example: Renames a property 'totalScore' to 'cumulativeScore'.
        /// </summary>
        /// <param name="data">The JObject representation of the save data to be modified.</param>
        public void Apply(JObject data)
        {
            // Find the token for the old property. The path is an example.
            var scoreToken = data.SelectToken("playerStats.totalScore");

            if (scoreToken != null && scoreToken.Parent is JObject parent)
            {
                // Create a new property with the new name and the old value.
                var newProperty = new JProperty("cumulativeScore", scoreToken.Value<long>());
                
                // Add the new property to the parent object.
                parent.Add(newProperty);

                // Remove the old property.
                scoreToken.Parent.Remove();
            }

            // Example 2: Add a new property with a default value
            var settingsToken = data.SelectToken("settings");
            if (settingsToken is JObject settings)
            {
                if (settings["hapticsEnabled"] == null)
                {
                    settings.Add(new JProperty("hapticsEnabled", true));
                }
            }
        }
    }
}