using System;
using System.IO;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces
{
    public interface IResultsFileStorage
    {
        void Insert(Guid scenarioId, string fileId, Stream stream, bool compress);
        Stream Get(Guid scenarioId, string fileId, bool compressed);
        bool Delete(Guid scenarioId, string fileId);
        bool Exists(Guid scenarioId, string fileId);
        void Clear();
        void Flush();

        int Count { get; }
    }
}
