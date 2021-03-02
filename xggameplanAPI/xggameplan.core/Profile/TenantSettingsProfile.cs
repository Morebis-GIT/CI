using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;

namespace xggameplan.Profile
{
    internal class TenantSettingsProfile : AutoMapper.Profile
    {
        public TenantSettingsProfile()
        {
            //to update TenantSettings data
            CreateMap<TenantSettings, TenantSettings>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<TenantSettings, TenantTimeSettings>();
        }
    }
}
