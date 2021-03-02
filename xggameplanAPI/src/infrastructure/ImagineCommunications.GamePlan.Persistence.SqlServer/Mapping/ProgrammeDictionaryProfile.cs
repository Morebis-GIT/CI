using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class ProgrammeDictionaryProfile : Profile
    {
        public ProgrammeDictionaryProfile()
        {
            _ = CreateMap<ProgrammeDictionary, Domain.ProgrammeDictionaries.ProgrammeDictionary>().ReverseMap();
        }
    }
}
