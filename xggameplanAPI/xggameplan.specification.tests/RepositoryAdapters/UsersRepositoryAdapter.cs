using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Shared.System.Users;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class UsersRepositoryAdapter : RepositoryTestAdapter<User, IUsersRepository, int>
    {
        public UsersRepositoryAdapter(IScenarioDbContext dbContext, IUsersRepository repository)
            : base(dbContext, repository)
        {
        }

        protected override User Add(User model)
        {
            Repository.Insert(model);
            return model;
        }

        protected override IEnumerable<User> AddRange(params User[] models)
        {
            foreach(var user in models)
            {
                Repository.Insert(user);
            }
            return models;
        }

        protected override int Count() =>
            throw new NotImplementedException();

        protected override void Delete(int id) =>
            throw new NotImplementedException();

        protected override IEnumerable<User> GetAll() =>
            Repository.GetAll();

        protected override User GetById(int id) =>
            Repository.GetById(id);

        protected override void Truncate() =>
            throw new NotImplementedException();

        protected override User Update(User model)
        {
            Repository.Update(model);
            return model;
        }
    }
}
