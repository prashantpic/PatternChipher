using System;

namespace PatternCipher.Infrastructure.Firebase.Common
{
    /// <summary>
    /// Represents the outcome of a Firebase operation that does not return a value.
    /// It encapsulates success or failure status and provides access to an error object on failure.
    /// </summary>
    public readonly struct FirebaseResult
    {
        /// <summary>
        /// Gets a value indicating whether the operation was successful.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Gets the error details if the operation failed. Will be null on success.
        /// </summary>
        public FirebaseError Error { get; }

        private FirebaseResult(bool isSuccess, FirebaseError error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        /// <summary>
        /// Creates a success result.
        /// </summary>
        public static FirebaseResult Success() => new FirebaseResult(true, default);

        /// <summary>
        /// Creates a failure result with the specified error.
        /// </summary>
        public static FirebaseResult Failure(FirebaseError error) => new FirebaseResult(false, error);
    }

    /// <summary>
    /// Represents the outcome of a Firebase operation that returns a value of type <typeparamref name="T"/>.
    /// It encapsulates success or failure and provides access to the result value or an error object.
    /// </summary>
    /// <typeparam name="T">The type of the value returned by the operation.</typeparam>
    public readonly struct FirebaseResult<T>
    {
        /// <summary>
        /// Gets a value indicating whether the operation was successful.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Gets the successful result value. Will be default on failure.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Gets the error details if the operation failed. Will be null on success.
        /// </summary>
        public FirebaseError Error { get; }

        private FirebaseResult(bool isSuccess, T value, FirebaseError error)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
        }

        /// <summary>
        /// Creates a success result with the specified value.
        /// </summary>
        public static FirebaseResult<T> Success(T value) => new FirebaseResult<T>(true, value, default);

        /// <summary>
        /// Creates a failure result with the specified error.
        /// </summary>
        public static FirebaseResult<T> Failure(FirebaseError error) => new FirebaseResult<T>(false, default, error);
    }
}