using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ClashExceptions
{
    public class ClashException : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ClashExceptionType FromType { get; set; }
        public ClashExceptionType ToType { get; set; }
        public IncludeOrExclude IncludeOrExclude { get; set; }
        public string FromValue { get; set; }
        public string ToValue { get; set; }
        public string ExternalRef { get; set; }

        public ICollection<ClashExceptionsTimeAndDow> ClashExceptionsTimeAndDows { get; set; } =
            new List<ClashExceptionsTimeAndDow>();
    }
}
