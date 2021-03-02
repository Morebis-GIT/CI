using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects.AgCampaigns;
using ImagineCommunications.GamePlan.Domain.BusinessTypes.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.ProgrammeCategory;
using ImagineCommunications.GamePlan.Domain.ProgrammeDictionaries;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using NodaTime;
using xggameplan.AuditEvents;
using xggameplan.core.Extensions;
using xggameplan.core.FeatureManagement.Interfaces;
using xggameplan.core.Services.OptimiserInputFilesSerialisers.Interfaces;
using xggameplan.Extensions;
using xggameplan.Model.AutoGen;

namespace xggameplan.core.Services.OptimiserInputFilesSerialisers.Campaigns
{
    /// <summary>
    /// Serializes campaign collection into xml file.
    /// </summary>
    /// <seealso cref="xggameplan.core.Services.OptimiserInputFilesSerialisers.SerializerBase" />
    /// <seealso cref="xggameplan.core.Services.OptimiserInputFilesSerialisers.Interfaces.ICampaignSerializer" />
    public abstract class CampaignSerializerBase : SerializerBase, ICampaignSerializer
    {
        private const string ShortDateFormat = "yyyyMMdd";
        private const string TimeFormat = "hhmmss";
        private const string FileName = "v_camp_list.xml";

        private readonly IClock _clock;
        private readonly IMapper _mapper;
        private readonly bool _includeChannelGroup;
        private readonly bool _mergeUniqueDayParts;
        private readonly bool _includeCampaignPrice;

        [Flags]
        protected enum RunSteps
        {
            None = 0,
            CXgSmooth = 1,
            CXgIsr = 2,
            CXgRightSizer = 4,
            CXgOptimiser = 8,
            CXgRightSizerCamp = 16,
            CXgRightSizerDetail = 32
        }

        /// <summary>Prepares the related data.</summary>
        /// <param name="campaigns">The campaigns.</param>
        protected abstract void PrepareRelatedData(IReadOnlyCollection<Campaign> campaigns);

        /// <summary>Resolves the product.</summary>
        /// <param name="campaign">The campaign.</param>
        /// <returns></returns>
        protected abstract Product ResolveProduct(Campaign campaign);

        /// <summary>Resolves root clash code for the product clash.</summary>
        /// <param name="campaign">The campaign.</param>
        /// <returns></returns>
        protected abstract string ResolveRootClashCode(Campaign campaign);

        /// <summary>Initializes a new instance of the <see cref="CampaignSerializerBase" /> class.</summary>
        /// <param name="auditEventRepository">The audit event repository.</param>
        /// <param name="featureManager">The feature manager.</param>
        /// <param name="clock">The clock.</param>
        /// <param name="mapper">The mapper.</param>
        protected CampaignSerializerBase(
            IAuditEventRepository auditEventRepository,
            IFeatureManager featureManager,
            IClock clock,
            IMapper mapper) : base(auditEventRepository)
        {
            _clock = clock;
            _mapper = mapper;

            _includeChannelGroup =
                featureManager.IsEnabled(ProductFeature.IncludeChannelGroupFileForOptimiser);
            _mergeUniqueDayParts = featureManager.IsEnabled(ProductFeature.StrikeWeightDayPartsMerge);
            _includeCampaignPrice = featureManager.IsEnabled(ProductFeature.NominalPrice);
        }

        public string Filename => FileName;

