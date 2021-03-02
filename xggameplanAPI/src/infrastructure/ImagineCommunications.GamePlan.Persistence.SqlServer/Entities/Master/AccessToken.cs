using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master
{
    public class AccessToken: IIdentityPrimaryKey
    {
        public int Id { get; set; }

        public string Token { get; set; }

        public int UserId { get; set; }

        public DateTime ValidUntilValue { get; set; }
    }
}
