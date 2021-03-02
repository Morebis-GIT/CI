using System;
using System.Collections.Generic;
using System.Linq;
using xgCore.xgGamePlan.ApiEndPoints.Models.Passes;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.LibrarySalesAreaPassPriorities
{
    public class LibrarySalesAreaPassPriorityModel
    {
        public Guid Uid { get; set; }

        public string Name { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        public bool IsDefault { get; set; }

        public string DaysOfWeek { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public IEnumerable<SalesAreaPriorityModel> SalesAreaPriorities { get; set; }

        public override bool Equals(object obj) =>
            obj is LibrarySalesAreaPassPriorityModel model &&
            Uid.Equals(model.Uid) &&
            Name == model.Name &&
            EqualityComparer<TimeSpan?>.Default.Equals(StartTime, model.StartTime) &&
            EqualityComparer<TimeSpan?>.Default.Equals(EndTime, model.EndTime) &&
            IsDefault == model.IsDefault &&
            DaysOfWeek == model.DaysOfWeek &&
            DateCreated == model.DateCreated &&
            DateModified == model.DateModified &&
            SalesAreaPriorities.Count() == model.SalesAreaPriorities.Count() &&
            SalesAreaPriorities.All(model.SalesAreaPriorities.Contains);

        public override int GetHashCode()
        {
            int hashCode = 808164088;
            hashCode = hashCode * -1521134295 + EqualityComparer<Guid>.Default.GetHashCode(Uid);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<TimeSpan?>.Default.GetHashCode(StartTime);
            hashCode = hashCode * -1521134295 + EqualityComparer<TimeSpan?>.Default.GetHashCode(EndTime);
            hashCode = hashCode * -1521134295 + IsDefault.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(DaysOfWeek);
            hashCode = hashCode * -1521134295 + DateCreated.GetHashCode();
            hashCode = hashCode * -1521134295 + DateModified.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<IEnumerable<SalesAreaPriorityModel>>.Default.GetHashCode(SalesAreaPriorities);
            return hashCode;
        }
    }
}
