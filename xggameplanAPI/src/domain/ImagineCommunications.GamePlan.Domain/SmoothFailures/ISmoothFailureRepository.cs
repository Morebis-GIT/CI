using System;
using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.SmoothFailures
{
    public interface ISmoothFailureRepository
    {
        void AddRange(IEnumerable<SmoothFailure> items);

        IEnumerable<SmoothFailure> GetByRunId(Guid runId);

        void RemoveByRunId(Guid runId);

        void SaveChanges();
        void Truncate();
    }
}
