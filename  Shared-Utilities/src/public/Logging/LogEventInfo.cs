using System;
using System.Collections.Generic;

namespace PatternCipher.Utilities.Logging
{
    /// <summary>
    /// Data Transfer Object (DTO) for holding structured properties associated with a log event.
    /// </summary>
    /// <remarks>
    /// Encapsulates additional structured data with log messages (e.g., generator seed, configuration parameters, game state) for detailed diagnostics.
    /// Allows attaching arbitrary key-value pairs to a log entry for richer context.
    /// </remarks>
    public class LogEventInfo
    {
        /// <summary>
        /// A dictionary of custom properties for the log event.
        /// </summary>
        public IDictionary<string, object> Properties { get; }

        /// <summary>
        /// Initializes a new instance of the LogEventInfo class.
        /// </summary>
        public LogEventInfo()
        {
            Properties = new Dictionary<string, object>();
        }

        /// <summary>
        /// Initializes a new instance of the LogEventInfo class with properties from another dictionary.
        /// </summary>
        /// <param name="properties">A dictionary containing initial properties.</param>
        public LogEventInfo(IDictionary<string, object> properties)
        {
            Properties = new Dictionary<string, object>(properties ?? new Dictionary<string, object>());
        }

        /// <summary>
        /// Adds a property to the log event info.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The property value.</param>
        /// <returns>The current LogEventInfo instance for fluent chaining.</returns>
        public LogEventInfo WithProperty(string key, object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            Properties[key] = value;
            return this;
        }
    }
}