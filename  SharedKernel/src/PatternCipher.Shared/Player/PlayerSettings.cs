namespace PatternCipher.Shared.Player
{
    /// <summary>
    /// Data model for storing all user-configurable game settings.
    /// </summary>
    public class PlayerSettings
    {
        public float MusicVolume { get; set; } = 0.8f;
        public float SfxVolume { get; set; } = 1.0f;
        public bool IsMusicMuted { get; set; } = false;
        public bool IsSfxMuted { get; set; } = false;
        public string ColorblindMode { get; set; } = "None"; // e.g., "None", "Deuteranopia"
        public bool AnalyticsConsent { get; set; } = true; // Default state may depend on region
        public bool ReducedMotion { get; set; } = false;
        public bool HapticsEnabled { get; set; } = true;
        public string Language { get; set; } = "en";
    }
}