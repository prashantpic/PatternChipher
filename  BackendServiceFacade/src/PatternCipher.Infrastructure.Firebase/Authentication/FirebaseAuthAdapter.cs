using Firebase;
using Firebase.Auth;
using PatternCipher.Application.Services;
using PatternCipher.Infrastructure.Firebase.Common;
using System;
using System.Threading.Tasks;

namespace PatternCipher.Infrastructure.Firebase.Authentication
{
    /// <summary>
    /// Implements the IAuthenticationService interface using the Firebase Authentication SDK.
    /// This adapter translates application-level authentication requests into Firebase-specific calls.
    /// </summary>
    public class FirebaseAuthAdapter : IAuthenticationService, IDisposable
    {
        private readonly FirebaseAuth _auth;

        /// <inheritdoc/>
        public event Action<string> OnUserSignedIn;

        /// <inheritdoc/>
        public event Action OnUserSignedOut;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirebaseAuthAdapter"/> class.
        /// </summary>
        public FirebaseAuthAdapter()
        {
            _auth = FirebaseAuth.DefaultInstance;
            _auth.StateChanged += HandleAuthStateChanged;
        }

        /// <inheritdoc/>
        public bool IsSignedIn() => _auth.CurrentUser != null;

        /// <inheritdoc/>
        public string GetCurrentUserId() => _auth.CurrentUser?.UserId;

        /// <inheritdoc/>
        public async Task<FirebaseResult<string>> SignInAnonymouslyAsync()
        {
            try
            {
                FirebaseUser user = await _auth.SignInAnonymouslyAsync();
                return FirebaseResult<string>.Success(user.UserId);
            }
            catch (FirebaseException ex)
            {
                return FirebaseResult<string>.Failure(new FirebaseError(ex.ErrorCode, ex.Message, ex));
            }
            catch (Exception ex)
            {
                return FirebaseResult<string>.Failure(new FirebaseError(0, ex.Message, ex));
            }
        }

        /// <inheritdoc/>
        public async Task<FirebaseResult<string>> SignInWithGoogleAsync(string idToken)
        {
            try
            {
                Credential credential = GoogleAuthProvider.GetCredential(idToken, null);
                FirebaseUser user = await _auth.SignInWithCredentialAsync(credential);
                return FirebaseResult<string>.Success(user.UserId);
            }
            catch (FirebaseException ex)
            {
                return FirebaseResult<string>.Failure(new FirebaseError(ex.ErrorCode, ex.Message, ex));
            }
            catch (Exception ex)
            {
                return FirebaseResult<string>.Failure(new FirebaseError(0, ex.Message, ex));
            }
        }

        /// <inheritdoc/>
        public async Task<FirebaseResult<string>> SignInWithAppleAsync(string idToken)
        {
            try
            {
                var provider = new OAuthProvider("apple.com");
                Credential credential = provider.GetCredential(idToken, null, null); // nonce can be added here if needed
                FirebaseUser user = await _auth.SignInWithCredentialAsync(credential);
                return FirebaseResult<string>.Success(user.UserId);
            }
            catch (FirebaseException ex)
            {
                return FirebaseResult<string>.Failure(new FirebaseError(ex.ErrorCode, ex.Message, ex));
            }
            catch (Exception ex)
            {
                return FirebaseResult<string>.Failure(new FirebaseError(0, ex.Message, ex));
            }
        }

        /// <inheritdoc/>
        public void SignOut()
        {
            _auth.SignOut();
        }

        private void HandleAuthStateChanged(object sender, EventArgs e)
        {
            if (_auth.CurrentUser != null)
            {
                OnUserSignedIn?.Invoke(_auth.CurrentUser.UserId);
            }
            else
            {
                OnUserSignedOut?.Invoke();
            }
        }

        /// <summary>
        /// Cleans up resources and detaches the event handler.
        /// </summary>
        public void Dispose()
        {
            _auth.StateChanged -= HandleAuthStateChanged;
        }
    }
}