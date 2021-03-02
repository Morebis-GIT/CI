using System.Collections.Generic;

namespace xggameplan.common.Model
{
    public class GroupResult
    {
        public object Key { get; set; }

        public int Count { get; set; }

        public double Total { get; set; }

        public IEnumerable<GroupResult> SubGroups { get; set; }

        public override string ToString() => $"{Key} ({Count})";
    }
}
