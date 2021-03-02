using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessTypes.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositions;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Services;
using xggameplan.core.Extensions;
using xggameplan.core.FeatureManagement.Interfaces;
using xggameplan.core.Helpers;
using xggameplan.Extensions;
using xggameplan.Model.AutoGen;

namespace xggameplan.core.Services.OptimiserInputFilesSerialisers
{
    public class SpotSerialiser
    {
        public SpotSerialiser(string folderName)
        {
            if (String.IsNullOrWhiteSpace(folderName))
            {
                throw new ArgumentNullException(nameof(folderName));
            }

            FolderName = folderName;
        }

        private string FolderName { get; }

        /// <summary>
        /// Spots filename.
        /// </summary>
        public static string Filename => "v_spot_list.xml";

        public void Serialise(
            List<Spot> spots,
            List<SalesArea> prioritySalesAreas,
            IReadOnlyCollection<BreakWithProgramme> breaks,
            List<Campaign> campaigns,
            List<Demographic> demographics,
            List<BusinessType> businessTypes,
            List<Product> allProducts,
            List<Clash> allClashes,
            AgSpot agSpot,
            IFeatureManager featureManager)
        {
            ToAgSpot(
                spots,
                prioritySalesAreas,
                breaks,
                campaigns,
                demographics,
                businessTypes,
                allProducts,
                allClashes,
                agSpot,
                featureManager)
            .Serialize(Path.Combine(FolderName, Filename));
        }

        private static AgSpotsSerialization ToAgSpot(
            List<Spot> spots,
            List<SalesArea> salesAreas,
            IReadOnlyCollection<BreakWithProgramme> breaks,
            List<Campaign> campaigns,
            List<Demographic> demographics,
            List<BusinessType> businessTypes,
            List<Product> products,
            List<Clash> clashes,
            AgSpot agSpot,
            IFeatureManager featureManager)
        {
            var agSpots = new ConcurrentBag<AgSpot>();

            Dictionary<string, SalesArea> salesAreasByName = SalesArea.IndexListByName(salesAreas);
            Dictionary<string, Campaign> campaignsByExternalId = Campaign.IndexListByExternalId(campaigns);
            Dictionary<string, bool> businessTypesInclusionSettings = businessTypes
                .ToDictionary(bt => bt.Code, bt => bt.IncludeConversionIndex);
            Dictionary<string, BreakWithProgramme> breaksWithProgrammeBySalesAreaAndExternalBreakRef = BreakWithProgramme.IndexListBySalesAreaAndExternalBreakRef(breaks);
            IImmutableDictionary<string, Product> productByExternalId = products.IndexListByExternalID();
            IImmutableDictionary<string, Clash> clashByExternalRef = Clash.IndexListByExternalRef(clashes);

            var clashRoots = ClashHelper.CalculateClashTopParents(clashByExternalRef);

            var threadSafeSpotsCollection = spots.ToImmutableList();
            var includeNominalPrice = featureManager.IsEnabled(nameof(ProductFeature.NominalPrice));
            var includePositionInBreak = featureManager.IsEnabled(nameof(ProductFeature.PositionInBreak));

            _ = Parallel.ForEach(threadSafeSpotsCollection, spot => agSpots.Add(
                    GetAgSpot(
                        spot,
                        threadSafeSpotsCollection,
                        salesAreasByName,
                        campaignsByExternalId,
                        businessTypesInclusionSettings,
                        breaksWithProgrammeBySalesAreaAndExternalBreakRef,
                        productByExternalId,
                        clashByExternalRef,
                        demographics,
                        agSpot.Clone(),
                        includeNominalPrice,
                        includePositionInBreak,
                        clashRoots
                        )
                    )
                );

            var serialization = new AgSpotsSerialization();
            return serialization.MapFrom(agSpots.ToList());
        }

