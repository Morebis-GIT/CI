using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant
{
    public class EmailNotificationSettings
    {
        public bool Enabled { get; set; }

        public string SenderAddress { get; set; }

        public List<string> RecipientAddresses { get; set; }

        public List<string> CCAddresses { get; set; }
    }
}
