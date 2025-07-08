using System;
using System.Collections.Generic;

namespace PatternCipher.Shared.Player
{
    /// <summary>
    /// Defines the root structure for a player's entire local save data.
    /// This is the primary object to be serialized/deserialized for persistence.
    /// </summary>
    public class PlayerProfile
    {
        public string PlayerId { get; set; }
        public string SaveSchemaVersion { get; set; }
        public PlayerSettings Settings { get; set; }
        public Dictionary<string, LevelCompletionStatus> LevelStatuses { get; set; }
        public DateTime TimestampOfLastSave { get; set; }
        public DateTime TimestampOfFirstAppOpen { get; set; }
        public string AppVersionAtLastSave { get; set; }

        // Parameterless constructor for deserialization
        public PlayerProfile()
        {
            Settings = new PlayerSettings();
            LevelStatuses = new Dictionary<string, LevelCompletionStatus>();
        }
    }
}