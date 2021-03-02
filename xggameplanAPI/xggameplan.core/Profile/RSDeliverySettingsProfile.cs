using ImagineCommunications.GamePlan.Domain.Optimizer.RSSettings.Objects;
using xggameplan.Model;

namespace xggameplan.Profile
{
    public class RSDeliverySettingsProfile : AutoMapper.Profile
    {
        public RSDeliverySettingsProfile()
        {
            CreateMap<RSDeliverySettingsModel, RSDeliverySettings>();
            CreateMap<RSDeliverySettings, RSDeliverySettingsModel>();
        }
    }
}
