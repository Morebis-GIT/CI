using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects.AgCampaigns;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.DayParts.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.LengthFactors;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRGlobalSettings.Objects;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRSettings.Objects;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSGlobalSettings.Objects;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSSettings.Objects;
using ImagineCommunications.GamePlan.Domain.Passes;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositionGroups.Objects;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositions;
using ImagineCommunications.GamePlan.Domain.ProgrammeCategory;
using ImagineCommunications.GamePlan.Domain.ProgrammeDictionaries;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Runs.Settings;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.System.Models;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Domain.Shared.Universes;
using ImagineCommunications.GamePlan.Domain.SpotBookingRules;
using ImagineCommunications.GamePlan.Domain.TotalRatings;
using xggameplan.common.Extensions;
using xggameplan.core.Extensions;
using xggameplan.core.FeatureManagement.Interfaces;
using xggameplan.Extensions;
using xggameplan.model.AutoGen.AgCampaignBreakRequirements;
using xggameplan.model.AutoGen.AgCampaignInclusionList;
using xggameplan.model.AutoGen.AgLengthFactors;
using xggameplan.model.AutoGen.AgSpotBookingRules;
using xggameplan.model.AutoGen.AgStandardDayPartGroups;
using xggameplan.model.AutoGen.AgStandardDayParts;
using xggameplan.Model.AutoGen;

namespace xggameplan.AutoGen.AgDataPopulation
{
    /// <summary>
    /// Extension method for ag xml data population
    /// </summary>
    public static class AgDataPopulation
    {
        public static AgISRParamsSerialization ToAgISRParams(this List<SalesArea> salesAreas, Run run,
            List<Demographic> demographics, string excludeBreakType, List<ISRSettings> isrSettingsList,
            ISRGlobalSettings isrGlobalSettings, in AgISRTimeBand agISRTimeBand)
        {
            var agISRParams = new List<AgISRParam>();

            foreach (var isrSettings in isrSettingsList)
            {
                var salesArea = salesAreas.First(sa => sa.Name == isrSettings.SalesArea);
                var agISRParam = new AgISRParam()
                {
                    Description = string.Format("ISR Description for {0}", salesArea.Name),
                    BreakType = String.IsNullOrEmpty(isrSettings.BreakType)
                        ? "00"
                        : isrSettings.BreakType.Substring(0, 2),
                    StartDate = AgConversions.ToAgDateYYYYMMDDAsString(run.StartDate),
                    EndDate = AgConversions.ToAgDateYYYYMMDDAsString(run.EndDate),
                    EfficiencyThreshold = isrSettings.DefaultEfficiencyThreshold,
                    ExcludePremiumBreaks = AgConversions.ToAgBooleanAs1or0(
                        !String.IsNullOrEmpty(excludeBreakType) &&
                        excludeBreakType.ToUpper().Equals("PREMIUM")), // Doesn't follow standard Y/N
                    ExcludePublicHolidays =
                        AgConversions.ToAgBooleanAs1or0(isrSettings
                            .ExcludePublicHolidays), // Doesn't follow standard Y/N
                    ExcludeSchoolHolidays =
                        AgConversions.ToAgBooleanAs1or0(isrSettings
                            .ExcludeSchoolHolidays), // Doesn't follow standard Y/N
                    ExcludeProgrammeSpots = isrGlobalSettings != null &&
                                            isrGlobalSettings.ExcludeSpotsBookedByProgrammeRequirements ? 1 : 0
                };

                agISRParam.AgISRSalesAreas = new AgISRSalesAreas();
                agISRParam.AgISRSalesAreas.Add(new AgISRSalesArea() { SalesAreaNo = salesArea.CustomId });
                agISRParam.NbrSalesAreas = agISRParam.AgISRSalesAreas.Count;

                // Demographics
                agISRParam.AgISRDemographics = new AgISRDemographics();
                if (isrSettings.DemographicsSettings != null && isrSettings.DemographicsSettings.Any())
                {
                    foreach (var isrDemographicSettings in isrSettings.DemographicsSettings)
                    {
                        Demographic demographic =
                            demographics.Find(d => d.ExternalRef == isrDemographicSettings.DemographicId);
                        agISRParam.AgISRDemographics.Add(new AgISRDemographic()
                        {
                            DemographicNo = demographic.Id,
                            EfficiencyThreshold = isrDemographicSettings.EfficiencyThreshold
                        });
                    }
                }

                agISRParam.NbrDemographics = agISRParam.AgISRDemographics.Count;

                var agISRTimeBandClone = agISRTimeBand.Clone();

                agISRTimeBandClone.Days = AgConversions.ToAgDaysOfWeekAsInt(isrSettings.SelectableDays);
                agISRTimeBandClone.StartTime = isrSettings.StartTime == null
                    ? AgConversions.ToAgTimeAsHHMMSS(new TimeSpan(0, 0, 0))
                    : AgConversions.ToAgTimeAsHHMMSS((TimeSpan)isrSettings.StartTime);
                agISRTimeBandClone.EndTime = isrSettings.EndTime == null
                    ? AgConversions.ToAgTimeAsHHMMSS(new TimeSpan(23, 59, 59))
                    : AgConversions.ToAgTimeAsHHMMSS((TimeSpan)isrSettings.EndTime);

                // Time bands
                agISRParam.AgISRTimeBands = new AgISRTimeBands();
                agISRParam.AgISRTimeBands.Add(agISRTimeBandClone);
                agISRParam.NbrTimesBands = agISRParam.AgISRTimeBands.Count;

                agISRParams.Add(agISRParam);
            }

            var serializationobj = new AgISRParamsSerialization();
            AgISRParamsSerialization result = serializationobj.MapFrom(agISRParams);
            return result;
        }

        public static AgRSParamsSerialization ToAgRSParams(this List<SalesArea> salesAreas, List<Demographic> demographics,
            List<RSSettings> rsSettingsList, RSGlobalSettings rsGlobalSettings)
        {
            var agRSParams = new List<AgRSParam>();

            foreach (var rsSettings in rsSettingsList)
            {
                var salesArea = salesAreas.First(sa => sa.Name == rsSettings.SalesArea);
                var agRSParam = new AgRSParam()
                {
                    SalesAreaNo = salesArea.CustomId,
                    ExcludeProgrammeSpots = rsGlobalSettings != null && rsGlobalSettings.ExcludeSpotsBookedByProgrammeRequirements ? 1 : 0
                };

                // Default delivery settings
                agRSParam.AgRSDeliverySettingsList = new AgRSDeliverySettingsList();
                foreach (var agRSDeliverySettings in rsSettings.DefaultDeliverySettingsList.Select(rsDeliverySettings => new AgRSDeliverySettings()
                {
                    DaysToCampaignEnd = rsDeliverySettings.DaysToCampaignEnd,
                    LowerLimitOfOverDelivery = rsDeliverySettings.LowerLimitOfOverDelivery,
                    UpperLimitOfOverDelivery = rsDeliverySettings.UpperLimitOfOverDelivery
                }))
                {
                    agRSParam.AgRSDeliverySettingsList.Add(agRSDeliverySettings);
                }

                // Demographics
                agRSParam.AgRSDemographics = new AgRSDemographics();
                if (rsSettings.DemographicsSettings != null && rsSettings.DemographicsSettings.Any())
                {
                    foreach (var rsDemographicSettings in rsSettings.DemographicsSettings)
                    {
                        Demographic demographic =
                            demographics.Find(d => d.ExternalRef == rsDemographicSettings.DemographicId);

                        AgRSDemographic agRSDemographic = new AgRSDemographic()
                        {
                            DemographicNo = demographic.Id
                        };
                        agRSDemographic.AgRSDeliverySettingsList = new AgRSDeliverySettingsList();

                        foreach (var rsDeliverySettings in rsDemographicSettings.DeliverySettingsList)
                        {
                            AgRSDeliverySettings agRSDeliverySettings = new AgRSDeliverySettings()
                            {
                                DaysToCampaignEnd = rsDeliverySettings.DaysToCampaignEnd,
                                LowerLimitOfOverDelivery = rsDeliverySettings.LowerLimitOfOverDelivery,
                                UpperLimitOfOverDelivery = rsDeliverySettings.UpperLimitOfOverDelivery
                            };
                            agRSDemographic.AgRSDeliverySettingsList.Add(agRSDeliverySettings);
                        }

                        agRSDemographic.NbrDeliverySettings = agRSDemographic.AgRSDeliverySettingsList.Count;
                        agRSParam.AgRSDemographics.Add(agRSDemographic);
                    }
                }

                agRSParam.NbrRules = agRSParam.AgRSDeliverySettingsList.Count;
                agRSParam.NbrDemographics = agRSParam.AgRSDemographics.Count;
                agRSParams.Add(agRSParam);
            }

            var serializationobj = new AgRSParamsSerialization();

            return serializationobj.MapFrom(agRSParams);
        }

