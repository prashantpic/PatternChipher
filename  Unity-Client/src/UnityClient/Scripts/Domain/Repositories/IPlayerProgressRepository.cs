using System.Threading.Tasks;
using PatternCipher.Client.Domain.Aggregates; // For PlayerProgressAggregate

// If PlayerProgressAggregate is not defined elsewhere yet, create a placeholder for compilation.
// This should ideally be in its own file: PatternCipher.Client.Domain.Aggregates.PlayerProgressAggregate.cs
namespace PatternCipher.Client.Domain.Aggregates
{
    // Placeholder: Actual PlayerProgressAggregate would have properties for progress
    public class PlayerProgressAggregate 
    {
        public string PlayerId { get; set; }
        public int TotalStars { get; set; }
        // ... other overall progress data
    }
}


namespace PatternCipher.Client.Domain.Repositories
{
    public interface IPlayerProgressRepository
    {
        Task SaveProgressAsync(PlayerProgressAggregate progress);
        Task<PlayerProgressAggregate> LoadProgressAsync();
        Task DeleteProgressAsync(); // Optional: For resetting player progress
    }
}