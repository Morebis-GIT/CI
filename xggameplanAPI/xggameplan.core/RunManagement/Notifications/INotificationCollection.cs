using xggameplan.RunManagement.Notifications;

namespace xggameplan.core.RunManagement.Notifications
{
    public interface INotificationCollection
    {
        void Add<T>(INotification<T> notification);

        INotification<T> GetNotification<T>();
    }
}
