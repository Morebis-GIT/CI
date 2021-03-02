using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Models;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    internal class NullClashExceptionChecker
        : IClashExceptionChecker
    {
        public List<CheckClashExceptionResult> CheckClashExceptions(
            SmoothBreak smoothBreak,
            Spot spot)
        {
            return Enumerable.Empty<CheckClashExceptionResult>().ToList();
        }
    }
}
