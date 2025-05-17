namespace PatternCipher.Client.Domain.Entities
{
    public enum SpecialType
    {
        None,
        Bomb,
        LineClearHorizontal,
        LineClearVertical,
        ColorBomb,
        // Add more special types
    }

    public class Symbol
    {
        public string Id { get; private set; }
        public bool IsSpecial { get; private set; }
        public SpecialType SpecialAbilityType { get; private set; }
        // Add other gameplay-relevant characteristics if needed

        public Symbol(string id, bool isSpecial = false, SpecialType specialType = SpecialType.None)
        {
            Id = id;
            IsSpecial = isSpecial;
            SpecialAbilityType = specialType;
        }
    }
}