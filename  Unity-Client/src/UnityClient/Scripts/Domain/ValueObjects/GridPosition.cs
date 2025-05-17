using System;

namespace PatternCipher.Client.Domain.ValueObjects
{
    public struct GridPosition : IEquatable<GridPosition>
    {
        public readonly int Row;
        public readonly int Column;

        public GridPosition(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public bool IsAdjacentTo(GridPosition other)
        {
            int rowDiff = Math.Abs(Row - other.Row);
            int colDiff = Math.Abs(Column - other.Column);
            return (rowDiff == 1 && colDiff == 0) || (rowDiff == 0 && colDiff == 1);
        }
        
        // Manhattan distance
        public int DistanceTo(GridPosition other)
        {
            return Math.Abs(Row - other.Row) + Math.Abs(Column - other.Column);
        }

        public override bool Equals(object obj)
        {
            return obj is GridPosition other && Equals(other);
        }

        public bool Equals(GridPosition other)
        {
            return Row == other.Row && Column == other.Column;
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 23 + Row.GetHashCode();
                hash = hash * 23 + Column.GetHashCode();
                return hash;
            }
        }

        public static bool operator ==(GridPosition left, GridPosition right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GridPosition left, GridPosition right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return $"({Row}, {Column})";
        }

        public static GridPosition Zero => new GridPosition(0, 0);
        public static GridPosition Up => new GridPosition(-1, 0);    // Assuming row decreases upwards
        public static GridPosition Down => new GridPosition(1, 0);  // Assuming row increases downwards
        public static GridPosition Left => new GridPosition(0, -1);
        public static GridPosition Right => new GridPosition(0, 1);

        public GridPosition Add(GridPosition other)
        {
            return new GridPosition(Row + other.Row, Column + other.Column);
        }
         public GridPosition Subtract(GridPosition other)
        {
            return new GridPosition(Row - other.Row, Column - other.Column);
        }
    }
}