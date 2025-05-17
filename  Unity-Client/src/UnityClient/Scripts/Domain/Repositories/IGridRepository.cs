using System.Threading.Tasks;
using PatternCipher.Client.Domain.Aggregates; // For GridAggregate

namespace PatternCipher.Client.Domain.Repositories
{
    public interface IGridRepository
    {
        Task SaveGridStateAsync(GridAggregate grid, string saveSlotId);
        Task<GridAggregate> LoadGridStateAsync(string saveSlotId);
        Task DeleteGridStateAsync(string saveSlotId); // Optional: For clearing a slot
        Task<bool> HasSavedGridStateAsync(string saveSlotId); // Optional: To check existence
    }
}