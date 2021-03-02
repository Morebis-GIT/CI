using xggameplan.core.Hubs;

namespace xggameplan.taskexecutor
{
    public class HubNotificationStub<TModel> : IHubNotification<TModel>
        where TModel : class
    {
        public void Notify(TModel notification)
        {
        }

        public void NotifyGroup(string group, TModel notification)
        {
        }

        public void NotifyIndividual(string connectionId, TModel notification)
        {
        }
    }
}
