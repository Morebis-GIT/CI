using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.DefaultParameters
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>")]
    public class AgCampaignProgramme : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public Guid AutoBookDefaultParametersId { get; set; }

        public string StartDate { get; set; }
        public string EndDate { get; set; }        
        public double AgCampaignProgrammeRequirement_Required { get; set; }
        public double AgCampaignProgrammeRequirement_TgtRequired { get; set; }
        public double AgCampaignProgrammeRequirement_SareRequired { get; set; }
        public double AgCampaignProgrammeRequirement_Supplied { get; set; }
        public int NbrCategoryOrProgrammeList { get; set; }
        public List<AgCampaignProgrammeProgrammeCategory> CategoryOrProgramme { get; set; }
        public int NbrSalesAreas { get; set; }
        public List<int> SalesAreas { get; set; }
        public int NumberTimeBands { get; set; }
        public List<AgTimeBand> TimeBands { get; set; }
    }
}
