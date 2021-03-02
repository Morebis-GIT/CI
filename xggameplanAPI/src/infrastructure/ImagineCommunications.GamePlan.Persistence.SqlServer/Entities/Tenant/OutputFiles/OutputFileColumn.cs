using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.OutputFiles
{
    public class OutputFileColumn : IIdentityPrimaryKey
    {
        public int Id { get; set; }

        public int OutputFileId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string DataType { get; set; }
    }
}
