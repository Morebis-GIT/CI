using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class DeliveryCappingGroupProfile : Profile
    {
        public DeliveryCappingGroupProfile()
        {
            CreateMap<DeliveryCappingGroup, Domain.DeliveryCappingGroup.DeliveryCappingGroup>().ReverseMap();
        }
    }
}