        /// <summary>
        /// Convert program into Auto gen program model
        /// </summary>
        /// <param name="programmes">program list</param>
        /// <param name="salesAreas">sales area</param>
        /// <param name="programmeDictionaries"></param>
        /// <param name="startDayOfWeek">StartDayOfWeek</param>
        /// <param name="mapper">Auto mapper</param>
        /// <returns></returns>
        public static AgProgrammesSerialization ToAgProgramme(
            this List<Programme> programmes,
            List<SalesArea> salesAreas,
            IReadOnlyCollection<ProgrammeDictionary> programmeDictionaries,
            DayOfWeek startDayOfWeek,
            IMapper mapper)
        {
            var agprogram = mapper
                .Map<List<AgProgramme>>(Tuple.Create(programmes, salesAreas, programmeDictionaries, startDayOfWeek))
                .ToList();

            var serialization = new AgProgrammesSerialization();
            return serialization.MapFrom(agprogram);
        }

        public static AgRestrictionsSerialisation ToAgRestriction(this List<Restriction> restrictions,
            IReadOnlyCollection<ProgrammeDictionary> programmeDictionaries,
            List<ProgrammeCategoryHierarchy> programmeCategories,
            List<SalesArea> salesAreas,
            DateTime runEndDate,
            List<Clash> allClashes,
            IMapper mapper,
            AgRestriction agRestriction)
        {
            var agRestrictions =
            mapper.Map<List<AgRestriction>>(Tuple.Create(restrictions, programmeCategories, programmeDictionaries, salesAreas, runEndDate, allClashes, agRestriction));
            var serialization = new AgRestrictionsSerialisation();
            return serialization.MapFrom(agRestrictions);
        }

        public static AgWeeksSerialization ToAgWeek(this Dictionary<DateTime, DateTime> dates, IMapper mapper)
        {
            var agweek = mapper.Map<List<AgWeek>>(dates);
            var serialization = new AgWeeksSerialization();
            return serialization.MapFrom(agweek);
        }

        public static AgCampaignInclusionListSerialization ToAgCampaignInclusionsList(this List<AgCampaignInclusion> agCampaignInclusion)
        {
            var serializer = new AgCampaignInclusionListSerialization();
            return serializer.MapFrom(agCampaignInclusion);
        }

        public static AgChannelGroupsSerialization ToAgChannelGroup(
            this IReadOnlyCollection<Tuple<int, int, SalesAreaGroup>> channelGroup, List<SalesArea> salesAreas, IMapper mapper)
        {
            List<AgChannelGroup> agChannelGroups = null;
            try
            {
                agChannelGroups = mapper.Map<List<AgChannelGroup>>(Tuple.Create(channelGroup, salesAreas.ToList()));
                var serialization = new AgChannelGroupsSerialization();
                return serialization.MapFrom(agChannelGroups);
            }
            finally
            {
                agChannelGroups.DisposeAll();
            }
        }

        public static AgTimeRestrictionsSerialisation ToAgTimeRestriction(this List<Campaign> campaigns,
            List<SalesArea> salesAreas, IMapper mapper, out int totalTimeRestrictions)
        {
            var campaignsWithRestrictions = campaigns
                .Where(c => c.TimeRestrictions != null && c.TimeRestrictions.Count > 0)
                .ToList();

            totalTimeRestrictions = campaignsWithRestrictions.Count;

            if (totalTimeRestrictions == 0)
            {
                return null;
            }

            var agTimeRestrictions = campaignsWithRestrictions
                .SelectMany(campaign => campaign
                    .TimeRestrictions
                    .SelectMany(t => mapper.Map<List<AgTimeRestriction>>(Tuple.Create(t, campaign.CustomId, salesAreas))))
                .ToList();

            var serialization = new AgTimeRestrictionsSerialisation();
            return serialization.MapFrom(agTimeRestrictions);
        }

        public static AgBookingPositionGroupsSerialization ToAgBookingPositionGroups(this List<Campaign> campaigns,
            List<BookingPositionGroup> allPositionGroups, List<BookingPosition> allBookingPositions,
            List<SalesArea> allSalesAreas, IMapper mapper, out int totalPositionGroups)
        {
            var campaignsWithPositionGroups = campaigns
                .Where(c => c.BookingPositionGroups != null && c.BookingPositionGroups.Any()).ToList();

            totalPositionGroups = campaignsWithPositionGroups.Sum(c => c.BookingPositionGroups.Count);
            if (totalPositionGroups == 0 || allPositionGroups?.Count == 0)
            {
                return null;
            }

            var serialization = new AgBookingPositionGroupsSerialization();
            var bookingPositionsDictionary = allBookingPositions.ToDictionary(p => p.Position, p => p);
            var positionGroupsDictionary = allPositionGroups.ToDictionary(g => g.GroupId, g => g);

            var agBookingPositionGroups = campaignsWithPositionGroups.SelectMany(campaign =>
            {
                return campaign.BookingPositionGroups.SelectMany(g =>
                {
                    if (!positionGroupsDictionary.ContainsKey(g.GroupId))
                    {
                        throw new Exception($"Error getting Booking Position Group, GroupId:{g.GroupId} CampaignNo:{campaign.CustomId}");
                    }
                    var positionGroup = positionGroupsDictionary[g.GroupId];
                    var positionGroupAssociations = new List<AgPositionGroupAssociation>();

                    foreach (var association in positionGroup.PositionGroupAssociations)
                    {
                        if (!bookingPositionsDictionary.ContainsKey(association.BookingPosition))
                        {
                            throw new Exception(
                                $"Error getting Booking Position, BookingPosition:{association.BookingPosition} GroupId:{g.GroupId} CampaignNo:{campaign.CustomId}");
                        }
                        positionGroupAssociations.Add(mapper.Map<AgPositionGroupAssociation>((association,
                            bookingPositionsDictionary[association.BookingPosition])));
                    }

                    return mapper.Map<List<AgBookingPositionGroup>>((g, campaign.CustomId, positionGroupAssociations, allSalesAreas));
                });
            }).ToList();

            return serialization.MapFrom(agBookingPositionGroups);
        }

        public static AgProgRestrictionsSerialisation ToAgProgrammesSerialization(
            this List<Campaign> campaigns,
            List<SalesArea> salesAreas,
            List<ProgrammeCategoryHierarchy> programmeCategories,
            List<ProgrammeEpisode> programmeEpisodes,
            IReadOnlyCollection<ProgrammeDictionary> programmeDictionaries,
            AgProgRestriction agProgRestriction)
        {
            /**
             * 0xCAFEF00D : Move this into another class
            **/
            List<AgProgRestriction> MapProgrammeCategory(
                ProgrammeRestriction programmeRestriction,
                int campaignCustomid,
                AgProgRestriction agProgRestriction)
            {
                List<SalesArea> mappedSalesAreas;
                if (programmeRestriction.SalesAreas == null || programmeRestriction.SalesAreas.Count == 0)
                {
                    mappedSalesAreas = salesAreas;
                }
                else
                {
                    mappedSalesAreas = salesAreas.Where(s => programmeRestriction.SalesAreas.Contains(s.Name)).ToList();
                }

                var categories = programmeCategories
                    .Where(c => programmeRestriction.CategoryOrProgramme.Any(p => p.Equals(c.Name, StringComparison.OrdinalIgnoreCase)))
                    ?.ToList();

                if (categories.Count == 0)
                {
                    return null;
                }

                return mappedSalesAreas
                    .SelectMany(salesArea =>
                    {
                        return categories.Select(category =>
                        {
                            var agProgRestrictionClone = agProgRestriction.Clone();

                            agProgRestrictionClone.CampaignNo = campaignCustomid;
                            agProgRestrictionClone.SalesAreaNo = salesArea?.CustomId ?? 0;
                            agProgRestrictionClone.PrgcNo = category.Id;
                            agProgRestrictionClone.IncludeExcludeFlag = programmeRestriction.IsIncludeOrExclude?.ToUpperInvariant();

                            return agProgRestrictionClone;
                        }).ToList();
                    }).ToList();
            }

            /**
             * 0xCAFEF00D : Move this into another class
            **/

            List<AgProgRestriction> MapProgrammeDictionary(
                ProgrammeRestriction programmeRestriction,
                int campaignCustomid,
                AgProgRestriction agProgRestriction,
                List<ProgrammeEpisode> programmeEpisodes)
            {
                var programmes = programmeDictionaries
                    .Where(c => programmeRestriction.CategoryOrProgramme.Any(p => p.Equals(c.ExternalReference.ToString(), StringComparison.OrdinalIgnoreCase)))
                    ?.ToList();

                if (programmes.Count == 0)
                {
                    return null;
                }

                return salesAreas
                    .SelectMany(salesArea =>
                    {
                        return programmes.SelectMany(program =>
                        {
                            var episodes = programmeEpisodes.Where(e => e.ProgrammeExternalReference == program.ExternalReference);

                            return episodes.DefaultIfEmpty().Select(ep =>
                                {
                                    var agProgRestrictionClone = agProgRestriction.Clone();

                                    agProgRestrictionClone.CampaignNo = campaignCustomid;
                                    agProgRestrictionClone.SalesAreaNo = salesArea?.CustomId ?? 0;
                                    agProgRestrictionClone.ProgNo = program.Id;
                                    agProgRestrictionClone.IncludeExcludeFlag = programmeRestriction.IsIncludeOrExclude?.ToUpperInvariant();
                                    agProgRestrictionClone.EpisNo = ep?.Number ?? 0;

                                    return agProgRestrictionClone;
                                }).ToList();
                        }).ToList();
                    }).ToList();
            }

            var agProgrammeRestrictions = campaigns
                .SelectMany(campaign =>
                {
                    return campaign
                        .ProgrammeRestrictions
                        .SelectMany(programmeRestriction =>
                            programmeRestriction.IsCategoryOrProgramme.Equals(CategoryOrProgramme.C.ToString(), StringComparison.OrdinalIgnoreCase)
                                ? MapProgrammeCategory(programmeRestriction, campaign.CustomId, agProgRestriction)
                                : MapProgrammeDictionary(programmeRestriction, campaign.CustomId, agProgRestriction, programmeEpisodes)
                        )
                        .ToList();
                })
                .ToList();

            if (agProgrammeRestrictions.Any())
            {
                var serialization = new AgProgRestrictionsSerialisation();
                return serialization.MapFrom(agProgrammeRestrictions);
            }

            return null;
        }

