using System;
using System.Collections.Generic;
// using PatternCipher.Client.Domain.ValueObjects; // If settings use domain value objects

namespace PatternCipher.Client.DataPersistence.Models
{
    [System.Serializable]
    public class GameSettingsData // Renamed from GameSettings to avoid conflict if domain GameSettings exists
    {
        public float MasterVolume { get; set; } = 1.0f;
        public float MusicVolume { get; set; } = 0.8f;
        public float SfxVolume { get; set; } = 0.8f;
        public bool HapticsEnabled { get; set; } = true;
        public string Language { get; set; } = "en";
        public bool ColorblindModeEnabled { get; set; } = false;
        // Add other game settings here
    }

    [System.Serializable]
    public class LevelRecord
    {
        public int LevelId { get; set; }
        public int HighScore { get; set; } = 0;
        public int StarsEarned { get; set; } = 0;
        public bool IsUnlocked { get; set; } = false;
        public bool IsCompleted { get; set; } = false;
        // Add other per-level tracking data if needed
        public int BestMoves { get; set; } = int.MaxValue;
        public float BestTimeSeconds { get; set; } = float.MaxValue;
    }

    [System.Serializable]
    public class PlayerProfileData
    {
        public int SchemaVersion { get; set; } = 1; // For data migration
        public string UserId { get; set; } // Firebase User ID or local unique ID
        public string PlayerName { get; set; } = "Player";
        
        public int TotalStars { get; set; } = 0;
        public int HighestLevelUnlocked { get; set; } = 1; // Assuming level 1 is default unlocked
        
        public Dictionary<int, LevelRecord> LevelRecords { get; set; } // Key: LevelId
        public GameSettingsData Settings { get; set; }
        
        // Example: Currencies or inventory
        public int SoftCurrency { get; set; } = 0;
        public int HardCurrency { get; set; } = 0;
        // public Dictionary<string, int> InventoryItems { get; set; } // Key: ItemId, Value: Count

        public DateTime LastPlayedTimestamp { get; set; }
        public DateTime CreatedTimestamp { get; set; }

        public PlayerProfileData()
        {
            LevelRecords = new Dictionary<int, LevelRecord>();
            Settings = new GameSettingsData();
            CreatedTimestamp = DateTime.UtcNow;
            LastPlayedTimestamp = DateTime.UtcNow;
        }
    }
}