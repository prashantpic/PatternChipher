using System;

namespace PatternCipher.Client.Domain.ValueObjects
{
    [System.Serializable] // Useful for Unity Inspector if embedded in MonoBehaviours
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

            // Adjacent if one coordinate differs by 1 and the other is the same
            // (Manhattan distance of 1)
            return (rowDiff == 1 && colDiff == 0) || (rowDiff == 0 && colDiff == 1);
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

        // Example utility methods
        public static GridPosition Zero => new GridPosition(0, 0);
        public static GridPosition Up => new GridPosition(-1, 0);    // Assuming (0,0) is top-left, Row increases downwards
        public static GridPosition Down => new GridPosition(1, 0);
        public static GridPosition Left => new GridPosition(0, -1);
        public static GridPosition Right => new GridPosition(0, 1);

        public static GridPosition operator +(GridPosition a, GridPosition b)
        {
            return new GridPosition(a.Row + b.Row, a.Column + b.Column);
        }
        
        public static GridPosition operator -(GridPosition a, GridPosition b)
        {
            return new GridPosition(a.Row - b.Row, a.Column - b.Column);
        }
    }
}