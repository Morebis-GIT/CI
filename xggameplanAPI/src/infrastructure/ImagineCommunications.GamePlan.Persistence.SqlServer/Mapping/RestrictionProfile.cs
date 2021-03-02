using AutoMapper;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Restrictions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories;
using xggameplan.core.Extensions.AutoMapper;
using Restriction = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Restrictions.Restriction;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class RestrictionProfile : Profile
    {
        public RestrictionProfile()
        {
            _ = CreateMap<Restriction, Domain.BusinessRules.Restrictions.Objects.Restriction>()
                .ForMember(d => d.LiveProgrammeIndicator, o => o.MapFrom(src => src.LiveProgrammeIndicator))
                .ReverseMap();

            CreateMap<string, RestrictionSalesArea>()
                .ForMember(d => d.SalesAreaId,
                    opts => opts.FromEntityCache(opt => opt.Entity<SalesArea>(x => x.Id)))
                .ReverseMap()
                .FromEntityCache(x => x.SalesAreaId, opt => opt.Entity<SalesArea>(x => x.Name));

            _ = CreateMap<RestrictionSearchDto, Domain.BusinessRules.Restrictions.Objects.Restriction>();

            _ = CreateMap<RestrictionSearchDto, RestrictionDescription>();
        }
    }
}
