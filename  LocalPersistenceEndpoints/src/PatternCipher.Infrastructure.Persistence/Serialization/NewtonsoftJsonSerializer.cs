using Newtonsoft.Json;

namespace PatternCipher.Infrastructure.Persistence.Serialization
{
    /// <summary>
    /// A concrete implementation of IJsonSerializer using the powerful and widely-used Newtonsoft.Json library.
    /// </summary>
    public class NewtonsoftJsonSerializer : IJsonSerializer
    {
        private readonly JsonSerializerSettings _settings;

        public NewtonsoftJsonSerializer()
        {
            _settings = new JsonSerializerSettings
            {
                // Example of settings:
                // TypeNameHandling = TypeNameHandling.Auto, 
                // Formatting = Formatting.Indented 
            };
        }

        /// <summary>
        /// Serializes the specified object to a JSON string.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>A JSON string representation of the object.</returns>
        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, _settings);
        }

        /// <summary>
        /// Deserializes the JSON string to the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, _settings)!;
        }
    }
}