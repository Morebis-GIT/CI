using System;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Programmes
{
    public class ProgrammeCategoryLink
    {
        public Guid ProgrammeId { get; set; }
        public int ProgrammeCategoryId { get; set; }

        public Programme Programme { get; set; }
        public ProgrammeCategory ProgrammeCategory { get; set; }
    }
}
