using ImagineCommunications.GamePlan.Domain.Shared;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using xggameplan.core.Hubs;

namespace xggameplan.Hubs
{
    [HubName("InformationalNotificationHub")]
    public class InfoMessageNotificationHub : Hub<IClientContract<InfoMessageNotification>>
    {
    }
}
