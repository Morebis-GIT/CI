using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Runs.Queries;

namespace ImagineCommunications.GamePlan.Domain.Runs
{
    public interface IRunRepository
    {
        Run Find(Guid id);

        Run FindByScenarioId(Guid scenarioId);

        IEnumerable<Run> FindByIds(IEnumerable<Guid> ids);

        IEnumerable<Run> GetByScenarioId(Guid scenarioId);

        Guid GetRunIdForScenario(Guid scenarioId);

        //IEnumerable<Run> GetByPassId(int passId);

        IEnumerable<Run> GetAll();

        IEnumerable<Run> GetAllActive();

        IEnumerable<RunsWithScenarioIdTransformerResult> GetRunsWithScenarioId();

        IEnumerable<Run> FindTriggeredInLandmark();

        IEnumerable<Run> FindLandmarkRuns();

        void Add(Run run);

        void Update(Run run);

        void Remove(Guid id);

        void SaveChanges();

        PagedQueryResult<RunExtendedSearchModel> Search(RunSearchQueryModel queryModel, StringMatchRules freeTextMatchRules);

        IEnumerable<Run> GetRunsByCampaignExternalIdsAndStatus(IEnumerable<string> externalIds, RunStatus runStatus);

        IEnumerable<Run> GetRunsByDeliveryCappingGroupId(int id);

        void UpdateRange(IEnumerable<Run> runs);

        bool Exists(Guid id);
    }
}
