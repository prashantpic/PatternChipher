using Firebase.Analytics;
using UnityEngine; // For Debug
using System.Collections.Generic; // For List

namespace PatternCipher.Client.Infrastructure.Firebase
{
    public class FirebaseAnalyticsService
    {
        public bool IsInitialized { get; private set; } = false;
        private bool _isAnalyticsCollectionEnabled = true; // Default, can be controlled by user consent/settings

        // Feature flag for enabling/disabling Analytics itself, can be from Remote Config or local settings
        private const string ENABLE_ANALYTICS_FLAG = "EnableAnalytics"; 
        private FirebaseRemoteConfigService _remoteConfigService; // Optional: if flag is remote

        public FirebaseAnalyticsService(FirebaseRemoteConfigService remoteConfigService = null)
        {
             _remoteConfigService = remoteConfigService;
            // Initialization of FirebaseAnalytics happens after FirebaseApp is ready.
        }

        public void Initialize()
        {
            if (IsInitialized) return;

            // Assuming FirebaseApp.CheckAndFixDependenciesAsync() has been called
            // FirebaseAnalytics. désiradeSetAnalyticsCollectionEnabled happens implicitly if Firebase is init
            
            // Check master flag for analytics
            bool masterAnalyticsFlag = true;
            if (_remoteConfigService != null && _remoteConfigService.IsConfigFetchedAndActivated)
            {
                masterAnalyticsFlag = _remoteConfigService.GetBool(ENABLE_ANALYTICS_FLAG, true);
            }
            else
            {
                // Fallback to local PlayerPrefs or default if RemoteConfig not ready/used
                masterAnalyticsFlag = PlayerPrefs.GetInt("EnableAnalyticsMasterFlag", 1) == 1;
            }

            _isAnalyticsCollectionEnabled = masterAnalyticsFlag; // Initial state based on master flag
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(_isAnalyticsCollectionEnabled && GetUserConsent()); // Also check user consent

            IsInitialized = true;
            Debug.Log($"FirebaseAnalyticsService Initialized. Collection enabled: {FirebaseAnalytics.IsAnalyticsEnabled()}");
        }
        
        private bool GetUserConsent()
        {
            // Placeholder for actual user consent logic (e.g., from a UserConsentManager)
            return PlayerPrefs.GetInt("UserConsentForAnalytics", 1) == 1; // Default to true for placeholder
        }

        public void SetAnalyticsUserConsent(bool hasConsented)
        {
            PlayerPrefs.SetInt("UserConsentForAnalytics", hasConsented ? 1 : 0);
            PlayerPrefs.Save();
            // Re-evaluate collection status
            bool masterAnalyticsFlag = _remoteConfigService?.GetBool(ENABLE_ANALYTICS_FLAG, true) ?? (PlayerPrefs.GetInt("EnableAnalyticsMasterFlag", 1) == 1);
            SetAnalyticsCollectionEnabled(masterAnalyticsFlag && hasConsented);
        }


        public void SetAnalyticsCollectionEnabled(bool enabled)
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("FirebaseAnalyticsService: SetAnalyticsCollectionEnabled called before initialization.");
                // Store the desired state to apply upon initialization if needed
                _isAnalyticsCollectionEnabled = enabled; // This will be applied with master flag logic in Initialize
                return;
            }
            
            _isAnalyticsCollectionEnabled = enabled; // This is the specific call after checking master flags and consent
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(_isAnalyticsCollectionEnabled);
            Debug.Log($"FirebaseAnalyticsService: Analytics collection explicitly set to: {_isAnalyticsCollectionEnabled}");
        }

        public void LogEvent(string eventName, params Parameter[] parameters)
        {
            if (!IsInitialized || !FirebaseAnalytics.IsAnalyticsEnabled())
            {
                Debug.LogWarning($"FirebaseAnalyticsService: Analytics not initialized or collection disabled. Event '{eventName}' not logged.");
                return;
            }

            if (string.IsNullOrEmpty(eventName))
            {
                Debug.LogError("FirebaseAnalyticsService: Event name cannot be null or empty.");
                return;
            }

            if (parameters == null || parameters.Length == 0)
            {
                FirebaseAnalytics.LogEvent(eventName);
            }
            else
            {
                FirebaseAnalytics.LogEvent(eventName, parameters);
            }
            Debug.Log($"FirebaseAnalyticsService: Logged event '{eventName}'.");
        }
        
        public void LogEvent(string eventName, Dictionary<string, object> parameters)
        {
            if (!IsInitialized || !FirebaseAnalytics.IsAnalyticsEnabled()) return;
            if (string.IsNullOrEmpty(eventName)) return;

            if (parameters == null || parameters.Count == 0)
            {
                FirebaseAnalytics.LogEvent(eventName);
            }
            else
            {
                List<Parameter> firebaseParams = new List<Parameter>();
                foreach(var kvp in parameters)
                {
                    if (kvp.Value is string strVal) firebaseParams.Add(new Parameter(kvp.Key, strVal));
                    else if (kvp.Value is long longVal) firebaseParams.Add(new Parameter(kvp.Key, longVal));
                    else if (kvp.Value is double doubleVal) firebaseParams.Add(new Parameter(kvp.Key, doubleVal));
                    else if (kvp.Value is int intVal) firebaseParams.Add(new Parameter(kvp.Key, (long)intVal)); // Convert int to long
                    else if (kvp.Value is bool boolVal) firebaseParams.Add(new Parameter(kvp.Key, boolVal ? 1L: 0L)); // Convert bool to long
                    // Add other type conversions as needed
                }
                FirebaseAnalytics.LogEvent(eventName, firebaseParams.ToArray());
            }
            Debug.Log($"FirebaseAnalyticsService: Logged event '{eventName}' with dictionary params.");
        }


        public void SetUserProperty(string name, string value)
        {
            if (!IsInitialized || !FirebaseAnalytics.IsAnalyticsEnabled())
            {
                 Debug.LogWarning($"FirebaseAnalyticsService: Analytics not initialized or collection disabled. User property '{name}' not set.");
                return;
            }
             if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("FirebaseAnalyticsService: User property name cannot be null or empty.");
                return;
            }

            FirebaseAnalytics.SetUserProperty(name, value);
            Debug.Log($"FirebaseAnalyticsService: Set user property '{name}' to '{value}'.");
        }

        public void SetUserId(string userId)
        {
            if (!IsInitialized || !FirebaseAnalytics.IsAnalyticsEnabled()) return;
            FirebaseAnalytics.SetUserId(userId);
            Debug.Log($"FirebaseAnalyticsService: Set UserId to '{userId}'.");
        }
    }
}