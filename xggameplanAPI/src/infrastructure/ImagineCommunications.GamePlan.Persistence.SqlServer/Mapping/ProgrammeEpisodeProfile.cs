using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Programmes;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class ProgrammeEpisodeProfile : Profile
    {
        public ProgrammeEpisodeProfile()
        {
            _ = CreateMap<ProgrammeEpisode, Domain.Shared.Programmes.Objects.ProgrammeEpisode>()
                .ForMember(dest => dest.ProgrammeExternalReference,
                    opt => opt.MapFrom(src =>
                        src.ProgrammeDictionary == null ? null : src.ProgrammeDictionary.ExternalReference));
        }
    }
}
