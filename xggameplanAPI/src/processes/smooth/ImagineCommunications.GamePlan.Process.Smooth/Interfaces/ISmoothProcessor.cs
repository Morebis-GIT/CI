using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;

namespace ImagineCommunications.GamePlan.Process.Smooth.Interfaces
{
    public interface ISmoothProcessor
    {
        void Execute(
            Run run,
            IReadOnlyList<SalesArea> salesAreas,
            Action<string> raiseInfo,
            Action<string> raiseWarning,
            Action<string, Exception> raiseException);
    }
}
