using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Shared.ProgrammeClassifications;
using ImagineCommunications.Gameplan.Integration.Contracts.ProgrammeClassification;

namespace ImagineCommunications.GamePlan.Intelligence.Configurations
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ProgrammeClassificationCreated, ProgrammeClassification>();
            CreateMap<IEnumerable<ProgrammeClassificationCreated>, IEnumerable<ProgrammeClassification>>();
        }
    }
}
