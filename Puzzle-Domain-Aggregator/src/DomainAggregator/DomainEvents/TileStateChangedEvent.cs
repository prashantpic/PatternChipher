using System;
using System.Collections.Generic;
using System.Linq;
using PatternCipher.Domain.Entities;

namespace PatternCipher.Domain.DomainEvents
{
    /// <summary>
    /// Domain event raised when a tile's state changes due to player interaction or game logic.
    /// Signals that one or more tiles have changed their state (symbol, type, locked status).
    /// </summary>
    public class TileStateChangedEvent
    {
        public Guid PuzzleId { get; }
        public IEnumerable<Tile> AffectedTiles { get; }

        public TileStateChangedEvent(Guid puzzleId, IEnumerable<Tile> affectedTiles)
        {
            PuzzleId = puzzleId;
            AffectedTiles = affectedTiles?.ToList().AsReadOnly() ?? throw new ArgumentNullException(nameof(affectedTiles));
        }
    }
}