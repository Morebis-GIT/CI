using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;

namespace ImagineCommunications.GamePlan.Domain.Shared.Products
{
    /// <summary>
    /// Extends domain <see cref="Product"/> model functionality.
    /// </summary>
    public static class ProductExtensions
    {
        /// <summary>
        /// Indexes list by ExternalID
        /// </summary>
        /// <param name="products"></param>
        /// <returns></returns>
        public static IImmutableDictionary<string, Product> IndexListByExternalID(this IEnumerable<Product> products)
        {
            if (products is null)
            {
                throw new ArgumentNullException(nameof(products));
            }

            var productsByExternalId = new Dictionary<string, Product>();

            foreach (var product in products)
            {
                if (!productsByExternalId.ContainsKey(product.Externalidentifier))
                {
                    productsByExternalId.Add(product.Externalidentifier, product);
                }
            }

            return productsByExternalId.ToImmutableDictionary();
        }
    }
}
