using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.AnalysisGroups.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Runs.Settings;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.System.Models;
using xggameplan.common.Extensions;
using xggameplan.model.External;
using xggameplan.Model;

namespace xggameplan.Profile
{
    public class RunProfile : AutoMapper.Profile
    {
        // TODO: This can be moved to somewhere in shared or common (Technical Debt)
        private static readonly TimeSpan DefaultBroadcastDayStartTime = new TimeSpan(6, 0, 0);
        private static readonly TimeSpan DefaultBroadcastDayEndTime = new TimeSpan(5, 59, 59);

        public RunProfile()
        {
            CreateMap<CreateRunModel, Run>()
                .IncludeBase<RunModelBase, Run>();

            CreateMap<UpdateRunModel, Run>()
               .IncludeBase<RunModelBase, Run>();

            CreateMap<RunExtendedSearchModel, Run>();

            CreateMap<RunModelBase, Run>()
               .ForMember(d => d.StartDate, o => o.MapFrom(s => s.StartDate.Date))
               .ForMember(d => d.EndDate, o => o.MapFrom(s => s.EndDate.Date))
               .ForMember(d => d.ISRDateRange, o => o.MapFrom(s => s.ISR ?
                                                                        ProcessAndGetDateRange(s.ISRDateRange, s.StartDate, s.EndDate) :
                                                                        s.ISRDateRange))
               .ForMember(d => d.SmoothDateRange, o => o.MapFrom(s => s.Smooth ?
                                                                      ProcessAndGetDateRange(s.SmoothDateRange, s.StartDate, s.EndDate) :
                                                                      s.SmoothDateRange))
               .ForMember(d => d.OptimisationDateRange, o => o.MapFrom(s => s.Optimisation ?
                                                                                 ProcessAndGetDateRange(s.OptimisationDateRange, s.StartDate, s.EndDate) :
                                                                                 s.OptimisationDateRange))
               .ForMember(d => d.RightSizerDateRange, o => o.MapFrom(s => s.RightSizer ?
                                                                               ProcessAndGetDateRange(s.RightSizerDateRange, s.StartDate, s.EndDate) :
                                                                               s.RightSizerDateRange))
               .ForMember(d => d.Scenarios, o => o.MapFrom(s => s.Scenarios))
               .ForMember(d => d.Campaigns, o => o.MapFrom(s => s.Campaigns))
               .ForMember(d => d.SalesAreaPriorities, o => o.MapFrom((s, run, salesAreaPriorities, context) => GetSalesAreaPriorities(s.SalesAreaPriorities, context.Mapper)))
               .ForMember(d => d.CampaignsProcessesSettings, o => o.MapFrom((s, run, campaignRunProcessesSettings, context) => GetCampaignRunProcessesSettings(s.CampaignsProcessesSettings, context.Mapper)))
               .ForMember(d => d.PositionInProgramme, o => o.MapFrom(s => ParsePositionInProgrammeEnumValue(s.PositionInProgramme)));

            CreateMap<InventoryStatus, InventoryStatusModel>().ReverseMap();
            CreateMap<Run, RunModel>()
                .ForMember(d => d.PositionInProgramme, o => o.MapFrom(s => s.PositionInProgramme.GetDescription()))
                .ReverseMap();

            CreateMap<Tuple<Run, List<Scenario>>, RunModel>()
                .ForMember(r => r.Id, exp => exp.MapFrom(s => s.Item1.Id))
                .ForMember(r => r.IsLocked, exp => exp.MapFrom(s => s.Item1.IsLocked))
                .ForMember(r => r.InventoryLock, exp => exp.MapFrom(s => s.Item1.InventoryLock))
                .ForMember(r => r.ExcludedInventoryStatuses, exp => exp.MapFrom(s => s.Item1.ExcludedInventoryStatuses))
                .ForMember(r => r.AnalysisGroupTargets, exp => exp.MapFrom(s => s.Item1.AnalysisGroupTargets.OrderBy(x => x.SortIndex)))
                .ForMember(r => r.ScheduleSettings, exp => exp.MapFrom(s => s.Item1.ScheduleSettings))
                .ForMember(r => r.CreatedDateTime, exp => exp.MapFrom(s => s.Item1.CreatedDateTime))
                .ForMember(r => r.Campaigns, exp => exp.MapFrom(s => s.Item1.Campaigns))
                .ForMember(r => r.Author, exp => exp.MapFrom(s => s.Item1.Author))
                .ForMember(r => r.Description, exp => exp.MapFrom(s => s.Item1.Description))
                .ForMember(r => r.EndDate, exp => exp.MapFrom(s => s.Item1.EndDate))
                .ForMember(r => r.EndTime, exp => exp.MapFrom(s => s.Item1.EndTime))
                .ForMember(r => r.ExecuteStartedDateTime, exp => exp.MapFrom(s => s.Item1.ExecuteStartedDateTime))
                .ForMember(r => r.FirstScenarioStartedDateTime, exp => exp.MapFrom(s => s.Item1.FirstScenarioStartedDateTime))
                .ForMember(r => r.IncludeEfficiencyFactor, exp => exp.MapFrom(s => s.Item1.IncludeEfficiencyFactor))
                .ForMember(r => r.ISR, exp => exp.MapFrom(s => s.Item1.ISR))
                .ForMember(r => r.LastModifiedDateTime, exp => exp.MapFrom(s => s.Item1.LastModifiedDateTime))
                .ForMember(r => r.LastScenarioCompletedDateTime, exp => exp.MapFrom(s => s.Item1.LastScenarioCompletedDateTime))
                .ForMember(r => r.Optimisation, exp => exp.MapFrom(s => s.Item1.Optimisation))
                .ForMember(r => r.Real, exp => exp.MapFrom(s => s.Item1.Real))
                .ForMember(r => r.RightSizer, exp => exp.MapFrom(s => s.Item1.RightSizer))
                .ForMember(r => r.Smooth, exp => exp.MapFrom(s => s.Item1.Smooth))
                .ForMember(r => r.Scenarios, exp => exp.MapFrom((s, runModel, scenarioModels, context) => GetBasicScenarioModels(s, context.Mapper)))
                .ForMember(r => r.StartDate, exp => exp.MapFrom(s => s.Item1.StartDate))
                .ForMember(r => r.StartTime, exp => exp.MapFrom(s => s.Item1.StartTime))
                .ForMember(r => r.Objectives, exp => exp.MapFrom(s => s.Item1.Objectives))
                .ForMember(r => r.SpreadProgramming, exp => exp.MapFrom(s => s.Item1.SpreadProgramming))
                .ForMember(r => r.IgnoreZeroPercentageSplit, exp => exp.MapFrom(s => s.Item1.IgnoreZeroPercentageSplit))
                .ForMember(r => r.BookTargetArea, exp => exp.MapFrom(s => s.Item1.BookTargetArea))
                .ForMember(r => r.ISRDateRange, exp => exp.MapFrom(s => s.Item1.ISRDateRange))
                .ForMember(r => r.SmoothDateRange, exp => exp.MapFrom(s => s.Item1.SmoothDateRange))
                .ForMember(r => r.OptimisationDateRange, exp => exp.MapFrom(s => s.Item1.OptimisationDateRange))
                .ForMember(r => r.RightSizerDateRange, exp => exp.MapFrom(s => s.Item1.RightSizerDateRange))
                .ForMember(r => r.EfficiencyPeriod, o => o.MapFrom(s => s.Item1.EfficiencyPeriod))
                .ForMember(r => r.NumberOfWeeks, o => o.MapFrom(s => s.Item1.NumberOfWeeks))
                .ForMember(r => r.Manual, o => o.MapFrom(s => s.Item1.Manual))
                .ForMember(r => r.Status, o => o.MapFrom(s => s.Item1.RunStatus))
                .ForMember(r => r.HasNonPendingScenario, o => o.MapFrom(s => s.Item1.Scenarios != null && s.Item1.Scenarios.Count > 0 && s.Item1.Scenarios.Find(i => !Equals(i.Status, ScenarioStatuses.Pending)) != null))
                .ForMember(r => r.HasAllScenarioCompletedSuccessfully, o => o.MapFrom(s => s.Item1.Scenarios != null && s.Item1.Scenarios.Count > 0 && s.Item1.Scenarios.Find(i => !Equals(i.Status, ScenarioStatuses.CompletedSuccess)) == null))
                .ForMember(r => r.PositionInProgramme, o => o.MapFrom(s => s.Item1.PositionInProgramme.GetDescription()));

            CreateMap<Tuple<Run, List<Scenario>, List<Pass>, Dictionary<int, AnalysisGroupNameModel>, Guid>, RunModel>()
                .ForMember(r => r.ExternalStatus, exp => exp.MapFrom(s => s.Item1.ExternalStatus))
                .ForMember(r => r.Id, exp => exp.MapFrom(s => s.Item1.Id))
                .ForMember(r => r.IsLocked, exp => exp.MapFrom(s => s.Item1.IsLocked))
                .ForMember(r => r.InventoryLock, exp => exp.MapFrom(s => s.Item1.InventoryLock))
                .ForMember(r => r.ExcludedInventoryStatuses, exp => exp.MapFrom(s => s.Item1.ExcludedInventoryStatuses))
                .ForMember(r => r.AnalysisGroupTargets, exp => exp.MapFrom((s, runModel, scenarioModels, context) => GetAnalysisGroupTargetModels(s, context.Mapper)))
                .ForMember(r => r.ScheduleSettings, exp => exp.MapFrom(s => s.Item1.ScheduleSettings))
                .ForMember(r => r.CreatedDateTime, exp => exp.MapFrom(s => s.Item1.CreatedDateTime))
                .ForMember(r => r.Campaigns, exp => exp.MapFrom(s => s.Item1.Campaigns))
                .ForMember(r => r.CampaignsProcessesSettings, exp => exp.MapFrom((s, model, campaignRunProcessesSettingsModels, context) => GetCampaignRunProcessesSettingsModels(s.Item1.CampaignsProcessesSettings, context.Mapper)))
                .ForMember(r => r.Author, exp => exp.MapFrom(s => s.Item1.Author))
                .ForMember(r => r.Description, exp => exp.MapFrom(s => s.Item1.Description))
                .ForMember(r => r.EndDate, exp => exp.MapFrom(s => s.Item1.EndDate))
                .ForMember(r => r.EndTime, exp => exp.MapFrom(s => s.Item1.EndTime))
                .ForMember(r => r.ExecuteStartedDateTime, exp => exp.MapFrom(s => s.Item1.ExecuteStartedDateTime))
                .ForMember(r => r.FirstScenarioStartedDateTime, exp => exp.MapFrom(s => s.Item1.FirstScenarioStartedDateTime))
                .ForMember(r => r.ISR, exp => exp.MapFrom(s => s.Item1.ISR))
                .ForMember(r => r.LastModifiedDateTime, exp => exp.MapFrom(s => s.Item1.LastModifiedDateTime))
                .ForMember(r => r.LastScenarioCompletedDateTime, exp => exp.MapFrom(s => s.Item1.LastScenarioCompletedDateTime))
                .ForMember(r => r.Optimisation, exp => exp.MapFrom(s => s.Item1.Optimisation))
                .ForMember(r => r.Real, exp => exp.MapFrom(s => s.Item1.Real))
                .ForMember(r => r.RightSizer, exp => exp.MapFrom(s => s.Item1.RightSizer))
                .ForMember(r => r.Smooth, exp => exp.MapFrom(s => s.Item1.Smooth))
                .ForMember(r => r.Scenarios, exp => exp.MapFrom((s, runModel, scenarioModels, context) => GetScenarioModels(s, context.Mapper)))
                .ForMember(r => r.StartDate, exp => exp.MapFrom(s => s.Item1.StartDate))
                .ForMember(r => r.StartTime, exp => exp.MapFrom(s => s.Item1.StartTime))
                .ForMember(r => r.Objectives, exp => exp.MapFrom(s => s.Item1.Objectives))
                .ForMember(r => r.SpreadProgramming, exp => exp.MapFrom(s => s.Item1.SpreadProgramming))
                .ForMember(r => r.SkipLockedBreaks, exp => exp.MapFrom(s => s.Item1.SkipLockedBreaks))
                .ForMember(r => r.IgnorePremiumCategoryBreaks, exp => exp.MapFrom(s => s.Item1.IgnorePremiumCategoryBreaks))
                .ForMember(r => r.ExcludeBankHolidays, exp => exp.MapFrom(s => s.Item1.ExcludeBankHolidays))
                .ForMember(r => r.ExcludeSchoolHolidays, exp => exp.MapFrom(s => s.Item1.ExcludeSchoolHolidays))
                .ForMember(r => r.IncludeEfficiencyFactor, exp => exp.MapFrom(s => s.Item1.IncludeEfficiencyFactor))
                .ForMember(r => r.IgnoreZeroPercentageSplit, exp => exp.MapFrom(s => s.Item1.IgnoreZeroPercentageSplit))
                .ForMember(r => r.BookTargetArea, exp => exp.MapFrom(s => s.Item1.BookTargetArea))
                .ForMember(r => r.ISRDateRange, exp => exp.MapFrom(s => s.Item1.ISRDateRange))
                .ForMember(r => r.SmoothDateRange, exp => exp.MapFrom(s => s.Item1.SmoothDateRange))
                .ForMember(r => r.OptimisationDateRange, exp => exp.MapFrom(s => s.Item1.OptimisationDateRange))
                .ForMember(r => r.RightSizerDateRange, exp => exp.MapFrom(s => s.Item1.RightSizerDateRange))
                .ForMember(r => r.SalesAreaPriorities, o => o.MapFrom((s, model, salesAreaPriorityModels, context) => GetSalesAreaPriorityModels(s.Item1.SalesAreaPriorities, context.Mapper)))
                .ForMember(r => r.EfficiencyPeriod, o => o.MapFrom(s => s.Item1.EfficiencyPeriod))
                .ForMember(r => r.NumberOfWeeks, o => o.MapFrom(s => s.Item1.NumberOfWeeks))
                .ForMember(r => r.Manual, o => o.MapFrom(s => s.Item1.Manual))
                .ForMember(r => r.Status, o => o.MapFrom(s => s.Item1.RunStatus))
                .ForMember(r => r.HasNonPendingScenario, o => o.MapFrom(s => s.Item1.Scenarios != null && s.Item1.Scenarios.Count > 0 && s.Item1.Scenarios.Find(i => !Equals(i.Status, ScenarioStatuses.Pending)) != null))
                .ForMember(r => r.HasAllScenarioCompletedSuccessfully, o => o.MapFrom(s => s.Item1.Scenarios != null && s.Item1.Scenarios.Count > 0 && s.Item1.Scenarios.Find(i => !Equals(i.Status, ScenarioStatuses.CompletedSuccess)) == null))
                .ForMember(r => r.BRSConfigurationTemplateId, o => o.MapFrom(s => s.Item1.BRSConfigurationTemplateId))
                .ForMember(r => r.RunTypeId, o => o.MapFrom(s => s.Item1.RunTypeId))
                .ForMember(r => r.PositionInProgramme, o => o.MapFrom(s => s.Item1.PositionInProgramme.GetDescription()));

            CreateMap<AnalysisGroupTarget, AnalysisGroupTargetModel>();
            CreateMap<CreateOrUpdateAnalysisGroupTargetModel, AnalysisGroupTarget>();

            CreateMap<RunScheduleSettings, RunScheduleSettingsModel>().ReverseMap();

            CreateMap<ScheduledRunSettingsModel, xggameplan.model.Internal.Landmark.ScheduledRunSettingsModel>();
        }

