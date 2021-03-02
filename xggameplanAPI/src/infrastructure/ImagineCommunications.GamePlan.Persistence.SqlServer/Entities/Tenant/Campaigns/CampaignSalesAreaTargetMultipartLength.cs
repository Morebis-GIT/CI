using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns
{
    public class CampaignSalesAreaTargetMultipartLength : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int CampaignSalesAreaTargetMultipartId { get; set; }
        public TimeSpan Length { get; set; }
        public int BookingPosition { get; set; }
        public int Sequencing { get; set; }
    }
}
