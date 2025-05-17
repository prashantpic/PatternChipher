using System;
using System.Collections.Generic;
using System.Linq;
using PatternCipher.Domain.ValueObjects;

namespace PatternCipher.Domain.Entities
{
    /// <summary>
    /// Entity representing the game grid structure and its tiles.
    /// Models the structure and content of the puzzle grid, including its dimensions and the collection of Tiles.
    /// </summary>
    public class PuzzleGrid
    {
        public GridDimensions Dimensions { get; }
        private readonly Dictionary<TilePosition, Tile> _tiles;

        public PuzzleGrid(GridDimensions dimensions, IEnumerable<Tile> initialTiles)
        {
            Dimensions = dimensions;
            _tiles = new Dictionary<TilePosition, Tile>();

            if (initialTiles == null)
            {
                throw new ArgumentNullException(nameof(initialTiles));
            }

            foreach (var tile in initialTiles)
            {
                if (!IsPositionValid(tile.Position))
                {
                    throw new ArgumentOutOfRangeException(nameof(initialTiles), $"Tile position {tile.Position} is out of bounds for grid dimensions {dimensions}.");
                }
                if (_tiles.ContainsKey(tile.Position))
                {
                    throw new ArgumentException($"Duplicate tile position {tile.Position} in initial tiles.", nameof(initialTiles));
                }
                _tiles[tile.Position] = tile;
            }
        }

        public bool IsPositionValid(TilePosition pos)
        {
            return pos.X >= 0 && pos.X < Dimensions.Columns &&
                   pos.Y >= 0 && pos.Y < Dimensions.Rows;
        }

        public Tile GetTile(TilePosition pos)
        {
            if (!IsPositionValid(pos))
            {
                throw new ArgumentOutOfRangeException(nameof(pos), $"Position {pos} is out of grid bounds.");
            }
            return _tiles.TryGetValue(pos, out var tile) ? tile : null; // Or throw if tile must exist
        }

        public void SetTile(TilePosition pos, Tile tile)
        {
            if (!IsPositionValid(pos))
            {
                throw new ArgumentOutOfRangeException(nameof(pos), $"Position {pos} is out of grid bounds.");
            }
            if (tile == null)
            {
                throw new ArgumentNullException(nameof(tile));
            }
            if (tile.Position != pos)
            {
                 // Optional: enforce tile's internal position matches, or update it.
                 // For now, assume the tile passed is correctly positioned or its position property will be updated by caller.
                 // tile.SetPosition(pos); // If Tile had a public SetPosition and we want to enforce consistency.
            }
            _tiles[pos] = tile;
        }

        public void SwapTiles(TilePosition pos1, TilePosition pos2)
        {
            if (!IsPositionValid(pos1))
            {
                throw new ArgumentOutOfRangeException(nameof(pos1), $"Position {pos1} is out of grid bounds.");
            }
            if (!IsPositionValid(pos2))
            {
                throw new ArgumentOutOfRangeException(nameof(pos2), $"Position {pos2} is out of grid bounds.");
            }

            var tile1 = GetTile(pos1);
            var tile2 = GetTile(pos2);

            if (tile1 == null) throw new InvalidOperationException($"No tile at position {pos1} to swap.");
            if (tile2 == null) throw new InvalidOperationException($"No tile at position {pos2} to swap.");

            // Swap in dictionary
            _tiles[pos1] = tile2;
            _tiles[pos2] = tile1;

            // Update tile's internal position
            tile1.SetPosition(pos2);
            tile2.SetPosition(pos1);
        }

        public IEnumerable<Tile> GetAllTiles()
        {
            return _tiles.Values.ToList().AsReadOnly();
        }
        
        // GetNeighbors method from SDS
        public IEnumerable<TilePosition> GetNeighbors(TilePosition pos)
        {
            if (!IsPositionValid(pos))
            {
                throw new ArgumentOutOfRangeException(nameof(pos), $"Position {pos} is out of grid bounds.");
            }

            var neighbors = new List<TilePosition>();
            int[] dX = { 0, 0, 1, -1 }; // Right, Left, Down, Up
            int[] dY = { 1, -1, 0, 0 }; // Corresponds to Columns for X, Rows for Y

            for (int i = 0; i < 4; i++)
            {
                var neighborPos = new TilePosition(pos.X + dX[i], pos.Y + dY[i]);
                if (IsPositionValid(neighborPos))
                {
                    neighbors.Add(neighborPos);
                }
            }
            return neighbors;
        }

        // ApplyStateChange method from SDS
        public void ApplyStateChange(TilePosition pos, TileState newState)
        {
            var tile = GetTile(pos);
            if (tile == null)
            {
                throw new InvalidOperationException($"No tile at position {pos} to apply state change.");
            }
            tile.SetState(newState);
        }
    }
}