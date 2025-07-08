using Firebase.Firestore;

namespace PatternCipher.Infrastructure.Firebase.CloudSave.DTOs
{
    /// <summary>
    /// Data Transfer Object representing the player's settings as stored in Firestore.
    /// </summary>
    [FirestoreData]
    public class PlayerSettingsDto
    {
        [FirestoreProperty]
        public bool IsSoundEnabled { get; set; }

        [FirestoreProperty]
        public bool IsHapticsEnabled { get; set; }
    }

    /// <summary>
    /// Data Transfer Object representing the player's progress as stored in Firestore.
    /// </summary>
    [FirestoreData]
    public class PlayerProgressDto
    {
        [FirestoreProperty]
        public int LastCompletedLevel { get; set; }
    }

    /// <summary>
    /// Data Transfer Object representing the player profile as it is stored in Cloud Firestore.
    /// This class uses Firestore-specific attributes to guide serialization.
    /// </summary>
    [FirestoreData]
    public class PlayerProfileDto
    {
        /// <summary>
        /// The player's configurable settings.
        /// </summary>
        [FirestoreProperty]
        public PlayerSettingsDto Settings { get; set; }

        /// <summary>
        /// The player's game progress.
        /// </summary>
        [FirestoreProperty]
        public PlayerProgressDto Progress { get; set; }

        /// <summary>
        /// A server-side timestamp indicating when the profile was last updated.
        /// Firestore automatically populates this field upon writing.
        /// </summary>
        [FirestoreProperty, ServerTimestamp]
        public Timestamp LastUpdated { get; set; }
    }
}