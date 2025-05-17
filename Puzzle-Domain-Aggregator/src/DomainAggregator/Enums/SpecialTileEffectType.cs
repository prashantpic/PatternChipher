namespace PatternCipher.Domain.Enums
{
    /// <summary>
    /// Enumeration for different types of effects that special tiles can produce.
    /// </summary>
    public enum SpecialTileEffectType
    {
        ClearRow,
        ClearColumn,
        ChangeSymbol,
        UnlockAdjacent
    }
}