using System.Threading.Tasks;

namespace PatternCipher.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// An interface for abstracting the raw file I/O operations,
    /// making the PersistenceService independent of the specific file system API.
    /// </summary>
    public interface IFileRepository
    {
        /// <summary>
        /// Asynchronously reads the entire content of a file.
        /// </summary>
        /// <param name="filePath">The full path to the file.</param>
        /// <returns>A task that represents the asynchronous read operation. The task result contains the file content as a string.</returns>
        Task<string> ReadAllTextAsync(string filePath);

        /// <summary>
        /// Asynchronously writes the given content to a file, overwriting it if it already exists.
        /// </summary>
        /// <param name="filePath">The full path to the file.</param>
        /// <param name="content">The string content to write to the file.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        Task WriteAllTextAsync(string filePath, string content);

        /// <summary>
        /// Checks if the specified file exists.
        /// </summary>
        /// <param name="filePath">The full path to the file.</param>
        /// <returns>True if the file exists, otherwise false.</returns>
        bool FileExists(string filePath);

        /// <summary>
        /// Deletes the specified file.
        /// </summary>
        /// <param name="filePath">The full path to the file.</param>
        void DeleteFile(string filePath);
    }
}