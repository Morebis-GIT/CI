namespace xggameplan.core.Hubs
{
    public interface IHubNotification<in TModel> where TModel : class
    {
        void Notify(TModel notification);

        void NotifyGroup(string group, TModel notification);

        void NotifyIndividual(string connectionId, TModel notification);
    }
}
