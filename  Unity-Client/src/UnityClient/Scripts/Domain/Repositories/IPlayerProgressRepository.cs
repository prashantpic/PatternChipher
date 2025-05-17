using System.Threading.Tasks;
using PatternCipher.Client.Domain.Aggregates; // For PlayerProgressAggregate (assumed to exist)

// Assuming PlayerProgressAggregate exists in this namespace or is imported
// namespace PatternCipher.Client.Domain.Aggregates { public class PlayerProgressAggregate { /* ... */ } }


namespace PatternCipher.Client.Domain.Repositories
{
    public interface IPlayerProgressRepository
    {
        Task SaveProgressAsync(PlayerProgressAggregate progress);
        Task<PlayerProgressAggregate> LoadProgressAsync();
        Task DeleteProgressAsync();
        Task<bool> HasSavedProgressAsync();
    }
}