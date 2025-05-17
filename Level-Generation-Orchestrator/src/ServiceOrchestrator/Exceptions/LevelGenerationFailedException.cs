using System;

namespace PatternCipher.Services.Exceptions
{
    /// <summary>
    /// Custom exception for failures during level generation.
    /// Indicates that the overall level generation process failed after all primary attempts
    /// and fallback strategies have been exhausted.
    /// </summary>
    [Serializable]
    public class LevelGenerationFailedException : Exception
    {
        public LevelGenerationFailedException() { }
        public LevelGenerationFailedException(string message) : base(message) { }
        public LevelGenerationFailedException(string message, Exception inner) : base(message, inner) { }
        protected LevelGenerationFailedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}