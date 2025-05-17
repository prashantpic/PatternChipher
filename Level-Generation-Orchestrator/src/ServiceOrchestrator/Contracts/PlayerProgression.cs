using System.Collections.Generic;

namespace PatternCipher.Services.Contracts
{
    /// <summary>
    /// Data contract representing player's current progression.
    /// Encapsulates data about the player's progress in the game, used by IDifficultyManager.
    /// </summary>
    public class PlayerProgression
    {
        /// <summary>
        /// The current level number or stage the player has reached.
        /// </summary>
        public int CurrentLevel { get; set; }

        /// <summary>
        /// A list of features, mechanics, or content types the player has unlocked.
        /// </summary>
        public List<string> UnlockedFeatures { get; set; }

        public PlayerProgression()
        {
            UnlockedFeatures = new List<string>();
        }
    }
}