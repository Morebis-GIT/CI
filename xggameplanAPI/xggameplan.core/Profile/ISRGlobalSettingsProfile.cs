using ImagineCommunications.GamePlan.Domain.Optimizer.ISRGlobalSettings.Objects;
using xggameplan.Model;

namespace xggameplan.Profile
{
    internal class ISRGlobalSettingsProfile : AutoMapper.Profile
    {
        public ISRGlobalSettingsProfile()
        {
            CreateMap<ISRGlobalSettingsModel, ISRGlobalSettings>().ReverseMap();
        }
    }
}
