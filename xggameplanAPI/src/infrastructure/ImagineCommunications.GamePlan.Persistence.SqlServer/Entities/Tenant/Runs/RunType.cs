using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant
{
    public class RunType : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Hidden { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string DefaultKPI { get; set; }

        public ICollection<RunTypeAnalysisGroup> RunTypeAnalysisGroups { get; set; } = new HashSet<RunTypeAnalysisGroup>();

        public RunLandmarkScheduleSettings RunLandmarkScheduleSettings { get; set; }
    }
}