        public static AgWeightingsSerialization ToAgWeighting(this List<Pass> passes, Scenario scenario)
        {
            var agWeightings = new List<AgWeighting>();

            // TODO: Optimize this
            foreach (Pass pass in passes)
            {
                foreach (Weighting weighting in pass.Weightings)
                {
                    agWeightings.Add(new AgWeighting()
                    {
                        ScenarioNo = scenario.CustomId,
                        PassNo = pass.Id,
                        Value = Convert.ToInt32(weighting.Value),
                        WeightingNo = weighting.RuleId
                    });
                }
            }

            var serializationobj = new AgWeightingsSerialization();

            return serializationobj.MapFrom(agWeightings);
        }

        public static AgTolerancesSerialization ToAgTolerance(this List<Pass> passes, Scenario scenario, IFeatureManager featureManager)
        {
            var agTolerances = new List<AgTolerance>();
            var targetSalesAreaEnabled = featureManager.IsEnabled(nameof(ProductFeature.TargetSalesArea));

            // TODO: Optimize this
            foreach (Pass pass in passes)
            {
                foreach (Tolerance tolerance in pass.Tolerances)
                {
                    var agTolerance = new AgTolerance
                    {
                        ScenarioNo = scenario.CustomId,
                        ToleranceNo = tolerance.RuleId,
                        PassNo = pass.Id,
                        MaxPercent = tolerance.Over,
                        MinimumPercent = tolerance.Under,
                        IgnoreYn = AgConversions.ToAgBooleanAsString(tolerance.Ignore),
                        BookTargetAreaYn = AgConversions.ToAgBooleanAsString(targetSalesAreaEnabled && tolerance.BookTargetArea)
                    };

                    switch (tolerance.ForceOverUnder)
                    {
                        case ForceOverUnder.None:
                            agTolerance.BookUntilMaxYn = AgConversions.ToAgBooleanAsString(false);
                            agTolerance.BookUntilMinYn = AgConversions.ToAgBooleanAsString(false);
                            break;

                        case ForceOverUnder.Over:
                            agTolerance.BookUntilMaxYn = AgConversions.ToAgBooleanAsString(true);
                            agTolerance.BookUntilMinYn = AgConversions.ToAgBooleanAsString(false);
                            break;

                        case ForceOverUnder.Under:
                            agTolerance.BookUntilMaxYn = AgConversions.ToAgBooleanAsString(false);
                            agTolerance.BookUntilMinYn = AgConversions.ToAgBooleanAsString(true);
                            break;
                    }

                    agTolerances.Add(agTolerance);
                }
            }

            var serializationobj = new AgTolerancesSerialization();

            return serializationobj.MapFrom(agTolerances);
        }

        public static AgRulesSerialization ToAgRule(this List<Pass> passes, Scenario scenario)
        {
            var agRules = new List<AgRule>();

            // TODO: Optimize this
            foreach (Pass pass in passes)
            {
                var rules = pass.Rules.Where(r => r.InternalType.ToUpper() == "SLOTTING CONTROLS").ToList();

                foreach (PassRule rule in rules)
                {
                    double value = -1;
                    double peakValue = -1;
                    double offPeakValue = -1;

                    if (rule.PeakValue != null)
                    {
                        peakValue = Convert.ToDouble(rule.PeakValue);
                        offPeakValue = Convert.ToDouble(rule.Value);
                    }
                    else
                    {
                        value = Convert.ToDouble(rule.Value);
                    }

                    agRules.Add(new AgRule()
                    {
                        ScenarioNo = scenario.CustomId,
                        SlottingControlNo = rule.RuleId,        // TODO: Check this
                        PassNo = pass.Id,
                        Value = value,
                        PeakValue = peakValue,
                        OffPeakValue = offPeakValue,
                        Ignore = AgConversions.ToAgBooleanAs1or0(rule.Ignore)
                    });
                }
            }

            var serializationobj = new AgRulesSerialization();
            return serializationobj.MapFrom(agRules);
        }

        /// <summary>
        /// Populate Pass Defaults Data From Run Or Pass Level
        /// </summary>
        /// <param name="passes">Pass List</param>
        /// <param name="scenario">Scenario</param>
        /// <param name="run">Run</param>
        /// <param name="tenantSettings">Tenant settings</param>
        /// <returns></returns>
        public static AgPassDefaultsSerialization ToAgPassDefault(this List<Pass> passes, Scenario scenario, Run run, TenantSettings tenantSettings)
        {
            var agPassDefaults = new List<AgPassDefault>();
            var startDate = new DateTime();
            var endDate = new DateTime();
            var startTime = new TimeSpan();
            var endTime = new TimeSpan();

            const string defaultDaysOfWeek = "1111111";

            foreach (Pass pass in passes)
            {
                var passSalesAreaPriorities = pass.PassSalesAreaPriorities;

                startDate = (DateTime)(passSalesAreaPriorities?.StartDate != null ? passSalesAreaPriorities.StartDate : run.StartDate);

                endDate = (DateTime)(passSalesAreaPriorities?.EndDate != null ? passSalesAreaPriorities.EndDate : run.EndDate);

                startTime = (TimeSpan)(passSalesAreaPriorities?.StartTime != null ? passSalesAreaPriorities.StartTime : run.StartTime);

                endTime = (TimeSpan)(passSalesAreaPriorities?.EndTime != null ? passSalesAreaPriorities.EndTime : run.EndTime);

                var timeSlices = passSalesAreaPriorities != null ? GeneratePassTimeSlices(passSalesAreaPriorities, tenantSettings) : null;

                var timeSlicesCount = timeSlices?.Count ?? 0;

                agPassDefaults.Add(new AgPassDefault()
                {
                    ScenarioNo = scenario.CustomId,
                    PassNo = pass.Id,
                    PassSeqNo = pass.Id,
                    StartDate = AgConversions.ToAgDateYYYYMMDDAsString(startDate),
                    EndDate = AgConversions.ToAgDateYYYYMMDDAsString(endDate),
                    StartTime = AgConversions.ToAgTimeAsHHMMSS(startTime),
                    EndTime = AgConversions.IsEndOfBroadCastingDay(endTime) ? AgConversions.broadCastDayValue : AgConversions.ToAgTimeAsHHMMSS(endTime),
                    DaysOfWeek = AgConversions.ToAgDaysAsInt(passSalesAreaPriorities?.DaysOfWeek ?? defaultDaysOfWeek),
                    ScenarioDescription = string.Format("Scenario {0}", scenario.Id),     // Scenario Description
                    PassDescription = string.Format("Pass {0}", pass.Id),                 // Pass Description
                    TimeSlicesCount = timeSlicesCount,
                    TimeSliceList = timeSlices
                });
            }

            var serializationobj = new AgPassDefaultsSerialization();
            return serializationobj.MapFrom(agPassDefaults);
        }

