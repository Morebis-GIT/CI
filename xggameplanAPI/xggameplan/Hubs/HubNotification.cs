using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using xggameplan.core.Hubs;

namespace xggameplan.Hubs
{
    public class HubNotification<THub, TModel> : IHubNotification<TModel>
        where THub : Hub<IClientContract<TModel>>
        where TModel : class
    {
        private readonly string _hubName;
        private IHubContext<IClientContract<TModel>> _hubContext;

        protected virtual IHubContext<IClientContract<TModel>> HubContext =>
            _hubContext ??
            (_hubContext = GlobalHost.ConnectionManager.GetHubContext<THub, IClientContract<TModel>>());

        public virtual IHubConnectionContext<IClientContract<TModel>> Clients => HubContext.Clients;

        public virtual void Notify(TModel notification) =>
            Clients.All.Notify(notification);

        public virtual void NotifyGroup(string group, TModel notification) =>
            Clients.Group(group).Notify(notification);

        public virtual void NotifyIndividual(string connectionId, TModel notification) =>
            Clients.Client(connectionId).Notify(notification);
    }
}
