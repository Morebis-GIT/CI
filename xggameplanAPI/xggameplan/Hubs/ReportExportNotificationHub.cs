using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace xggameplan.Hubs
{
    [HubName("ReportExportNotificationHub")]
    public class ReportExportNotificationHub : Hub
    {
        private IHubContext _hub => GlobalHost.ConnectionManager.GetHubContext<ReportExportNotificationHub>();

        public void NotifyGroup(ReportExportStatusNotification notification)
        {
            _ = _hub.Clients.Group(notification.reportReference).Notify(notification);
        }

        [HubMethodName("Subscribe")]
        public async Task Subscribe(string groupName)
        {
            await Groups.Add(Context.ConnectionId, groupName);
        }

        [HubMethodName("Unsubscribe")]
        public async Task Unsubscribe(string groupName)
        {
            await Groups.Remove(Context.ConnectionId, groupName);
        }
    }
}
