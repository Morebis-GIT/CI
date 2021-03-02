using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.InventoryStatuses;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class InventoryTypeProfile : Profile
    {
        public InventoryTypeProfile()
        {
            CreateMap<Domain.InventoryStatuses.Objects.InventoryType, InventoryType>()
                .ForMember(dest => dest.LockTypes, opt =>
                {
                    opt.PreCondition(s => s.LockTypes != null);
                    opt.MapFrom(s => s.LockTypes.Select(lt => new InventoryTypeLockType {LockType = lt}));
                })
                .ReverseMap()
                .ForMember(dest => dest.LockTypes, opt =>
                {
                    opt.PreCondition(s => s.LockTypes != null);
                    opt.MapFrom(s => s.LockTypes.Select(lt => lt.LockType));
                });
        }
    }
}