        /// <summary>
        /// Generate Pass time slices for off-peak, peak and midnight times
        /// </summary>
        /// <param name="passSalesAreaPriorities">Sale sArea Priorities</param>
        /// <param name="tenantSettings">Tenant settings</param>
        /// <returns></returns>
        private static AgPassTimeSliceList GeneratePassTimeSlices(PassSalesAreaPriority passSalesAreaPriorities, TenantSettings tenantSettings)
        {
            if (string.IsNullOrWhiteSpace(tenantSettings.PeakStartTime)
            || string.IsNullOrWhiteSpace(tenantSettings.PeakEndTime))
            {
                throw new ArgumentNullException(nameof(tenantSettings), "Peak daypart is not set. Please check the tenant settings.");
            }

            if (string.IsNullOrWhiteSpace(tenantSettings.MidnightStartTime)
            || string.IsNullOrWhiteSpace(tenantSettings.MidnightEndTime))
            {
                throw new ArgumentNullException(nameof(tenantSettings), "Midnight daypart is not set. Please check the tenant settings.");
            }

            var timeSlices = new List<TimeRange>();

            if (passSalesAreaPriorities.IsOffPeakTime)
            {
                var offPeakStartTime = new TimeSpan(0, 0, 0);
                var offPeakEndTime = new TimeSpan(99, 59, 59);

                timeSlices.Add(new TimeRange(offPeakStartTime, offPeakEndTime));

                AddOrSplitPassTimeSlice(timeSlices, tenantSettings.PeakStartTime, tenantSettings.PeakEndTime, false);

                AddOrSplitPassTimeSlice(timeSlices, tenantSettings.MidnightStartTime, tenantSettings.MidnightEndTime, false);
            }

            if (passSalesAreaPriorities.IsPeakTime)
            {
                AddOrSplitPassTimeSlice(timeSlices, tenantSettings.PeakStartTime, tenantSettings.PeakEndTime);
            }

            if (passSalesAreaPriorities.IsMidnightTime)
            {
                AddOrSplitPassTimeSlice(timeSlices, tenantSettings.MidnightStartTime, tenantSettings.MidnightEndTime);
            }

            var agPassTimeSlices = new AgPassTimeSliceList();

            foreach (var timeSlice in timeSlices.OrderBy(t => t.StartTime))
            {
                agPassTimeSlices.Add(ToAgPassTimeSlice(timeSlice));
            }

            return agPassTimeSlices;
        }

        /// <summary>
        /// Populate Rating Points Data From Pass Level
        /// </summary>
        /// <param name="passes">Scenario passes</param>
        /// <param name="allSalesAreas">All sales areas</param>
        /// <param name="tenantSettings">Tenant settings</param>
        /// <returns></returns>
        public static AgRatingPointsSerialization ToAgRatingPoints(this List<Pass> passes, List<SalesArea> allSalesAreas, TenantSettings tenantSettings)
        {
            if (string.IsNullOrWhiteSpace(tenantSettings.PeakStartTime)
            || string.IsNullOrWhiteSpace(tenantSettings.PeakEndTime))
            {
                throw new ArgumentNullException(nameof(tenantSettings), "Peak daypart is not set. Please check the tenant settings.");
            }

            if (string.IsNullOrWhiteSpace(tenantSettings.MidnightStartTime)
            || string.IsNullOrWhiteSpace(tenantSettings.MidnightEndTime))
            {
                throw new ArgumentNullException(nameof(tenantSettings), "Midnight daypart is not set. Please check the tenant settings.");
            }

            var agRatingPoints = new List<AgRatingPoint>();

            var offPeakStartTime = new TimeSpan(0, 0, 0);
            var offPeakEndTime = new TimeSpan(99, 59, 59);

            var offPeakDayparts = new List<TimeRange> { new TimeRange(offPeakStartTime, offPeakEndTime) };

            AddOrSplitPassTimeSlice(offPeakDayparts, tenantSettings.PeakStartTime, tenantSettings.PeakEndTime, false);

            AddOrSplitPassTimeSlice(offPeakDayparts, tenantSettings.MidnightStartTime, tenantSettings.MidnightEndTime, false);

            foreach (var pass in passes)
            {
                if ((pass.RatingPoints != null && pass.RatingPoints.Any()) == false)
                {
                    continue;
                }

                foreach (var ratingPoint in pass.RatingPoints)
                {
                    var mappedSalesAreas = new List<SalesArea>();

                    if (ratingPoint.SalesAreas == null || ratingPoint.SalesAreas.Any() == false)
                    {
                        mappedSalesAreas = allSalesAreas;
                    }
                    else
                    {
                        mappedSalesAreas = allSalesAreas.Where(s => ratingPoint.SalesAreas.Contains(s.Name)).ToList();
                    }

                    foreach (var salesArea in mappedSalesAreas)
                    {
                        var offPeakValue = ratingPoint.OffPeakValue.HasValue ? ratingPoint.OffPeakValue
                            : pass.RatingPoints.FirstOrDefault(t => t.SalesAreas.Contains(salesArea.Name) && t.OffPeakValue.HasValue && t != ratingPoint)?.OffPeakValue;

                        var peakValue = ratingPoint.PeakValue.HasValue ? ratingPoint.PeakValue
                            : pass.RatingPoints.FirstOrDefault(t => t.SalesAreas.Contains(salesArea.Name) && t.PeakValue.HasValue && t != ratingPoint)?.PeakValue;

                        var midnightToDawnValue = ratingPoint.MidnightToDawnValue.HasValue ? ratingPoint.MidnightToDawnValue
                            : pass.RatingPoints.FirstOrDefault(t => t.SalesAreas.Contains(salesArea.Name) && t.MidnightToDawnValue.HasValue && t != ratingPoint)?.MidnightToDawnValue;

                        var ratingPointsForDayparts = GenerateRatingPointsForDayparts(offPeakValue, peakValue, midnightToDawnValue, offPeakDayparts, tenantSettings);

                        var ratingPointsForDaypartsCount = ratingPointsForDayparts.Count;

                        agRatingPoints.Add(new AgRatingPoint
                        {
                            PassId = pass.Id,
                            SalesAreaNumber = salesArea.CustomId,
                            RatingPointsForDaypartsList = ratingPointsForDayparts,
                            DaypartsCount = ratingPointsForDaypartsCount
                        });
                    }
                }
            }

            var serializationobj = new AgRatingPointsSerialization();
            return serializationobj.MapFrom(agRatingPoints);
        }

        /// <summary>
        /// Generate Rating Points For off-peak, peak and midnight dayparts
        /// </summary>
        /// <param name="offPeakRatingPoint">Off-Peak Rating Point value</param>
        /// <param name="peakRatingPoint">Peak Rating Point value</param>
        /// <param name="midnightToDawnRatingPoint">
        /// Midnight to dawn Rating Point value
        /// </param>
        /// <param name="offPeakDayparts">Off-Peak Dayparts</param>
        /// <param name="tenantSettings">Tenant settings</param>
        /// <returns></returns>
        private static AgRatingPointsForDaypartsList GenerateRatingPointsForDayparts(double? offPeakRatingPoint, double? peakRatingPoint, double? midnightToDawnRatingPoint,
            List<TimeRange> offPeakDayparts, TenantSettings tenantSettings)
        {
            var ratingPointsForDayparts = new AgRatingPointsForDaypartsList();

            if (offPeakRatingPoint.HasValue)
            {
                foreach (var offPeakDaypart in offPeakDayparts)
                {
                    ratingPointsForDayparts.Add(new AgRatingPointForDaypart
                    {
                        StartTime = AgConversions.ToAgTimeAsTotalHHMMSS(offPeakDaypart.StartTime),
                        EndTime = AgConversions.ToAgTimeAsTotalHHMMSS(offPeakDaypart.EndTime),
                        Value = offPeakRatingPoint.Value
                    });
                }
            }

            if (peakRatingPoint.HasValue)
            {
                ratingPointsForDayparts.Add(new AgRatingPointForDaypart
                {
                    StartTime = tenantSettings.PeakStartTime,
                    EndTime = tenantSettings.PeakEndTime,
                    Value = peakRatingPoint.Value
                });
            }

            if (midnightToDawnRatingPoint.HasValue)
            {
                ratingPointsForDayparts.Add(new AgRatingPointForDaypart
                {
                    StartTime = tenantSettings.MidnightStartTime,
                    EndTime = tenantSettings.MidnightEndTime,
                    Value = midnightToDawnRatingPoint.Value
                });
            }

            return ratingPointsForDayparts;
        }

        /// <summary>
        /// Convert time slice to Ag time slice
        /// </summary>
        /// <param name="timeSlice">Time slice with TimeSpans</param>
        /// <returns></returns>
        private static AgPassTimeSlice ToAgPassTimeSlice(TimeRange timeSlice)
        {
            return new AgPassTimeSlice
            {
                StartTime = AgConversions.ToAgTimeAsTotalHHMMSS(timeSlice.StartTime),
                EndTime = AgConversions.ToAgTimeAsTotalHHMMSS(timeSlice.EndTime)
            };
        }

