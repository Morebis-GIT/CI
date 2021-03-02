using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.AnalysisGroups.Objects;

namespace ImagineCommunications.GamePlan.Domain.AnalysisGroups
{
    public interface IAnalysisGroupRepository
    {
        void Add(AnalysisGroup analysisGroup);

        void Delete(int id);

        IEnumerable<AnalysisGroup> GetAll();

        AnalysisGroup Get(int id);

        AnalysisGroup GetIncludingSoftDeleted(int id);

        IEnumerable<AnalysisGroupNameModel> GetByIds(IEnumerable<int> ids, bool onlyActive = false);

        AnalysisGroup GetByName(string name);

        void SaveChanges();

        void Update(AnalysisGroup analysisGroup);
    }
}
