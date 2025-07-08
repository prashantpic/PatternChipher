using Firebase.Analytics;
using PatternCipher.Application.Services;
using System.Collections.Generic;
using System.Linq;

namespace PatternCipher.Infrastructure.Firebase.Analytics
{
    /// <summary>
    /// Implements the IAnalyticsService interface using the Firebase Analytics SDK.
    /// This adapter translates application-level event logging into Firebase-specific calls.
    /// </summary>
    public class FirebaseAnalyticsAdapter : IAnalyticsService
    {
        /// <inheritdoc/>
        public void LogEvent(string eventName)
        {
            FirebaseAnalytics.LogEvent(eventName);
        }

        /// <inheritdoc/>
        public void LogEvent(string eventName, Dictionary<string, object> parameters)
        {
            if (parameters == null || parameters.Count == 0)
            {
                LogEvent(eventName);
                return;
            }

            var firebaseParameters = new List<Parameter>();
            foreach (var kvp in parameters)
            {
                if (kvp.Value is string stringValue)
                {
                    firebaseParameters.Add(new Parameter(kvp.Key, stringValue));
                }
                else if (kvp.Value is int intValue)
                {
                    firebaseParameters.Add(new Parameter(kvp.Key, intValue));
                }
                else if (kvp.Value is long longValue)
                {
                    firebaseParameters.Add(new Parameter(kvp.Key, longValue));
                }
                else if (kvp.Value is float floatValue)
                {
                    firebaseParameters.Add(new Parameter(kvp.Key, floatValue));
                }
                else if (kvp.Value is double doubleValue)
                {
                    firebaseParameters.Add(new Parameter(kvp.Key, doubleValue));
                }
                else if (kvp.Value is bool boolValue)
                {
                    firebaseParameters.Add(new Parameter(kvp.Key, boolValue ? 1L : 0L));
                }
            }
            
            FirebaseAnalytics.LogEvent(eventName, firebaseParameters.ToArray());
        }

        /// <inheritdoc/>
        public void SetUserProperty(string propertyName, string value)
        {
            FirebaseAnalytics.SetUserProperty(propertyName, value);
        }
    }
}