        /// <summary>
        /// Add time slice to the list, handling overlaps, or split existing
        /// time slice, excluding time range from it
        /// </summary>
        /// <param name="timeSlices">Time slices with TimeSpans</param>
        /// <param name="agStartTime">ag start time</param>
        /// <param name="AgEndTime">ag end time</param>
        /// <returns></returns>
        private static void AddOrSplitPassTimeSlice(List<TimeRange> timeSlices, string agStartTime, string agEndTime, bool addTimeSlice = true)
        {
            var startTime = AgConversions.ParseTotalHHMMSSFormat(agStartTime);
            var endTime = AgConversions.ParseTotalHHMMSSFormat(agEndTime);

            if (timeSlices.Any())
            {
                var overlappedTimeSlice = timeSlices.FirstOrDefault(t => t.StartTime <= startTime && t.EndTime > startTime ||
                t.StartTime > startTime && t.StartTime <= endTime);

                if (!overlappedTimeSlice.Equals(default(TimeRange)))
                {
                    if (startTime > overlappedTimeSlice.StartTime)
                    {
                        timeSlices.Add(new TimeRange(overlappedTimeSlice.StartTime, startTime.Add(TimeSpan.FromSeconds(-1))));
                    }

                    if (endTime < overlappedTimeSlice.EndTime)
                    {
                        timeSlices.Add(new TimeRange(endTime.Add(TimeSpan.FromSeconds(1)), overlappedTimeSlice.EndTime));
                    }

                    timeSlices.Remove(overlappedTimeSlice);
                }
            }

            if (addTimeSlice)
            {
                timeSlices.Add(new TimeRange(startTime, endTime));
            }
        }

        /// <summary>
        /// Populate Sales Area Pass Priority Data From Run Or Pass Level
        /// </summary>
        /// <param name="passes">Pass List</param>
        /// <param name="scenario">Scenario</param>
        /// <param name="salesAreas">Sales areas</param>
        /// <param name="run">Run</param>
        /// <param name="mapper">Auto mapper</param>
        /// <returns></returns>
        public static AgSalesAreaPassPrioritySerialization ToAgSalesAreaPassPriority(this List<Pass> passes, Scenario scenario, List<SalesArea> salesAreas, Run run, IMapper mapper)
        {
            var agSalesAreaPassPriorities = new List<AgSalesAreaPassPriority>();

            if (passes?.Any() == true && scenario.Passes != null)
            {
                var salesAreaNo = 0;
                var tempSalesAreaPriorityList = new List<SalesAreaPriority>();
                var passSalesAreaPriority = new PassSalesAreaPriority();

                foreach (var passReference in scenario.Passes)
                {
                    passSalesAreaPriority = passes.FirstOrDefault(p => p.Id == passReference.Id).PassSalesAreaPriorities;

                    //if there is no sales area pass priority data in pass level, get this data from run level
                    if (passSalesAreaPriority?.SalesAreaPriorities?.Any() == true)
                    {
                        //Pass Level
                        tempSalesAreaPriorityList = passSalesAreaPriority.SalesAreaPriorities.Select(a => new SalesAreaPriority
                        {
                            Priority = a.Priority,
                            SalesArea = a.SalesArea
                        }).ToList();
                    }
                    else
                    {
                        //Run Level
                        tempSalesAreaPriorityList = run.SalesAreaPriorities;
                    }

                    foreach (var sapp in tempSalesAreaPriorityList)
                    {
                        // sales area should be in the sales area list and
                        // priority should not be exclude
                        salesAreaNo = salesAreas.FirstOrDefault(s => s.Name == sapp.SalesArea)?.CustomId ?? 0;
                        if (salesAreaNo > 0 && (int)sapp.Priority > 0)
                        {
                            agSalesAreaPassPriorities.Add(new AgSalesAreaPassPriority()
                            {
                                PassNo = passReference.Id,
                                Priority = (int)sapp.Priority,
                                SalesAreaNo = salesAreaNo
                            });
                        }
                    }
                }
            }

            var serializationobj = new AgSalesAreaPassPrioritySerialization();
            return serializationobj.MapFrom(agSalesAreaPassPriorities);
        }

        public static AgDefaultsSerialization ToAgDefault(this List<Pass> passes, Scenario scenario, IFeatureManager featureManager)
        {
            var agDefaults = new List<AgDefault>();
            var zeroRatedBreaksEnabled = featureManager.IsEnabled(nameof(ProductFeature.ZeroRatedBreaksMapping));

            foreach (Pass pass in passes)
            {
                General minimumEfficiencyRule = pass.General.Where(r => r.RuleId == RuleIDs.MinimumEfficiency).First();
                var minEfficiency = Convert.ToInt32(minimumEfficiencyRule.Value);
                General maximumEfficiencyRankRule = pass.General.Where(r => r.RuleId == RuleIDs.MaximumRank).First();
                var maxRank = Convert.ToInt32(maximumEfficiencyRankRule.Value);
                General demographicBandRule = pass.General.Where(r => r.RuleId == RuleIDs.DemographBandingTolerance).First();
                var demoBandTolerance = Convert.ToInt32(demographicBandRule.Value);
                General centreBreakRatioRule = pass.General.Where(r => r.RuleId == RuleIDs.DefaultCentreBreakRatio).First();
                var centreBreakRatio = Convert.ToInt32(centreBreakRatioRule.Value);
                General useCampaignMaxSpotRatingsRule = pass.General.Where(r => r.RuleId == RuleIDs.UseCampaignMaxSpotRatings).First();
                var useCampaignMaxSpotRatings = Convert.ToInt32(useCampaignMaxSpotRatingsRule.Value);
                General useSponsorExclusivityRule = pass.General.Where(r => r.RuleId == RuleIDs.UseSponsorExclusivity).FirstOrDefault();
                var useSponsorExclusivity = useSponsorExclusivityRule != null ? Convert.ToInt32(useSponsorExclusivityRule.Value) : 0;
                General dayPartGroup = pass.General.FirstOrDefault(r => r.RuleId == RuleIDs.DayPartGroups);
                var useDayPartGroup = dayPartGroup != null ? Convert.ToInt32(dayPartGroup.Value) : 1;

                var (evenDistributionZeroRatingSpots, zeroRatedBreaks) = GetZeroRatedBreaksRulesValues(pass, zeroRatedBreaksEnabled);

                agDefaults.Add(new AgDefault
                {
                    ScenarioNo = scenario.CustomId,
                    PassNo = pass.Id,
                    DefaultCentreBreakRatio = centreBreakRatio,
                    DemographicBandTolerance = demoBandTolerance,
                    MaximumEfficiencyRank = maxRank,
                    MinimumEfficiency = minEfficiency,
                    UseCampaignMaxSpotRatings = useCampaignMaxSpotRatings,
                    UseSponsorExclusivity = useSponsorExclusivity,
                    EvenDistributionZeroRatingSpots = evenDistributionZeroRatingSpots,
                    ZeroRatedBreaks = zeroRatedBreaks,
                    DayPartGroup = featureManager.IsEnabled(nameof(ProductFeature.DayPartGroup)) ? useDayPartGroup : 1
                });
            }

            var serializationobj = new AgDefaultsSerialization();
            return serializationobj.MapFrom(agDefaults);
        }

        private static (int evenDistributionZeroRatingSpots, int zeroRatedBreaks) GetZeroRatedBreaksRulesValues(Pass pass, bool zeroRatedBreaksEnabled)
        {
            if (!zeroRatedBreaksEnabled)
            {
                return (0, 0);
            }

            var evenDistributionZeroRatingSpotsValue = pass.General.First(r => r.RuleId == RuleIDs.EvenDistributionZeroRatingSpots).Value;
            var evenDistributionZeroRatingSpots = Convert.ToInt32(evenDistributionZeroRatingSpotsValue, CultureInfo.InvariantCulture);
            var zeroRatedBreaksRuleValue = pass.General.First(r => r.RuleId == RuleIDs.ZeroRatedBreaks).Value;
            var zeroRatedBreaks = Convert.ToInt32(zeroRatedBreaksRuleValue, CultureInfo.InvariantCulture);

            return (evenDistributionZeroRatingSpots, zeroRatedBreaks);
        }

        public static AgUniversesSerialization ToAgUniverse(this List<SalesArea> salesareas, List<Universe> universes, List<Demographic> demographics, IMapper mapper)
        {
            var agUniverses = new List<AgUniverse>();

            foreach (SalesArea salesArea in salesareas)
            {
                var salesAreaUniverses = universes.Where(u => u.SalesArea == salesArea.Name).OrderBy(u => u.StartDate).ThenBy(u => u.Demographic);
                foreach (var salesAreaUniverse in salesAreaUniverses)
                {
                    agUniverses.Add(new AgUniverse()
                    {
                        DemographicNo = Convert.ToInt32(demographics.Find(d => d.ExternalRef == salesAreaUniverse.Demographic).Id),
                        SalesAreaNo = salesArea.CustomId,
                        StartDate = AgConversions.ToAgDateYYYYMMDDAsString(salesAreaUniverse.StartDate),
                        EndDate = AgConversions.ToAgDateYYYYMMDDAsString(salesAreaUniverse.EndDate),
                        UniverseNo = salesAreaUniverse.UniverseValue
                    });
                }
            }

            var serializationobj = new AgUniversesSerialization();
            return serializationobj.MapFrom(agUniverses);
        }

