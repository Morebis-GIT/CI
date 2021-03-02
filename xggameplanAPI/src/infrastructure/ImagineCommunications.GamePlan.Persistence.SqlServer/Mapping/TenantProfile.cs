using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class TenantProfile : Profile
    {
        public TenantProfile() {
            CreateMap<DatabaseProviderConfiguration, Entities.Master.DatabaseProviderConfiguration>().ReverseMap();
            CreateMap<Tenant, Entities.Master.Tenant>().ReverseMap();
        }
    }
}
