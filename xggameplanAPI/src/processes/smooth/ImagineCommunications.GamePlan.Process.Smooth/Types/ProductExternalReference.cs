using System;
using System.Diagnostics;

namespace ImagineCommunications.GamePlan.Process.Smooth.Types
{
    [DebuggerDisplay("{_productExternalReference}")]
    public readonly struct ProductExternalReference
        : IEquatable<ProductExternalReference>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly string _productExternalReference;

        private ProductExternalReference(string ProductExternalReference) =>
            _productExternalReference = ProductExternalReference;

        public readonly bool Equals(ProductExternalReference other) =>
            _productExternalReference == other._productExternalReference;

        public override readonly bool Equals(object obj)
        {
            return obj switch
            {
                ProductExternalReference ber => _productExternalReference.Equals(ber._productExternalReference),
                string str => _productExternalReference.Equals(str),
                _ => false,
            };
        }

        public override readonly int GetHashCode() =>
            _productExternalReference.GetHashCode();

        public override readonly string ToString() =>
            _productExternalReference;

        public static bool operator ==(
            ProductExternalReference left,
            ProductExternalReference right) => left.Equals(right);

        public static bool operator !=(
            ProductExternalReference left,
            ProductExternalReference right) => !left.Equals(right);

        public static implicit operator ProductExternalReference(string value) =>
            new ProductExternalReference(value);

        public static implicit operator String(ProductExternalReference value) =>
            value._productExternalReference;
    }
}
