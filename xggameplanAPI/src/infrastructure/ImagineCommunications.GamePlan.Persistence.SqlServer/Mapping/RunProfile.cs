using System.Globalization;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Runs.Settings;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Shared.System.Models;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Dto.Internal;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;
using xggameplan.core.Extensions.AutoMapper;
using DomainPositionInProgramme = ImagineCommunications.GamePlan.Domain.Runs.PositionInProgramme;
using ExternalRunInfo = ImagineCommunications.GamePlan.Domain.Runs.Objects.ExternalRunInfo;
using ExternalRunInfoEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs.ExternalRunInfo;
using Run = ImagineCommunications.GamePlan.Domain.Runs.Objects.Run;
using RunEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs.Run;
using RunScenario = ImagineCommunications.GamePlan.Domain.Runs.Objects.RunScenario;
using RunScenarioEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs.RunScenario;
using RunScheduleSettings = ImagineCommunications.GamePlan.Domain.Runs.Objects.RunScheduleSettings;
using RunScheduleSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs.RunScheduleSettings;
using SalesAreaPriorityType = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.SalesAreaPriorityType;
using SqlPositioninProgramme = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.PositionInProgramme;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class RunProfile : Profile
    {
        public RunProfile()
        {
            _ = CreateMap<AuthorModel, RunAuthor>()
                 .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.Id))
                 .ForMember(dest => dest.Id, opt => opt.Ignore())
                 .ReverseMap()
                 .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AuthorId))
                 .ForSourceMember(src => src.Id, opt => opt.DoNotValidate());

            _ = CreateMap<AnalysisGroupTarget, RunAnalysisGroupTarget>()
                          .ForMember(dest => dest.AnalysisGroupTargetId, opt => opt.MapFrom(src => src.Id))
                          .ForMember(dest => dest.Id, opt => opt.Ignore())
                          .ReverseMap()
                          .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AnalysisGroupTargetId))
                          .ForSourceMember(src => src.Id, opt => opt.DoNotValidate());

            _ = CreateMap<RunScheduleSettings, RunScheduleSettingsEntity>().ReverseMap();

            _ = CreateMap<CampaignRunProcessesSettings, RunCampaignProcessesSettings>()
                      .ForMember(dest => dest.RightSizerLevel,
                          opt => opt.MapFrom(src => (RightSizerLevel?)src.RightSizerLevel))
                      .ReverseMap()
                      .ForMember(dest => dest.RightSizerLevel,
                          opt => opt.MapFrom(src => (Domain.Campaigns.RightSizerLevel?)src.RightSizerLevel));

            _ = CreateMap<CampaignReference, RunCampaignReference>().ReverseMap();
            _ = CreateMap<InventoryLock, RunInventoryLock>().ReverseMap();
            _ = CreateMap<InventoryStatus, RunInventoryStatus>().ReverseMap();

            _ = CreateMap<SalesAreaPriority, RunSalesAreaPriority>()
                          .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => (SalesAreaPriorityType)src.Priority))
                          .ForMember(dest => dest.SalesArea, opt => opt.Ignore())
                          .ForMember(dest => dest.SalesAreaId,
                            opt => opt.FromEntityCache(
                                src => src.SalesArea,
                                o => o.Entity<SalesArea>(x => x.Id)
                                )
                           )
                          .ReverseMap()
                          .ForMember(dest => dest.Priority,
                              opt => opt.MapFrom(src => (Domain.Shared.System.Models.SalesAreaPriorityType)src.Priority))
                          .ForMember(dest => dest.SalesArea,
                            opt => opt.FromEntityCache(
                                src => src.SalesAreaId,
                                s => s.Entity<SalesArea>(x => x.Name)
                                .CheckNavigationPropertyFirst(x => x.SalesArea)
                                )
                           );

            _ = CreateMap<RunScenario, RunScenarioEntity>()
                        .ForMember(dest => dest.ScenarioId, opt => opt.MapFrom(src => src.Id))
                        .ForMember(dest => dest.Id, opt => opt.Ignore())
                        .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (ScenarioStatus)src.Status))
                        .ReverseMap()
                        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ScenarioId))
                        .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (ScenarioStatuses)src.Status))
                        .ForSourceMember(src => src.Id, opt => opt.DoNotValidate());

            _ = CreateMap<Run, RunEntity>()
                      .ForMember(dest => dest.RightSizerDateStart, opt => opt.MapFrom(src => src.RightSizerDateRange.Start))
                      .ForMember(dest => dest.RightSizerDateEnd, opt => opt.MapFrom(src => src.RightSizerDateRange.End))
                      .ForMember(dest => dest.ISRDateStart, opt => opt.MapFrom(src => src.ISRDateRange.Start))
                      .ForMember(dest => dest.ISRDateEnd, opt => opt.MapFrom(src => src.ISRDateRange.End))
                      .ForMember(dest => dest.OptimisationDateStart,
                          opt => opt.MapFrom(src => src.OptimisationDateRange.Start))
                      .ForMember(dest => dest.OptimisationDateEnd, opt => opt.MapFrom(src => src.OptimisationDateRange.End))
                      .ForMember(dest => dest.SmoothDateStart, opt => opt.MapFrom(src => src.SmoothDateRange.Start))
                      .ForMember(dest => dest.SmoothDateEnd, opt => opt.MapFrom(src => src.SmoothDateRange.End))
                      .ForMember(dest => dest.EfficiencyPeriod,
                          opt => opt.MapFrom(src => (EfficiencyCalculationPeriod)src.EfficiencyPeriod))
                      .ForMember(dest => dest.RunStatus, opt => opt.MapFrom(src => (RunStatus)src.RunStatus))
                      .ForMember(dest => dest.PositionInProgramme, opt => opt.MapFrom(src => (SqlPositioninProgramme)(int)src.PositionInProgramme))
                      .ReverseMap()
                      .ForMember(dest => dest.RightSizerDateRange,
                          opt => opt.MapFrom(src => new DateRange(src.RightSizerDateStart, src.RightSizerDateEnd)))
                      .ForMember(dest => dest.ISRDateRange,
                          opt => opt.MapFrom(src => new DateRange(src.ISRDateStart, src.ISRDateEnd)))
                      .ForMember(dest => dest.OptimisationDateRange,
                          opt => opt.MapFrom(src => new DateRange(src.OptimisationDateStart, src.OptimisationDateEnd)))
                      .ForMember(dest => dest.SmoothDateRange,
                          opt => opt.MapFrom(src => new DateRange(src.SmoothDateStart, src.SmoothDateEnd)))
                      .ForMember(dest => dest.EfficiencyPeriod,
                          opt => opt.MapFrom(src => (Domain.Runs.EfficiencyCalculationPeriod)src.EfficiencyPeriod))
                      .ForMember(dest => dest.RunStatus, opt => opt.Ignore())
                      .ForMember(x => x.Scenarios, opt => opt.MapFrom(e => e.Scenarios.OrderBy(x => x.Order)))
                      .ForMember(dest => dest.PositionInProgramme, opt => opt.MapFrom(src => (DomainPositionInProgramme)(int)src.PositionInProgramme));

            _ = CreateMap<RunDto, RunExtendedSearchModel>()
                       .ForMember(dest => dest.SmoothDateRange,
                           opt => opt.MapFrom(src => new DateRange(src.SmoothDateStart, src.SmoothDateEnd)))
                       .ForMember(dest => dest.ISRDateRange,
                           opt => opt.MapFrom(src => new DateRange(src.ISRDateStart, src.ISRDateEnd)))
                       .ForMember(dest => dest.OptimisationDateRange,
                           opt => opt.MapFrom(src => new DateRange(src.OptimisationDateStart, src.OptimisationDateEnd)))
                       .ForMember(dest => dest.RightSizerDateRange,
                           opt => opt.MapFrom(src => new DateRange(src.RightSizerDateStart, src.RightSizerDateEnd)))
                       .ForMember(dest => dest.Author,
                           opt => opt.MapFrom(src => new AuthorModel { Id = src.AuthorId, Name = src.AuthorName }))
                       .ForMember(dest => dest.Scenarios, opt => opt.MapFrom(src => src.RunScenarios))
                       .ForMember(dest => dest.ScenarioIds,
                           opt => opt.MapFrom(src => src.Scenarios.Select(x => x.Id.ToString()).ToArray()))
                       .ForMember(dest => dest.ScenarioNames,
                           opt => opt.MapFrom(src => src.Scenarios.Select(x => x.Name).ToArray()))
                       .ForMember(dest => dest.PassIds,
                           opt => opt.MapFrom(src => src.Passes.Select(x => x.Id.ToString(CultureInfo.InvariantCulture)).ToArray()))
                       .ForMember(dest => dest.PassNames,
                           opt => opt.MapFrom(src => src.Passes.Select(x => x.Name).ToArray()));

            _ = CreateMap<ExternalRunInfo, ExternalRunInfoEntity>().ReverseMap();
        }
    }
}
