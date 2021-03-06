﻿using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    public interface IRestrictionChecker
    {
        List<CheckRestrictionResult> CheckRestrictions(Programme programme, Break oneBreak, Spot spot, SalesArea salesArea, IReadOnlyCollection<Break> breaksBeingSmoothed, IReadOnlyCollection<Programme> scheduleProgrammes);
    }
}
