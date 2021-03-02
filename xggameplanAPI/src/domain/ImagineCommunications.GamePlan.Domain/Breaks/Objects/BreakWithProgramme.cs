using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Types;

namespace ImagineCommunications.GamePlan.Domain.Breaks.Objects
{
    public class BreakWithProgramme
    {
        public Break Break { get; set; }

        public List<string> ProgrammeCategories { get; set; }

        public int ProgrammeNo { get; set; }

        public int PrgtNo { get; set; }

        public string ProgrammeExternalreference { get; set; }

        public List<Rating> Predictions { get; set; }

        public BreakPosition PositionInProgram { get; set; }

        public int? EpisodeNumber { get; set; }

        /// <summary>Sets the position in program based on the internal <see cref="Break"/> position in program value.</summary>
        public void SetBreakPositionInProgram()
        {
            PositionInProgram = Break.PositionInProg;
        }

        /// <summary>
        /// Sets the position in program based on the specified parameters.
        /// </summary>
        /// <param name="totalCountZeroBased">Index of the last element inside calculation group.</param>
        /// <param name="currentPosition">Current position of the break inside calculation group.</param>
        public void SetPositionInProgram(int totalCountZeroBased, int currentPosition)
        {
            PositionInProgram = currentPosition < totalCountZeroBased
                ? BreakPosition.C
                : BreakPosition.E;
        }

        /// <summary>
        /// Indexes list by SalesArea and ExternalBreakRef
        /// </summary>
        /// <param name="breaksWithProgramme"></param>
        /// <returns></returns>
        public static Dictionary<string, BreakWithProgramme> IndexListBySalesAreaAndExternalBreakRef(
            IEnumerable<BreakWithProgramme> breaksWithProgramme)
        {
            var breakWithProgrammesIndexed = new Dictionary<string, BreakWithProgramme>();

            foreach (var breakWithProgamme in breaksWithProgramme)
            {
                var breakKey = GetIndexKeyBySalesAreaAndExternalBreakRef(breakWithProgamme.Break.SalesArea,
                    breakWithProgamme.Break.ExternalBreakRef);

                if (!breakWithProgrammesIndexed.ContainsKey(breakKey))
                {
                    breakWithProgrammesIndexed.Add(breakKey, breakWithProgamme);
                }
            }

            return breakWithProgrammesIndexed;
        }

        public static string GetIndexKeyBySalesAreaAndExternalBreakRef(string salesArea, string externalBreakRef)
        {
            return string.Format("{0}{1}{2}", salesArea, (char)0, externalBreakRef);
        }
    }
}
