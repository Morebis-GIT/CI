using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Programmes;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Schedules
{
    public class ScheduleProgramme : IUniqueIdentifierPrimaryKey
    {
        public Guid ProgrammeId { get; set; }
        public int ScheduleId { get; set; }

        public Programme Programme { get; set; }
        public Schedule Schedule { get; set; }

        Guid ISinglePrimaryKey<Guid>.Id
        {
            get => ProgrammeId;
            set => ProgrammeId = value;
        }
    }
}
