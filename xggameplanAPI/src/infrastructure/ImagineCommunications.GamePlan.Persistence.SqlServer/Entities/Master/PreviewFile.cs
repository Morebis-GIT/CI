using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master
{
    public class PreviewFile : IIdentityPrimaryKey
    {
        public PreviewFile() { }

        public int Id { get; set; }
        public string Location { get; set; }

        public string FileName { get; set; }

        public string FileExtension { get; set; }
        public string ContentType { get; set; }
        public long? ContentLength { get; set; }

        public int? UserId { get; set; }

        public int? TenantId { get; set; }

        public byte[] Content { get; private set; }

        public void SetContentBytes(byte[] value) => Content = value;

    }
}
