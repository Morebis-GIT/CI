using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.SmoothPassEntities
{
    public class SmoothPassDefaultIteration: IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int SmoothPassIterationRecordId { get; set; }

        public int Sequence { get; set; }

        public bool RespectCampaignClash { get; set; }

        public ProductClashRules ProductClashRules { get; set; }

        public bool RespectSpotTime { get; set; }

        public SpotPositionRules BreakPositionRules { get; set; }

        public SpotPositionRules RequestedPositionInBreakRules { get; set; }

        public bool RespectRestrictions { get; set; }

        public bool RespectClashExceptions { get; set; }
    }
}