        /// <summary>Serializes campaigns into the specified folder.</summary>
        /// <param name="folderName">Name of the folder.</param>
        /// <param name="run">The run.</param>
        /// <param name="campaigns">The campaigns.</param>
        /// <param name="demographics">The demographics.</param>
        /// <param name="salesAreas">The sales areas.</param>
        /// <param name="programmeDictionaries">The programme dictionaries.</param>
        /// <param name="programmeCategories">The programme categories.</param>
        /// <param name="autoBookDefaultParameters">The automatic book default parameters.</param>
        /// <param name="channelGroups">The channel groups.</param>
        public void Serialize(
            string folderName,
            Run run,
            IReadOnlyCollection<Campaign> campaigns,
            IReadOnlyCollection<BusinessType> businessTypes,
            IReadOnlyCollection<Demographic> demographics,
            IReadOnlyCollection<SalesArea> salesAreas,
            IReadOnlyCollection<ProgrammeDictionary> programmeDictionaries,
            IReadOnlyCollection<ProgrammeCategoryHierarchy> programmeCategories,
            IAutoBookDefaultParameters autoBookDefaultParameters,
            out IReadOnlyCollection<Tuple<int, int, SalesAreaGroup>> channelGroups,
            out List<AgCampaignInclusion> campaignIncludeFunctions)
        {
            channelGroups = null;
            campaignIncludeFunctions = null;

            if (campaigns is null || campaigns.Count == 0)
            {
                return;
            }

            RaiseInfo(
                $"Started populating {Filename}. Total campaigns - {campaigns.Count}, Current Time - {_clock.GetCurrentInstant().ToDateTimeUtc()}");

            var channelGroup = new List<Tuple<int, int, SalesAreaGroup>>();

            PrepareRelatedData(campaigns);

            var dayPartNo = 0;
            var agCampaigns = campaigns.Select(campaign =>
            {
                var demographicNo = demographics
                    .FirstOrDefault(d => d.ExternalRef.Equals(campaign.DemoGraphic))?.Id ?? 0;

                int.TryParse(campaign.Product, out var productCode);
                var product = productCode != 0
                    ? ResolveProduct(campaign)
                    : null;

                var agCampaignSalesAreaList = LoadAgCampaignSalesAreas(
                    campaign,
                    salesAreas,
                    autoBookDefaultParameters.AgCampaignSalesArea,
                    ref dayPartNo,
                    ref channelGroup);

                var agCampaignSalesAreas = new AgCampaignSalesAreas();

                if (!(agCampaignSalesAreaList is null))
                {
                    agCampaignSalesAreas.AddRange(agCampaignSalesAreaList);
                }

                var agCampaignProgrammeList = LoadAgCampaignProgrammes(
                    campaign.ProgrammesList,
                    salesAreas,
                    programmeDictionaries,
                    programmeCategories);

                var agCampaignClone = autoBookDefaultParameters.AgCampaign.Clone();

                agCampaignClone.CampaignNo = campaign.CustomId;
                agCampaignClone.ExternalNo = campaign.ExternalId;
                agCampaignClone.DemographicNo = demographicNo;
                agCampaignClone.DealNo = campaign.CustomId;
                agCampaignClone.ProductCode = productCode;
                agCampaignClone.ClearanceCode = campaign.ExpectedClearanceCode ?? "";
                agCampaignClone.RevenueBudget = Convert.ToInt32(campaign.RevenueBudget);
                agCampaignClone.StartDate = campaign.StartDateTime.ToString(ShortDateFormat);
                agCampaignClone.EndDate = campaign.EndDateTime.ToString(ShortDateFormat);
                agCampaignClone.RootClashCode = string.IsNullOrWhiteSpace(product?.ClashCode)
                    ? productCode.ToString()
                    : ResolveRootClashCode(campaign);
                agCampaignClone.ClashCode = string.IsNullOrWhiteSpace(product?.ClashCode)
                    ? productCode.ToString()
                    : product.ClashCode;
                agCampaignClone.AdvertiserIdentifier = product?.AdvertiserIdentifier;
                agCampaignClone.ClashNo = 0;
                agCampaignClone.BusinessType = businessTypes.FirstOrDefault(x =>
                                                   x.Code == campaign.BusinessType && x.IncludeConversionIndex)?.Code
                                               ?? string.Empty;
                agCampaignClone.AgCampaignRequirement =
                    _mapper.Map<AgRequirement>(Tuple.Create(campaign.TargetRatings, campaign.ActualRatings));
                agCampaignClone.AgCampaignSalesAreas = agCampaignSalesAreas;
                agCampaignClone.NbrAgCampagignSalesArea = agCampaignSalesAreaList?.Count ?? 0;
                agCampaignClone.MaxAgCampagignSalesArea = agCampaignSalesAreaList?.Count ?? 0;
                agCampaignClone.IncludeFunctions = (int)GetIncludeFunctions(run, campaign);
                agCampaignClone.CampaignSpotMaxRatings = campaign.CampaignSpotMaxRatings;
                agCampaignClone.NbrAgCampaignProgrammeList = agCampaignProgrammeList.Count;
                agCampaignClone.AgProgrammeList = agCampaignProgrammeList;
                agCampaignClone.DeliveryCurrency = (int)campaign.DeliveryCurrency;
                agCampaignClone.StopBooking = AgConversions.ToAgBooleanAsString(campaign.StopBooking);

                return agCampaignClone;
            }).ToList();

            new AgCampaignsSerialization().MapFrom(agCampaigns).Serialize(Path.Combine(folderName, Filename));
            channelGroups = channelGroup;

            campaignIncludeFunctions = agCampaigns.Select(campaign =>
                new AgCampaignInclusion() { CampaignNo = campaign.CampaignNo, IncludeFunctions = campaign.IncludeFunctions }).ToList();

            RaiseInfo(
                $"Finished populating {Filename}. Total campaigns - {campaigns.Count}, Current Time - {_clock.GetCurrentInstant().ToDateTimeUtc()}");
        }

