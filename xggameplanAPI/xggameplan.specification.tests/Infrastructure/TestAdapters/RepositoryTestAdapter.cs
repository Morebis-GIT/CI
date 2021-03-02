using xggameplan.specification.tests.Infrastructure.TestModelConverters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.Infrastructure.TestAdapters
{
    public abstract class RepositoryTestAdapter<TRepositoryModel, TRepository, TIdType> :
        RepositoryTestAdapter<TRepositoryModel, TRepository, TIdType, RepositoryTestContext>
        where TRepositoryModel : class
        where TRepository : class
    {
        protected RepositoryTestAdapter(IScenarioDbContext dbContext, TRepository repository) : base(dbContext, repository)
        {
        }
    }

    public abstract class RepositoryTestAdapter<TRepositoryModel, TRepository, TIdType, TTestContext> :
        ConvertibleRepositoryTestAdapter<TRepositoryModel, TRepositoryModel, TRepository, TIdType, TTestContext>
        where TRepositoryModel : class
        where TRepository : class
        where TTestContext : RepositoryTestContext, new()
    {
        private ITestModelConverter<TRepositoryModel, TRepositoryModel> _modelConverter;

        protected RepositoryTestAdapter(IScenarioDbContext dbContext, TRepository repository) : base(dbContext, repository)
        {
        }

        protected override ITestModelConverter<TRepositoryModel, TRepositoryModel> ModelConverter =>
            _modelConverter ?? (_modelConverter = new ReferenceTestModelConverter<TRepositoryModel>());
    }
}
