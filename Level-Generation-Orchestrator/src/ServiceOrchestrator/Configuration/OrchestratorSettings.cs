using UnityEngine;

namespace PatternCipher.Services.Configuration
{
    /// <summary>
    /// Configuration settings for the level generation orchestrator.
    /// Holds configurable parameters for the orchestrator, such as generation timeouts,
    /// retry counts for fallback strategies, default fallback parameters, and URLs for Firebase Functions.
    /// </summary>
    [CreateAssetMenu(fileName = "OrchestratorSettings", menuName = "PatternCipher/Service Orchestrator/Orchestrator Settings")]
    public class OrchestratorSettings : ScriptableObject
    {
        [Tooltip("Maximum time allowed for a single generation pipeline attempt, in seconds.")]
        [field: SerializeField]
        public float MaxGenerationTimeSeconds { get; private set; } = 2.0f;

        [Tooltip("Total number of fallback attempts after an initial generation failure. Does not include the initial attempt.")]
        [field: SerializeField]
        public int MaxRetryAttempts { get; private set; } = 3;
        
        [Tooltip("The full URL for the Firebase Cloud Function used for server-side level validation.")]
        [field: SerializeField]
        public string FirebaseValidationFunctionUrl { get; private set; } = "YOUR_FIREBASE_FUNCTION_URL_HERE"; // Placeholder
    }
}