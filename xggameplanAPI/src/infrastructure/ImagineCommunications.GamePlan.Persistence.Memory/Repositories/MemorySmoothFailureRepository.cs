using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.SmoothFailures;

namespace ImagineCommunications.GamePlan.Persistence.Memory.Repositories
{
    public class MemorySmoothFailureRepository :
        MemoryRepositoryBase<SmoothFailure>,
        ISmoothFailureRepository
    {
        public void Dispose() { }

        public void AddRange(IEnumerable<SmoothFailure> items) { }

        public IEnumerable<SmoothFailure> GetByRunId(Guid runId)
        {
            return GetAllItems(sf => sf.RunId == runId)
                .OrderBy(sf => sf.SalesArea)
                .ThenBy(sf => sf.TypeId)
                .ThenBy(sf => sf.ExternalSpotRef)
                .ThenBy(sf => sf.ExternalBreakRef)
                .ToList();
        }

        public void RemoveByRunId(Guid runId)
        {
            DeleteAllItems(sf => sf.RunId == runId);
        }

        public void SaveChanges() { }

        public void Truncate()
        {
            DeleteAllItems();
        }
    }
}
