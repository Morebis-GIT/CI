using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.DefaultParameters
{
    public class AgAval : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public Guid AutoBookDefaultParametersId { get; set; }

        public int SalesAreaNo { get; set; }
        public int OpenAvail { get; set; }
    }
}