        public static AgPublicHolidaysSerialization ToAgPublicHolidays(this List<SalesArea> salesareas,
            DateTime startDateTime, DateTime endDateTime)
        {
            var agPublicHolidays = (from salesArea in salesareas
                                    let holidayDates = GetDatesFromDateRanges(salesArea.PublicHolidays)?.Distinct().ToList()
                                    where holidayDates != null
                                    from publicHoliday in holidayDates.Where(
                                        d => d.Date >= startDateTime.Date && d.Date < endDateTime.Date.AddDays(1))
                                    select new AgPublicHoliday()
                                    {
                                        Date = AgConversions.ToAgDateYYYYMMDDAsString(publicHoliday),
                                        SalesAreaNo = salesArea.CustomId
                                    }).ToList();

            var serializationobj = new AgPublicHolidaysSerialization();
            return serializationobj.MapFrom(agPublicHolidays);
        }

        public static AgSchoolHolidaysSerialization ToAgSchoolHolidays(this List<SalesArea> salesareas,
            DateTime startDateTime, DateTime endDateTime)
        {
            var agSchoolHolidays = (from salesArea in salesareas
                                    let holidayDates = GetDatesFromDateRanges(salesArea.SchoolHolidays)?.Distinct().ToList()
                                    where holidayDates != null
                                    from schoolHoliday in holidayDates.Where(
                                        d => d.Date >= startDateTime.Date && d.Date < endDateTime.Date.AddDays(1))
                                    select new AgSchoolHoliday()
                                    {
                                        Date = AgConversions.ToAgDateYYYYMMDDAsString(schoolHoliday),
                                        SalesAreaNo = salesArea.CustomId
                                    }).ToList();

            var serializationobj = new AgSchoolHolidaysSerialization();
            return serializationobj.MapFrom(agSchoolHolidays);
        }

        public static AgParamsSerialization ToAgParams(this Scenario scenario, Run run, double zeroRatingValue, DateTime systemDate, IFeatureManager featureManager, double? openAirtimeFactor)
        {
            if (run.EfficiencyPeriod == EfficiencyCalculationPeriod.NumberOfWeeks && !run.NumberOfWeeks.HasValue)
            {
                throw new ArgumentException(
                    $"The {nameof(run)} argument's {nameof(run.NumberOfWeeks)} property cannot be null when {nameof(run.EfficiencyPeriod)} is Number Of Weeks.",
                    nameof(run));
            }

            var agParams = new List<AgParam>();

            const double DefaultOpenAirtimeFactor = 1.0;

            var newParamsEntry = new AgParam()
            {
                ScenarioNumber = scenario.CustomId,
                DupScenarioNumber = scenario.CustomId,
                StartDate = AgConversions.ToAgDateYYYYMMDDAsString(run.StartDate),
                EndDate = AgConversions.ToAgDateYYYYMMDDAsString(run.EndDate),
                SystemDate = AgConversions.ToAgDateYYYYMMDDAsString(systemDate),
                SpreadProgramming = AgConversions.ToAgBooleanAs1or0(run.SpreadProgramming),
                IgnoreZeroPercentageSplit = featureManager.IsEnabled(nameof(ProductFeature.SalesAreaZeroRevenueSplit)) ? AgConversions.ToAgBooleanAs1or0(run.IgnoreZeroPercentageSplit) : default,
                BookTargetArea = featureManager.IsEnabled(nameof(ProductFeature.TargetSalesArea)) ? AgConversions.ToAgBooleanAs1or0(run.BookTargetArea) : default,
                EfficiencyPeriod = AgConversions.ToAgEfficiencyPeriod(run.EfficiencyPeriod),
                NumberOfWeeks = run.NumberOfWeeks.Value,
                ZeroRatingValue = featureManager.IsEnabled(nameof(ProductFeature.ZeroRatedBreaksMapping)) ? zeroRatingValue : TenantSettings.DefaultAutoBookTargetedZeroRatedBreaks,
                OpenAirtimeFactor = featureManager.IsEnabled(nameof(ProductFeature.ScenarioPerformanceMeasurementKPIs)) && openAirtimeFactor.HasValue
                ? openAirtimeFactor.Value
                : DefaultOpenAirtimeFactor
            };

            if (featureManager.IsEnabled(nameof(ProductFeature.SkipLockedBreaks)))
            {
                newParamsEntry.SkipLockedBreaks = AgConversions.ToAgBooleanAs1or0(run.SkipLockedBreaks);
                newParamsEntry.IgnorePremiumCategoryBreaks = AgConversions.ToAgBooleanAs1or0(run.IgnorePremiumCategoryBreaks);
                newParamsEntry.ExcludeBankHolidays = AgConversions.ToAgBooleanAs1or0(run.ExcludeBankHolidays);
                newParamsEntry.ExcludeSchoolHolidays = AgConversions.ToAgBooleanAs1or0(run.ExcludeSchoolHolidays);
            }

            newParamsEntry.PositionInProgramme = run.PositionInProgramme.GetDescription();

            agParams.Add(newParamsEntry);

            var serializationobj = new AgParamsSerialization();
            return serializationobj.MapFrom(agParams);
        }

        /// <summary>
        /// Populate Campaign Priority Rounds From Scenario
        /// </summary>
        /// <param name="scenario">Scenario</param>
        /// <returns></returns>
        public static AgCampaignPriorityRoundsSerialization ToAgCampaignPriorityRound(this Scenario scenario)
        {
            var agCampaignPriorityRounds = new List<AgCampaignPriorityRound>();

            var campaignPriorityRounds = scenario.CampaignPriorityRounds;

            if (campaignPriorityRounds != null && campaignPriorityRounds.Rounds != null)
            {
                foreach (var cpr in campaignPriorityRounds.Rounds.OrderBy(r => r.Number))
                {
                    agCampaignPriorityRounds.Add(new AgCampaignPriorityRound
                    {
                        Number = cpr.Number,
                        ProcessingMode = 0,
                        PriorityFrom = cpr.PriorityFrom,
                        PriorityTo = cpr.PriorityTo,
                        Description = $"Round {cpr.Number}"
                    });
                }

                if (agCampaignPriorityRounds.Any())
                {
                    agCampaignPriorityRounds[0].IsProgrammeInclusionsRound =
                        Convert.ToInt32(campaignPriorityRounds.ContainsInclusionRound);
                }
            }

            var serializationobj = new AgCampaignPriorityRoundsSerialization();
            return serializationobj.MapFrom(agCampaignPriorityRounds);
        }

        /// <summary>
        /// Populate Campaign Pass Priorities From Scenario
        /// </summary>
        /// <param name="scenario">Scenario</param>
        /// <returns></returns>
        public static AgScenarioCampaignPassSerialization ToAgScenarioCampaignPass(this Scenario scenario)
        {
            var agScenarioCampaignPass = new List<AgScenarioCampaignPass>();

            foreach (var cpp in scenario.CampaignPassPriorities)
            {
                foreach (var pp in cpp.PassPriorities)
                {
                    //if priority is not equal exclude
                    if (pp.Priority > 0)
                    {
                        agScenarioCampaignPass.Add(new AgScenarioCampaignPass()
                        {
                            CampaignCustomNo = cpp.Campaign.CustomId,
                            Priority = pp.Priority,
                            PassNo = pp.PassId
                        });
                    }
                }
            }

            var serializationobj = new AgScenarioCampaignPassSerialization();
            return serializationobj.MapFrom(agScenarioCampaignPass);
        }

        public static AgProgrammeRepetitionsSerialization ToAgProgrammeRepetitions(this Scenario scenario, List<Pass> passes)
        {
            if (passes != null && passes.Any() &&
                passes.Any(p => p.ProgrammeRepetitions != null && p.ProgrammeRepetitions.Any()))
            {
                var agProgrammeRepetitions = new List<AgProgrammeRepetition>();
                var passes2 = passes.ToList()
                    .Where(p => p?.ProgrammeRepetitions != null && p.ProgrammeRepetitions.Any());
                agProgrammeRepetitions = passes2.ToList().SelectMany(pass =>
                {
                    return pass.ProgrammeRepetitions.Select(programmeRepetition =>
                    {
                        var factor = 0d;
                        var peakFactor = 0d;
                        var offPeakFactor = 0d;
                        if (programmeRepetition.PeakFactor != null)
                        {
                            peakFactor = (double)programmeRepetition.PeakFactor;
                            offPeakFactor = programmeRepetition.Factor;
                        }
                        else
                        {
                            factor = programmeRepetition.Factor;
                        }

                        return new AgProgrammeRepetition()
                        {
                            Minutes = programmeRepetition.Minutes,
                            PassNo = pass.Id,
                            RepetitionFactor = factor,
                            PeakRepetitionFactor = peakFactor,
                            OffPeakRepetitionFactor = offPeakFactor,
                        };
                    });
                }).ToList();

                var serializationobj = new AgProgrammeRepetitionsSerialization();
                return serializationobj.MapFrom(agProgrammeRepetitions);
            }

            return null;
        }

