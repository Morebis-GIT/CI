using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.DefaultParameters
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>")]
    public class AgCampaignSalesArea : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public Guid AutoBookDefaultParametersId { get; set; }
        public NestedType Type { get; set; }

        public int SalesAreaNo { get; set; }
        public int ChannelGroupNo { get; set; }
        public int CampaignNo { get; set; }
        public double RevenuePercentage { get; set; }
        public int MultiPartOnly { get; set; }
        public int AgCampaignSalesAreaPtrRef_SalesAreaNo { get; set; }
        public int AgCampaignSalesAreaPtrRef_ClassId { get; set; }
        public double AgSalesAreaCampaignRequirement_Required { get; set; }
        public double AgSalesAreaCampaignRequirement_TgtRequired { get; set; }
        public double AgSalesAreaCampaignRequirement_SareRequired { get; set; }        
        public double AgSalesAreaCampaignRequirement_Supplied { get; set; }
        public int NbrAgLengths { get; set; }
        public List<AgLength> AgLengths { get; set; }
        public int MaxBreaks { get; set; }
        public int NbrAgStrikeWeights { get; set; }
        public List<AgStrikeWeight> AgStrikeWeights { get; set; }
        public int NbrAgDayParts { get; set; }
        public List<AgDayPart> AgDayParts { get; set; }
        public int NbrParts { get; set; }
        public List<AgPart> AgParts { get; set; }
        public int NbrPartsLengths { get; set; }
        public List<AgPartLength> AgPartsLengths { get; set; }
        public int CentreBreakRatio { get; set; }
        public int EndBreakRatio { get; set; }
    }

    public enum NestedType
    {
        CollectionItem,
        TypeMember        
    }
}
