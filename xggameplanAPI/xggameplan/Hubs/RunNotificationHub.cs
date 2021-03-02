using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using Microsoft.AspNet.SignalR;
using xggameplan.core.Hubs;

namespace xggameplan.Hubs
{
    public class RunNotificationHub : Hub<IClientContract<RunNotification>>
    {
    }
}
