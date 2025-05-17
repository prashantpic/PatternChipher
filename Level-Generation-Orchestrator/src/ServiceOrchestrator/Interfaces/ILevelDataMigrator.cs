using PatternCipher.Services.Contracts;
using System.Threading.Tasks;

namespace PatternCipher.Services.Interfaces
{
    /// <summary>
    /// Interface for migrating generated level data formats.
    /// Defines the contract for migrating generated level data
    /// between different schema versions.
    /// </summary>
    public interface ILevelDataMigrator
    {
        /// <summary>
        /// Asynchronously migrates level data from an older version to the latest format.
        /// </summary>
        /// <param name="oldLevelData">The level data in its original (potentially old) format. This is expected to be the raw layout data part.</param>
        /// <param name="sourceVersion">The version number of the <paramref name="oldLevelData"/>.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the <see cref="GeneratedLevelData"/> in the latest version format.
        /// Note: The design document indicates this method might return `GeneratedLevelData`.
        /// However, the parameters suggest it operates on `oldLevelData` (raw layout) and returns
        /// a `GeneratedLevelData` which implies it might need to reconstruct/re-wrap.
        /// For consistency, we'll assume it's intended to produce a full `GeneratedLevelData` object
        /// that has its `RawLayoutData` migrated, and other fields possibly re-evaluated or set.
        /// The caller, LevelGenerationService, will use the version from the DTO to pass to this method.
        /// The output will be a new DTO with the *latest* version.
        /// </task>
        Task<GeneratedLevelData> MigrateToLatestAsync(GeneratedLevelData oldLevelDto, int sourceVersion);
    }
}