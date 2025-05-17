using System;

namespace PatternCipher.Utilities.Serialization
{
    /// <summary>
    /// Defines the public contract for serialization and deserialization services, allowing abstraction of specific implementations.
    /// </summary>
    public interface ISerializationService
    {
        /// <summary>
        /// Serializes an object to a string representation.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="dataToSerialize">The object instance to serialize.</param>
        /// <returns>A string representation of the serialized object.</returns>
        string Serialize<T>(T dataToSerialize);

        /// <summary>
        /// Serializes an object to a byte array.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="dataToSerialize">The object instance to serialize.</param>
        /// <returns>A byte array representation of the serialized object.</returns>
        byte[] SerializeToBytes<T>(T dataToSerialize);

        /// <summary>
        /// Deserializes a string representation to an object.
        /// </summary>
        /// <typeparam name="T">The target type for deserialization.</typeparam>
        /// <param name="serializedData">The string containing the serialized data.</param>
        /// <returns>An object instance deserialized from the string.</returns>
        T Deserialize<T>(string serializedData);

        /// <summary>
        /// Deserializes a byte array to an object.
        /// </summary>
        /// <typeparam name="T">The target type for deserialization.</typeparam>
        /// <param name="serializedData">The byte array containing the serialized data.</param>
        /// <returns>An object instance deserialized from the byte array.</returns>
        T DeserializeFromBytes<T>(byte[] serializedData);
    }
}