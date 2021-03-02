using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.SmoothFailures;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Models;

namespace ImagineCommunications.GamePlan.Process.Smooth.Dtos
{
    /// <summary>
    /// Result for Smooth programme
    /// </summary>
    public class SmoothProgramme
    {
        public SmoothProgramme(Programme programme) => Prog = programme;

        public Programme Prog { get; set; }

        public List<SmoothBreak> ProgSmoothBreaks { get; set; }

        public List<Recommendation> Recommendations { get; set; }

        public List<SmoothFailure> SmoothFailures { get; set; }

        public List<Spot> SpotsToBatchSave { get; } = new List<Spot>();

        public SalesArea SalesArea { get; set; }

        public int BreaksWithPreviousSpots { get; set; }

        public int BreaksWithoutPreviousSpots { get; set; }
    }
}
