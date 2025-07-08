using System.IO;
using System.Threading.Tasks;

namespace PatternCipher.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// A repository implementation that interacts directly with the local file system
    /// to persist and retrieve string-based data, such as serialized JSON.
    /// </summary>
    public class LocalFileRepository : IFileRepository
    {
        private readonly string _persistentDataPath;

        /// <summary>
        /// Initializes a new instance of the LocalFileRepository.
        /// </summary>
        /// <param name="persistentDataPath">The base path for persistent storage (e.g., Unity's Application.persistentDataPath).</param>
        public LocalFileRepository(string persistentDataPath)
        {
            _persistentDataPath = persistentDataPath;
            if (!Directory.Exists(_persistentDataPath))
            {
                Directory.CreateDirectory(_persistentDataPath);
            }
        }

        private string GetFullPath(string fileName)
        {
            return Path.Combine(_persistentDataPath, fileName);
        }

        /// <inheritdoc/>
        public bool FileExists(string fileName)
        {
            return File.Exists(GetFullPath(fileName));
        }

        /// <inheritdoc/>
        public void DeleteFile(string fileName)
        {
            var fullPath = GetFullPath(fileName);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        /// <inheritdoc/>
        public async Task<string> ReadAllTextAsync(string fileName)
        {
            var fullPath = GetFullPath(fileName);
            // Use Task.Run to offload the synchronous file I/O from the main thread (e.g., Unity's UI thread).
            return await Task.Run(() => File.ReadAllText(fullPath));
        }

        /// <inheritdoc/>
        public async Task WriteAllTextAsync(string fileName, string content)
        {
            var fullPath = GetFullPath(fileName);
            // Use Task.Run to offload the synchronous file I/O from the main thread.
            await Task.Run(() => File.WriteAllText(fullPath, content));
        }
    }
}