using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers
{
    /// <summary>
    /// Details for testing of Smooth for a programme
    /// </summary>
    internal class TestSmoothProgrammeDetails
    {
        /// <summary>
        /// Programme
        /// </summary>
        public Programme Programme { get; set; }

        /// <summary>
        /// Smooth programme
        /// </summary>
        public SmoothProgramme SmoothProgramme { get; set; }

        /// <summary>
        /// Spots for programme
        /// </summary>
        public IReadOnlyCollection<Spot> ProgrammeSpots { get; set; }

        /// <summary>
        /// Breaks for programme
        /// </summary>
        public IReadOnlyCollection<Break> ProgrammeBreaks { get; set; }
    }
}
