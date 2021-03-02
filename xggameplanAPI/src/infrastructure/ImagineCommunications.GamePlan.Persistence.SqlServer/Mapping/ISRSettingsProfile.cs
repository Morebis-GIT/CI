using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;
using xggameplan.core.Extensions.AutoMapper;
using ISRDemographicSettings = ImagineCommunications.GamePlan.Domain.Optimizer.ISRSettings.Objects.ISRDemographicSettings;
using ISRDemographicSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ISRSettings.ISRDemographicSettings;
using ISRSettings = ImagineCommunications.GamePlan.Domain.Optimizer.ISRSettings.Objects.ISRSettings;
using ISRSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ISRSettings.ISRSettings;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class ISRSettingsProfile : Profile
    {
        public ISRSettingsProfile()
        {
            _ = CreateMap<ISRSettings, ISRSettingsEntity>()
                .ForMember(d => d.SalesArea, o => o.Ignore())
                .ForMember(d => d.SalesAreaId,
                    o => o.FromEntityCache(src => src.SalesArea, opts => opts.Entity<SalesArea>(x => x.Id)))
                .ReverseMap()
                .ForMember(d => d.SalesArea,
                    opts => opts.FromEntityCache(src => src.SalesAreaId,
                        s => s.Entity<SalesArea>(x => x.Name).CheckNavigationPropertyFirst(x => x.SalesArea)));

            _ = CreateMap<ISRDemographicSettings, ISRDemographicSettingsEntity>()
                .ReverseMap();
        }
    }
}
