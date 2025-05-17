using System.Threading.Tasks;

namespace PatternCipher.Domain.Interfaces
{
    /// <summary>
    /// Abstracts the source of remote configurations like versioned rule sets.
    /// Implementation likely using Firebase Remote Config via REPO-UNITY-CLIENT or another infrastructure layer.
    /// </summary>
    public interface IRemoteConfigProvider
    {
        /// <summary>
        /// Gets a configuration value of a specified type.
        /// </summary>
        /// <typeparam name="T">The type of the configuration value.</typeparam>
        /// <param name="key">The configuration key.</param>
        /// <param name="defaultValue">The default value to return if the key is not found or type conversion fails.</param>
        /// <returns>The configuration value.</returns>
        T GetConfigValue<T>(string key, T defaultValue);

        /// <summary>
        /// Fetches the latest configurations from the remote source.
        /// </summary>
        Task FetchConfigsAsync();
    }
}