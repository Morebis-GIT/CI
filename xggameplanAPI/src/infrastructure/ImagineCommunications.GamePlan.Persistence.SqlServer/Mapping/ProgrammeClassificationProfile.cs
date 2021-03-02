using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class ProgrammeClassificationProfile : Profile
    {
        public ProgrammeClassificationProfile()
        {
            CreateMap<ProgrammeClassification, Domain.Shared.ProgrammeClassifications.ProgrammeClassification>().ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
