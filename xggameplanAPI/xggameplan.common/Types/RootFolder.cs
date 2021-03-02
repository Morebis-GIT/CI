using System;

namespace xggameplan.common.Types
{
    public class RootFolder : IEquatable<RootFolder>
    {
        private readonly string _value;

        public RootFolder(string value)
        {
            _value = value;
        }

        public static implicit operator string(RootFolder value) => value._value;

        public static implicit operator RootFolder(string value) => new RootFolder(value);

        public override bool Equals(object obj)
        {
            if (obj is RootFolder rf)
            {
                return Equals(rf);
            }

            return false;
        }

        public bool Equals(RootFolder other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return _value == other._value;
        }

        public override int GetHashCode() => _value != null ? _value.GetHashCode() : 0;

        public static bool operator ==(RootFolder left, RootFolder right) => Equals(left, right);

        public static bool operator !=(RootFolder left, RootFolder right) => !Equals(left, right);

        public override string ToString() => _value;
    }
}