        /// <summary>Loads the ag campaign sales areas.</summary>
        /// <param name="campaign">The campaign.</param>
        /// <param name="salesAreas">The sales areas.</param>
        /// <param name="agCampaignSalesArea">The ag campaign sales area.</param>
        /// <param name="dayPartNo">The day part no.</param>
        /// <param name="channelGroup">The channel group.</param>
        /// <returns></returns>
        protected List<AgCampaignSalesArea> LoadAgCampaignSalesAreas(
            Campaign campaign,
            IReadOnlyCollection<SalesArea> salesAreas,
            AgCampaignSalesArea agCampaignSalesArea,
            ref int dayPartNo,
            ref List<Tuple<int, int, SalesAreaGroup>> channelGroup)
        {
            if (campaign.SalesAreaCampaignTarget is null || campaign.SalesAreaCampaignTarget.Count == 0)
            {
                return null;
            }

            var counter = 1;
            var salesAreaTargetMap = salesAreas
                .Where(x => x.TargetAreaName != null)
                .Select(x => x.TargetAreaName)
                .Distinct()
                .ToDictionary(x => x, x => counter++);

            var agCampaignSalesAreaList = new List<AgCampaignSalesArea>();
            foreach (var salesAreaCampaignTarget in campaign.SalesAreaCampaignTarget)
            {
                int salesAreaNo;
                int channelGroupNo;

                if (_includeChannelGroup)
                {
                    // In this case we use old channel group calculation logic
                    // (assigning this value to both sales areas and channel group properties).

                    GetCampaignChannelGroup(salesAreaCampaignTarget.SalesAreaGroup, campaign.CustomId,
                        ref channelGroup, out salesAreaNo);
                    channelGroupNo = salesAreaNo;
                }
                else
                {
                    // In this case we use logic which is to calculate ChannelGroup "saoc_data.tgt_sare_no" value
                    // using the SalesArea TargetAreaName property and use SalesArea CustomIds for everything else.

                    var salesArea = salesAreas.FirstOrDefault(s => s.Name.Equals(salesAreaCampaignTarget.SalesArea));

                    salesAreaNo = salesArea?.CustomId ?? 0;
                    channelGroupNo = salesArea is null || salesArea.TargetAreaName is null
                        ? 0
                        : salesAreaTargetMap[salesArea.TargetAreaName];
                }

                var totalDesiredSplit = salesAreaCampaignTarget.Multiparts?.Sum(l => l.DesiredPercentageSplit) ?? 0;
                var totalCurrentSplit = salesAreaCampaignTarget.Multiparts?.Sum(l => l.CurrentPercentageSplit) ?? 0;

                var agLengthList = salesAreaCampaignTarget.Multiparts != null
                    ? _mapper.Map<List<AgLength>>(Tuple.Create(salesAreaCampaignTarget.Multiparts,
                        campaign.CustomId, salesAreaNo))
                    : null;

                var agLengths = new AgLengths();

                if (!(agLengthList is null))
                {
                    agLengths.AddRange(agLengthList);
                }

                var strikeWeights = salesAreaCampaignTarget.CampaignTargets
                    ?.Where(t => t.StrikeWeights != null && t.StrikeWeights.Any())
                    .SelectMany(s => s.StrikeWeights).ToList();

                var agStrikeWeightList = strikeWeights != null && strikeWeights.Any()
                    ? _mapper.Map<List<AgStrikeWeight>>(Tuple.Create(strikeWeights, campaign.CustomId,
                        salesAreaNo))
                    : null;

                var agStrikeWeights = new AgStrikeWeights();

                if (!(agStrikeWeightList is null))
                {
                    agStrikeWeights.AddRange(agStrikeWeightList);
                }

                var agDayPartList = LoadAgDayParts(
                    strikeWeights,
                    campaign.CustomId,
                    salesAreaNo,
                    ref dayPartNo);

                var agPartList = !(agDayPartList is null) && agDayPartList.Any()
                    ? _mapper.Map<List<AgPart>>(agDayPartList)
                    : null;

                var agParts = new AgParts();

                if (!(agPartList is null))
                {
                    agParts.AddRange(agPartList);
                }

                if (_mergeUniqueDayParts)
                {
                    agDayPartList = MergeUniqueDayParts(agDayPartList, agParts);
                }

                var agDayParts = new AgDayParts();

                if (!(agDayPartList is null))
                {
                    agDayParts.AddRange(agDayPartList);
                }

                var agDayPartLengths = agDayPartList?.Count > 0
                    ? agDayPartList.Select(x => x.AgDayPartLengths).ToList()
                    : null;
                var agPartLengthList = _mapper.Map<List<AgPartLength>>(agDayPartLengths);

                var agPartLengths = new AgPartLengths();

                if (!(agPartLengthList is null))
                {
                    agPartLengths.AddRange(agPartLengthList);
                }

                var agCampaignSalesAreaClone = agCampaignSalesArea.Clone();
                agCampaignSalesAreaClone.SalesAreaNo = salesAreaNo;
                agCampaignSalesAreaClone.ChannelGroupNo = channelGroupNo;
                agCampaignSalesAreaClone.CampaignNo = campaign.CustomId;
                agCampaignSalesAreaClone.AgSalesAreaCampaignRequirement
                    = _mapper.Map<AgRequirement>(Tuple.Create(totalDesiredSplit, totalCurrentSplit));
                agCampaignSalesAreaClone.AgCampaignSalesAreaPtrRef
                    = agCampaignSalesArea.AgCampaignSalesAreaPtrRef.Clone();
                agCampaignSalesAreaClone.AgCampaignSalesAreaPtrRef.SalesAreaNo = salesAreaNo;
                agCampaignSalesAreaClone.AgLengths = agLengths;
                agCampaignSalesAreaClone.NbrAgLengths = agLengthList?.Count ?? 0;
                agCampaignSalesAreaClone.AgStrikeWeights = agStrikeWeights;
                agCampaignSalesAreaClone.NbrAgStrikeWeights = agStrikeWeightList?.Count ?? 0;
                agCampaignSalesAreaClone.AgDayParts = agDayParts;
                agCampaignSalesAreaClone.NbrAgDayParts = agDayPartList?.Count ?? 0;
                agCampaignSalesAreaClone.AgParts = agParts;
                agCampaignSalesAreaClone.NbrParts = agPartList?.Count ?? 0;
                agCampaignSalesAreaClone.NbrPartsLengths = agPartLengthList?.Count ?? 0;
                agCampaignSalesAreaClone.AgPartsLengths = agPartLengths;
                agCampaignSalesAreaClone.StopBooking =
                    AgConversions.ToAgBooleanAsString(salesAreaCampaignTarget.StopBooking);

                agCampaignSalesAreaList.Add(agCampaignSalesAreaClone);
            }

            return agCampaignSalesAreaList;
        }

