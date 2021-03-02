using System;
using System.Collections.Generic;
using xggameplan.specification.tests.Infrastructure.RepositoryMethod;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;
using ImagineCommunications.GamePlan.Domain.Shared.System.AccessTokens;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class AccessTokenRepositoryAdapter : RepositoryTestAdapter<AccessToken, IAccessTokensRepository, string>
    {
        public AccessTokenRepositoryAdapter(
            IScenarioDbContext dbContext,
            IAccessTokensRepository repository
            ) : base(dbContext, repository) { }

        protected override AccessToken Add(AccessToken model)
        {
            Repository.Insert(model);
            return model;
        }

        protected override IEnumerable<AccessToken> AddRange(params AccessToken[] models) =>
            throw new NotImplementedException();

        protected override int Count() =>
            throw new NotImplementedException();

        protected override void Delete(string id) =>
            Repository.Delete(id);

        protected override IEnumerable<AccessToken> GetAll() =>
            throw new NotImplementedException();

        protected override AccessToken GetById(string id) =>
            Repository.Find(id);

        protected override void Truncate() =>
            throw new NotImplementedException();

        protected override AccessToken Update(AccessToken model) =>
            throw new NotImplementedException();

        [RepositoryMethod]
        protected CallMethodResult RemoveAllExpired()
        {
            _ = Repository.RemoveAllExpired();
            DbContext.SaveChanges();
            return CallMethodResult.CreateHandled();
        }
    }
}
