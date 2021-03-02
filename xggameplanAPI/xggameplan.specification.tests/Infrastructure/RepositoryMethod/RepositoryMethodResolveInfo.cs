using System.Reflection;

namespace xggameplan.specification.tests.Infrastructure.RepositoryMethod
{
    public class RepositoryMethodResolveInfo
    {
        public MethodInfo Method { get; set; }
        public object Instance { get; set; }
        public object[] Parameters { get; set; }
    }
}