        public static AgSlottingLimitsSerialization ToAgSlottingLimits(this Scenario scenario, List<Pass> passes, List<Demographic> demographs)
        {
            if (passes != null && passes.Any() &&
                passes.Any(p => p.SlottingLimits != null && p.SlottingLimits.Any()))
            {
                var passes2 = passes.ToList()
                    .Where(p => p?.SlottingLimits != null && p.SlottingLimits.Any());
                var agSlottingLimits = passes2.ToList().SelectMany(pass =>
                {
                    return pass.SlottingLimits.Select(slottingLimit =>
                        new AgSlottingLimit()
                        {
                            PassNo = pass.Id,
                            BandingTolerance = slottingLimit.BandingTolerance,
                            DemographNo =
                                demographs.FirstOrDefault(d => d.ExternalRef.Equals(slottingLimit.Demographs,
                                    StringComparison.OrdinalIgnoreCase))?.Id ?? 0,
                            MaximumEfficiency = slottingLimit.MaximumEfficiency,
                            MinimumEfficiency = slottingLimit.MinimumEfficiency
                        }).ToList();
                }).ToList();

                var serializationobj = new AgSlottingLimitsSerialization();
                return serializationobj.MapFrom(agSlottingLimits);
            }

            return null;
        }

        public static AgPeakStartEndTimeSerialization ToPeakStartAndEndTime(this Scenario scenario,
            IEnumerable<SalesArea> salesAreas, string peakStartTime, string peakEndTime, AgPeakStartEndTime agPeakStartEndTime)
        {
            var agPeakStartEndTimes = new List<AgPeakStartEndTime>();

            if (AgConversions.isValidHHMMSSFormat(peakStartTime) &&
                AgConversions.isValidHHMMSSFormat(peakEndTime))
            {
                agPeakStartEndTimes = salesAreas.Select(salesArea =>
                {
                    var agPeakStartEndTimeClone = agPeakStartEndTime.Clone();

                    agPeakStartEndTimeClone.SalesArea = salesArea.CustomId;
                    agPeakStartEndTimeClone.ScenarioNumber = scenario.CustomId;
                    agPeakStartEndTimeClone.StartTimeOfDayPart = peakStartTime;
                    agPeakStartEndTimeClone.EndTimeOfDayPart = peakEndTime;

                    return agPeakStartEndTimeClone;
                }).ToList();
            }

            var serializationObj = new AgPeakStartEndTimeSerialization();
            return serializationObj.MapFrom(agPeakStartEndTimes);
        }

        public static AgBreakExclusionsSerialization ToAgBreakExclusions(this Scenario scenario, List<Pass> passes,
            List<SalesArea> salesAreas)
        {
            if (passes != null && passes.Any() &&
                passes.Any(p => p.BreakExclusions != null && p.BreakExclusions.Any()))
            {
                var passes2 = passes.ToList()
                    .Where(p => p?.BreakExclusions != null && p.BreakExclusions.Any());
                var agBreakExclusions = passes2.ToList().SelectMany(pass =>
                {
                    return pass.BreakExclusions.Select(breakExclusion =>
                        new AgBreakExclusion()
                        {
                            PassNo = pass.Id,
                            SalesAreaNo = salesAreas.FirstOrDefault(s => s.Name == breakExclusion.SalesArea)?.CustomId ?? 0,
                            StartDate = breakExclusion.StartDate.ToString("yyyyMMdd"),
                            EndDate = breakExclusion.EndDate.ToString("yyyyMMdd"),
                            StartTime = breakExclusion.StartTime?.ToString("hhmmss") ?? string.Empty,
                            EndTime = breakExclusion.EndTime?.ToString("hhmmss") ?? string.Empty,
                            SelectableDays = breakExclusion.SelectableDays.GetSelectedDays()
                        }).ToList();
                }).ToList();

                var serializationobj = new AgBreakExclusionsSerialization();
                return serializationobj.MapFrom(agBreakExclusions);
            }
            return null;
        }

        public static AgTotalRatingsSerialization ToAgTotalRatings(this List<TotalRating> totalRatings, List<Demographic> demographics, List<SalesArea> salesAreas)
        {
            var indexedSalesAreas = salesAreas.ToDictionary(x => x.Name);
            var indexedDemographics = demographics.ToDictionary(x => x.ExternalRef.ToLowerInvariant());

            var agTotalRatings = totalRatings.Select(totalRating => new AgTotalRating
                {
                    SalesAreaNo = indexedSalesAreas.TryGetValue(totalRating.SalesArea, out var salesArea) ? salesArea.CustomId : 0,
                    DemographNo = indexedDemographics.TryGetValue(totalRating.Demograph.ToLowerInvariant(), out var demographic) ? demographic.Id : 0,
                    DaypartGroupNo = totalRating.DaypartGroup,
                    DaypartNo = totalRating.Daypart,
                    Date = AgConversions.ToAgDateYYYYMMDDAsString(totalRating.Date),
                    TotalRatings = totalRating.TotalRatings
                })
                .ToList();

            var serializationObj = new AgTotalRatingsSerialization();
            return serializationObj.MapFrom(agTotalRatings);
        }

        public static AgStandardDayPartsSerialization ToAgStandardDayParts(this List<StandardDayPart> dayParts, List<SalesArea> salesAreas)
        {
            var agStandardDayParts = dayParts.Select(daypart => new AgStandardDayPart()
            {
                DaypartNo = daypart.DayPartId,
                SalesAreaNo = salesAreas.FirstOrDefault(s => s.Name == daypart.SalesArea)?.CustomId ?? 0,
                Name = daypart.Name,
                Order = daypart.Order,
                NbrTimeslices = daypart.Timeslices.Count,
                Timeslices = daypart.Timeslices.Select(slice => new AgStandardDayPartTimeslice
                {
                    StartDay = slice.StartDay,
                    EndDay = slice.EndDay,
                    StartTime = AgConversions.ToAgTimeAsTotalHHMMSS(slice.StartTime),
                    EndTime = AgConversions.ToAgTimeAsTotalHHMMSS(slice.EndTime)
                }).ToList()
            }).ToList();

            var serializationObj = new AgStandardDayPartsSerialization();
            return serializationObj.MapFrom(agStandardDayParts);
        }

        public static AgStandardDayPartGroupsSerialization ToAgStandardDayPartGroups(this List<StandardDayPartGroup> dayPartGroups, List<SalesArea> salesAreas, List<Demographic> demographics)
        {
            var agStandardDayPartGroups = dayPartGroups.Select(group => new AgStandardDayPartGroup
            {
                DayPartGroupNo = group.GroupId,
                SalesAreaNo = salesAreas.FirstOrDefault(s => s.Name == group.SalesArea)?.CustomId ?? 0,
                DemographicNo =
                        demographics.FirstOrDefault(d => d.ExternalRef.Equals(group.Demographic,
                            StringComparison.OrdinalIgnoreCase))?.Id ?? 0,
                Optimizer = AgConversions.ToAgBooleanAsString(group.Optimizer),
                Policy = AgConversions.ToAgBooleanAsString(group.Policy),
                RatingReplacement = AgConversions.ToAgBooleanAsString(group.RatingReplacement),
                NbrDayParts = group.Splits?.Count ?? 0,
                Splits = group.Splits?.Select(split => new AgStandardDayPartSplit
                {
                    DayPartNo = split.DayPartId,
                    Split = split.Split
                }).ToList()
            }).ToList();

            var serializationObj = new AgStandardDayPartGroupsSerialization();
            return serializationObj.MapFrom(agStandardDayPartGroups);
        }

        public static AgSpotBookingRulesSerialization ToAgSpotBookingRules(this List<SpotBookingRule> spotBookingRules, List<SalesArea> allSalesAreas)
        {
            var agSpotBookingRules = new List<AgSpotBookingRule>();
            foreach (var spotBookingRule in spotBookingRules)
            {
                List<SalesArea> salesAreas;
                if (spotBookingRule.SalesAreas == null || !spotBookingRule.SalesAreas.Any())
                {
                    salesAreas = allSalesAreas;
                }
                else
                {
                    salesAreas = allSalesAreas.Where(s => spotBookingRule.SalesAreas.Contains(s.Name)).ToList();
                }

                agSpotBookingRules.AddRange(salesAreas.Select(salesArea => new AgSpotBookingRule
                {
                    SalesAreaNo = salesArea?.CustomId ?? 0,
                    MinBreakLength = (int)spotBookingRule.MinBreakLength.TotalSeconds,
                    MaxBreakLength = (int)spotBookingRule.MaxBreakLength.TotalSeconds,
                    SpotLength = (int)spotBookingRule.SpotLength.TotalSeconds,
                    MaxSpots = spotBookingRule.MaxSpots,
                    BreakType = spotBookingRule.BreakType
                }));
            }

            var serializationObj = new AgSpotBookingRulesSerialization();
            return serializationObj.MapFrom(agSpotBookingRules);
        }

