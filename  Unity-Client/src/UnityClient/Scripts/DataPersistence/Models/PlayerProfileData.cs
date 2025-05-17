using System;
using System.Collections.Generic;
using PatternCipher.Client.Domain.ValueObjects; // For GridPosition, potentially

namespace PatternCipher.Client.DataPersistence.Models
{
    [Serializable]
    public class GameSettingsData
    {
        public float MasterVolume { get; set; } = 1.0f;
        public float MusicVolume { get; set; } = 0.8f;
        public float SfxVolume { get; set; } = 0.8f;
        public bool HapticsEnabled { get; set; } = true;
        public string Language { get; set; } = "en";
        public bool ColorblindModeEnabled { get; set; } = false;
        // Add other settings
    }

    [Serializable]
    public class LevelRecordData
    {
        public string LevelId { get; set; } // Changed from int to string
        public int HighScore { get; set; } = 0;
        public int StarsEarned { get; set; } = 0;
        public bool IsCompleted { get; set; } = false;
        public int BestMoves { get; set; } = int.MaxValue; // Or 0 if not played
        public float BestTimeSeconds { get; set; } = float.MaxValue; // Or 0 if not played

        // Add other per-level stats if needed
    }
    
    [Serializable]
    public class PlayerInventoryItemData
    {
        public string ItemId { get; set; }
        public int Quantity { get; set; }
    }


    [Serializable]
    public class PlayerProfileData
    {
        public int SchemaVersion { get; set; } = 1; // Current schema version
        public string UserId { get; set; } // Firebase User ID or local unique ID
        public string PlayerName { get; set; } = "Player";
        public int TotalStars { get; set; } = 0;
        public int HighestLevelUnlocked { get; set; } = 1; // Or string ID if levels are not sequential ints
        public long LastPlayedTimestamp { get; set; } = 0;
        
        public Dictionary<string, LevelRecordData> LevelRecords { get; set; } // Key: LevelId (string)
        public GameSettingsData Settings { get; set; }
        public List<PlayerInventoryItemData> Inventory { get; set; } // Example of inventory

        // Currency, XP, etc.
        public int SoftCurrency { get; set; } = 0;
        public int HardCurrency { get; set; } = 0;


        public PlayerProfileData()
        {
            LevelRecords = new Dictionary<string, LevelRecordData>();
            Settings = new GameSettingsData();
            Inventory = new List<PlayerInventoryItemData>();
            // Set default values upon creation if needed
            UserId = Guid.NewGuid().ToString(); // Default for local anonymous player
        }
    }
}