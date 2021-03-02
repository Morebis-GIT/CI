using ImagineCommunications.GamePlan.Domain.Runs.Settings;
using xggameplan.Model;

namespace xggameplan.Profile
{
    internal class InventoryLockProfile : AutoMapper.Profile
    {
        public InventoryLockProfile()
        {
            CreateMap<InventoryLock, InventoryLockModel>().ReverseMap();
        }
    }
}
