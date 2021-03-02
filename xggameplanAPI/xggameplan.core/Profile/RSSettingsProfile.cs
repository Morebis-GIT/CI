using ImagineCommunications.GamePlan.Domain.Optimizer.RSSettings.Objects;
using xggameplan.Model;

namespace xggameplan.Profile
{
    public class RSSettingsProfile : AutoMapper.Profile
    {
        public RSSettingsProfile()
        {
            CreateMap<RSSettingsModel, RSSettings>();
            CreateMap<RSSettings, RSSettingsModel>();
        }
    }
}
