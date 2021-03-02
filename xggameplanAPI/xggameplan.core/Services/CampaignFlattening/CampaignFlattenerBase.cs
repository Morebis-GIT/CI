using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using xggameplan.core.Interfaces;
using xggameplan.Model;

namespace xggameplan.core.Services.CampaignFlattening
{
    /// <summary>
    /// Gets the flattened view of all the campaigns.
    /// </summary>
    public abstract class CampaignFlattenerBase : ICampaignFlattener
    {
        private const string TempDay = "NNNNNNN";
        private const string TimeFormat = "hhmmss";

        private readonly IMapper _mapper;

        /// <summary>Initializes a new instance of the <see cref="CampaignFlattenerBase" /> class.</summary>
        /// <param name="mapper">The mapper.</param>
        protected CampaignFlattenerBase(IMapper mapper)
        {
            _mapper = mapper;
        }

        /// <summary>Prepares the related campaign data.</summary>
        /// <param name="campaigns">The campaigns.</param>
        protected abstract void PrepareRelatedCampaignData(IReadOnlyCollection<Campaign> campaigns);

        /// <summary>Resolves the demographic.</summary>
        /// <param name="campaign">The campaign.</param>
        /// <returns></returns>
        protected abstract Demographic ResolveDemographic(Campaign campaign);

        /// <summary>Resolves the product.</summary>
        /// <param name="campaign">The campaign.</param>
        /// <returns></returns>
        protected abstract Product ResolveProduct(Campaign campaign);

        /// <summary>Resolves the clash.</summary>
        /// <param name="campaign">The campaign.</param>
        /// <returns></returns>
        protected abstract Clash ResolveClash(Campaign campaign);

        /// <summary></summary>
        /// <param name="campaign"></param>
        protected abstract Clash ResolveParentClash(Campaign campaign);

        /// <summary>
        /// Converts the specified campaign list into the list of <see cref="CampaignFlattenedModel"/>>.
        /// </summary>
        public List<CampaignFlattenedModel> Flatten(IReadOnlyCollection<Campaign> campaigns)
        {
            var campaignFlattenedModelList = new List<CampaignFlattenedModel>();

            if (campaigns is null || campaigns.Count == 0)
            {
                return campaignFlattenedModelList;
            }

            PrepareRelatedCampaignData(campaigns);

            foreach (var item in campaigns)
            {
                var baseCampaignFlattenedModel = new CampaignBaseFlattenedModel()
                {
                    ExternalCampaignRef = item.ExternalId,
                    CampaignName = item.Name,
                    CampaignGroup = item.CampaignGroup,
                    BusinessType = item.BusinessType,
                    CampaignStartDate = item.StartDateTime,
                    CampaignEndDate = item.EndDateTime,
                    DaysToEndOfCampign = (item.EndDateTime - DateTime.Now).Days,
                    CampaignTargetRatings = item.TargetRatings,
                    CampaignActualRatings = item.ActualRatings,
                    CampaignTargetActualDiff = item.ActualRatings - item.TargetRatings,
                    CampaignTargetAchievedPct = GetCampaignTargetAchievedPct()
                };

                Demographic demographic = null;

                if (!(item.DemoGraphic is null))
                {
                    demographic = ResolveDemographic(item);
                }

                baseCampaignFlattenedModel.DemographicName = demographic?.Name ?? string.Empty;
                var product = ResolveProduct(item);

                if (!(product is null))
                {
                    baseCampaignFlattenedModel.AgencyName = product.AgencyName;
                    baseCampaignFlattenedModel.ProductName = product.Name;
                    baseCampaignFlattenedModel.AdvertiserName = product.AdvertiserName;
                    var childClash = ResolveClash(item);

                    if (!(childClash is null))
                    {
                        baseCampaignFlattenedModel.ClashName = childClash.Description;
                        var parentClash = ResolveParentClash(item);
                        if (!(parentClash is null))
                        {
                            baseCampaignFlattenedModel.ParentClashName = parentClash.Description;
                        }
                    }
                }

                foreach (var salesAreaCampaignTarget in item.SalesAreaCampaignTarget)
                {
                    var flattenedSaleAreaCampaignTarget = FlattenSaleAreaCampaignTarget(salesAreaCampaignTarget);
                    var flattenedDuration = FlattenDuration(salesAreaCampaignTarget);

                    if (salesAreaCampaignTarget.CampaignTargets is null ||
                        !salesAreaCampaignTarget.CampaignTargets.Any())
                    {
                        var mappedModel = MapToCampaignFlattenModel(
                            baseCampaignFlattenedModel,
                            flattenedSaleAreaCampaignTarget,
                            flattenedDuration);
                        campaignFlattenedModelList.Add(mappedModel);

                        continue;
                    }

                    foreach (var campaignTarget in salesAreaCampaignTarget.CampaignTargets)
                    {
                        foreach (var strikeWeight in campaignTarget.StrikeWeights)
                        {
                            var flattenedStrikeWeight = FlattenStrikeWeight(strikeWeight);

                            if (strikeWeight.DayParts is null || !strikeWeight.DayParts.Any())
                            {
                                var mappedModel = MapToCampaignFlattenModel(
                                    baseCampaignFlattenedModel,
                                    flattenedSaleAreaCampaignTarget,
                                    flattenedDuration,
                                    flattenedStrikeWeight);
                                campaignFlattenedModelList.Add(mappedModel);

                                continue;
                            }

                            foreach (var dayPart in strikeWeight.DayParts)
                            {
                                var flattenedDayPart = FlattenDayPart(dayPart);
                                var mappedModel = MapToCampaignFlattenModel(
                                    baseCampaignFlattenedModel,
                                    flattenedSaleAreaCampaignTarget,
                                    flattenedDuration,
                                    flattenedStrikeWeight,
                                    flattenedDayPart);
                                campaignFlattenedModelList.Add(mappedModel);
                            }
                        }
                    }
                }

                decimal GetCampaignTargetAchievedPct() =>
                    Math.Round(item.TargetRatings == 0 ? 0 : item.ActualRatings / item.TargetRatings * 100, 2,
                        MidpointRounding.AwayFromZero);
            }

            return campaignFlattenedModelList;
        }

