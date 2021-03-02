using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.SmoothPassEntities
{
    public class SmoothPassUnplacedIteration: IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int SmoothPassIterationRecordId { get; set; }

        public int Sequence { get; set; }

        public bool RespectSpotTime { get; set; }

        public bool RespectCampaignClash { get; set; }

        public ProductClashRules ProductClashRule { get; set; }

        public bool RespectRestrictions { get; set; }

        public bool RespectClashExceptions { get; set; }
    }
}