        public static AgInventoryStatusesSerialization ToAgInventoryStatuses(this List<InventoryStatus> inventoryStatuses)
        {
            var agInventoryStatuses = inventoryStatuses
                .Select(inventoryStatus => new AgInventoryStatus { InventoryCode = inventoryStatus.InventoryCode })
                .ToList();
            var serializationObj = new AgInventoryStatusesSerialization();
            return serializationObj.MapFrom(agInventoryStatuses);
        }

        /// <summary>
        /// Gets the ratings schedule date for the break based on the ratings frequency
        /// </summary>
        /// <param name="break"></param>
        /// <param name="ratingsFrequency"></param>
        /// <returns></returns>
        private static DateTime GetRatingScheduleDateTimeForBreak(Break @break, TimeSpan ratingsFrequency)
        {
            DateTime scheduleDateTime = new DateTime(@break.ScheduledDate.Year, @break.ScheduledDate.Month, @break.ScheduledDate.Day, 0, 0, 0, DateTimeKind.Utc).AddTicks(-ratingsFrequency.Ticks);
            do
            {
                scheduleDateTime = scheduleDateTime.Add(ratingsFrequency);
                if (scheduleDateTime == @break.ScheduledDate)
                {
                    return scheduleDateTime;
                }
                else if (scheduleDateTime > @break.ScheduledDate)
                {
                    return scheduleDateTime.AddTicks(-ratingsFrequency.Ticks);
                }
            } while (true);
        }

        private static List<DateTime> GetDatesFromDateRanges(List<DateRange> dateRanges)
        {
            if (dateRanges != null && dateRanges.Any())
            {
                return dateRanges.SelectMany(d =>
                {
                    if (d.Start == d.End)
                    {
                        return new List<DateTime>() { d.Start };
                    }

                    var days = (d.End - d.Start).Days + 1; // incl. endDate

                    return Enumerable.Range(0, days)
                        .Select(i => d.Start.AddDays(i))
                        .ToList();
                }).ToList();
            }

            return new List<DateTime>();
        }

        public static AgCampaignBreakTypesSerialisation ToAgCampaignBreakTypes(this List<Campaign> campaigns,
            IMapper mapper, out int totalCampaignBreakTypes)
        {
            var campaignsWithBreakTypes = campaigns
                .Where(c => c.BreakType != null && c.BreakType.Any()).ToList();
            totalCampaignBreakTypes = campaignsWithBreakTypes.Count;
            if (totalCampaignBreakTypes == 0)
            {
                return null;
            }

            var agCampaignBreakTypes = campaignsWithBreakTypes.SelectMany(campaign =>
            {
                return campaign.BreakType.Select(bt =>
                {
                    return mapper.Map<AgCampaignBreakType>(Tuple.Create(campaign.CustomId, bt));
                });
            }).ToList();
            var serialization = new AgCampaignBreakTypesSerialisation();
            return serialization.MapFrom(agCampaignBreakTypes);
        }

        public static AgProgrammeProgrammeCategoryMapSerialization ToAgProgrammeProgrammeCategoryMapSerialization(
            this IEnumerable<Programme> programmes, IEnumerable<ProgrammeDictionary> programmeDictionaries, IEnumerable<ProgrammeCategoryHierarchy> programCategories)
        {
            var agProgrammeProgrammeCategoryMapsSerialization = new AgProgrammeProgrammeCategoryMapSerialization();
            var agProgrammeProgrammeCategoryMaps = new AgProgrammeProgrammeCategoryMappings();
            var programmeIndex = new Dictionary<string, Programme>();
            var categoriesIndex = programCategories.ToDictionary(c => c.Name.Trim().ToUpperInvariant());

            foreach (var item in programmes.Where(p => p.ProgrammeCategories != null && p.ProgrammeCategories.Any()))
            {
                if (!programmeIndex.ContainsKey(item.ExternalReference))
                {
                    programmeIndex[item.ExternalReference] = item;
                }
            }

            foreach (var programmeDictionaryPair in programmeDictionaries)
            {
                if (!programmeIndex.TryGetValue(programmeDictionaryPair.ExternalReference, out var programme))
                {
                    continue;
                }

                foreach (var programmeCategory in programme.ProgrammeCategories)
                {
                    if (categoriesIndex.TryGetValue(programmeCategory.Trim().ToUpperInvariant(), out var relatedProgramCategory))
                    {
                        var item = new AgProgrammeProgrammeCategoryMap
                        {
                            ProgrammeNumber = programmeDictionaryPair.Id,
                            CategoryNumber = relatedProgramCategory.Id
                        };

                        agProgrammeProgrammeCategoryMaps.Add(item);
                    }
                }
            }

            return agProgrammeProgrammeCategoryMaps.Any()
                ? agProgrammeProgrammeCategoryMapsSerialization.MapFrom(agProgrammeProgrammeCategoryMaps)
                : null;
        }

        public static AgLengthFactorSalesAreaGroupSerialization ToAgLengthFactorGroups(this IEnumerable<LengthFactor> plainLengthFactors, IEnumerable<SalesArea> salesAreas)
        {
            var agLengthFactors = plainLengthFactors
                .Join(salesAreas, x => x.SalesArea, y => y.Name, (x, y) => new
                {
                    SalesAreaNo = y.CustomId,
                    Duration = (int)x.Duration.TotalSeconds,
                    Factor = x.Factor
                })
                .GroupBy(x => x.SalesAreaNo)
                .Select(x =>
                {
                    var lengthFactors = x.Select(g => new AgLengthFactor
                    {
                        Duration = g.Duration,
                        Factor = g.Factor
                    }).ToList();

                    var lengthFactorsList = new AgLengthFactorsList();
                    lengthFactorsList.AddRange(lengthFactors);

                    return new AgLengthFactorSalesAreaGroup
                    {
                        SalesAreaNo = x.Key,
                        NbrGroups = 1,
                        LengthFactorGroups = new AgLengthFactorGroupsList
                        {
                            new AgLengthFactorGroup
                            {
                                LengthFactorGroupNo = x.Key,
                                NbrLengthFactors = lengthFactors.Count,
                                LengthFactors = lengthFactorsList
                            }
                        }
                    };
                })
                .ToList();

            return agLengthFactors.Any()
                ? AgLengthFactorSalesAreaGroupSerialization.MapFrom(agLengthFactors)
                : null;
        }

        public static AgCampaignBreakRequirementsSerialization ToAgCampaignBreakRequirement(this IEnumerable<Campaign> campaigns, List<SalesArea> salesAreas)
        {
            var agCampaignBreakRequirements = campaigns
                .Where(x => (x.BreakRequirement?.CentreBreakRequirement != null && x.BreakRequirement?.EndBreakRequirement != null))
                .Select(x =>
                {
                    var centreBreakRequirement = x.BreakRequirement.CentreBreakRequirement;
                    var endBreakRequirement = x.BreakRequirement.EndBreakRequirement;

                    var centreCurrentPercentageSplit = centreBreakRequirement.CurrentPercentageSplit;
                    var centreDesiredPercentageSplit = centreBreakRequirement.DesiredPercentageSplit;

                    var endCurrentPercentageSplit = endBreakRequirement.CurrentPercentageSplit;
                    var endDesiredPercentageSplit = endBreakRequirement.DesiredPercentageSplit;

                    return new AgCampaignBreakRequirement
                    {
                        CampaignId = x.CustomId,
                        SalesAreaId = salesAreas.FirstOrDefault(s => x.BreakRequirement.SalesArea == s.ShortName)?.CustomId ?? 0,
                        CentreBreakRequirement = LoadAgRequirement((decimal)centreCurrentPercentageSplit, (decimal)centreDesiredPercentageSplit),
                        EndBreakRequirement = LoadAgRequirement((decimal)endCurrentPercentageSplit, (decimal)endDesiredPercentageSplit)
                    };
                }).ToList();

            return agCampaignBreakRequirements.Any()
                ? AgCampaignBreakRequirementsSerialization.MapFrom(agCampaignBreakRequirements)
                : null;
        }

        private static AgRequirement LoadAgRequirement(decimal currentPercentageSplit, decimal desiredPercentageSplit)
        {
            return new AgRequirement
            {
                Required = desiredPercentageSplit,
                TgtRequired = desiredPercentageSplit,
                SareRequired = desiredPercentageSplit,
                Supplied = currentPercentageSplit
            };
        }
    }
}
