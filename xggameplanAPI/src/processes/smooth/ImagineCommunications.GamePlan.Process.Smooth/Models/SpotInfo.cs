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
                var spotInfo = new SpotInfo()
                {
                    Uid = spot.Uid,
                    ExternalBreakRefAtRunStart = spot.ExternalBreakNo,
                    ProductAdvertiserIdentifier = String.Empty,
                    ProductClashCode = String.Empty,
                };

                if (!String.IsNullOrWhiteSpace(spot.Product))
                {
                    if (productsByExternalRef.TryGetValue(spot.Product, out Product product))
                    {
                        if (!(product is null))
                        {
                            spotInfo.ProductAdvertiserIdentifier = product.AdvertiserIdentifier;

                            if (product.ClashCode != null)
                            {
                                spotInfo.ProductClashCode = product.ClashCode;

                                if (clashesByExternalRef.TryGetValue(product.ClashCode, out Clash clash))
                                {
                                    if (!(clash is null))
                                    {
                                        if (clash.ParentExternalidentifier != null)
                                        {
                                            spotInfo.ParentProductClashCode = clash.ParentExternalidentifier;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                spotInfos.Add(spot.Uid, spotInfo);
            }

            return spotInfos;
        }
    }
}
