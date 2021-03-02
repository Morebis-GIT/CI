using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class ProgrammeCategoryProfile : Profile
    {
        public ProgrammeCategoryProfile()
        {
            CreateMap<ProgrammeCategoryHierarchy, Domain.ProgrammeCategory.ProgrammeCategoryHierarchy>().ReverseMap();
        }
    }
}
