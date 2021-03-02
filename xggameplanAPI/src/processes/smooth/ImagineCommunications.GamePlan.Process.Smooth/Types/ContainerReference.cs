using System;
using System.Diagnostics;

namespace ImagineCommunications.GamePlan.Process.Smooth.Types
{
    /// <summary>
    /// Represents a Break Container Reference. Initialise with a full
    /// Break External Reference.
    /// </summary>
    /// <seealso cref="System.IEquatable{ImagineCommunications.GamePlan.Process.Smooth.Types.ContainerReference}" />
    [DebuggerDisplay("{_containerReference}")]
    public readonly struct ContainerReference
        : IEquatable<ContainerReference>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private const string InvalidContainerReferenceValue = "invalid-0";

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private const int InvalidContainerNumber = Int32.MinValue;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly string _containerReference;

        public readonly int ContainerNumber { get; }

        private ContainerReference(
            string breakExternalReference,
            int containerNumber)
        {
            _containerReference = breakExternalReference;
            ContainerNumber = containerNumber;
        }

        public readonly bool Equals(ContainerReference other) =>
            _containerReference == other._containerReference;

        public readonly bool Equals(string other) =>
            _containerReference.Equals(other);

        public override readonly bool Equals(object obj) =>
            obj switch
            {
                ContainerReference cr => _containerReference.Equals(cr._containerReference),
                string str => _containerReference.Equals(str),
                _ => false,
            };

        public override readonly int GetHashCode() =>
            _containerReference.GetHashCode();

        /// <summary>Converts the value to string.</summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override readonly string ToString() =>
            _containerReference;

        public static bool operator ==(
            in ContainerReference left,
            in ContainerReference right) => left.Equals(right);

        public static bool operator !=(
            in ContainerReference left,
            in ContainerReference right) => !left.Equals(right);

        public static bool operator ==(
            in ContainerReference left,
            string right) => left.Equals(right);

        public static bool operator !=(
            in ContainerReference left,
            string right) => !left.Equals(right);

        public static implicit operator ContainerReference(string value)
        {
            (string containerReference, int containerNumber) = ContainerReferenceValue(value);

            if (containerReference.Equals(InvalidContainerReferenceValue, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("The value is not a valid container reference.", nameof(value));
            }

            return new ContainerReference(containerReference, containerNumber);
        }

        public static implicit operator String(in ContainerReference value) =>
            value._containerReference;

        public static bool TryParse(BreakExternalReference breakExternalReference, out ContainerReference cr)
        {
            var (crValue, containerNumber) = ContainerReferenceValue(breakExternalReference);
            if (crValue.Equals(InvalidContainerReferenceValue, StringComparison.OrdinalIgnoreCase))
            {
                cr = new ContainerReference(InvalidContainerReferenceValue, InvalidContainerNumber);
                return false;
            }

            cr = new ContainerReference(crValue, containerNumber);
            return true;
        }

        private static (string containerReference, int containerNumber)
        ContainerReferenceValue(string breakExternalReference)
        {
            (string, int) invalidValue = (InvalidContainerReferenceValue, InvalidContainerNumber);

            if (String.IsNullOrWhiteSpace(breakExternalReference))
            {
                return invalidValue;
            }

            var breakExternalReferenceParts = breakExternalReference
                .Split(new[] { '-' },
                StringSplitOptions.RemoveEmptyEntries);

            if (breakExternalReferenceParts.Length < 3)
            {
                return invalidValue;
            }

            if (!Int32.TryParse(breakExternalReferenceParts[2], out _))
            {
                return invalidValue;
            }

            if (!Int32.TryParse(breakExternalReferenceParts[1], out var containerNumber))
            {
                return invalidValue;
            }

            return ($"{breakExternalReferenceParts[0]}-{containerNumber.ToString()}", containerNumber);
        }
    }
}
