using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Spots;

namespace ImagineCommunications.GamePlan.Process.Smooth.Models
{
    /// <summary>
    /// Additional spot information to save us having to repeatedly look it up.
    /// </summary>
    public class SpotInfo
    {
        public Guid Uid { get; set; }
        public string ExternalBreakRefAtRunStart { get; set; }
        public string ProductAdvertiserIdentifier { get; set; }
        public string ProductClashCode { get; set; }
        public string ParentProductClashCode { get; set; }
        public bool NoPlaceAttempt { get; set; }

        public static IReadOnlyDictionary<Guid, SpotInfo> Factory(
            IReadOnlyCollection<Spot> spots,
            IReadOnlyDictionary<string, Product> productsByExternalRef,
            IReadOnlyDictionary<string, Clash> clashesByExternalRef
            )
        {
            if (spots.Count == 0)
            {
                return new Dictionary<Guid, SpotInfo>();
            }

            var spotInfos = new Dictionary<Guid, SpotInfo>();

            foreach (var spot in spots)
            {
                var (productAdvertiserIdentifier, productClashCode, parentProductClashCode) =
                    ProductInfo(productsByExternalRef, clashesByExternalRef, spot);

                var spotInfo = new SpotInfo()
                {
                    Uid = spot.Uid,
                    ExternalBreakRefAtRunStart = spot.ExternalBreakNo,
                    ProductAdvertiserIdentifier = productAdvertiserIdentifier,
                    ProductClashCode = productClashCode,
                    ParentProductClashCode = parentProductClashCode,
                    NoPlaceAttempt = false
                };

                spotInfos.Add(spot.Uid, spotInfo);
            }

            return spotInfos;
        }

        private static (string productAdvertiserIdentifier, string productClashCode, string parentProductClashCode)
        ProductInfo(
            IReadOnlyDictionary<string, Product> productsByExternalRef,
            IReadOnlyDictionary<string, Clash> clashesByExternalRef,
            Spot spot)
        {
            (
                string productAdvertiserIdentifier,
                string productClashCode,
                string parentProductClashCode
            ) result = (String.Empty, String.Empty, null);

            if (String.IsNullOrWhiteSpace(spot.Product))
            {
                return result;
            }

            if (!productsByExternalRef.TryGetValue(spot.Product, out Product product))
            {
                return result;
            }

            if (product is null)
            {
                return result;
            }

            result.productAdvertiserIdentifier = product.AdvertiserIdentifier;

            if (product.ClashCode is null)
            {
                return result;
            }

            result.productClashCode = product.ClashCode;

            if (!clashesByExternalRef.TryGetValue(product.ClashCode, out Clash clash))
            {
                return result;
            }

            if (clash is null)
            {
                return result;
            }

            if (clash.ParentExternalidentifier is null)
            {
                return result;
            }

            result.parentProductClashCode = clash.ParentExternalidentifier;

            return result;
        }
    }
}
