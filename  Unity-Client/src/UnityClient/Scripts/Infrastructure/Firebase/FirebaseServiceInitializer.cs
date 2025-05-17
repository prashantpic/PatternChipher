using System.Threading.Tasks;
using Firebase;
using Firebase.Analytics;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.RemoteConfig;
using UnityEngine; // For Debug

namespace PatternCipher.Client.Infrastructure.Firebase
{
    public class FirebaseServiceInitializer
    {
        public bool IsFirebaseInitialized { get; private set; } = false;

        public async Task InitializeFirebaseAsync()
        {
            FirebaseApp app = null;
            try
            {
                var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
                if (dependencyStatus == DependencyStatus.Available)
                {
                    // Create and hold a reference to your FirebaseApp,
                    // otherwise it will be garbage collected.
                    app = FirebaseApp.DefaultInstance;

                    // TODO: Set up Firebase services as needed
                    InitializeAnalytics();
                    InitializeAuth();
                    InitializeFirestore();
                    InitializeRemoteConfig();
                    // InitializeCrashlytics(); // If using Crashlytics

                    IsFirebaseInitialized = true;
                    Debug.Log("Firebase initialized successfully.");
                }
                else
                {
                    Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                    IsFirebaseInitialized = false;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Exception during Firebase initialization: {e.Message}");
                IsFirebaseInitialized = false;
            }
        }

        private void InitializeAnalytics()
        {
            // Firebase Analytics is initialized automatically if the package is present.
            // You can set default consent here if needed, or control collection later.
            // FirebaseAnalytics.SetAnalyticsCollectionEnabled(true); // Example
            Debug.Log("Firebase Analytics ready.");
        }

        private void InitializeAuth()
        {
            // Firebase Auth instance can be accessed via FirebaseAuth.DefaultInstance
            // You might want to attach state changed listeners here or in a dedicated Auth service
            // FirebaseAuth.DefaultInstance.StateChanged += AuthStateChanged;
            Debug.Log("Firebase Auth ready.");
        }

        private void InitializeFirestore()
        {
            // Firebase Firestore instance can be accessed via FirebaseFirestore.DefaultInstance
            // You might want to configure settings like persistence or timestamps here.
            // FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            // var settings = db.Settings;
            // settings.PersistenceEnabled = true; // Example for offline persistence
            // settings.AreTimestampsInSnapshotsEnabled = true;
            // db.Settings = settings;
            Debug.Log("Firebase Firestore ready.");
        }

        private async void InitializeRemoteConfig()
        {
            // Set default Remote Config values.
            // These are used if the Riche Config data isn't fetched successfully,
            // or if the app is running offline.
            var defaults = new System.Collections.Generic.Dictionary<string, object>();
            // Example:
            // defaults.Add("welcome_message", "Welcome to our game!");
            // defaults.Add("feature_flag_new_level", false);
            
            await FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults);
            Debug.Log("Firebase Remote Config defaults set.");

            // Fetch and activate can be done here or later by a dedicated service.
            // FetchAndActivateRemoteConfig(); 
        }

        // Example method, can be called by RemoteConfigService
        public async Task FetchAndActivateRemoteConfig()
        {
            if (!IsFirebaseInitialized) return;
            try
            {
                Debug.Log("Fetching Remote Config data...");
                await FirebaseRemoteConfig.DefaultInstance.FetchAsync(System.TimeSpan.Zero); // Fetch with no cache expiry for testing
                bool activated = await FirebaseRemoteConfig.DefaultInstance.ActivateAsync();
                if (activated)
                {
                    Debug.Log("Remote Config fetched and activated.");
                }
                else
                {
                    Debug.Log("Remote Config fetched but no new values to activate.");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error fetching or activating Remote Config: {e.Message}");
            }
        }

        // Placeholder for AuthStateChanged event handler if used
        // private void AuthStateChanged(object sender, System.EventArgs eventArgs)
        // {
        //     FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        //     if (user != null)
        //     {
        //         Debug.Log($"Firebase Auth: User is signed in with ID: {user.UserId}");
        //     }
        //     else
        //     {
        //         Debug.Log("Firebase Auth: User is signed out.");
        //     }
        // }
    }
}