using System;
using System.Diagnostics;

namespace ImagineCommunications.GamePlan.Process.Smooth.Types
{
    [DebuggerDisplay("{_breakExternalReference}")]
    public readonly struct BreakExternalReference
        : IEquatable<BreakExternalReference>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly string _breakExternalReference;

        private BreakExternalReference(string breakExternalReference) =>
            _breakExternalReference = breakExternalReference;

        public readonly bool Equals(BreakExternalReference other) =>
            _breakExternalReference == other._breakExternalReference;

        public override readonly bool Equals(object obj)
        {
            switch (obj)
            {
                case BreakExternalReference ber:
                    return _breakExternalReference.Equals(ber._breakExternalReference);

                case string str:
                    return _breakExternalReference.Equals(str);

                default:
                    return false;
            }
        }

        public override readonly int GetHashCode() =>
            _breakExternalReference.GetHashCode();

        /// <summary>Converts the value to string.</summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override readonly string ToString() =>
            _breakExternalReference;

        public static bool operator ==(
            BreakExternalReference left,
            BreakExternalReference right) => left.Equals(right);

        public static bool operator !=(
            BreakExternalReference left,
            BreakExternalReference right) => !left.Equals(right);

        public static implicit operator BreakExternalReference(string value) =>
            new BreakExternalReference(value);

        public static implicit operator String(BreakExternalReference value) =>
            value._breakExternalReference;
    }
}
