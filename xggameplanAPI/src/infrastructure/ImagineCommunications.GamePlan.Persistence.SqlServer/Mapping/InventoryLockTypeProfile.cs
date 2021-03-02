using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.InventoryStatuses;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class InventoryLockTypeProfile : Profile
    {
        public InventoryLockTypeProfile()
        {
            CreateMap<InventoryLockType, Domain.InventoryStatuses.Objects.InventoryLockType>().ReverseMap();
        }
    }
}
