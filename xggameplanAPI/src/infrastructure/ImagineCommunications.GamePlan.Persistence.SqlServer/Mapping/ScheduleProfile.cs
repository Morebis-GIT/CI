using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Schedules;
using xggameplan.core.Extensions.AutoMapper;
using Schedule = ImagineCommunications.GamePlan.Domain.Shared.Schedules.Schedule;
using ScheduleEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Schedules.Schedule;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class ScheduleProfile : Profile
    {
        public ScheduleProfile()
        {
            _ = CreateMap<ScheduleEntity, Schedule>()
                .ForMember(d => d.SalesArea, opts => opts.FromEntityCache(src => src.SalesAreaId,
                    s => s.Entity<SalesArea>(x => x.Name).CheckNavigationPropertyFirst(x => x.SalesArea)))
                .ReverseMap()
                .ForMember(dest => dest.Breaks, opt => opt.Ignore())
                .ForMember(dest => dest.Programmes, opt => opt.Ignore())
                .ForMember(d => d.SalesArea, opt => opt.Ignore())
                .ForMember(d => d.SalesAreaId,
                    o => o.FromEntityCache(src => src.SalesArea, opts => opts.Entity<SalesArea>(x => x.Id)));

            _ = CreateMap<ScheduleBreak, Domain.Breaks.Objects.Break>()
                .ForMember(d => d.PositionInProg, opt => opt.MapFrom(src => (BreakPosition)src.PositionInProg))
                .ForMember(d => d.BreakEfficiencyList, opt => opt.MapFrom(src => src.BreakEfficiencies))
                .ForMember(d => d.SalesArea,
                    opts => opts.FromEntityCache(src => src.SalesAreaId,
                        s => s.Entity<SalesArea>(x => x.Name).CheckNavigationPropertyFirst(x => x.SalesArea)))
                .ReverseMap()
                .ForMember(d => d.BreakEfficiencies, opt => opt.Ignore())
                .ForMember(d => d.SalesArea, opt => opt.Ignore())
                .ForMember(d => d.SalesAreaId,
                    o => o.FromEntityCache(src => src.SalesArea, opts => opts.Entity<SalesArea>(x => x.Id)));

            _ = CreateMap<ScheduleBreakEfficiency, Domain.Breaks.Objects.BreakEfficiency>().ReverseMap();

            CreateMap<ScheduleProgramme, Domain.Shared.Programmes.Objects.Programme>()
                .ConvertUsing((schProg, prog, rc) =>
                    rc.Mapper.Map<Domain.Shared.Programmes.Objects.Programme>(schProg.Programme));

            _ = CreateMap<Domain.Shared.Programmes.Objects.Programme, ScheduleProgramme>()
                .ForMember(dest => dest.ProgrammeId, opt => opt.MapFrom(src => src.Id));

            _ = CreateMap<ScheduleBreak, Domain.Breaks.Objects.BreakSimple>();
        }
    }
}
