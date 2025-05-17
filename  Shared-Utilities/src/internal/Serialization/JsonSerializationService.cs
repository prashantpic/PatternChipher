using System;
using System.Text;
using Newtonsoft.Json;
using PatternCipher.Utilities.Serialization;
using PatternCipher.Utilities.Common.Exceptions;

namespace PatternCipher.Utilities.Serialization.Internal
{
    /// <summary>
    /// Internal implementation of ISerializationService using Newtonsoft.Json for JSON serialization and deserialization.
    /// </summary>
    /// <remarks>
    /// Implements the ISerializationService interface by wrapping Newtonsoft.Json's JsonConvert methods for robust JSON processing. Handles common serialization settings.
    /// </remarks>
    internal class JsonSerializationService : ISerializationService
    {
        private readonly JsonSerializerSettings _settings;

        public JsonSerializationService()
        {
            // Default settings for common use cases. Can be extended.
            _settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore, //REQ-10-003 implicitly suggests handling standard formats. Ignoring nulls is common.
                Formatting = Formatting.None, // Compact for storage/transmission
                // Consider adding TypeNameHandling if polymorphism is needed for serialized types,
                // but this adds complexity and potential security concerns if not handled carefully.
                // TypeNameHandling = TypeNameHandling.Auto,
                ReferenceLoopHandling = ReferenceLoopHandling.Error // Default and safest for most cases
            };
        }

        /// <summary>
        /// Serializes an object to a string representation using Newtonsoft.Json.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="dataToSerialize">The object instance to serialize.</param>
        /// <returns>A JSON string representation of the serialized object.</returns>
        /// <exception cref="SerializationException">Thrown if serialization fails.</exception>
        public string Serialize<T>(T dataToSerialize)
        {
            try
            {
                return JsonConvert.SerializeObject(dataToSerialize, _settings);
            }
            catch (Exception ex)
            {
                throw new SerializationException($"Failed to serialize object of type {typeof(T).FullName}.", ex);
            }
        }

        /// <summary>
        /// Serializes an object to a byte array representation (UTF8 encoded JSON string).
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="dataToSerialize">The object instance to serialize.</param>
        /// <returns>A byte array containing the UTF8 encoded JSON string.</returns>
        /// <exception cref="SerializationException">Thrown if serialization fails.</exception>
        public byte[] SerializeToBytes<T>(T dataToSerialize)
        {
            try
            {
                string jsonString = Serialize(dataToSerialize);
                return Encoding.UTF8.GetBytes(jsonString);
            }
            catch (SerializationException) // Re-throw our specific exception
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializationException($"Failed to serialize object of type {typeof(T).FullName} to bytes.", ex);
            }
        }

        /// <summary>
        /// Deserializes a string representation (JSON) to an object using Newtonsoft.Json.
        /// </summary>
        /// <typeparam name="T">The target type for deserialization.</typeparam>
        /// <param name="serializedData">The string containing the JSON data.</param>
        /// <returns>An object instance deserialized from the string.</returns>
        /// <exception cref="SerializationException">Thrown if deserialization fails or input is invalid.</exception>
        public T Deserialize<T>(string serializedData)
        {
            if (string.IsNullOrEmpty(serializedData))
            {
                 throw new SerializationException($"Cannot deserialize null or empty string to type {typeof(T).FullName}.");
            }

            try
            {
                T? result = JsonConvert.DeserializeObject<T>(serializedData, _settings);
                if (result == null && Nullable.GetUnderlyingType(typeof(T)) == null && typeof(T).IsClass) // Check if T is a non-nullable reference type and result is null
                {
                    // This case might occur if JSON is "null" and T is a non-nullable reference type.
                    // Depending on strictness, either throw or allow it.
                    // Newtonsoft.Json might return null for reference types if the JSON string is "null".
                    // For value types, it usually throws if "null" is passed and type is not Nullable<V>.
                }
                return result!; // The '!' assumes if T is non-nullable, JsonConvert won't return null for valid JSON.
                                // This assumption depends on _settings and the nature of T.
            }
            catch (JsonSerializationException jsex) // Specific Newtonsoft exception
            {
                throw new SerializationException($"Failed to deserialize string to object of type {typeof(T).FullName}. JSON error: {jsex.Message}", jsex);
            }
            catch (Exception ex)
            {
                throw new SerializationException($"Failed to deserialize string to object of type {typeof(T).FullName}.", ex);
            }
        }

        /// <summary>
        /// Deserializes a byte array (UTF8 encoded JSON string) to an object.
        /// </summary>
        /// <typeparam name="T">The target type for deserialization.</typeparam>
        /// <param name="serializedData">The byte array containing the UTF8 encoded JSON data.</param>
        /// <returns>An object instance deserialized from the byte array.</returns>
        /// <exception cref="SerializationException">Thrown if deserialization fails or input is invalid.</exception>
        public T DeserializeFromBytes<T>(byte[] serializedData)
        {
            if (serializedData == null || serializedData.Length == 0)
            {
                 throw new SerializationException($"Cannot deserialize null or empty byte array to type {typeof(T).FullName}.");
            }

            try
            {
                string jsonString = Encoding.UTF8.GetString(serializedData);
                return Deserialize<T>(jsonString);
            }
            catch (SerializationException) // Re-throw our specific exception from Deserialize<T>
            {
                throw;
            }
            catch (DecoderFallbackException dfex) // Specific exception for UTF8 decoding issues
            {
                throw new SerializationException($"Failed to decode byte array (not valid UTF-8) for type {typeof(T).FullName}.", dfex);
            }
            catch (Exception ex)
            {
                throw new SerializationException($"Failed to deserialize bytes to object of type {typeof(T).FullName}.", ex);
            }
        }
    }
}