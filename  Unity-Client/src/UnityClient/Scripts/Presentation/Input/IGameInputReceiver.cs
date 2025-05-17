using PatternCipher.Client.Domain.ValueObjects;

namespace PatternCipher.Client.Presentation.Input
{
    public interface IGameInputReceiver
    {
        void ReceiveTileTap(GridPosition position);
        void ReceiveTileDrag(GridPosition startPosition, GridPosition endPosition);
        // Add other relevant gameplay input actions as needed, e.g.,
        // void ReceiveSwipe(GridPosition startPosition, SwipeDirection direction);
    }
}