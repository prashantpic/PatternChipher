using PatternCipher.Client.Domain.ValueObjects; // For GridPosition

namespace PatternCipher.Client.Presentation.Input
{
    public interface IGameInputReceiver
    {
        void HandleTileTap(GridPosition pos);
        void HandleTileSwap(GridPosition pos1, GridPosition pos2);
        // As per instructions: ReceiveTileTap(GridPosition position), ReceiveTileDrag(GridPosition start, GridPosition end)
        // The original prompt description for IGameInputReceiver methods (HandleTileTap, HandleTileSwap) 
        // is slightly different from the later specific instruction (ReceiveTileTap, ReceiveTileDrag).
        // Using the specific instruction for IGameInputReceiver.cs file itself.
        void ReceiveTileTap(GridPosition position);
        void ReceiveTileDrag(GridPosition start, GridPosition end);
    }
}