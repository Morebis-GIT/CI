using ImagineCommunications.GamePlan.Domain.EfficiencySettings;
using xggameplan.model.External;

namespace xggameplan.Profile
{
    internal class EfficiencySettingsProfile : AutoMapper.Profile
    {
        public EfficiencySettingsProfile()
        {
            CreateMap<EfficiencySettings, EfficiencySettingsModel>().ReverseMap();
        }
    }
}
