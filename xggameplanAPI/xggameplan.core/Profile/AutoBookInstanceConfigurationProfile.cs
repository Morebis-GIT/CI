using ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration.Objects;
using xggameplan.Model;

namespace xggameplan.Profile
{
    internal class AutoBookInstanceConfigurationProfile : AutoMapper.Profile
    {
        public AutoBookInstanceConfigurationProfile()
        {
            CreateMap<AutoBookInstanceConfiguration, AutoBookInstanceConfigurationModel>();
            CreateMap<AutoBookInstanceConfigurationModel, AutoBookInstanceConfiguration>();
        }
    }
}
