using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Programmes;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;
using xggameplan.core.Extensions.AutoMapper;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class ProgrammeProfile : Profile
    {
        public ProgrammeProfile()
        {
            _ = CreateMap<Programme, Domain.Shared.Programmes.Objects.Programme>()
                .ForMember(dest => dest.ProgrammeName,
                    opt => opt.MapFrom(src => src.ProgrammeDictionary == null ? null : src.ProgrammeDictionary.Name))
                .ForMember(dest => dest.Description,
                    opt => opt.MapFrom(src =>
                        src.ProgrammeDictionary == null ? null : src.ProgrammeDictionary.Description))
                .ForMember(dest => dest.Classification,
                    opt => opt.MapFrom(src =>
                        src.ProgrammeDictionary == null ? null : src.ProgrammeDictionary.Classification))
                .ForMember(dest => dest.ExternalReference,
                    opt => opt.MapFrom(src =>
                        src.ProgrammeDictionary == null ? null : src.ProgrammeDictionary.ExternalReference))
                .ForMember(dest => dest.ProgrammeCategories, opt =>
                {
                    opt.PreCondition(src => !(src.ProgrammeCategoryLinks is null));
                    opt.MapFrom(src =>
                        src.ProgrammeCategoryLinks.Where(x => x.ProgrammeCategory != null)
                            .Select(x => x.ProgrammeCategory.Name));
                })
                .ForMember(d => d.SalesArea,
                    opts => opts.FromEntityCache(src => src.SalesAreaId,
                        s => s.Entity<SalesArea>(x => x.Name).CheckNavigationPropertyFirst(x => x.SalesArea)));

            _ = CreateMap<Domain.Shared.Programmes.Objects.Programme, Programme>()
                .ForMember(dest => dest.Episode, opt => opt.Ignore())
                .ForMember(d => d.SalesArea, opt => opt.Ignore())
                .ForMember(d => d.SalesAreaId,
                    o => o.FromEntityCache(src => src.SalesArea, opts => opts.Entity<SalesArea>(x => x.Id)));
            ;

            _ = CreateMap<Domain.Shared.Programmes.Objects.Programme, ProgrammeDictionary>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ProgrammeName))
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
