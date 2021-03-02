using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using xggameplan.core.Hubs;

namespace xggameplan.Hubs
{
    [HubName("ExternalRunNotificationHub")]
    public class LandmarkRunStatusNotificationHub : Hub<IClientContract<LandmarkRunStatusNotification>>
    {
    }
}
