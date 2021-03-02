using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;

namespace ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Queries
{
    public class RestrictionSearchQueryModel : BaseQueryModel
    {
        public List<string> SalesAreaNames { set; get; }
        public bool MatchAllSpecifiedSalesAreas { set; get; }
        public DateTime? DateRangeStart { set; get; }
        public DateTime? DateRangeEnd { set; get; }
        public RestrictionType? RestrictionType { set; get; }
        public RestrictionOrder OrderBy { set; get; }
        public OrderDirection OrderDirection { get; set; }
    }
}