        /// <summary>Loads the ag campaign programmes.</summary>
        /// <param name="campaignProgrammes">The campaign programmes.</param>
        /// <param name="salesAreas">The sales areas.</param>
        /// <param name="programmeDictionaries">The programme dictionaries.</param>
        /// <param name="programmeCategories">The programme categories.</param>
        /// <returns></returns>
        protected AgCampaignProgrammeList LoadAgCampaignProgrammes(
            IReadOnlyCollection<CampaignProgramme> campaignProgrammes,
            IReadOnlyCollection<SalesArea> salesAreas,
            IReadOnlyCollection<ProgrammeDictionary> programmeDictionaries,
            IReadOnlyCollection<ProgrammeCategoryHierarchy> programmeCategories)
        {
            var agCampaignProgrammes = new AgCampaignProgrammeList();

            if (campaignProgrammes is null || campaignProgrammes.Count == 0)
            {
                return agCampaignProgrammes;
            }

            foreach (var campaignProgramme in campaignProgrammes)
            {
                var mappedSalesAreas = LoadAgSalesAreas(campaignProgramme, salesAreas);
                var mappedProgrammeCategories =
                    LoadAgProgrammeProgrammeCategory(campaignProgramme, programmeDictionaries, programmeCategories);
                var mappedTimeBands = LoadAgTimeBands(campaignProgramme.Timeband);

                var agCampaignProgramme = new AgCampaignProgramme
                {
                    StartDate = campaignProgramme.StartDate.ToString(ShortDateFormat),
                    EndDate = campaignProgramme.EndDate.ToString(ShortDateFormat),
                    NbrSalesAreas = mappedSalesAreas.Count,
                    SalesAreas = mappedSalesAreas,
                    NbrCategoryOrProgrammeList = mappedProgrammeCategories.Count,
                    CategoryOrProgramme = mappedProgrammeCategories,
                    NumberTimeBands = mappedTimeBands.Count,
                    TimeBands = mappedTimeBands,
                    AgCampaignProgrammeRequirement = new AgRequirement
                    {
                        Required = campaignProgramme.DesiredPercentageSplit,
                        TgtRequired = campaignProgramme.DesiredPercentageSplit,
                        SareRequired = campaignProgramme.DesiredPercentageSplit,
                        Supplied = campaignProgramme.CurrentPercentageSplit
                    }
                };

                agCampaignProgrammes.Add(agCampaignProgramme);
            }

            return agCampaignProgrammes;
        }

