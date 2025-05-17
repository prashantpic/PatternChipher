using System;

namespace PatternCipher.Domain.ValueObjects
{
    /// <summary>
    /// Value object representing the dimensions (rows, columns) of the puzzle grid.
    /// </summary>
    public readonly struct GridDimensions : IEquatable<GridDimensions>
    {
        public int Rows { get; }
        public int Columns { get; }

        public GridDimensions(int rows, int columns)
        {
            if (rows <= 0)
                throw new ArgumentOutOfRangeException(nameof(rows), "Rows must be positive.");
            if (columns <= 0)
                throw new ArgumentOutOfRangeException(nameof(columns), "Columns must be positive.");
            Rows = rows;
            Columns = columns;
        }

        public override bool Equals(object? obj)
        {
            return obj is GridDimensions dimensions && Equals(dimensions);
        }

        public bool Equals(GridDimensions other)
        {
            return Rows == other.Rows && Columns == other.Columns;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Rows, Columns);
        }

        public static bool operator ==(GridDimensions left, GridDimensions right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GridDimensions left, GridDimensions right)
        {
            return !(left == right);
        }

        public override string ToString() => $"{Rows}x{Columns}";
    }
}