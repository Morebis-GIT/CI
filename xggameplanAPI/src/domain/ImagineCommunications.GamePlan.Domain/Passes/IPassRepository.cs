using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.Passes.Queries;

namespace ImagineCommunications.GamePlan.Domain.Passes
{
    public interface IPassRepository
    {
        Pass Get(int id);

        Pass FindByName(string name, bool isLibraried);

        IEnumerable<Pass> FindByIds(IEnumerable<int> ids);

        IEnumerable<Pass> GetAll();

        IEnumerable<int> GetLibraryIds();

        IEnumerable<Pass> FindByScenarioId(Guid scenarioId);

        SearchResultModel<PassDigestListItem> MinimalDataSearch(
            SearchQueryDto queryModel,
            bool isLibraried);

        PagedQueryResult<Pass> Search(PassSearchQueryModel queryModel, StringMatchRules freeTextMatchRules);

        void Add(Pass pass);

        void Add(IEnumerable<Pass> items);

        void Delete(int id);

        void Remove(IEnumerable<int> ids);

        void RemoveByScenarioId(Guid scenarioId);

        void Update(Pass pass);

        void Update(IEnumerable<Pass> passes);

        void SaveChanges();
    }
}