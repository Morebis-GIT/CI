using System;

namespace xggameplan.specification.tests.Infrastructure.TestAdapters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RepositoryMethodAttribute : Attribute
    {
        public string MethodName { get; set; }
    }
}
