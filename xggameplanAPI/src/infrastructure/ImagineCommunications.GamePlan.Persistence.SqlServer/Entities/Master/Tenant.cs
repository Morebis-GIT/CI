using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master
{
    public class Tenant: IIdentityPrimaryKey, IPreviewable
    {
        public Tenant()
        {
        }

        public int Id { get; set; }

        public string Name { get; private set; }

        public string DefaultTheme { get; private set; }

        public DatabaseProviderConfiguration TenantDb { get; private set; }

        public PreviewFile Preview { get; set; }
    }
}
