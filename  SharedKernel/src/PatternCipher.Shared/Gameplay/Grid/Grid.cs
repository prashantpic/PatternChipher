using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PatternCipher.Shared.Gameplay.Grid
{
    /// <summary>
    /// A data model representing the game board, containing a collection of tiles and its dimensions.
    /// Provides a clean interface for querying the state of the grid.
    /// </summary>
    public class Grid
    {
        public int Width { get; }
        public int Height { get; }
        private readonly IReadOnlyDictionary<GridPosition, Tile> _tiles;

        public Grid(int width, int height, IEnumerable<Tile> initialTiles)
        {
            Width = width;
            Height = height;
            _tiles = new ReadOnlyDictionary<GridPosition, Tile>(initialTiles.ToDictionary(t => t.Position));
        }

        public Tile GetTileAt(GridPosition position)
        {
            return _tiles.TryGetValue(position, out var tile) ? tile : null;
        }
        
        public bool TryGetTileAt(GridPosition position, out Tile tile)
        {
            return _tiles.TryGetValue(position, out tile);
        }

        public IEnumerable<Tile> GetAllTiles()
        {
            return _tiles.Values;
        }
    }
}