using System.ComponentModel;

namespace ImagineCommunications.GamePlan.Domain.Shared.System.Models
{
    public enum SalesAreaPriorityType
    {
        [Description("Exclude")]
        Exclude = 0,
        [Description("Priority 1")]
        Priority1 = 1,
        [Description("Priority 2")]
        Priority2 = 2,
        [Description("Priority 3")]
        Priority3 = 3
    }
}
