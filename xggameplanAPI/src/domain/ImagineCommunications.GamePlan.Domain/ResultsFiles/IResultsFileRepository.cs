using System;

namespace ImagineCommunications.GamePlan.Domain.ResultsFiles
{
    public interface IResultsFileRepository
    {
        void Insert(Guid scenarioId, string fileId, string localFolder);

        void Get(Guid scenarioId, string fileId, bool compressed, string localFolder);

        void Delete(Guid scenario, string fileId);

        bool Exists(Guid scenarioId, string fileId);
    }
}