        /// <summary>Flattens the sale area campaign target.</summary>
        /// <param name="salesAreaCampaignTarget">The sales area campaign target.</param>
        /// <returns></returns>
        public SalesAreaCampaignTargetFlattenModel FlattenSaleAreaCampaignTarget(
            SalesAreaCampaignTarget salesAreaCampaignTarget)
        {
            var salesAreaCampaignTargetFlattenModel = new SalesAreaCampaignTargetFlattenModel();

            if (salesAreaCampaignTarget is null)
            {
                return salesAreaCampaignTargetFlattenModel;
            }

            salesAreaCampaignTargetFlattenModel.SalesAreaGroupName = salesAreaCampaignTarget.SalesAreaGroup?.GroupName;

            if (!(salesAreaCampaignTarget.Multiparts is null))
            {
                salesAreaCampaignTargetFlattenModel.SalesAreaGroupTargetRatings =
                    salesAreaCampaignTarget.Multiparts.Sum(m => m.DesiredPercentageSplit);
                salesAreaCampaignTargetFlattenModel.SalesAreaGroupActualRatings =
                    salesAreaCampaignTarget.Multiparts.Sum(m => m.CurrentPercentageSplit);
            }

            salesAreaCampaignTargetFlattenModel.SalesAreaGroupTargetActualDiff =
                salesAreaCampaignTargetFlattenModel.SalesAreaGroupActualRatings -
                salesAreaCampaignTargetFlattenModel.SalesAreaGroupTargetRatings;

            salesAreaCampaignTargetFlattenModel.SalesAreaGroupTargetAchievedPct = GetSalesAreaGroupTargetAchievedPct();
            return salesAreaCampaignTargetFlattenModel;

            decimal GetSalesAreaGroupTargetAchievedPct() =>
                Math.Round(
                    salesAreaCampaignTargetFlattenModel.SalesAreaGroupTargetRatings == 0
                        ? 0
                        : salesAreaCampaignTargetFlattenModel.SalesAreaGroupActualRatings /
                        salesAreaCampaignTargetFlattenModel.SalesAreaGroupTargetRatings * 100, 2,
                    MidpointRounding.AwayFromZero);
        }