        /// <summary>Loads the ag sales areas.</summary>
        /// <param name="campaignProgramme">The campaign programme.</param>
        /// <param name="salesAreas">The sales areas.</param>
        /// <returns></returns>
        protected AgSalesAreas LoadAgSalesAreas(CampaignProgramme campaignProgramme, IEnumerable<SalesArea> salesAreas)
        {
            var mapped = campaignProgramme.SalesAreas.Any()
                ? salesAreas.Where(s => campaignProgramme.SalesAreas.Contains(s.Name))
                    .Select(s => new AgSalesArea { SalesAreaNumber = s.CustomId }).ToList()
                : salesAreas.Select(s => new AgSalesArea { SalesAreaNumber = s.CustomId }).ToList();

            var agSalesAreas = new AgSalesAreas();
            agSalesAreas.AddRange(mapped);

            return agSalesAreas;
        }

        /// <summary>Loads the ag programme programme category.</summary>
        /// <param name="campaignProgramme">The campaign programme.</param>
        /// <param name="programmeDictionaries">The programme dictionaries.</param>
        /// <param name="programmeCategories">The programme categories.</param>
        /// <returns></returns>
        protected AgCampaignProgrammeProgrammeCategories LoadAgProgrammeProgrammeCategory(
            CampaignProgramme campaignProgramme,
            IEnumerable<ProgrammeDictionary> programmeDictionaries,
            IEnumerable<ProgrammeCategoryHierarchy> programmeCategories)
        {
            var mapped = new AgCampaignProgrammeProgrammeCategories();

            if (campaignProgramme.IsCategoryOrProgramme.EqualInvariantCultureIgnoreCase(
                CategoryOrProgramme.P.ToString()))
            {
                var programmes = programmeDictionaries
                    .Where(c => campaignProgramme.CategoryOrProgramme.Any(p =>
                        p.Equals(c.ExternalReference.ToString(), StringComparison.OrdinalIgnoreCase)))
                    .Select(c => new AgCampaignProgrammeProgrammeCategory
                    {
                        ProgrammeNumber = c.Id,
                        CategoryNumber = 0
                    })
                    .ToList();

                mapped.AddRange(programmes);
            }
            else
            {
                var categories = programmeCategories
                    .Where(c =>
                        campaignProgramme.CategoryOrProgramme.Any(p => p.EqualInvariantCultureIgnoreCase(c.Name)))
                    .Select(c => new AgCampaignProgrammeProgrammeCategory
                    {
                        CategoryNumber = c.Id,
                        ProgrammeNumber = 0
                    })
                    .ToList();

                mapped.AddRange(categories);
            }

            return mapped;
        }

