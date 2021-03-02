using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.DefaultParameters
{
    public class AgMultiPart : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int AgLengthId { get; set; }

        public int CampaignNo { get; set; }
        public int SalesAreaNo { get; set; }
        public int MultiPartNo { get; set; }
        public int SeqNo { get; set; }
        public int SpotLength { get; set; }
        public int BookingPosition { get; set; }
        public double PriceFactor { get; set; }
    }
}
