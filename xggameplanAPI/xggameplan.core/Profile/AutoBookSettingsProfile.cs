using ImagineCommunications.GamePlan.Domain.AutoBookApi.Settings;
using xggameplan.Model;

namespace xggameplan.Profile
{
    internal class AutoBookSettingsProfile : AutoMapper.Profile
    {
        public AutoBookSettingsProfile()
        {
            CreateMap<AutoBookSettings, AutoBookSettingsModel>();
            CreateMap<AutoBookSettingsModel, AutoBookSettings>();

            //to update AutoBookSettings data
            CreateMap<AutoBookSettings, AutoBookSettings>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
