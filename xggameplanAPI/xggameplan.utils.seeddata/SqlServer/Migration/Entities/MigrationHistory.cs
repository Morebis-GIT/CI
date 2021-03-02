using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace xggameplan.utils.seeddata.SqlServer.Migration.Entities
{
    public class MigrationHistory : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public string CollectionName { get; set; }
        public DateTime Date { get; set; }
    }
}