        private static List<AnalysisGroupTargetModel> GetAnalysisGroupTargetModels(
            Tuple<Run, List<Scenario>, List<Pass>, Dictionary<int, AnalysisGroupNameModel>, Guid> src, IMapper mapper)
        {
            var (run, scenarios, passes, analysisGroups, defaultScenarioId) = src;
            var result = new List<AnalysisGroupTargetModel>();

            foreach (var analysisGroup in run.AnalysisGroupTargets.OrderBy(x => x.SortIndex))
            {
                var analysisGroupModel = mapper.Map<AnalysisGroupTargetModel>(analysisGroup);
                analysisGroupModel.AnalysisGroupName = analysisGroups.ContainsKey(analysisGroup.AnalysisGroupId)
                    ? analysisGroups[analysisGroup.AnalysisGroupId].Name
                    : null;

                result.Add(analysisGroupModel);
            }

            return result;
        }

        private static List<ScenarioModel> GetScenarioModels(Tuple<Run, List<Scenario>, List<Pass>, Dictionary<int, AnalysisGroupNameModel>, Guid> src, IMapper mapper)
        {
            var (run, scenarios, passes, analysisGroups, defaultScenarioId) = src;

            var scenarioModels = new List<ScenarioModel>();
            run.Scenarios.ForEach(runScenario =>
            scenarioModels.Add(mapper.Map<ScenarioModel>(Tuple.Create(runScenario, scenarios, passes, defaultScenarioId))));
            return scenarioModels;
        }

