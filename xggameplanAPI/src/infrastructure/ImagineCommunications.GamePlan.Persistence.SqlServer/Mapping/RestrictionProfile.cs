using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Restrictions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories;
using Restriction = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Restrictions.Restriction;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class RestrictionProfile : Profile
    {
        public RestrictionProfile()
        {
            _ = CreateMap<Restriction, Domain.BusinessRules.Restrictions.Objects.Restriction>()
            .ForMember(d => d.SalesAreas, o => o.MapFrom(src =>
                                        src.SalesAreas.Select(x => x.SalesArea)))
            .ForMember(d => d.LiveProgrammeIndicator, o => o.MapFrom(src => src.LiveProgrammeIndicator))
            .ForMember(d => d.ProductCode, o => o.MapFrom(src => Convert.ToInt32(src.ProductCode)))
            .ReverseMap()
            .ForMember(d => d.SalesAreas, o => o.MapFrom(src =>
                                        src.SalesAreas == null ? new List<RestrictionSalesArea>() :
                                        src.SalesAreas.Select(x => new RestrictionSalesArea()
                                        {
                                            SalesArea = x,
                                            RestrictionId = src.Id
                                        })));

            _ = CreateMap<RestrictionSearchDto, Domain.BusinessRules.Restrictions.Objects.Restriction>()
                .ForMember(d => d.SalesAreas, o => o.MapFrom(src =>
                    src.SalesAreas.Select(x => x.SalesArea)));
            _ = CreateMap<RestrictionSearchDto, RestrictionDescription>();
        }
    }
}
