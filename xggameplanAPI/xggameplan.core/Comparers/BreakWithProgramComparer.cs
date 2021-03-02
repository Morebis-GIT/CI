using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;

namespace xggameplan.core.Comparers
{
    public class BreakWithProgramComparer : EqualityComparer<BreakWithProgramme>
    {
        public override bool Equals(BreakWithProgramme x, BreakWithProgramme y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            return x.Break.Id == y.Break.Id;
        }

        public override int GetHashCode(BreakWithProgramme obj)
        {
            return obj != null ? obj.Break.Id.GetHashCode() : 0;
        }
    }
}
