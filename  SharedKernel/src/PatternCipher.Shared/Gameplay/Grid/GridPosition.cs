namespace PatternCipher.Shared.Gameplay.Grid
{
    /// <summary>
    /// A value object representing a tile's position on the grid (column X, row Y).
    /// Implemented as a readonly record struct for immutability and value-based equality.
    /// </summary>
    /// <param name="X">The horizontal coordinate (column).</param>
    /// <param name="Y">The vertical coordinate (row).</param>
    public readonly record struct GridPosition(int X, int Y);
}