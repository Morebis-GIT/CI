using xggameplan.specification.tests.Infrastructure.RepositoryMethod;

namespace xggameplan.specification.tests.Interfaces
{
    public interface IRepositoryMethodExecutor
    {
        CallMethodResult Execute(string methodName, IRepositoryMethodParameters parameters = null);
    }
}
