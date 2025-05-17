using System;
using System.Threading.Tasks;
using Firebase.RemoteConfig;
using UnityEngine; // For Debug
using System.Collections.Generic; // For Dictionary

namespace PatternCipher.Client.Infrastructure.Firebase
{
    public class FirebaseRemoteConfigService
    {
        public bool IsInitialized { get; private set; } = false;
        public bool IsConfigFetchedAndActivated { get; private set; } = false;

        // Feature flag for enabling/disabling Remote Config usage itself
        private const string ENABLE_REMOTE_CONFIG_FLAG = "EnableRemoteConfig"; 

        public FirebaseRemoteConfigService()
        {
            // Initialization of FirebaseRemoteConfig.DefaultInstance should happen after FirebaseApp is ready.
        }

        public async Task InitializeAsync(Dictionary<string, object> defaultValues = null)
        {
            if (IsInitialized) return;

            // Assuming FirebaseApp.CheckAndFixDependenciesAsync() has been called elsewhere
            // and FirebaseRemoteConfig.DefaultInstance is available.
            
            if (defaultValues != null)
            {
                await FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaultValues);
                Debug.Log("FirebaseRemoteConfigService: Default values set.");
            }
            
            IsInitialized = true;
            Debug.Log("FirebaseRemoteConfigService Initialized.");

            // Optionally fetch and activate immediately if that's the desired flow
            // await FetchAndActivateAsync();
        }

        public async Task<bool> FetchAndActivateAsync()
        {
            if (!IsInitialized)
            {
                Debug.LogError("FirebaseRemoteConfigService: Not initialized. Call InitializeAsync first.");
                return false;
            }

            Debug.Log("FirebaseRemoteConfigService: Fetching data...");
            try
            {
                // Set config settings (optional, e.g., for development mode with frequent fetches)
                ConfigSettings settings = new ConfigSettings {
                    MinimumFetchInternalInMilliseconds = 3600000 // 1 hour (production)
                    // For development: MinimumFetchInternalInMilliseconds = 0 
                };
                await FirebaseRemoteConfig.DefaultInstance.SetConfigSettingsAsync(settings);


                await FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero); // Fetch with no cache expiry for immediate effect for this call
                bool activated = await FirebaseRemoteConfig.DefaultInstance.ActivateAsync();
                
                if (activated)
                {
                    Debug.Log("FirebaseRemoteConfigService: Remote config fetched and activated successfully.");
                    IsConfigFetchedAndActivated = true;

                    // Example: Check master feature flag after activation
                    if (!GetBool(ENABLE_REMOTE_CONFIG_FLAG, true))
                    {
                        Debug.LogWarning("FirebaseRemoteConfigService: Remote Config is disabled by master flag. Using default values.");
                        // Potentially clear fetched values or revert to defaults logic here if needed
                    }
                }
                else
                {
                    Debug.LogWarning("FirebaseRemoteConfigService: Remote config fetched but no new values activated (using cached or defaults).");
                    // This is not an error, just means no changes or using cached values.
                    IsConfigFetchedAndActivated = true; // Still consider it "active" with whatever values it has
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"FirebaseRemoteConfigService: Error fetching or activating remote config: {ex.Message}");
                IsConfigFetchedAndActivated = false;
                return false;
            }
        }
        
        private bool CanUseRemoteConfig()
        {
            if (!IsInitialized || !IsConfigFetchedAndActivated) return false; // Must be initialized and fetched at least once
            return GetBool(ENABLE_REMOTE_CONFIG_FLAG, true); // Master switch
        }

        public ConfigValue GetValue(string key)
        {
            if (!IsInitialized)
            {
                Debug.LogWarning($"FirebaseRemoteConfigService: GetValue called before initialization for key: {key}. Returning default ConfigValue.");
                return new ConfigValue(); // Returns a default/empty ConfigValue
            }
            return FirebaseRemoteConfig.DefaultInstance.GetValue(key);
        }

        public string GetString(string key, string defaultValue = "")
        {
            if (!CanUseRemoteConfig()) return defaultValue;
            ConfigValue value = GetValue(key);
            return value.Source != ConfigSource.Default ? value.StringValue : defaultValue;
        }

        public bool GetBool(string key, bool defaultValue = false)
        {
            if (!IsInitialized) return defaultValue; // For ENABLE_REMOTE_CONFIG_FLAG itself, allow reading before full fetch
            if (key == ENABLE_REMOTE_CONFIG_FLAG && (!IsConfigFetchedAndActivated)) // Special handling for master flag
            {
                 ConfigValue val = FirebaseRemoteConfig.DefaultInstance.GetValue(key); // Check if default was set for it
                 return val.Source != ConfigSource.Default ? val.BooleanValue : defaultValue;
            }

            if (!CanUseRemoteConfig()) return defaultValue;
            ConfigValue value = GetValue(key);
            return value.Source != ConfigSource.Default ? value.BooleanValue : defaultValue;
        }

        public long GetLong(string key, long defaultValue = 0L)
        {
            if (!CanUseRemoteConfig()) return defaultValue;
            ConfigValue value = GetValue(key);
            return value.Source != ConfigSource.Default ? value.LongValue : defaultValue;
        }

        public double GetDouble(string key, double defaultValue = 0.0)
        {
            if (!CanUseRemoteConfig()) return defaultValue;
            ConfigValue value = GetValue(key);
            return value.Source != ConfigSource.Default ? value.DoubleValue : defaultValue;
        }
    }
}