        /// <summary>Flattens the duration.</summary>
        /// <param name="salesAreaCampaignTarget">The sales area campaign target.</param>
        /// <returns></returns>
        public DurationFlattenModel FlattenDuration(SalesAreaCampaignTarget salesAreaCampaignTarget)
        {
            var durationFlattenModel = new DurationFlattenModel();

            if (salesAreaCampaignTarget is null)
            {
                return durationFlattenModel;
            }

            if (!(salesAreaCampaignTarget.Multiparts is null))
            {
                durationFlattenModel.DurationTargetRatings =
                    salesAreaCampaignTarget.Multiparts.Sum(m => m.DesiredPercentageSplit);
                durationFlattenModel.DurationActualRatings =
                    salesAreaCampaignTarget.Multiparts.Sum(m => m.CurrentPercentageSplit);

                var ticks = salesAreaCampaignTarget.Multiparts.FirstOrDefault()?.Lengths?.Select(x => x.Length)
                    .FirstOrDefault().BclCompatibleTicks ?? default;
                durationFlattenModel.DurationSecs = Convert.ToInt32(TimeSpan.FromTicks(ticks).TotalSeconds);
            }

            durationFlattenModel.DurationTargetActualDiff =
                durationFlattenModel.DurationActualRatings - durationFlattenModel.DurationTargetRatings;
            durationFlattenModel.DurationTargetAchievedPct = GetDurationTargetAchievedPct();

            return durationFlattenModel;

            decimal GetDurationTargetAchievedPct() =>
                Math.Round(durationFlattenModel.DurationTargetRatings == 0
                        ? 0
                        : durationFlattenModel.DurationActualRatings / durationFlattenModel.DurationTargetRatings * 100, 2,
                    MidpointRounding.AwayFromZero);
        }

        /// <summary>Flattens the strike weight.</summary>
        /// <param name="strikeWeight">The strike weight.</param>
        /// <returns></returns>
        public StrikeWeightFlattenModel FlattenStrikeWeight(StrikeWeight strikeWeight)
        {
            var strikeWeightFlattenModel = new StrikeWeightFlattenModel();

            if (strikeWeight is null)
            {
                return strikeWeightFlattenModel;
            }

            strikeWeightFlattenModel.StrikeWeightStartDate = strikeWeight.StartDate;
            strikeWeightFlattenModel.StrikeWeightEndDate = strikeWeight.EndDate;
            strikeWeightFlattenModel.StrikeWeightTargetRatings = strikeWeight.DesiredPercentageSplit;
            strikeWeightFlattenModel.StrikeWeightActualRatings = strikeWeight.CurrentPercentageSplit;
            strikeWeightFlattenModel.StrikeWeightTargetActualDiff =
                strikeWeightFlattenModel.StrikeWeightActualRatings - strikeWeightFlattenModel.StrikeWeightTargetRatings;
            strikeWeightFlattenModel.StrikeWeightTargetAchievedPct =
                GetStrikeWeightTargetAchievedPct();

            return strikeWeightFlattenModel;

            decimal GetStrikeWeightTargetAchievedPct() =>
                Math.Round(strikeWeightFlattenModel.StrikeWeightTargetRatings == 0
                        ? 0
                        : strikeWeightFlattenModel.StrikeWeightActualRatings /
                        strikeWeightFlattenModel.StrikeWeightTargetRatings * 100, 2,
                    MidpointRounding.AwayFromZero);
        }

        /// <summary>Flattens the day part.</summary>
        /// <param name="dayPart">The day part.</param>
        /// <returns></returns>
        public DayPartFlattenModel FlattenDayPart(DayPart dayPart)
        {
            var dayPartFlattenModel = new DayPartFlattenModel();

            if (dayPart is null)
            {
                return dayPartFlattenModel;
            }

            dayPartFlattenModel.DaypartTimeAndDays = FlattenTimeSlices(dayPart.Timeslices);
            dayPartFlattenModel.DaypartTargetRatings = dayPart.DesiredPercentageSplit;
            dayPartFlattenModel.DaypartActualRatings = dayPart.CurrentPercentageSplit;
            dayPartFlattenModel.DaypartTargetActualDiff =
                dayPartFlattenModel.DaypartActualRatings - dayPartFlattenModel.DaypartTargetRatings;
            dayPartFlattenModel.DaypartTargetAchievedPct = GetDaypartTargetAchievedPct();

            return dayPartFlattenModel;

            decimal GetDaypartTargetAchievedPct() =>
                Math.Round(dayPartFlattenModel.DaypartTargetRatings == 0
                        ? 0
                        : dayPartFlattenModel.DaypartActualRatings / dayPartFlattenModel.DaypartTargetRatings * 100, 2,
                    MidpointRounding.AwayFromZero);
        }

