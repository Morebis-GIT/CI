using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.OutputFiles
{
    public class OutputFile : IIdentityPrimaryKey
    {
        public int Id { get; set; }

        public string FileId { get; set; }

        public string Description { get; set; }

        public string AutoBookFileName { get; set; }

        public ICollection<OutputFileColumn> Columns { get; set; } = new HashSet<OutputFileColumn>();
    }
}
