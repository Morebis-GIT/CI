using System;

namespace ImagineCommunications.GamePlan.Domain.IndexTypes
{
    /// <summary>
    /// Index type for restrictions. E.g. HFSS
    /// </summary>
    public class IndexType
    {
        public int Id { get; set; }

        public int CustomId { get; set; }

        public string Description { get; set; }

        public string SalesArea { get; set; }

        public string DemographicNo { get; set; }

        public string BaseDemographicNo { get; set; }

        public DateTime? BreakScheduleDate { get; set; }
    }
}
