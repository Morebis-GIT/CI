using ImagineCommunications.GamePlan.Domain.Optimizer.RSSettings.Objects;
using xggameplan.Model;

namespace xggameplan.Profile
{
    public class RSDemographicSettingsProfile : AutoMapper.Profile
    {
        public RSDemographicSettingsProfile()
        {
            CreateMap<RSDemographicSettingsModel, RSDemographicSettings>();
            CreateMap<RSDemographicSettings, RSDemographicSettingsModel>();
        }
    }
}
