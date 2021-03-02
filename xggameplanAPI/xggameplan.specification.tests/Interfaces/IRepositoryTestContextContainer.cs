using xggameplan.specification.tests.Infrastructure.TestAdapters;

namespace xggameplan.specification.tests.Interfaces
{
    public interface IRepositoryTestContextContainer
    {
        RepositoryTestContext GetTestContext();

        TTestContext GetTestContext<TTestContext>() where TTestContext : RepositoryTestContext;
    }
}
