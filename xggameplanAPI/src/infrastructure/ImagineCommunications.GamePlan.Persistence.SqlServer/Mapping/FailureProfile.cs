using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Failures;
using Failure = ImagineCommunications.GamePlan.Domain.ScenarioFailures.Objects.Failure;
using Failures = ImagineCommunications.GamePlan.Domain.ScenarioFailures.Objects.Failures;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class FailureProfile : Profile
    {
        public FailureProfile()
        {
            CreateMap<Failures, Entities.Tenant.Failures.Failure>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.ScenarioId, opt => opt.MapFrom(s => s.Id))
                .ReverseMap()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.ScenarioId))
                ;

            CreateMap<Failure, FailureItem>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.FailureId, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