        /// <summary>Loads the ag time bands.</summary>
        /// <param name="timeBands">The time bands.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">
        /// Value {timeBand.FromTime} of 'FromTime' of {nameof(Timeslice)} cannot be parsed to {nameof(TimeSpan)}
        /// or
        /// Value {timeBand.ToTime} of 'ToTime' of {nameof(Timeslice)} cannot be parsed to {nameof(TimeSpan)}
        /// </exception>
        protected AgTimeBands LoadAgTimeBands(IEnumerable<Timeslice> timeBands)
        {
            var mapped = new AgTimeBands();
            foreach (var timeBand in timeBands)
            {
                if (!TimeSpan.TryParse(timeBand.FromTime, out TimeSpan startTime))
                {
                    throw new ArgumentException(
                        $"Value {timeBand.FromTime} of '{nameof(Timeslice.FromTime)}' of {nameof(Timeslice)} cannot be parsed to {nameof(TimeSpan)}");
                }

                if (!TimeSpan.TryParse(timeBand.ToTime, out TimeSpan endTime))
                {
                    throw new ArgumentException(
                        $"Value {timeBand.ToTime} of '{nameof(Timeslice.ToTime)}' of {nameof(Timeslice)} cannot be parsed to {nameof(TimeSpan)}");
                }

                var days = timeBand.DowPattern.GetDaysOfWeek();

                mapped.Add(new AgTimeBand
                {
                    StartTime = startTime.ToString(TimeFormat),
                    EndTime = endTime.ToString(TimeFormat),
                    Days = AgConversions.ToAgDaysOfWeekAsInt(days)
                });
            }

            return mapped;
        }

        /// <summary>Loads the ag day parts.</summary>
        /// <param name="strikeWeights">The strike weights.</param>
        /// <param name="campaignNo">The campaign no.</param>
        /// <param name="channelGroupNo">The channel group no.</param>
        /// <param name="dayPartNo">The day part no.</param>
        /// <returns></returns>
        protected List<AgDayPart> LoadAgDayParts(
            IReadOnlyCollection<StrikeWeight> strikeWeights,
            int campaignNo,
            int channelGroupNo,
            ref int dayPartNo)
        {
            if (strikeWeights is null || strikeWeights.Count == 0)
            {
                return null;
            }

            var partNumber = dayPartNo;

            var res = strikeWeights.SelectMany(strikeWeight =>
            {
                var startDate = strikeWeight.StartDate.ToString(ShortDateFormat);
                var endDate = strikeWeight.EndDate.ToString(ShortDateFormat);

                return strikeWeight.DayParts.Select(dayPart =>
                {
                    partNumber++;

                    var agTimeSlices = new AgTimeSlices(dayPart.Timeslices?.SelectMany(timeSlice =>
                    {
                        var startTime = TimeSpan.Parse(timeSlice.FromTime);
                        var endTime = AdjustAgTimesliceToTime(TimeSpan.Parse(timeSlice.ToTime));

                        return timeSlice.DowPattern?.GetDayRange().Select(dayRange => new AgTimeSlice
                        {
                            CampaignNo = campaignNo,
                            SalesAreaNo = channelGroupNo,
                            DayPartNo = partNumber,
                            StartDate = startDate,
                            StartTime = startTime.ToString("hhmmss"),
                            EndTime = endTime.ToString("hhmmss"),
                            StartDay = dayRange?.Item1 ?? 0,
                            EndDay = dayRange?.Item2 ?? 0
                        }) ?? Enumerable.Empty<AgTimeSlice>();
                    }) ?? Enumerable.Empty<AgTimeSlice>());

                    var agDayPartLengthsList = dayPart.Lengths != null && dayPart.Lengths.Any()
                        ? _mapper.Map<List<AgDayPartLength>>(
                            Tuple.Create(dayPart.Lengths, campaignNo, channelGroupNo, partNumber, startDate))
                        : null;

                    var agDayPartLengths = new AgDayPartLengths();

                    if (!(agDayPartLengthsList is null))
                    {
                        agDayPartLengths.AddRange(agDayPartLengthsList);
                    }

                    return new AgDayPart
                    {
                        Name = dayPart.DayPartName ?? "NotSupplied",
                        SalesAreaNo = channelGroupNo,
                        CampaignNo = campaignNo,
                        DayPartNo = partNumber,
                        StartDate = startDate,
                        EndDate = endDate,
                        AgDayPartRequirement =
                            _mapper.Map<AgRequirement>(Tuple.Create(dayPart.DesiredPercentageSplit,
                                dayPart.CurrentPercentageSplit)),
                        AgTimeSlices = agTimeSlices,
                        NbrAgTimeSlices = agTimeSlices.Count,
                        SpotMaxRatings = dayPart.SpotMaxRatings,
                        NbrAgDayPartLengths = agDayPartLengthsList?.Count ?? 0,
                        AgDayPartLengths = agDayPartLengths,
                        CampaignPrice = _includeCampaignPrice ? dayPart.CampaignPrice : default
                    };
                });
            }).ToList();

            dayPartNo = partNumber;

            return res;
        }

