namespace PatternCipher.Client.Domain.Entities
{
    public enum SpecialType
    {
        None,
        Bomb,
        LineClear,
        ColorClear
        // Add more special types as needed
    }

    public class Symbol
    {
        public string Id { get; private set; }
        public bool IsSpecial { get; private set; }
        public SpecialType Type { get; private set; }

        // Constructor for a normal symbol
        public Symbol(string id)
        {
            Id = id;
            IsSpecial = false;
            Type = SpecialType.None;
        }

        // Constructor for a special symbol
        public Symbol(string id, SpecialType specialType)
        {
            Id = id;
            IsSpecial = true;
            Type = specialType;
        }

        // Add any domain-relevant properties or methods here
        // For example, rules for how this symbol interacts or matches.
    }
}