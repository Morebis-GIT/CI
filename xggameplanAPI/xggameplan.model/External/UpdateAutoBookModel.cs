using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage;

namespace xggameplan.Model
{
    /// <summary>
    /// Model for updating AutoBook instance
    /// </summary>
    public class UpdateAutoBookModel
    {
        public string Api { get; set; }
        public AutoBookStatuses Status { get; set; }

        public AutoBookTaskModel Task { get; set; }
    }
}