        private static List<ScenarioModel> GetBasicScenarioModels(Tuple<Run, List<Scenario>> src, IMapper mapper)
        {
            var (run, scenarios) = src;

            var scenarioModels = new List<ScenarioModel>();
            run.Scenarios.ForEach(runScenario =>
            scenarioModels.Add(mapper.Map<ScenarioModel>(Tuple.Create(runScenario, scenarios))));
            return scenarioModels;
        }

        private static IEnumerable<CampaignRunProcessesSettingsModel> GetCampaignRunProcessesSettingsModels(
            IEnumerable<CampaignRunProcessesSettings> settings, IMapper mapper)
        {
            return mapper.Map<IEnumerable<CampaignRunProcessesSettingsModel>>(settings);
        }

        private static IEnumerable<CampaignRunProcessesSettings> GetCampaignRunProcessesSettings(
            IEnumerable<CampaignRunProcessesSettingsModel> settingModels, IMapper mapper)
        {
            return mapper.Map<IEnumerable<CampaignRunProcessesSettings>>(settingModels);
        }

        private static List<SalesAreaPriorityModel> GetSalesAreaPriorityModels(
            IList<SalesAreaPriority> salesAreaPriorities, IMapper mapper)
        {
            return mapper.Map<List<SalesAreaPriorityModel>>(salesAreaPriorities);
        }

        private static List<SalesAreaPriority> GetSalesAreaPriorities(
            IList<SalesAreaPriorityModel> salesAreaPriorityModels, IMapper mapper)
        {
            return mapper.Map<List<SalesAreaPriority>>(salesAreaPriorityModels);
        }

        private static DateRange ProcessAndGetDateRange(DateRange dateRange, DateTime runStartDate, DateTime runEndDate)
        {
            return dateRange ?? CreateDateRange(runStartDate, runEndDate);
        }

        private static DateRange CreateDateRange(DateTime startDate, DateTime endDate)
        {
            return new DateRange(startDate.Date, endDate.Date);
        }

        private static PositionInProgramme ParsePositionInProgrammeEnumValue(string positionInProgramme) =>
            positionInProgramme switch
            {
                "A" => PositionInProgramme.All,
                "C" => PositionInProgramme.Centre,
                "E" => PositionInProgramme.End,
                _ => PositionInProgramme.All
            };
    }
}
