using System;

namespace PatternCipher.Services.Exceptions
{
    /// <summary>
    /// Custom exception for failures during level data migration.
    /// Indicates an error occurred during the migration of generated level data formats
    /// from an older version to a newer one.
    /// </summary>
    [Serializable]
    public class DataMigrationException : Exception
    {
        public DataMigrationException() { }
        public DataMigrationException(string message) : base(message) { }
        public DataMigrationException(string message, Exception inner) : base(message, inner) { }
        protected DataMigrationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}