using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios.Queries;

namespace ImagineCommunications.GamePlan.Domain.Scenarios
{
    public interface IScenarioRepository
    {
        Scenario Get(Guid id);

        Scenario FindByName(string name, bool isLibraried);

        IEnumerable<Scenario> FindByIds(IEnumerable<Guid> ids);

        IEnumerable<Scenario> GetAll();

        IEnumerable<Scenario> GetLibraried();

        void Add(Scenario scenario);

        void Add(IEnumerable<Scenario> items);

        IEnumerable<Scenario> GetByPassId(int passId);

        IEnumerable<ScenariosWithPassIdTransformerResult> GetScenariosWithPassId();

        IEnumerable<ScenariosWithPassIdTransformerResult> GetScenariosWithPassId(IEnumerable<Guid> scenarioIds);

        SearchResultModel<ScenarioDigestListItem> MinimalDataSearch(
            SearchQueryDto queryModel,
            bool isLibraried,
            IEnumerable<int> passesToInclude);

        void Delete(Guid id);

        void Remove(IEnumerable<Guid> ids);

        void Update(Scenario scenario);

        void Update(IEnumerable<Scenario> scenarios);

        void SaveChanges();
    }
}
