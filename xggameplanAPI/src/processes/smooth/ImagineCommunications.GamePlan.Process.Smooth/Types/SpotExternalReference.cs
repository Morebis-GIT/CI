using System;
using System.Diagnostics;

namespace ImagineCommunications.GamePlan.Process.Smooth.Types
{
    [DebuggerDisplay("{_spotExternalReference}")]
    public readonly struct SpotExternalReference
        : IEquatable<SpotExternalReference>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly string _spotExternalReference;

        private SpotExternalReference(string SpotExternalReference) =>
            _spotExternalReference = SpotExternalReference;

        public readonly bool Equals(SpotExternalReference other) =>
            _spotExternalReference == other._spotExternalReference;

        public override readonly bool Equals(object obj)
        {
            switch (obj)
            {
                case SpotExternalReference ber:
                    return _spotExternalReference.Equals(ber._spotExternalReference);

                case string str:
                    return _spotExternalReference.Equals(str);

                default:
                    return false;
            }
        }

        public override readonly int GetHashCode() =>
            _spotExternalReference.GetHashCode();

        public override readonly string ToString() =>
            _spotExternalReference;

        public static bool operator ==(
            SpotExternalReference left,
            SpotExternalReference right) => left.Equals(right);

        public static bool operator !=(
            SpotExternalReference left,
            SpotExternalReference right) => !left.Equals(right);

        public static implicit operator SpotExternalReference(string value) =>
            new SpotExternalReference(value);

        public static implicit operator String(SpotExternalReference value) =>
            value._spotExternalReference;
    }
}
