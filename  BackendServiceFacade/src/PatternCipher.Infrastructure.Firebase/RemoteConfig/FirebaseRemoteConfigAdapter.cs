using Firebase.RemoteConfig;
using PatternCipher.Application.Services;
using PatternCipher.Infrastructure.Firebase.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PatternCipher.Infrastructure.Firebase.RemoteConfig
{
    /// <summary>
    /// Implements the remote configuration service interface using Firebase Remote Config.
    /// It fetches, caches, and provides configuration values to the application.
    /// </summary>
    public class FirebaseRemoteConfigAdapter : IRemoteConfigService
    {
        private readonly FirebaseRemoteConfig _remoteConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirebaseRemoteConfigAdapter"/> class.
        /// </summary>
        public FirebaseRemoteConfigAdapter()
        {
            _remoteConfig = FirebaseRemoteConfig.DefaultInstance;
        }

        /// <inheritdoc/>
        public async Task<FirebaseResult> InitializeAsync(Dictionary<string, object> defaults)
        {
            try
            {
                await _remoteConfig.SetDefaultsAsync(defaults);
                // FetchAsync() will fetch the latest values from the backend.
                await _remoteConfig.FetchAsync(TimeSpan.Zero);
                // ActivateAsync() will make the fetched values available to the app.
                await _remoteConfig.ActivateAsync();
                return FirebaseResult.Success();
            }
            catch (Exception ex)
            {
                return FirebaseResult.Failure(new FirebaseError(0, ex.Message, ex));
            }
        }

        /// <inheritdoc/>
        public bool GetBool(string key, bool defaultValue)
        {
            ConfigValue value = _remoteConfig.GetValue(key);
            return value.Source != ValueSource.Default ? value.BooleanValue : defaultValue;
        }

        /// <inheritdoc/>
        public float GetFloat(string key, float defaultValue)
        {
            ConfigValue value = _remoteConfig.GetValue(key);
            return value.Source != ValueSource.Default ? (float)value.DoubleValue : defaultValue;
        }

        /// <inheritdoc/>
        public int GetInt(string key, int defaultValue)
        {
            ConfigValue value = _remoteConfig.GetValue(key);
            return value.Source != ValueSource.Default ? (int)value.LongValue : defaultValue;
        }

        /// <inheritdoc/>
        public string GetString(string key, string defaultValue)
        {
            ConfigValue value = _remoteConfig.GetValue(key);
            return value.Source != ValueSource.Default ? value.StringValue : defaultValue;
        }
    }
}