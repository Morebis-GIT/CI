using xggameplan.specification.tests.Infrastructure.RepositoryMethod;

namespace xggameplan.specification.tests.Interfaces
{
    public interface IRepositoryMethodResolveStrategy
    {
        RepositoryMethodResolveInfo Resolve(string methodName, IRepositoryMethodParameters parameters);
    }
}
