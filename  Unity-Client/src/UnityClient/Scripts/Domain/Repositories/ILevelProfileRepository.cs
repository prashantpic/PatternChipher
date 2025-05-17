using System.Threading.Tasks;
using PatternCipher.Client.Domain.Aggregates; // For LevelProfileAggregate

namespace PatternCipher.Client.Domain.Repositories
{
    public interface ILevelProfileRepository
    {
        Task SaveLevelProfileAsync(LevelProfileAggregate profile);
        Task<LevelProfileAggregate> LoadLevelProfileAsync(string levelId); // Assuming levelId is string
        Task DeleteLevelProfileAsync(string levelId);
        Task<bool> HasLevelProfileAsync(string levelId);
        // Potentially methods to load all profiles or profiles by status
        // Task<IEnumerable<LevelProfileAggregate>> LoadAllCompletedProfilesAsync();
    }
}