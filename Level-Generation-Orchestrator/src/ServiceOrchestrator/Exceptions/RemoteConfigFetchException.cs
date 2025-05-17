using System;

namespace PatternCipher.Services.Exceptions
{
    /// <summary>
    /// Custom exception for failures when fetching remote configuration.
    /// Indicates an error occurred while trying to retrieve data from Firebase Remote Config,
    /// critical for difficulty progression and generation rules.
    /// </summary>
    [Serializable]
    public class RemoteConfigFetchException : Exception
    {
        public RemoteConfigFetchException() { }
        public RemoteConfigFetchException(string message) : base(message) { }
        public RemoteConfigFetchException(string message, Exception inner) : base(message, inner) { }
        protected RemoteConfigFetchException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}