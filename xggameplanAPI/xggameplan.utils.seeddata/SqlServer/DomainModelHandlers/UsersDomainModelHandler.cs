using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.System.Users;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using MasterEntities = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class UsersDomainModelHandler : IDomainModelHandler<User>
    {
        private readonly IUsersRepository _usersRepository;
        private readonly ISqlServerMasterDbContext _dbContext;

        public UsersDomainModelHandler(IUsersRepository usersRepository, ISqlServerMasterDbContext dbContext)
        {
            _usersRepository = usersRepository;
            _dbContext = dbContext;
        }

        public User Add(User model)
        {
            _usersRepository.Insert(model);
            return model;
        }

        public void AddRange(params User[] models)
        {
            foreach (var model in models)
            {
                _ = Add(model);
            }
        }

        public int Count() => _dbContext.Query<MasterEntities.User>().Count();

        public void DeleteAll() => _dbContext.Truncate<MasterEntities.User>();

        public IEnumerable<User> GetAll() => _usersRepository.GetAll();
    }
}
