using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using Microsoft.AspNet.SignalR;
using xggameplan.core.Hubs;

namespace xggameplan.Hubs
{
    public class ScenarioNotificationHub : Hub<IClientContract<ScenarioNotificationModel>>
    {
    }
}
