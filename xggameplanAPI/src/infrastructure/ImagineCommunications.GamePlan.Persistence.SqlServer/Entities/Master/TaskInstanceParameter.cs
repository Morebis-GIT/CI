using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master
{
    public class TaskInstanceParameter : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public Guid TaskInstanceId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
