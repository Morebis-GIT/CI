using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage.Objects;

namespace xggameplan.AutoBooks
{
    public interface IAutoBookTaskReportRepository
    {
        AutoBookTaskReport Get(int Id);

        void Add(AutoBookTaskReport item);

        IEnumerable<AutoBookTaskReport> GetAll();

        IEnumerable<AutoBookTaskReport> GetAllByRunId(Guid runId);

        AutoBookTaskReport GetByScenarioId(Guid scenarioId);

        void DeleteRange(IEnumerable<AutoBookTaskReport> autoBookTaskReports);

        void Delete(int id);

        void SaveChanges();
    }
}
