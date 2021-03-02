using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Restrictions
{
    public class Restriction : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public Guid Uid { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string RestrictionDays { get; set; }
        public IncludeOrExcludeOrEither SchoolHolidayIndicator { get; set; }
        public IncludeOrExcludeOrEither PublicHolidayIndicator { get; set; }
        public IncludeOrExclude LiveProgrammeIndicator { get; set; }
        public RestrictionType RestrictionType { get; set; }
        public RestrictionBasis RestrictionBasis { get; set; }
        public string ExternalProgRef { get; set; }
        public string ProgrammeCategory { get; set; }
        public string ProgrammeClassification { get; set; }
        public IncludeOrExclude ProgrammeClassificationIndicator { get; set; }
        public int TimeToleranceMinsBefore { get; set; }
        public int TimeToleranceMinsAfter { get; set; }
        public int IndexType { get; set; }
        public int IndexThreshold { get; set; }
        public string ProductCode { get; set; }
        public string ClashCode { get; set; }
        public string ClearanceCode { get; set; }
        public string ClockNumber { get; set; }
        public string ExternalIdentifier { get; set; }
        public List<RestrictionSalesArea> SalesAreas { get; set; } = new List<RestrictionSalesArea>();
        public int EpisodeNumber { get; set; }
    }
}
