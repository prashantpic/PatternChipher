using PatternCipher.Services.Contracts;
using System.Threading.Tasks;

namespace PatternCipher.Services.Interfaces
{
    /// <summary>
    /// Adapter interface for the CloudSaveRepository.
    /// Abstracts interactions with the cloud save repository for persisting
    /// generated level data, including solution paths and par values.
    /// </summary>
    public interface ICloudSaveRepositoryAdapter
    {
        /// <summary>
        /// Asynchronously stores the generated level data to the cloud save repository.
        /// </summary>
        /// <param name="levelData">The generated level data to store.</param>
        /// <returns>A task that represents the asynchronous storage operation.</returns>
        Task StoreGeneratedLevelAsync(GeneratedLevelData levelData);
    }
}