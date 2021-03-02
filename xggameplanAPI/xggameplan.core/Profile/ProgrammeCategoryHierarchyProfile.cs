using ImagineCommunications.GamePlan.Domain.ProgrammeCategory;
using xggameplan.model.External;

namespace xggameplan.core.Profile
{
    internal class ProgrammeCategoryHierarchyProfile : AutoMapper.Profile
    {
        public ProgrammeCategoryHierarchyProfile()
        {
            CreateMap<ProgrammeCategoryHierarchy, ProgrammeCategoryHierarchyModel>().ReverseMap();
        }
    }
}
