using PatternCipher.Services.Contracts;
using System.Threading.Tasks;

namespace PatternCipher.Services.Interfaces
{
    /// <summary>
    /// Adapter interface for Firebase Functions based validation from REPO-FIREBASE-BACKEND.
    /// Abstracts server-side validation calls to Firebase Functions for generated levels.
    /// </summary>
    public interface IFirebaseValidationServiceAdapter
    {
        /// <summary>
        /// Asynchronously validates a generated level on the server using Firebase Functions.
        /// </summary>
        /// <param name="validationRequest">The request containing data for level validation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains a <see cref="LevelValidationResult"/> indicating the outcome of the server-side validation.
        /// </returns>
        Task<LevelValidationResult> ValidateLevelOnServerAsync(LevelValidationRequest validationRequest);
    }
}