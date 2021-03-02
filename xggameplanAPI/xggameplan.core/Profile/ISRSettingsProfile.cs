using ImagineCommunications.GamePlan.Domain.Optimizer.ISRSettings.Objects;
using xggameplan.Model;

namespace xggameplan.Profile
{
    internal class ISRSettingsProfile : AutoMapper.Profile
    {
        public ISRSettingsProfile()
        {
            CreateMap<ISRSettingsModel, ISRSettings>();
        }
    }
}
