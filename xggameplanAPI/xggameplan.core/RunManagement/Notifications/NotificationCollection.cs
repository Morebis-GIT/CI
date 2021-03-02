using System.Collections.Generic;
using xggameplan.RunManagement.Notifications;

namespace xggameplan.core.RunManagement.Notifications
{
    public class NotificationCollection : INotificationCollection
    {
        private readonly List<object> _notifications = new List<object>();

        public void Add<T>(INotification<T> notification)
        {
            _notifications.Add(notification);
        }

        public INotification<T> GetNotification<T>()
        {
            foreach (object notification in _notifications)
            {
                if (notification is INotification<T>)
                {
                    return (INotification<T>)notification;
                }
            }
            return null;
        }
    }
}
