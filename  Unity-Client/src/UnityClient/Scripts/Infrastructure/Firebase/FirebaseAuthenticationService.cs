using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using UnityEngine; // For Debug

namespace PatternCipher.Client.Infrastructure.Firebase
{
    public class FirebaseAuthenticationService
    {
        private FirebaseAuth _auth;
        public FirebaseUser CurrentUser => _auth?.CurrentUser;
        public event Action<FirebaseUser> OnAuthStateChanged;

        public bool IsInitialized { get; private set; } = false;

        public FirebaseAuthenticationService()
        {
            // Initialization of _auth should happen after FirebaseApp is ready.
            // This constructor is just for instantiation. Call InitializeAsync separately.
        }

        public async Task InitializeAsync()
        {
            if (IsInitialized) return;

            // Assuming FirebaseApp.CheckAndFixDependenciesAsync() has been called elsewhere (e.g., FirebaseServiceInitializer)
            _auth = FirebaseAuth.DefaultInstance;
            if (_auth != null)
            {
                _auth.StateChanged += AuthStateChanged;
                AuthStateChanged(this, null); // Initial check
                IsInitialized = true;
                Debug.Log("FirebaseAuthenticationService Initialized.");
            }
            else
            {
                Debug.LogError("FirebaseAuthenticationService: FirebaseAuth.DefaultInstance is null. Firebase not initialized correctly.");
            }
            await Task.CompletedTask; // Or return a Task from an actual async init if needed
        }

        private void AuthStateChanged(object sender, EventArgs eventArgs)
        {
            OnAuthStateChanged?.Invoke(_auth?.CurrentUser);
            if (_auth.CurrentUser == null)
            {
                Debug.Log("FirebaseAuthenticationService: User signed out.");
            }
            else
            {
                Debug.Log($"FirebaseAuthenticationService: User signed in: {CurrentUser.UserId}");
            }
        }

        public async Task<FirebaseUser> SignInAnonymouslyAsync()
        {
            if (!IsInitialized) { Debug.LogError("Firebase Auth not initialized."); return null; }
            try
            {
                FirebaseUser newUser = await _auth.SignInAnonymouslyAsync();
                Debug.Log($"FirebaseAuthenticationService: Signed in anonymously: {newUser.UserId}");
                return newUser;
            }
            catch (Exception ex)
            {
                Debug.LogError($"FirebaseAuthenticationService: Anonymous sign-in failed: {ex.Message}");
                return null;
            }
        }

        public async Task<FirebaseUser> SignInWithGoogleAsync(string idToken) // idToken obtained from Google Sign-In SDK
        {
            if (!IsInitialized) { Debug.LogError("Firebase Auth not initialized."); return null; }
            if (string.IsNullOrEmpty(idToken))
            {
                 Debug.LogError("FirebaseAuthenticationService: Google ID Token is null or empty.");
                 return null;
            }

            Credential credential = GoogleAuthProvider.GetCredential(idToken, null);
            try
            {
                FirebaseUser newUser = await _auth.SignInWithCredentialAsync(credential);
                Debug.Log($"FirebaseAuthenticationService: Signed in with Google: {newUser.UserId}");
                return newUser;
            }
            catch (Exception ex)
            {
                Debug.LogError($"FirebaseAuthenticationService: Google sign-in failed: {ex.Message}");
                return null;
            }
        }

        public async Task<FirebaseUser> SignInWithAppleAsync(string idToken, string rawNonce) // Parameters from Apple Sign-In
        {
             if (!IsInitialized) { Debug.LogError("Firebase Auth not initialized."); return null; }
            if (string.IsNullOrEmpty(idToken) || string.IsNullOrEmpty(rawNonce))
            {
                 Debug.LogError("FirebaseAuthenticationService: Apple ID Token or Nonce is null or empty.");
                 return null;
            }
            Credential credential = OAuthProvider.GetCredential("apple.com", idToken, rawNonce, null);
            try
            {
                FirebaseUser newUser = await _auth.SignInWithCredentialAsync(credential);
                Debug.Log($"FirebaseAuthenticationService: Signed in with Apple: {newUser.UserId}");
                return newUser;
            }
            catch (Exception ex)
            {
                Debug.LogError($"FirebaseAuthenticationService: Apple sign-in failed: {ex.Message}");
                return null;
            }
        }
        
        public async Task LinkWithGoogleAsync(string idToken)
        {
            if (!IsInitialized || CurrentUser == null) { Debug.LogError("Firebase Auth not initialized or no user signed in."); return; }
            if (string.IsNullOrEmpty(idToken)) return;

            Credential credential = GoogleAuthProvider.GetCredential(idToken, null);
            try
            {
                await CurrentUser.LinkWithCredentialAsync(credential);
                Debug.Log("FirebaseAuthenticationService: Successfully linked Google account.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"FirebaseAuthenticationService: Google link failed: {ex.Message}");
            }
        }


        public void SignOut()
        {
            if (!IsInitialized) { Debug.LogError("Firebase Auth not initialized."); return; }
            _auth.SignOut();
        }
        
        public bool IsUserSignedIn()
        {
            return IsInitialized && CurrentUser != null;
        }

        public string GetCurrentUserId()
        {
            return IsUserSignedIn() ? CurrentUser.UserId : null;
        }

        public void Teardown()
        {
            if (_auth != null)
            {
                _auth.StateChanged -= AuthStateChanged;
            }
            IsInitialized = false;
        }
    }
}