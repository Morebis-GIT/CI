using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.IndexTypes;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Universes;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using ImagineCommunications.GamePlan.Domain.SmoothFailureMessages;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Objects;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces;
using ImagineCommunications.GamePlan.Process.Smooth.Services;

using static xggameplan.common.Helpers.LogAsString;

namespace ImagineCommunications.GamePlan.Process.Smooth.Models
{
    /// <summary>
    /// Data required to Smooth sales areas in parallel. Do not add sales area
    /// specific data!
    /// </summary>
    public sealed class ImmutableSmoothData
    {
        // Date and sales area specific models
        public IImmutableList<Break> BreaksForAllSalesAreasForSmoothPeriod { get; set; }

        // All models as a collection
        public IImmutableList<Clash> Clashes { get; set; }
        public IImmutableList<ClashException> ClashExceptions { get; set; }
        public IImmutableList<IndexType> IndexTypes { get; set; }
        public IImmutableList<Product> Products { get; set; }
        public IImmutableList<Restriction> Restrictions { get; set; }
        public IImmutableList<Universe> Universes { get; set; }
        public IImmutableList<SmoothFailureMessage> SmoothFailureMessages { get; set; }
        public IImmutableList<Sponsorship> SponsorshipRestrictions { get; private set; }

        // Aggregated model collections
        public IImmutableDictionary<string, (string Name, string campaignGroup)> CampaignsByExternalRef { get; set; }
        public IImmutableDictionary<string, Clash> ClashesByExternalRef { get; set; }
        public IImmutableDictionary<string, Product> ProductsByExternalRef { get; set; }

        // Global models
        public ISmoothConfiguration SmoothConfigurationReader { get; set; }

        public SmoothConfiguration SmoothConfiguration { get; set; }

        private ImmutableSmoothData()
        { }

        /// <summary>
        /// Create an instance of the <see cref="ImmutableSmoothData"/> class
        /// with model data loaded for the given sales areas and time period.
        /// </summary>
        /// <returns></returns>
        public static ImmutableSmoothData Create(
            IModelLoaders modelLoadingService,
            IReadOnlyCollection<string> salesAreaNames,
            DateTimeRange smoothPeriod,
            Action<string> raiseInfo
            )
        {
            var (smoothStartDateTime, smoothEndDateTime) = smoothPeriod;

            IImmutableList<Break> breaksForAllSalesAreasForSmoothPeriod = ImmutableList<Break>.Empty;
            IImmutableList<Campaign> campaigns = ImmutableList<Campaign>.Empty;
            IImmutableList<Clash> clashes = ImmutableList<Clash>.Empty;
            IImmutableList<ClashException> clashExceptions = ImmutableList<ClashException>.Empty;
            IImmutableList<IndexType> indexTypes = ImmutableList<IndexType>.Empty;
            IImmutableList<Product> products = ImmutableList<Product>.Empty;
            IImmutableList<Restriction> restrictions = ImmutableList<Restriction>.Empty;
            IImmutableList<SmoothFailureMessage> smoothFailureMessages = ImmutableList<SmoothFailureMessage>.Empty;
            IImmutableList<Sponsorship> sponsorshipRestrictions = ImmutableList<Sponsorship>.Empty;
            IImmutableList<Universe> universes = ImmutableList<Universe>.Empty;
            SmoothConfiguration smoothConfiguration = null;

            Parallel.Invoke(
                () => breaksForAllSalesAreasForSmoothPeriod = modelLoadingService
                        .GetAllBreaksForSalesAreaForSmoothPeriod(
                            (smoothStartDateTime, smoothEndDateTime),
                            salesAreaNames
                            ),
                () => campaigns = modelLoadingService.GetAllCampaigns(),
                () => clashes = modelLoadingService.GetAllClashes(),
                () => clashExceptions = modelLoadingService.GetAllClashException(),
                () => indexTypes = modelLoadingService.GetAllIndexTypes(),
                () => products = modelLoadingService.GetAllProducts(),
                () => restrictions = modelLoadingService.GetAllRestrictions(),
                () => smoothConfiguration = modelLoadingService.GetSmoothConfiguration(),
                () => smoothFailureMessages = modelLoadingService.GetAllSmoothFailureMessages(),
                () => sponsorshipRestrictions = modelLoadingService.GetAllSponsorshipRestrictions(),
                () => universes = modelLoadingService.GetAllUniverses()
                );

            raiseInfo($"Loaded {Log(breaksForAllSalesAreasForSmoothPeriod.Count)} breaks");
            raiseInfo($"Loaded {Log(campaigns.Count)} campaigns");
            raiseInfo($"Loaded {Log(clashes.Count)} clashes");
            raiseInfo($"Loaded {Log(clashExceptions.Count)} clash exceptions");
            raiseInfo($"Loaded {Log(indexTypes.Count)} index types");
            raiseInfo($"Loaded {Log(products.Count)} products");
            raiseInfo($"Loaded {Log(restrictions.Count)} restrictions");
            raiseInfo($"Loaded {Log(smoothFailureMessages.Count)} smooth failure messages");
            raiseInfo($"Loaded {Log(sponsorshipRestrictions.Count)} sponsorship restrictions");
            raiseInfo($"Loaded {Log(universes.Count)} universes");

            var campaignsByExternalRef = ImmutableDictionary<string, (string Name, string campaignGroup)>.Empty;
            var clashesByExternalRef = ImmutableDictionary<string, Clash>.Empty;
            var productsByExternalRef = ImmutableDictionary<string, Product>.Empty;

            Parallel.Invoke(
                () => campaignsByExternalRef = IndexCampaignsByExternalId(campaigns),
                () => clashesByExternalRef = Clash
                        .IndexListByExternalRef(clashes)
                        .ToImmutableDictionary(),
                () => productsByExternalRef = products
                        .IndexListByExternalID()
                        .ToImmutableDictionary()
                );

            ISmoothConfiguration smoothConfigurationReader = new SmoothConfigurationReader(smoothConfiguration);

            return new ImmutableSmoothData
            {
                BreaksForAllSalesAreasForSmoothPeriod = breaksForAllSalesAreasForSmoothPeriod,
                Clashes = clashes,
                ClashExceptions = clashExceptions,
                IndexTypes = indexTypes,
                Products = products,
                Restrictions = restrictions,
                SmoothFailureMessages = smoothFailureMessages,
                SponsorshipRestrictions = sponsorshipRestrictions,
                Universes = universes,
                SmoothConfiguration = smoothConfiguration,
                SmoothConfigurationReader = smoothConfigurationReader,
                CampaignsByExternalRef = campaignsByExternalRef,
                ClashesByExternalRef = clashesByExternalRef,
                ProductsByExternalRef = productsByExternalRef
            };
        }

        /// <summary>
        /// Indexes Campiagns by external Id. The index contains only the
        /// campaign's name and campaign group.
        /// </summary>
        /// <param name="campaigns">Campaigns to index.</param>
        /// <returns></returns>
        private static ImmutableDictionary<string, (string Name, string campaignGroup)>
        IndexCampaignsByExternalId(IEnumerable<Campaign> campaigns)
        {
            var campaignsByExternalId = new Dictionary<string, (string Name, string campaignGroup)>();

            foreach (var campaign in campaigns)
            {
                if (campaignsByExternalId.ContainsKey(campaign.ExternalId))
                {
                    continue;
                }

                campaignsByExternalId.Add(
                    campaign.ExternalId,
                    (campaign.Name, campaign.CampaignGroup)
                    );
            }

            return campaignsByExternalId.ToImmutableDictionary();
        }
    }
}
