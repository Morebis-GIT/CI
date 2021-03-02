using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Shared.System.Preview;

namespace ImagineCommunications.GamePlan.Domain.Shared.System.Users
{
    public interface IUsersRepository : IPreviewFileStorage
    {
        User GetById(int id);

        IEnumerable<User> GetByIds(List<int> ids);

        User GetByEmail(string email);

        void Update(User user);

        void Insert(User user);

        List<User> GetAll();

        void SaveChanges();

        byte[] GetContent(int entityId);
    }
}
