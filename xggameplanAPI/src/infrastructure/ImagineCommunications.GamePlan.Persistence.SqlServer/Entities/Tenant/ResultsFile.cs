using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant
{
    public class ResultsFile : IIdentityPrimaryKey
    {
        public int Id { get; set; }
            
        public Guid ScenarioId { get; set; }

        public string FileId { get; set; }

        public bool IsCompressed { get; set; }

        public byte[] FileContent { get; set; }
    }
}
