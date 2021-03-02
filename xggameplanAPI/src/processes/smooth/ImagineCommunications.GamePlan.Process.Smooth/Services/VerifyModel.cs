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

                Product product = null;
                if (!isMissingProductExternalRef)
                {
                    _ = productsByExternalRef.TryGetValue(productExternalReference, out product);
                }

                if (product is null && !isMissingProductExternalRef)
                {
                    var spotsWithMissingProducts = new List<string>(
                        allSpots
                            .Where(s => s.Product.Equals(productExternalReference, StringComparison.OrdinalIgnoreCase))
                            .Select(s => s.ExternalSpotRef)
                            );

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

                string productClashCode = product.ClashCode;

                if (!clashesByExternalRef.TryGetValue(productClashCode, out Clash childClash)
                    || childClash is null)
                {
                    RaiseWarning(
                        $"Product {productExternalReference} for sales area {salesAreaName} references " +
                        $"clash code {productClashCode} but the clash does not exist"
                        );

                    continue;
                }

                string parentExternalidentifier = childClash.ParentExternalidentifier;
                if (String.IsNullOrWhiteSpace(parentExternalidentifier))
                {
                    continue;
                }

                if (!clashesByExternalRef.TryGetValue(parentExternalidentifier, out Clash parentClash)
                    || parentClash is null)

                {
                    RaiseWarning(
                        $"Product {productExternalReference} for sales area {salesAreaName} " +
                        $"references clash code {productClashCode} but the parent clash " +
                        $"{parentExternalidentifier} does not exist"
                        );
                }
            }
        }
    }
}
