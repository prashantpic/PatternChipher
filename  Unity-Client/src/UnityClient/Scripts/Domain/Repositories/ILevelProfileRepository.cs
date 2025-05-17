using System.Threading.Tasks;
using PatternCipher.Client.Domain.Aggregates; // For LevelProfileAggregate

namespace PatternCipher.Client.Domain.Repositories
{
    public interface ILevelProfileRepository
    {
        Task SaveLevelProfileAsync(LevelProfileAggregate profile);
        Task<LevelProfileAggregate> LoadLevelProfileAsync(string levelId); // Assuming levelId is string, adjust if int
        Task DeleteLevelProfileAsync(string levelId); // Optional
        Task<bool> HasLevelProfileAsync(string levelId); // Optional
    }
}