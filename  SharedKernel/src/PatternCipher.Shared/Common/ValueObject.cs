using System.Collections.Generic;
using System.Linq;

namespace PatternCipher.Shared.Common
{
    /// <summary>
    /// Abstract base class for creating Value Objects, a core DDD tactical pattern.
    /// Value Objects are objects whose equality is based on their constituent values, not their identity.
    /// </summary>
    public abstract class ValueObject
    {
        /// <summary>
        /// Gets the components that are used for equality comparison.
        /// </summary>
        /// <returns>An enumerable of objects representing the equality components.</returns>
        protected abstract IEnumerable<object> GetEqualityComponents();

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }

            var other = (ValueObject)obj;
            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        public override int GetHashCode()
        {
            return GetEqualityComponents()
                .Select(x => x != null ? x.GetHashCode() : 0)
                .Aggregate(17, (current, next) => current * 23 + next);
        }

        public static bool operator ==(ValueObject a, ValueObject b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;
            return a.Equals(b);
        }

        public static bool operator !=(ValueObject a, ValueObject b)
        {
            return !(a == b);
        }
    }
}