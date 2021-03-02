using TechTalk.SpecFlow;
using xggameplan.specification.tests.Infrastructure.TestAdapters;

namespace xggameplan.specification.tests.Interfaces
{
    public interface IRepositoryAdapter : IRepositoryTestContextContainer
    {
        RepositoryExecuteResult ExecuteGivenAutoAdd(int count);

        RepositoryExecuteResult ExecuteGivenAdd(Table values);

        RepositoryExecuteResult ExecuteGivenAddRange(Table entities);

        RepositoryExecuteResult ExecuteAutoAdd(int count);

        RepositoryExecuteResult ExecuteAdd(Table values);

        RepositoryExecuteResult ExecuteAddRange(Table entities);

        RepositoryExecuteResult ExecuteUpdate(Table values);

        RepositoryExecuteResult ExecuteGetAll();

        RepositoryExecuteResult ExecuteGetById(string id);

        RepositoryExecuteResult ExecuteDelete(string id);

        RepositoryExecuteResult ExecuteTruncate();

        RepositoryExecuteResult ExecuteCount();

        RepositoryExecuteResult ExecuteMethod(string methodName, Table parameters = null);

        void CheckReceivedResult(Table values);
    }
}
