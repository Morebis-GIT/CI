using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs
{
    public class RunAuthor : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public Guid RunId { get; set; }
        public int AuthorId { get; set; }             // User.Id?
        public string Name { get; set; }
    }
}
