using System;
using System.Diagnostics;

namespace xggameplan.common.Types
{
    [DebuggerDisplay("{Value}")]
    public readonly struct Priority : IEquatable<Priority>, IComparable<Priority>
    {
        private int Value { get; }

        public Priority(int value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(
                    $"{value} is invalid priority value. It should be between 0 and {int.MaxValue}.",
                    nameof(value));
            }

            Value = value;
        }

        public static implicit operator int(Priority priority) => priority.Value;

        public static implicit operator Priority(int value) => new Priority(value);

        public int CompareTo(Priority other) => Value.CompareTo(other.Value);

        public override bool Equals(object obj)
        {
            if (obj is Priority priority)
            {
                return Equals(priority);
            }

            return false;
        }

        public bool Equals(Priority other)
        {
            return Value == other.Value;
        }

        public override int GetHashCode() => Value;

        public static bool operator ==(Priority left, Priority right) => Equals(left, right);

        public static bool operator !=(Priority left, Priority right) => !Equals(left, right);

        public static bool operator >(Priority left, Priority right) => left.Value > right.Value;

        public static bool operator <(Priority left, Priority right) => left.Value < right.Value;

        public static bool operator >=(Priority left, Priority right) => left.Value >= right.Value;

        public static bool operator <=(Priority left, Priority right) => left.Value <= right.Value;

        public static Priority operator +(Priority left, Priority right)
        {
            try
            {
                return left.Value + right.Value;
            }
            catch (OverflowException)
            {
                return Lowest;
            }
        }

        public static Priority operator -(Priority left, Priority right) => Math.Max(left.Value - right.Value, 0);

        public static Priority Highest => new Priority(0);

        public static Priority Lowest => new Priority(int.MaxValue);

        public static Priority Mid => new Priority(int.MaxValue / 2);

        public static Priority High => new Priority(int.MaxValue / 4);

        public static Priority Low => new Priority(int.MaxValue / 4 * 3);
    }
}
