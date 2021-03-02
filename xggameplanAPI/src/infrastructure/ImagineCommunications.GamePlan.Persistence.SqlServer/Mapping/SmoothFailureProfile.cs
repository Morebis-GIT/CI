using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth;
using SmoothFailure = ImagineCommunications.GamePlan.Domain.SmoothFailures.SmoothFailure;
using SmoothFailureEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.SmoothFailure;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class SmoothFailureProfile : Profile
    {
        public SmoothFailureProfile()
        {
            CreateMap<SmoothFailure, SmoothFailureEntity>()
                .ForMember(d => d.FailureMessagesMap,
                    opt => opt.MapFrom(s => s.MessageIds.Select(x => new SmoothFailureSmoothFailureMessage
                        {SmoothFailureMessageId = x})));

            CreateMap<SmoothFailureEntity, SmoothFailure>()
                .ForMember(d => d.MessageIds,
                    opt => opt.MapFrom(s => s.FailureMessagesMap.Select(x => x.SmoothFailureMessageId)));
        }
    }
}
