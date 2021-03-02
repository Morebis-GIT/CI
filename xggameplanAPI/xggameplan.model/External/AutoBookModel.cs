using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage;

namespace xggameplan.Model
{
    public class AutoBookModel
    {
        public string Id { get; set; }

        public string Api { get; set; }

        public AutoBookStatuses Status { get; set; }

        public AutoBookTaskModel Task { get; set; }

        public string Version { get; set; }

        public int InstanceConfigurationId { get; set; }
    }

}