        protected virtual TimeSpan AdjustAgTimesliceToTime(TimeSpan value) =>
            value.Seconds == 59 ? value : value.Add(new TimeSpan(0, 0, 59 - value.Seconds));

        /// <summary>Gets the campaign channel group.</summary>
        /// <param name="salesAreaGroup">The sales area group.</param>
        /// <param name="campaignNo">The campaign no.</param>
        /// <param name="channelGroup">The channel group.</param>
        /// <param name="channelGroupNo">The channel group no.</param>
        protected void GetCampaignChannelGroup(
            SalesAreaGroup salesAreaGroup,
            int campaignNo,
            ref List<Tuple<int, int, SalesAreaGroup>> channelGroup,
            out int channelGroupNo)
        {
            channelGroupNo = 1;
            //Tuple<int, int, SalesAreaGroup> --> ChannelgroupNo, CampaignNo, SaleAreaGroup

            if (channelGroup is null)
            {
                channelGroup = new List<Tuple<int, int, SalesAreaGroup>>();
            }

            if (salesAreaGroup is null)
            {
                channelGroupNo = 0;
                return;
            }

            if (channelGroup.Any())
            {
                var filterByName = channelGroup.Where(t =>
                    t.Item3.GroupName.Equals(salesAreaGroup.GroupName, StringComparison.OrdinalIgnoreCase)).ToList();
                var matchedCampaign = filterByName.FirstOrDefault(t => t.Item2 == campaignNo);
                if (!(matchedCampaign is null)) // if group name repeating within campaign
                {
                    channelGroupNo = matchedCampaign.Item1;
                    return;
                }

                if (filterByName.Any()) // if group name repeating across the campaigns
                {
                    channelGroupNo = filterByName.FirstOrDefault()?.Item1 ?? 0;
                }
                else
                {
                    channelGroupNo = (int)(channelGroup?.Max(t => t.Item1) + 1); // New group name
                }
            }

            channelGroup.Add(Tuple.Create(channelGroupNo, campaignNo, salesAreaGroup));
        }

        /// <summary>
        /// Merges unique DayParts from all StrikeWeights of certain SalesArea
        /// </summary>
        /// <param name="dayParts">
        /// DayParts from all StrikeWeights of certain SalesArea
        /// </param>
        /// <param name="mapper"></param>
        /// <returns>
        /// List of unique DayParts for SalesArea, with
        /// <see cref="AgDayPart.AgDayPartRequirement"/> being summed up
        /// </returns>
        protected List<AgDayPart> MergeUniqueDayParts(
            IReadOnlyCollection<AgDayPart> dayParts,
            AgParts agParts)
        {
            if ((dayParts?.Count ?? 0) == 0)
            {
                return null;
            }

            return dayParts
                .GroupBy(d => d.UniqueTimeSlicesHash)
                .Select(x =>
                {
                    var dayPartIds = x.Select(d => d.DayPartNo).ToList();

                    var groupedDayParts = x.Select(d => d).OrderBy(d => d.DayPartNo).ToList();
                    var dayPart = groupedDayParts.First();
                    decimal desiredPercentageSplit = 0,
                        currentPercentageSplit = 0,
                        campaignPrice = dayPart.CampaignPrice;

                    foreach (var item in groupedDayParts)
                    {
                        desiredPercentageSplit += item.AgDayPartRequirement.Required;
                        currentPercentageSplit += item.AgDayPartRequirement.Supplied;

                        if (item.CampaignPrice > campaignPrice)
                        {
                            campaignPrice = item.CampaignPrice;
                        }
                    }

                    dayPart.StartDate = "0";
                    dayPart.EndDate = "0";
                    dayPart.SpotMaxRatings = 0;
                    dayPart.CampaignPrice = campaignPrice;
                    dayPart.AgDayPartRequirement =
                        _mapper.Map<AgRequirement>(Tuple.Create(desiredPercentageSplit, currentPercentageSplit));

                    foreach (var t in dayPart.AgTimeSlices)
                    {
                        t.StartDate = "0";
                    }

                    var agPartsForUpdate = agParts.Where(d => dayPartIds.Contains(d.DayPartNo));
                    foreach (var part in agPartsForUpdate)
                    {
                        part.DayPartNo = dayPart.DayPartNo;
                    }

                    return dayPart;
                })
                .ToList();
        }

