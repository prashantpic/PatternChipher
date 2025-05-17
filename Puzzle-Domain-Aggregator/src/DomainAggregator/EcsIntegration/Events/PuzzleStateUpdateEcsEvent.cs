using System;

namespace PatternCipher.Domain.EcsIntegration.Events
{
    public enum EcsGameEventType
    {
        PuzzleSolved,
        MoveValidated,
        TileStateChanged
        // Add more specific event types as needed
    }

    /// <summary>
    /// Data-only struct representing a puzzle state update event for ECS systems.
    /// This struct would typically implement Unity.Entities.IComponentData if used directly in Unity ECS.
    /// </summary>
    public readonly struct PuzzleStateUpdateEcsEvent
    {
        public Guid PuzzleId { get; }
        public EcsGameEventType EventType { get; }
        // public object Payload { get; } // Example: Can hold event-specific data, consider typed payloads or union-like structs for complex events

        public PuzzleStateUpdateEcsEvent(Guid puzzleId, EcsGameEventType eventType /*, object payload = null */)
        {
            PuzzleId = puzzleId;
            EventType = eventType;
            // Payload = payload;
        }
    }
}