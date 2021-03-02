using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.SmoothFailures;
using ImagineCommunications.GamePlan.Domain.Spots;

namespace ImagineCommunications.GamePlan.Process.Smooth.Models
{
    /// <summary>
    /// A wrapper for a programme while Smoothing.
    /// </summary>
    public class SmoothProgramme
        : ICloneable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SmoothProgramme"/> class.
        /// </summary>
        /// <param name="salesArea">The sales area. Must not be <c>null</c>.</param>
        /// <param name="programme">The programme. Must not be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">
        /// salesArea
        /// or
        /// programme
        /// </exception>
        public SmoothProgramme(SalesArea salesArea, Programme programme)
        {
            if (salesArea is null)
            {
                throw new ArgumentNullException(nameof(salesArea));
            }

            if (programme is null)
            {
                throw new ArgumentNullException(nameof(programme));
            }

            SalesArea = salesArea;
            Programme = programme;
        }

        public Programme Programme { get; }

        public List<SmoothBreak> ProgrammeSmoothBreaks { get; } = new List<SmoothBreak>();

        /// <summary>Gets the break containers.</summary>
        /// <value>The break containers.</value>
        public BreakContainers BreakContainers { get; private set; }

        public List<Recommendation> Recommendations { get; } = new List<Recommendation>();

        public List<SmoothFailure> SmoothFailures { get; } = new List<SmoothFailure>();

        public List<Spot> SpotsToBatchSave { get; } = new List<Spot>();

        public SalesArea SalesArea { get; }

        /// <summary>Get the name of the sales area.</summary>
        /// <value>The name of the sales area.</value>
        public string SalesAreaName => SalesArea.Name;

        public int BreaksWithPreviousSpots { get; set; }

        public int BreaksWithoutPreviousSpots { get; set; }

        /// <summary>
        /// Initialise the programme's breaks as SmoothBreak instances and
        /// groups them by their containers.
        /// </summary>
        /// <param name="programmeBreaks"></param>
        public void InitialiseSmoothBreaks(IEnumerable<Break> programmeBreaks)
        {
            ProgrammeSmoothBreaks.Clear();

            int breakPosition = 0;

            foreach (Break oneBreak in programmeBreaks)
            {
                var smoothBreak = new SmoothBreak(oneBreak, ++breakPosition);
                smoothBreak.SmoothProgramme = this;

                ProgrammeSmoothBreaks.Add(smoothBreak);
            }

            BreakContainers = BreakContainers.GroupBreaks(programmeBreaks);
        }

        public object Clone() => (SmoothProgramme)MemberwiseClone();
    }
}
