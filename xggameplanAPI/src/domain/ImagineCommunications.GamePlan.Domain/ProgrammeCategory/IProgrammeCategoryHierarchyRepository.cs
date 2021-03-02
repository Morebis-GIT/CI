using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.ProgrammeCategory
{
    public interface IProgrammeCategoryHierarchyRepository
    {
        IEnumerable<ProgrammeCategoryHierarchy> GetAll();
        void AddRange(IEnumerable<ProgrammeCategoryHierarchy> programmeCategories);
        void SaveChanges();
        void Truncate();
        IEnumerable<ProgrammeCategoryHierarchy> Search(IEnumerable<string> programmeCategoryNames);
        ProgrammeCategoryHierarchy Get(int id);
    }
}