        /// <summary>Flattens the time slices.</summary>
        /// <param name="timeslices">The timeslices.</param>
        /// <returns></returns>
        public string FlattenTimeSlices(IEnumerable<Timeslice> timeslices)
        {
            var timesliceList = new List<string>();

            if (timeslices is null)
            {
                return string.Empty;
            }

            foreach (var timeSlice in timeslices)
            {
                var formattedTimeslice = FormatTimeslice(timeSlice);
                timesliceList.Add(formattedTimeslice);
            }

            return !timesliceList.Any() ? string.Empty : "(" + string.Join<string>("; ", timesliceList) + ")";

            string FormatTimeslice(Timeslice timeSlice)
            {
                var fromTime = string.IsNullOrEmpty(timeSlice.FromTime)
                    ? string.Empty
                    : TimeSpan.Parse(timeSlice.FromTime).ToString(TimeFormat, CultureInfo.InvariantCulture);

                var toTime = string.IsNullOrEmpty(timeSlice.ToTime)
                    ? " "
                    : TimeSpan.Parse(timeSlice.ToTime).ToString(TimeFormat, CultureInfo.InvariantCulture) + " ";

                var timeSeparator = string.IsNullOrEmpty(timeSlice.ToTime) || string.IsNullOrEmpty(timeSlice.FromTime)
                        ? string.Empty
                        : "-";

                return fromTime + timeSeparator + toTime + FlattenDowPattern(timeSlice.DowPattern);
            }
        }

        /// <summary>Flattens the dow pattern.</summary>
        /// <param name="dowPatterns">The dow patterns.</param>
        /// <returns></returns>
        public string FlattenDowPattern(IEnumerable<string> dowPatterns)
        {
            if (dowPatterns is null)
            {
                return string.Empty;
            }

            var tempCh = TempDay.ToCharArray();
            foreach (var dow in dowPatterns)
            {
                switch (dow)
                {
                    case "Sun":
                        tempCh[0] = 'Y';
                        break;

                    case "Mon":
                        tempCh[1] = 'Y';
                        break;

                    case "Tue":
                        tempCh[2] = 'Y';
                        break;

                    case "Wed":
                        tempCh[3] = 'Y';
                        break;

                    case "Thu":
                        tempCh[4] = 'Y';
                        break;

                    case "Fri":
                        tempCh[5] = 'Y';
                        break;

                    case "Sat":
                        tempCh[6] = 'Y';
                        break;

                    default:
                        break;
                }
            }

            return new string(tempCh);
        }

        /// <summary>Maps to campaign flatten model.</summary>
        /// <param name="baseCampaignFlattenedModel">The base campaign flattened model.</param>
        /// <param name="salesAreaCampaignTargetFlattenModel">The sales area campaign target flatten model.</param>
        /// <param name="durationFlattenModel">The duration flatten model.</param>
        /// <param name="strikeWeightFlattenModel">The strike weight flatten model.</param>
        /// <param name="dayPartFlattenModel">The day part flatten model.</param>
        /// <returns></returns>
        private CampaignFlattenedModel MapToCampaignFlattenModel(
            CampaignBaseFlattenedModel baseCampaignFlattenedModel,
            SalesAreaCampaignTargetFlattenModel salesAreaCampaignTargetFlattenModel,
            DurationFlattenModel durationFlattenModel,
            StrikeWeightFlattenModel strikeWeightFlattenModel = null,
            DayPartFlattenModel dayPartFlattenModel = null)
        {
            return _mapper.Map<CampaignFlattenedModel>(
                Tuple.Create(
                    baseCampaignFlattenedModel,
                    salesAreaCampaignTargetFlattenModel,
                    durationFlattenModel,
                    strikeWeightFlattenModel,
                    dayPartFlattenModel));
        }
    }
}
