using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage;

namespace xggameplan.Model
{
    /// <summary>
    /// Model for details of AutoBook status
    /// </summary>
    public class GetAutoBookStatusModel
    {
        public AutoBookStatuses Status { get; set; }
    }
}
