using System;
using PatternCipher.Client.Domain.ValueObjects; // For GridPosition

namespace PatternCipher.Client.Domain.ValueObjects
{
    [System.Serializable] // Useful for Unity Inspector
    public struct GridDimensions : IEquatable<GridDimensions>
    {
        public readonly int Rows;
        public readonly int Columns;

        public GridDimensions(int rows, int columns)
        {
            if (rows <= 0) throw new ArgumentOutOfRangeException(nameof(rows), "Rows must be positive.");
            if (columns <= 0) throw new ArgumentOutOfRangeException(nameof(columns), "Columns must be positive.");
            Rows = rows;
            Columns = columns;
        }

        public bool IsValidPosition(GridPosition position)
        {
            return position.Row >= 0 && position.Row < Rows &&
                   position.Column >= 0 && position.Column < Columns;
        }

        public int TotalCells => Rows * Columns;

        public override bool Equals(object obj)
        {
            return obj is GridDimensions other && Equals(other);
        }

        public bool Equals(GridDimensions other)
        {
            return Rows == other.Rows && Columns == other.Columns;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Rows.GetHashCode();
                hash = hash * 23 + Columns.GetHashCode();
                return hash;
            }
        }

        public static bool operator ==(GridDimensions left, GridDimensions right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GridDimensions left, GridDimensions right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return $"{Rows}x{Columns}";
        }
    }
}