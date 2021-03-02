using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Models;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    public interface IClashExceptionChecker
    {
        List<CheckClashExceptionResult> CheckClashExceptions(SmoothBreak smoothBreak, Spot spot);
    }
}
