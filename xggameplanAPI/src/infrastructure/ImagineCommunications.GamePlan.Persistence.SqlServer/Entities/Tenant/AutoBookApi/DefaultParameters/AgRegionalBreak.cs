using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.DefaultParameters
{
    public class AgRegionalBreak : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public Guid AutoBookDefaultParametersId { get; set; }

        public int TregNo { get; set; }
        public int OpenAvail { get; set; }
    }
}
