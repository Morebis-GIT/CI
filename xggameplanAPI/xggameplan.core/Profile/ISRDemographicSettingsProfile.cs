using ImagineCommunications.GamePlan.Domain.Optimizer.ISRSettings.Objects;
using xggameplan.Model;

namespace xggameplan.Profile
{
    internal class ISRDemographicSettingsProfile : AutoMapper.Profile
    {
        public ISRDemographicSettingsProfile()
        {
            CreateMap<ISRDemographicSettingsModel, ISRDemographicSettings>();
        }
    }
}
