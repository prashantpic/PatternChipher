namespace PatternCipher.Infrastructure.Persistence.Serialization
{
    /// <summary>
    /// An interface abstracting the JSON serialization/deserialization functionality
    /// to decouple the persistence logic from a specific JSON library.
    /// </summary>
    public interface IJsonSerializer
    {
        /// <summary>
        /// Serializes the specified object to a JSON string.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>A JSON string representation of the object.</returns>
        string Serialize(object obj);

        /// <summary>
        /// Deserializes the JSON string to the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        T Deserialize<T>(string json);
    }
}