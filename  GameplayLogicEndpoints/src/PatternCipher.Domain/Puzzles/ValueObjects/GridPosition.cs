using System;

namespace PatternCipher.Domain.Puzzles.ValueObjects
{
    /// <summary>
    /// Represents an immutable coordinate on the grid using a row and column.
    /// Provides value-based equality.
    /// </summary>
    public readonly record struct GridPosition(int Row, int Column)
    {
        /// <summary>
        /// Checks if another position is directly adjacent (not diagonally).
        /// </summary>
        /// <param name="other">The other position to compare against.</param>
        /// <returns>True if the Manhattan distance is 1, otherwise false.</returns>
        public bool IsAdjacentTo(GridPosition other)
        {
            // Manhattan distance is |x1 - x2| + |y1 - y2|
            return Math.Abs(Row - other.Row) + Math.Abs(Column - other.Column) == 1;
        }

        /// <summary>
        /// Provides a string representation of the grid position.
        /// </summary>
        /// <returns>A string in the format "(Row, Column)".</returns>
        public override string ToString() => $"({Row}, {Column})";
    }
}