        /// <summary>Gets the include functions.</summary>
        /// <param name="run">The run.</param>
        /// <param name="campaign">The campaign.</param>
        /// <returns></returns>
        private static RunSteps GetIncludeFunctions(Run run, Campaign campaign)
        {
            RunSteps incFunctions = RunSteps.None;

            if (run.Smooth)
            {
                incFunctions |= RunSteps.CXgSmooth;
            }

            if (run.ISR && IsIsrApplicableForCampaign(run, campaign))
            {
                incFunctions |= RunSteps.CXgIsr;
            }

            if (run.Optimisation && campaign.IncludeOptimisation)
            {
                incFunctions |= RunSteps.CXgOptimiser;
            }

            if (run.RightSizer && IsRightSizerApplicableForCampaign(run, campaign))
            {
                incFunctions |= RunSteps.CXgRightSizer;
                var resolvedRightSizerLevel = ResolveRightSizerLevelForCampaign(run, campaign);
                switch (resolvedRightSizerLevel)
                {
                    case RightSizerLevel.CampaignLevel:
                        incFunctions |= RunSteps.CXgRightSizerCamp;
                        break;

                    case RightSizerLevel.DetailLevel:
                        incFunctions |= RunSteps.CXgRightSizerDetail;
                        break;
                }
            }

            return incFunctions;
        }

        /// <summary>Determines whether [is isr applicable for campaign] [the specified run].</summary>
        /// <param name="run">The run.</param>
        /// <param name="campaign">The campaign.</param>
        /// <returns>
        ///   <c>true</c> if [is isr applicable for campaign] [the specified run]; otherwise, <c>false</c>.</returns>
        private static bool IsIsrApplicableForCampaign(Run run, Campaign campaign)
        {
            var lookupCampaignRunProcessesSettings =
                run.CampaignsProcessesSettings.FirstOrDefault(s => s.ExternalId == campaign.ExternalId);

            return lookupCampaignRunProcessesSettings?.InefficientSpotRemoval ?? campaign.InefficientSpotRemoval;
        }

        /// <summary>Determines whether [is right sizer applicable for campaign] [the specified run].</summary>
        /// <param name="run">The run.</param>
        /// <param name="campaign">The campaign.</param>
        /// <returns>
        ///   <c>true</c> if [is right sizer applicable for campaign] [the specified run]; otherwise, <c>false</c>.</returns>
        private static bool IsRightSizerApplicableForCampaign(Run run, Campaign campaign)
        {
            var lookupCampaignRunProcessesSettings =
                run.CampaignsProcessesSettings.FirstOrDefault(s => s.ExternalId == campaign.ExternalId);

            return lookupCampaignRunProcessesSettings?.IncludeRightSizer ?? campaign.IncludeRightSizer;
        }

        /// <summary>Resolves the right sizer level for campaign.</summary>
        /// <param name="run">The run.</param>
        /// <param name="campaign">The campaign.</param>
        /// <returns></returns>
        private static RightSizerLevel? ResolveRightSizerLevelForCampaign(Run run, Campaign campaign)
        {
            var lookupCampaignRunProcessesSettings =
                run.CampaignsProcessesSettings.FirstOrDefault(s => s.ExternalId == campaign.ExternalId);

            return lookupCampaignRunProcessesSettings != null
                ? lookupCampaignRunProcessesSettings.RightSizerLevel
                : campaign.RightSizerLevel;
        }
    }
}
