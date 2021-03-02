using ImagineCommunications.GamePlan.Domain.Optimizer.ISRSettings.Objects;
using xggameplan.Model;

namespace xggameplan.Profile
{
    internal class ISRSettingsModelProfile : AutoMapper.Profile
    {
        public ISRSettingsModelProfile()
        {
            CreateMap<ISRSettings, ISRSettingsModel>();
        }
    }
}
