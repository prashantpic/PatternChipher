using PatternCipher.Domain.Puzzles.ValueObjects;
using PatternCipher.Shared.Enums;
using PatternCipher.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PatternCipher.Domain.Puzzles.Entities
{
    /// <summary>
    /// Represents the game grid, which manages the collection of tiles and their spatial relationships.
    /// It is a complex entity within the Puzzle aggregate.
    /// </summary>
    public sealed class Grid
    {
        /// <summary>
        /// Gets the number of rows in the grid.
        /// </summary>
        public int Rows { get; }

        /// <summary>
        /// Gets the number of columns in the grid.
        /// </summary>
        public int Columns { get; }

        private readonly Dictionary<GridPosition, Tile> _tiles;

        /// <summary>
        /// Initializes a new instance of the <see cref="Grid"/> class.
        /// </summary>
        /// <param name="rows">The number of rows.</param>
        /// <param name="columns">The number of columns.</param>
        /// <param name="initialTiles">The collection of tiles to populate the grid with.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if rows or columns are not positive.</exception>
        /// <exception cref="ArgumentNullException">Thrown if initialTiles is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the number of tiles does not match grid dimensions.</exception>
        public Grid(int rows, int columns, IEnumerable<Tile> initialTiles)
        {
            if (rows <= 0) throw new ArgumentOutOfRangeException(nameof(rows), "Grid must have a positive number of rows.");
            if (columns <= 0) throw new ArgumentOutOfRangeException(nameof(columns), "Grid must have a positive number of columns.");
            if (initialTiles is null) throw new ArgumentNullException(nameof(initialTiles));
            
            Rows = rows;
            Columns = columns;

            var tileList = initialTiles.ToList();
            if (tileList.Count != rows * columns)
            {
                throw new ArgumentException("The number of initial tiles must match the grid dimensions (rows * columns).");
            }
            
            _tiles = tileList.ToDictionary(tile => tile.Position);
        }

        /// <summary>
        /// Gets the tile at the specified grid position.
        /// </summary>
        /// <param name="position">The position of the tile to retrieve.</param>
        /// <returns>The tile at the specified position.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the position is not on the grid.</exception>
        public Tile GetTileAt(GridPosition position)
        {
            if (_tiles.TryGetValue(position, out var tile))
            {
                return tile;
            }
            throw new ArgumentOutOfRangeException(nameof(position), $"Position {position} is not on the grid.");
        }

        /// <summary>
        /// Validates if a move is permissible on the current grid state.
        /// </summary>
        /// <param name="move">The move to validate.</param>
        /// <returns>True if the move is valid, otherwise false.</returns>
        public bool IsMoveValid(Move move)
        {
            if (!_tiles.ContainsKey(move.Position1)) return false;

            switch (move.Type)
            {
                case MoveType.Swap:
                    if (!_tiles.ContainsKey(move.Position2)) return false;
                    if (!move.Position1.IsAdjacentTo(move.Position2)) return false;

                    var tile1 = GetTileAt(move.Position1);
                    var tile2 = GetTileAt(move.Position2);
                    return !tile1.IsLocked && !tile2.IsLocked;

                case MoveType.UpdateState:
                    var tileToUpdate = GetTileAt(move.Position1);
                    return !tileToUpdate.IsLocked;
                
                default:
                    return false;
            }
        }
        
        /// <summary>
        /// Swaps the tiles at two specified positions. Assumes the move has already been validated.
        /// </summary>
        /// <param name="pos1">The first position.</param>
        /// <param name="pos2">The second position.</param>
        public void SwapTiles(GridPosition pos1, GridPosition pos2)
        {
            var tile1 = GetTileAt(pos1);
            var tile2 = GetTileAt(pos2);

            // Create new tile records with swapped positions to maintain immutability
            var newTile1 = tile1 with { Position = pos2 };
            var newTile2 = tile2 with { Position = pos1 };

            _tiles[pos2] = newTile1;
            _tiles[pos1] = newTile2;
        }

        /// <summary>
        /// Updates the state (e.g., symbol) of a tile at a given position.
        /// </summary>
        /// <param name="position">The position of the tile to update.</param>
        /// <param name="newSymbol">The new symbol for the tile.</param>
        public void UpdateTileState(GridPosition position, Symbol newSymbol)
        {
            var tile = GetTileAt(position);
            var updatedTile = tile with { Symbol = newSymbol };
            _tiles[position] = updatedTile;
        }
        
        /// <summary>
        /// Creates a deep copy of the grid. This is essential for state exploration algorithms like A*.
        /// </summary>
        /// <returns>A new Grid instance with a copy of all tiles.</returns>
        public Grid DeepCopy()
        {
            // The Tile record 'with' keyword creates shallow copies.
            // A simple copy of the values from the dictionary is sufficient as Tile is a record (immutable).
            var newTiles = _tiles.Values.Select(t => t); // Records are copied by value implicitly
            return new Grid(Rows, Columns, newTiles);
        }
    }
}