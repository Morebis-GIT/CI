using ImagineCommunications.GamePlan.Domain.Optimizer.RSGlobalSettings.Objects;
using xggameplan.Model;

namespace xggameplan.Profile
{
    internal class RSGlobalSettingsProfile : AutoMapper.Profile
    {
        public RSGlobalSettingsProfile()
        {
            CreateMap<RSGlobalSettingsModel, RSGlobalSettings>().ReverseMap();
        }
    }
}

