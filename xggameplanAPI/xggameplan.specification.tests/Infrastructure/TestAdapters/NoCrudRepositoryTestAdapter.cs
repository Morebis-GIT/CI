using System;
using System.Collections.Generic;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.Infrastructure.TestAdapters
{
    public abstract class NoCrudRepositoryTestAdapter<TRepositoryModel, TRepository, TIdType>
        : NoCrudRepositoryTestAdapter<TRepositoryModel, TRepository, TIdType, RepositoryTestContext>
        where TRepositoryModel : class
        where TRepository : class
    {
        protected NoCrudRepositoryTestAdapter(IScenarioDbContext dbContext, TRepository repository) : base(dbContext, repository)
        {
        }
    }

    public abstract class NoCrudRepositoryTestAdapter<TRepositoryModel, TRepository, TIdType, TTestContext>
        : RepositoryTestAdapter<TRepositoryModel, TRepository, TIdType, TTestContext>
        where TRepositoryModel : class
        where TRepository : class
        where TTestContext : RepositoryTestContext, new()
    {
        protected NoCrudRepositoryTestAdapter(IScenarioDbContext dbContext, TRepository repository) : base(dbContext, repository)
        {
        }

        protected override TRepositoryModel Add(TRepositoryModel model)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<TRepositoryModel> AddRange(params TRepositoryModel[] models)
        {
            throw new NotImplementedException();
        }

        protected override TRepositoryModel Update(TRepositoryModel model)
        {
            throw new NotImplementedException();
        }

        protected override void Delete(TIdType id)
        {
            throw new NotImplementedException();
        }

        protected override int Count()
        {
            throw new NotImplementedException();
        }

        protected override void Truncate()
        {
            throw new NotImplementedException();
        }
    }
}
