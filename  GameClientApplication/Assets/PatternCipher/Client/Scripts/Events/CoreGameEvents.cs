using PatternCipher.Client.Application;
// This using statement is a placeholder for where the data models would be defined.
using PatternCipher.Shared.Models;

namespace PatternCipher.Client.Events
{
    // --- MOCK DATA MODELS for compilation ---
    // These would be defined in their own files in the Shared assembly (REPO-PATT-012)
    namespace PatternCipher.Shared.Models
    {
        public class LevelDefinition 
        { 
            public string LevelId;
        }
        public class LevelResult 
        { 
            public int Score;
            public int Stars;
        }
    }
    // --- END MOCK DATA MODELS ---


    /// <summary>
    /// Contains the definitions for various event types (data payloads) used throughout the application.
    /// These structs serve as data contracts for communication via the GameEventSystem,
    /// ensuring type safety and clear communication payloads between decoupled systems.
    /// </summary>

    /// <summary>
    /// Published when the GameManager's main state changes.
    /// </summary>
    public struct GameStateChangedEvent
    {
        public GameState NewState;
    }

    /// <summary>
    /// Published when a new level is started.
    /// </summary>
    public struct LevelStartedEvent
    {
        public LevelDefinition Level;
    }

    /// <summary>
    /// Published when a level is completed, carrying the results.
    /// </summary>
    public struct LevelCompletedEvent
    {
        /// <summary>
        /// Contains the score, stars awarded, and other metrics for the completed level.
        /// </summary>
        public LevelResult Result;
    }

    /// <summary>
    /// A signal event published to request that all systems save their current state.
    /// This is typically fired on application pause or quit.
    /// </summary>
    public struct SaveGameRequestEvent { }
}