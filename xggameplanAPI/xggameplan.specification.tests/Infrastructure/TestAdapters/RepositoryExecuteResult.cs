using System;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.Infrastructure.TestAdapters
{
    public class RepositoryExecuteResult : IRepositoryTestContextContainer
    {
        private readonly RepositoryTestContext _testContext;

        public RepositoryExecuteResult(RepositoryTestContext testContext)
        {
            _testContext = testContext;
        }

        public RepositoryExecuteResult HandleError(bool throwException)
        {
            if (throwException && _testContext.LastException != null)
            {
                throw _testContext.LastException;
            }

            return this;
        }

        public RepositoryExecuteResult ThrowExceptionIfExists()
        {
            return HandleError(true);
        }

        public RepositoryTestContext GetTestContext()
        {
            return _testContext;
        }

        public TTestContext GetTestContext<TTestContext>() where TTestContext : RepositoryTestContext
        {
            var res = _testContext as TTestContext;
            if (res == null)
            {
                throw new Exception(
                    $"{typeof(TTestContext).Name} test context type can't be returned as requested {typeof(TTestContext).Name} type.");
            }

            return res;
        }
    }
}
