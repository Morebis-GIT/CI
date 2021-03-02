using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Campaigns.Projections;
using NodaTime;

namespace ImagineCommunications.GamePlan.Domain.Campaigns.Objects
{
    public class Campaign : IReducedCampaign, ICloneable
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Unique int Id
        /// </summary>
        public int CustomId { get; set; } = 0;

        public string ExternalId { get; set; }
        public string Name { get; set; }
        public string DemoGraphic { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string Product { get; set; }
        public double RevenueBudget { get; set; }
        public decimal TargetRatings { get; set; }
        public decimal ActualRatings { get; set; }
        public string CampaignGroup { get; set; }

        /// <summary>
        /// True if the ratings is a percentage. Otherwise the rating is a value and the false is
        /// returned.
        /// </summary>
        public bool IsPercentage { get; set; }

        public string Status { get; set; }
        public string BusinessType { get; set; }
        public CampaignDeliveryType DeliveryType { get; set; }
        public DeliveryCurrency DeliveryCurrency { get; set; }
        public bool IncludeOptimisation { get; set; }
        public bool TargetZeroRatedBreaks { get; set; }
        public bool InefficientSpotRemoval { get; set; }

        private bool _includeRightSizer;

        public bool IncludeRightSizer
        {
            get => _includeRightSizer;
            set
            {
                _includeRightSizer = value;

                if (!value)
                {
                    RightSizerLevel = null;
                }
                else if (!RightSizerLevel.HasValue)
                {
                    RightSizerLevel = Campaigns.RightSizerLevel.CampaignLevel;
                }
            }
        }

        public RightSizerLevel? RightSizerLevel { get; set; }

        public string ExpectedClearanceCode { get; set; }
        public int CampaignPassPriority { get; set; }

        public List<string> BreakType { get; set; }

        public string searchfields { get; set; }

        [DefaultValue("")]
        public List<TimeRestriction> TimeRestrictions { get; set; }

        [DefaultValue("")]
        public List<ProgrammeRestriction> ProgrammeRestrictions { get; set; }

        public List<CampaignProgramme> ProgrammesList { get; set; }

        public List<SalesAreaCampaignTarget> SalesAreaCampaignTarget { get; set; }
        public List<CampaignBookingPositionGroup> BookingPositionGroups { get; set; } = new List<CampaignBookingPositionGroup>();

        public List<CampaignPayback> CampaignPaybacks { get; set; }

        public int CampaignSpotMaxRatings { get; set; }
        public CampaignBreakRequirement BreakRequirement { get; set; }
        public bool StopBooking { get; set; }
        public string ActiveLength { get; set; }
        public decimal? RatingsDifferenceExcludingPayback { get; set; }
        public decimal? ValueDifference { get; set; }
        public decimal? ValueDifferenceExcludingPayback { get; set; }
        public decimal? AchievedPercentageTargetRatings { get; set; }
        public decimal? AchievedPercentageRevenueBudget { get; set; }

        /// <summary>
        /// Target Ratings (Excluding Payback)
        /// </summary>
        public double? TargetXP { get; set; }
        public double? RevenueBooked { get; set; }
        public DateTime? CreationDate { get; set; }
        public bool? AutomatedBooked { get; set; }
        public TopTail? TopTail { get; set; }
        public int? Spots { get; set; }

        public object Clone()
        {
            var campaign = (Campaign)MemberwiseClone();

            if (BreakType != null)
            {
                campaign.BreakType = new List<string>();
                BreakType.ForEach(bt => campaign.BreakType.Add((string)bt.Clone()));
            }

            if (TimeRestrictions != null)
            {
                campaign.TimeRestrictions = new List<TimeRestriction>();
                TimeRestrictions.ForEach(tr => campaign.TimeRestrictions.Add((TimeRestriction)tr.Clone()));
            }

            if (ProgrammeRestrictions != null)
            {
                campaign.ProgrammeRestrictions = new List<ProgrammeRestriction>();
                ProgrammeRestrictions.ForEach(pr => campaign.ProgrammeRestrictions.Add((ProgrammeRestriction)pr.Clone()));
            }

            if (SalesAreaCampaignTarget != null)
            {
                campaign.SalesAreaCampaignTarget = new List<SalesAreaCampaignTarget>();
                SalesAreaCampaignTarget.ForEach(sa => campaign.SalesAreaCampaignTarget.Add((SalesAreaCampaignTarget)sa.Clone()));
            }
            if (BookingPositionGroups != null)
            {
                campaign.BookingPositionGroups = new List<CampaignBookingPositionGroup>();
                BookingPositionGroups.ForEach(pg => campaign.BookingPositionGroups.Add((CampaignBookingPositionGroup)pg.Clone()));
            }
            if (CampaignPaybacks != null)
            {
                campaign.CampaignPaybacks = new List<CampaignPayback>();
                CampaignPaybacks.ForEach(cp => campaign.CampaignPaybacks.Add((CampaignPayback)cp.Clone()));
            }

            return campaign;
        }

        /// <summary>
        /// Indexes list by ExternalId
        /// </summary>
        /// <param name="campaigns"></param>
        /// <returns></returns>
        public static Dictionary<string, Campaign> IndexListByExternalId(
            IEnumerable<Campaign> campaigns
            )
        {
            var campaignsByExternalId = new Dictionary<string, Campaign>();

            foreach (var campaign in campaigns)
            {
                if (campaignsByExternalId.ContainsKey(campaign.ExternalId))
                {
                    continue;
                }

                campaignsByExternalId.Add(campaign.ExternalId, campaign);
            }

            return campaignsByExternalId;
        }

        /// <summary>
        /// Indexes list by CustomId
        /// </summary>
        /// <param name="campaigns"></param>
        /// <returns></returns>
        public static Dictionary<int, Campaign> IndexListByCustomId(IEnumerable<Campaign> campaigns)
        {
            var campaignsIndexed = new Dictionary<int, Campaign>();

            foreach (var campaign in campaigns)
            {
                if (!campaignsIndexed.ContainsKey(campaign.CustomId))
                {
                    campaignsIndexed.Add(campaign.CustomId, campaign);
                }
            }

            return campaignsIndexed;
        }

        /// <summary>
        /// Update the Campaign
        /// </summary>
        /// <param name="campaign"></param>
        /// <exception cref="ArgumentNullException">When campaign name is null or white space</exception>
        public void Update(Campaign campaign)
        {
            if (string.IsNullOrWhiteSpace(campaign.Name))
            {
                throw new ArgumentNullException(nameof(campaign.Name));
            }

            ExternalId = campaign.ExternalId;
            Name = campaign.Name;
            DemoGraphic = campaign.DemoGraphic;
            StartDateTime = campaign.StartDateTime;
            EndDateTime = campaign.EndDateTime;
            Product = campaign.Product;
            RevenueBudget = campaign.RevenueBudget;
            TargetRatings = campaign.TargetRatings;
            ActualRatings = campaign.ActualRatings;
            CampaignGroup = campaign.CampaignGroup;
            IsPercentage = campaign.IsPercentage;
            Status = campaign.Status;
            BusinessType = campaign.BusinessType;
            DeliveryType = campaign.DeliveryType;
            DeliveryCurrency = campaign.DeliveryCurrency;
            IncludeOptimisation = campaign.IncludeOptimisation;
            TargetZeroRatedBreaks = campaign.TargetZeroRatedBreaks;
            InefficientSpotRemoval = campaign.InefficientSpotRemoval;
            IncludeRightSizer = campaign.IncludeRightSizer;
            RightSizerLevel = campaign.RightSizerLevel;
            ExpectedClearanceCode = campaign.ExpectedClearanceCode;
            BreakType = campaign.BreakType;
            TimeRestrictions = campaign.TimeRestrictions;
            ProgrammeRestrictions = campaign.ProgrammeRestrictions;
            ProgrammesList = campaign.ProgrammesList;
            SalesAreaCampaignTarget = campaign.SalesAreaCampaignTarget;
            CampaignSpotMaxRatings = campaign.CampaignSpotMaxRatings;
            BookingPositionGroups = campaign.BookingPositionGroups;
        }

        public override string ToString()
        {
            return $"{Name ?? "[NoCampaignName]"}";
        }

        public void UpdateDerivedKPIs()
        {
            ActiveLength = CalculateActiveLength();
            RatingsDifferenceExcludingPayback = ActualRatings - ((decimal?)TargetXP ?? 0m);
            AchievedPercentageTargetRatings = TargetRatings != default ? (ActualRatings / TargetRatings) * 100m : default;

            if (RevenueBooked.HasValue)
            {
                ValueDifference = (decimal)(RevenueBooked) - (decimal)RevenueBudget;
                AchievedPercentageRevenueBudget = RevenueBudget != default ? ((decimal)RevenueBooked / (decimal)RevenueBudget) * 100m : default;
                ValueDifferenceExcludingPayback = (decimal)RevenueBooked - (decimal)RevenueBudget - CalculateTotalPayback();
            }
        }

        private string CalculateActiveLength()
        {
            var multipartsLengths = SalesAreaCampaignTarget
                ?.SelectMany(e => e.Multiparts
                    ?.SelectMany(j => j.Lengths?.Select(l => l.Length) ?? new List<Duration>()));

            var strikeWeightsLengths = SalesAreaCampaignTarget
                ?.SelectMany(saTarget => saTarget.CampaignTargets
                    ?.SelectMany(caTarget => caTarget.StrikeWeights
                        ?.SelectMany(sw => sw.Lengths?.Select(l => l.length) ?? new List<Duration>())));

            var dayPartsLengths = SalesAreaCampaignTarget
                ?.SelectMany(saTarget => saTarget.CampaignTargets
                    ?.SelectMany(caTarget => caTarget.StrikeWeights
                        ?.SelectMany(sw => sw.DayParts
                            ?.SelectMany(dp => dp.Lengths?.Select(l => l.Length) ?? new List<Duration>()))));

            var activeLengths = (multipartsLengths ?? new List<Duration>())
                .Concat(strikeWeightsLengths ?? new List<Duration>())
                .Concat(dayPartsLengths ?? new List<Duration>())
                .Distinct()
                .ToList();

            activeLengths.Sort();

            return String.Join("/", activeLengths.Select(e => e.TotalSeconds.ToString()));
        }

        public decimal CalculateTotalPayback() => CampaignPaybacks?.Sum(e => (decimal)e.Amount) ?? 0m;
    }
}
