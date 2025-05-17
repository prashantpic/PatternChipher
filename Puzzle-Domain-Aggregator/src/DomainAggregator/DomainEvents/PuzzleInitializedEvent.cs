using System;
using System.Collections.Generic;
using System.Linq;
using PatternCipher.Domain.Entities;
using PatternCipher.Domain.ValueObjects;

namespace PatternCipher.Domain.DomainEvents
{
    /// <summary>
    /// Domain event raised when a new puzzle instance is successfully initialized.
    /// Signals that a puzzle has been created and is ready for interaction.
    /// </summary>
    public class PuzzleInitializedEvent
    {
        public Guid PuzzleId { get; }
        public GridDimensions Dimensions { get; }
        public IEnumerable<Tile> InitialTiles { get; }

        public PuzzleInitializedEvent(Guid puzzleId, GridDimensions dimensions, IEnumerable<Tile> initialTiles)
        {
            PuzzleId = puzzleId;
            Dimensions = dimensions;
            InitialTiles = initialTiles?.ToList().AsReadOnly() ?? throw new ArgumentNullException(nameof(initialTiles));
        }
    }
}