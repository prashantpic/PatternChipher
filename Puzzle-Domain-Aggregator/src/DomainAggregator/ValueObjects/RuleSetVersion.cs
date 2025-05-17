using System;

namespace PatternCipher.Domain.ValueObjects
{
    /// <summary>
    /// Value object representing the version of a rule set.
    /// </summary>
    public readonly struct RuleSetVersion : IEquatable<RuleSetVersion>
    {
        public string Version { get; }

        public RuleSetVersion(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
                throw new ArgumentException("Version cannot be null or whitespace.", nameof(version));
            Version = version;
        }

        public override bool Equals(object? obj)
        {
            return obj is RuleSetVersion version && Equals(version);
        }

        public bool Equals(RuleSetVersion other)
        {
            return Version == other.Version;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Version);
        }

        public static bool operator ==(RuleSetVersion left, RuleSetVersion right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RuleSetVersion left, RuleSetVersion right)
        {
            return !(left == right);
        }

        public override string ToString() => Version;
    }
}