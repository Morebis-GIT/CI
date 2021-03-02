using ImagineCommunications.GamePlan.Domain.Optimizer.ISRSettings.Objects;
using xggameplan.Model;

namespace xggameplan.Profile
{
    internal class ISRDemographicSettingsModelProfile : AutoMapper.Profile
    {
        public ISRDemographicSettingsModelProfile()
        {
            CreateMap<ISRDemographicSettings, ISRDemographicSettingsModel>();
        }
    }
}
