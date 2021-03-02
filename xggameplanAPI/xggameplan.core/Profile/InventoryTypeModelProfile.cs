using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Objects;
using xggameplan.model.External;

namespace xggameplan.Profile
{
    internal class InventoryTypeModelProfile : AutoMapper.Profile
    {
        public InventoryTypeModelProfile()
        {
            CreateMap<InventoryType, InventoryTypeModel>()
                .ForMember(e => e.InventoryCode, o => o.MapFrom(s => s.InventoryCode.Trim())).ReverseMap();
        }
    }
}
