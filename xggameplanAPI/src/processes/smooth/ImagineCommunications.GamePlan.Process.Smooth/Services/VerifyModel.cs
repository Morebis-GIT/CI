using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Spots;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    public static class VerifyModel
    {
        public static void VerifySpotProductsReferences(
            string salesAreaName,
            IReadOnlyCollection<Spot> allSpots,
            IImmutableDictionary<string, Product> productsByExternalRef,
            IImmutableDictionary<string, Clash> clashesByExternalRef,
            Action<string> RaiseWarning
            )
        {
            var spotProductsExternalRefs = allSpots
                .Select(s => s.Product)
                .Where(p => !String.IsNullOrWhiteSpace(p))
                .Distinct();

            foreach (string productExternalReference in spotProductsExternalRefs)
            {
                bool isMissingProductExternalRef = String.IsNullOrWhiteSpace(productExternalReference);

                Product product = isMissingProductExternalRef
                        || !productsByExternalRef.ContainsKey(productExternalReference)
                    ? null
                    : productsByExternalRef[productExternalReference];

                if (product is null && !isMissingProductExternalRef)
                {
                    var spotsWithMissingProducts = new List<string>(
                        allSpots
                            .Where(s => s.Product.Equals(productExternalReference, StringComparison.OrdinalIgnoreCase))
                            .Select(s => s.ExternalSpotRef
                        ));

                    RaiseWarning(
                        $"Product {productExternalReference} for sales area {salesAreaName} " +
                        $"is referenced by spot(s) {String.Join(", ", spotsWithMissingProducts)} but it does not exist"
                        );

                    continue;
                }

                if (product is null || String.IsNullOrWhiteSpace(product.ClashCode))
                {
                    continue;
                }

                Clash childClash = ClashOrDefault(product.ClashCode);
                if (childClash is null)
                {
                    RaiseWarning(
                        $"Product {productExternalReference} for sales area {salesAreaName} references " +
                        $"clash code {product.ClashCode} but the clash does not exist"
                        );

                    continue;
                }

                if (String.IsNullOrWhiteSpace(childClash.ParentExternalidentifier))
                {
                    continue;
                }

                Clash parentClash = ClashOrDefault(childClash.ParentExternalidentifier);
                if (parentClash is null)
                {
                    RaiseWarning(
                        $"Product {productExternalReference} for sales area {salesAreaName} " +
                        $"references clash code {product.ClashCode} but the parent clash " +
                        $"{childClash.ParentExternalidentifier} does not exist"
                        );
                }

                //----------------
                // Local functions
                Clash ClashOrDefault(string clashCode) =>
                    clashesByExternalRef.ContainsKey(clashCode)
                        ? clashesByExternalRef[clashCode]
                        : null;
            }
        }
    }
}