        private static AgSpot GetAgSpot(
            Spot spot,
            IImmutableList<Spot> spots,
            Dictionary<string, SalesArea> salesAreasByName,
            Dictionary<string, Campaign> campaignsByExternalId,
            Dictionary<string, bool> businessTypesInclusionSettings,
            Dictionary<string, BreakWithProgramme> breaksBySalesAreaAndExternalBreakRef,
            IImmutableDictionary<string, Product> productByExternalId,
            IImmutableDictionary<string, Clash> clashByExternalRef,
            IReadOnlyCollection<Demographic> demographics,
            in AgSpot agSpot,
            bool includeNominalPrice,
            bool includePositionInBreak,
            IReadOnlyDictionary<string, string> clashRoots)
        {
            if (spot is null)
            {
                throw new ArgumentNullException(nameof(spot));
            }

            void SetBreakPartsToDefault(in AgSpot agSpot)
            {
                agSpot.BreakSalesAreaNo = 0;
                agSpot.BreakDate = String.Empty;
                agSpot.BreakTime = String.Empty;
                agSpot.BreakNo = 0;
            }

            if (String.IsNullOrEmpty(spot.ExternalBreakNo))
            {
                SetBreakPartsToDefault(agSpot);
            }
            else
            {
                string breakKey = BreakWithProgramme.GetIndexKeyBySalesAreaAndExternalBreakRef(spot.SalesArea, spot.ExternalBreakNo);

                if (breaksBySalesAreaAndExternalBreakRef.TryGetValue(breakKey, out BreakWithProgramme breakWithProgramme)
                    && breakWithProgramme != null)
                {
                    Break theBreak = breakWithProgramme.Break;

                    SalesArea breakSalesArea = salesAreasByName[theBreak.SalesArea];

                    agSpot.BreakSalesAreaNo = breakSalesArea.CustomId;
                    agSpot.BreakDate = AgConversions.ToAgDateYYYYMMDDAsString(theBreak.ScheduledDate);
                    agSpot.BreakTime = AgConversions.ToAgTimeAsHHMMSS(theBreak.ScheduledDate);
                    agSpot.BreakNo = theBreak.CustomId;
                }
                else
                {
                    SetBreakPartsToDefault(agSpot);
                }
            }

            campaignsByExternalId.TryGetValue(spot.ExternalCampaignNumber, out Campaign campaign);

            int productCode = 0;
            string clashCode = "";
            string rootClashCode = "";
            string advertiserIdentifier = "";
            string businessTypeCode = "";

            if (campaign is null)
            {
                var product = !String.IsNullOrWhiteSpace(spot.Product)
                    ? productByExternalId.ContainsKey(spot.Product)
                        ? productByExternalId[spot.Product]
                        : null
                    : null;

                if (product != null)
                {
                    advertiserIdentifier = product.AdvertiserIdentifier;
                }

                if (!String.IsNullOrWhiteSpace(product?.Externalidentifier))
                {
                    Int32.TryParse(product.Externalidentifier, out productCode);
                }

                var clash = !String.IsNullOrWhiteSpace(product?.ClashCode)
                    ? clashByExternalRef.ContainsKey(product.ClashCode)
                        ? clashByExternalRef[product.ClashCode]
                        : null
                    : null;

                if (!(clash is null))
                {
                    clashCode = !String.IsNullOrWhiteSpace(clash.Externalref)
                        ? clash.Externalref
                        : "";

                    rootClashCode = clashRoots.TryGetValue(clash.Externalref, out var rootClash) && !String.IsNullOrWhiteSpace(rootClash)
                        ? rootClash
                        : clashCode;
                }

                agSpot.CampaignNo = 0;
                agSpot.ISRLocked = AgConversions.ToAgBooleanAs1or0(spot.ClientPicked);
            }
            else
            {
                agSpot.CampaignNo = campaign.CustomId;
                agSpot.ISRLocked = AgConversions.ToAgBooleanAs1or0(spot.ClientPicked || !campaign.InefficientSpotRemoval);

                businessTypeCode = businessTypesInclusionSettings
                    .TryGetValue(campaign.BusinessType, out bool isIncluded) && isIncluded
                    ? campaign.BusinessType
                    : string.Empty;
            }

            agSpot.SpotNo = spot.CustomId;

            agSpot.MultipartIndicator = GetSpotMultipartIndicator(spot, spots);

            agSpot.SpotLength = (int)spot.SpotLength.ToTimeSpan().TotalSeconds;

            var spotSalesArea = salesAreasByName[spot.SalesArea];
            agSpot.SpotSalesAreaNo = spotSalesArea.CustomId;

            agSpot.PriceFactor = spot.SpotLength.ToTimeSpan().TotalSeconds / 30;

            agSpot.ClientPicked = AgConversions.ToAgBooleanAsString(spot.ClientPicked);

            agSpot.ProductCode = productCode;
            agSpot.ClashCode = clashCode;
            agSpot.AdvertiserIdentifier = advertiserIdentifier;
            agSpot.RootClashCode = rootClashCode;
            agSpot.NominalPrice = includeNominalPrice ? spot.NominalPrice : default;
            agSpot.BookingPosition = includePositionInBreak ? spot.BookingPosition : BookingPosition.NoDefaultPosition;
            agSpot.CampaignBusinessType = businessTypeCode;
            agSpot.Demographic = demographics.FirstOrDefault(d =>
                d.ExternalRef.Equals(spot.Demographic))?.Id ?? 0;

            return agSpot;
        }

        private static string GetSpotMultipartIndicator(
            Spot spot,
            IImmutableList<Spot> spots)
        {
            const string NotMultipartSpot = "N";
            const string TopTail = "T";
            const string Middle = "M";

            if (!spot.IsMultipartSpot)
            {
                return NotMultipartSpot;
            }

            IReadOnlyCollection<Spot> linkedMultipartSpots = BreakUtilities
                .GetLinkedMultipartSpots(
                    spot,
                    spots,
                    includeInputSpotInOutput: true);

            if (linkedMultipartSpots.Count == 0)
            {
                // This would indicate dirty data. The spot is marked as
                // multipart but no other parts were found.
                return NotMultipartSpot;
            }

            // Get position of spot, spots are already in correct order
            return linkedMultipartSpots
                .ToList()
                .IndexOf(spot) == 0
                    ? TopTail
                    : Middle;
        }
    }
}
