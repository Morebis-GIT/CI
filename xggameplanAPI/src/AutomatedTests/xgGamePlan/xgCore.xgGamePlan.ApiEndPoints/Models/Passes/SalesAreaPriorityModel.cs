using System.Collections.Generic;
using xgCore.xgGamePlan.ApiEndPoints.Models.SalesAreas;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Passes
{
    public class SalesAreaPriorityModel
    {
        public string SalesArea { get; set; }
        public SalesAreaPriorityType Priority { get; set; }

        public override bool Equals(object obj) =>
            obj is SalesAreaPriorityModel model &&
            SalesArea == model.SalesArea &&
            Priority == model.Priority;

        public override int GetHashCode()
        {
            int hashCode = 28884451;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(SalesArea);
            hashCode = hashCode * -1521134295 + Priority.GetHashCode();
            return hashCode;
        }
    }
}
