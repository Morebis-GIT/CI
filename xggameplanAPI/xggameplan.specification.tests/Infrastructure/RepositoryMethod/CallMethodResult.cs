using System;

namespace xggameplan.specification.tests.Infrastructure.RepositoryMethod
{
    public class CallMethodResult
    {
        public bool ResultIsHandled { get; set; }
        public Type ResultType { get; set; }
        public object Result { get; set; }

        public static CallMethodResult CreateHandled()
        {
            return new CallMethodResult
            {
                ResultIsHandled = true
            };
        }

        public static CallMethodResult Create<TResult>(TResult result)
        {
            return new CallMethodResult
            {
                Result = result,
                ResultType = result?.GetType() ?? typeof(TResult)
            };
        }
    }
}
