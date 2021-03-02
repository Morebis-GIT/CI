using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Dto.Internal;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.FunctionalAreas;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class FunctionalAreaProfile : Profile
    {
        public FunctionalAreaProfile()
        {
            //To DTO
            CreateMap<Domain.Shared.FunctionalAreas.Objects.FunctionalArea, FunctionalArea>()
                .ForMember(d => d.Descriptions, opt => opt.MapFrom(s => BuildFunctionalAreaDescription(s)))
                .ForMember(d => d.FunctionalAreaFaultTypes, opt => opt.MapFrom(s => BuildFunctionalAreaFaultTypes(s)));

            //From DTO
            CreateMap<FunctionalArea, FunctionalAreaDto>()
                .ForMember(d => d.Description,
                    opt => opt.MapFrom(
                        s => s.Descriptions.ToDictionary(x => x.LanguageAbbreviation, x => x.Description)))
                .ForMember(d => d.FaultTypeSelections,
                    opt => opt.MapFrom(s =>
                        s.FunctionalAreaFaultTypes.ToDictionary(k => k.FaultTypeId, v => v.IsSelected)));

            CreateMap<FunctionalAreaFaultType, Domain.Shared.FunctionalAreas.Objects.FaultType>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.FaultTypeId))
                .ForMember(d => d.Description,
                    opt => opt.MapFrom(s =>
                        s.FaultType.Descriptions.ToDictionary(x => x.LanguageAbbreviation, x => x.Description)))
                .ForMember(d => d.ShortName,
                    opt => opt.MapFrom(s =>
                        s.FaultType.Descriptions.ToDictionary(x => x.LanguageAbbreviation, x => x.ShortName)));

            CreateMap<FaultType, FaultTypeDto>()
                .ForMember(d => d.IsSelected, opt => opt.MapFrom(new FaultTypeSelectionValueResolver()))
                .ForMember(d => d.Description,
                    opt => opt.MapFrom(
                        s => s.Descriptions.ToDictionary(x => x.LanguageAbbreviation, x => x.Description)))
                .ForMember(d => d.ShortName,
                    opt => opt.MapFrom(
                        s => s.Descriptions.ToDictionary(x => x.LanguageAbbreviation, x => x.ShortName)));

            CreateMap<FaultType, Domain.Shared.FunctionalAreas.Objects.FaultType>()
                .ForMember(d => d.IsSelected, opt => opt.MapFrom(s => s.FunctionalAreaFaultType.IsSelected))
                .ForMember(d => d.Description,
                    opt => opt.MapFrom(
                        s => s.Descriptions.ToDictionary(x => x.LanguageAbbreviation, x => x.Description)))
                .ForMember(d => d.ShortName,
                    opt => opt.MapFrom(
                        s => s.Descriptions.ToDictionary(x => x.LanguageAbbreviation, x => x.ShortName)));
        }

        private static ICollection<FunctionalAreaFaultType> BuildFunctionalAreaFaultTypes(
            Domain.Shared.FunctionalAreas.Objects.FunctionalArea functionalArea)
        {
            return functionalArea.FaultTypes.Select(x => new FunctionalAreaFaultType
            {
                FunctionalAreaId = functionalArea.Id,
                FaultTypeId = x.Id,
                IsSelected = x.IsSelected,
                FaultType = new FaultType
                {
                    Id = x.Id,
                    Descriptions = BuildFaultAreaDescription(x)
                }
            }).ToList();
        }

        private static ICollection<FunctionalAreaDescription> BuildFunctionalAreaDescription(Domain.Shared.FunctionalAreas.Objects.FunctionalArea functionalArea)
        {
            return functionalArea.Description.Select(x => new FunctionalAreaDescription
            {
                FunctionalAreaId = functionalArea.Id,
                LanguageAbbreviation = x.Key,
                Description = x.Value
            }).ToList();
        }

        private static ICollection<FaultTypeDescription> BuildFaultAreaDescription(Domain.Shared.FunctionalAreas.Objects.FaultType faultType)
        {
            return faultType.Description.Join(faultType.ShortName, x => x.Key, y => y.Key, (x, y) => new FaultTypeDescription
            {
                FaultTypeId = faultType.Id,
                LanguageAbbreviation = x.Key,
                Description = x.Value,
                ShortName = y.Value
            }).ToList();
        }

        private class FaultTypeSelectionValueResolver : IValueResolver<FaultType, FaultTypeDto, bool>
        {
            public bool Resolve(FaultType source, FaultTypeDto destination, bool destMember, ResolutionContext context) =>
                context.Options.TryGetFaultTypeSelectionValue(out var value) && value;
        }
    }
}
