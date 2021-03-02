using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.SmoothFailures;
using xgcore.auditevents.Repository.File;

namespace ImagineCommunications.GamePlan.Persistence.File.Repositories
{
    public class FileSmoothFailureRepository
        : FileRepositoryBase, ISmoothFailureRepository
    {
        public FileSmoothFailureRepository(string folder)
            : base(folder, "smooth_failure") { }

        public void Dispose() { }

        public void AddRange(IEnumerable<SmoothFailure> items) { }

        public IEnumerable<SmoothFailure> GetByRunId(Guid runId) =>
            GetAllByType<SmoothFailure>(_folder, _type, sf => sf.RunId == runId)
                .OrderBy(sf => sf.SalesArea)
                .ThenBy(sf => sf.TypeId)
                .ThenBy(sf => sf.ExternalSpotRef)
                .ThenBy(sf => sf.ExternalBreakRef)
                .ToList();

        public void RemoveByRunId(Guid runId)
        {
            DeleteAllItems<SmoothFailure>(_folder, _type, sf => sf.RunId == runId);
        }

        public void SaveChanges() { }

        public void Truncate() => throw new NotImplementedException();
    }
}
