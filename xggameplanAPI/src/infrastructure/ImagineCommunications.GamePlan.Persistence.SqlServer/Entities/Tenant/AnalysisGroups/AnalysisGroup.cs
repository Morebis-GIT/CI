using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AnalysisGroups
{
    public class AnalysisGroup : IIdentityPrimaryKey, IAuditEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public int CreatedBy { get; set; }
        public DateTime DateModified { get; set; }
        public int ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<string> FilterAdvertiserExternalIds { get; set; } = new HashSet<string>();
        public ICollection<string> FilterAgencyExternalIds { get; set; } = new HashSet<string>();
        public ICollection<string> FilterAgencyGroupCodes { get; set; } = new HashSet<string>();
        public ICollection<string> FilterBusinessTypes { get; set; } = new HashSet<string>();
        public ICollection<string> FilterCampaignExternalIds { get; set; } = new HashSet<string>();
        public ICollection<string> FilterClashExternalRefs { get; set; } = new HashSet<string>();
        public ICollection<string> FilterProductExternalIds { get; set; } = new HashSet<string>();
        public ICollection<string> FilterReportingCategories { get; set; } = new HashSet<string>();
        public ICollection<int> FilterSalesExecExternalIds { get; set; } = new HashSet<int>();
    }
}
