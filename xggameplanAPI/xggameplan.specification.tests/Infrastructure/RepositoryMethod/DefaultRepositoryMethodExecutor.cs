using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Generic.Helpers;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.Infrastructure.RepositoryMethod
{
    public class DefaultRepositoryMethodExecutor<TRepository> : IRepositoryMethodExecutor
        where TRepository : class
    {
        private readonly TRepository _repository;
        private readonly IRepositoryAdapter _repositoryAdapter;


        private IEnumerable<IRepositoryMethodResolveStrategy> _repositoryMethodResolveStrategies;

        protected IEnumerable<IRepositoryMethodResolveStrategy> RepositoryMethodResolveStrategies =>
            _repositoryMethodResolveStrategies ?? (_repositoryMethodResolveStrategies =
                new IRepositoryMethodResolveStrategy[]
                {
                    new RepositoryAdapterMethodResolveStrategy<IRepositoryAdapter>(_repositoryAdapter),
                    new GenericTypeRepositoryMethodResolveStrategy<TRepository>(_repository)
                });

        protected virtual RepositoryMethodResolveInfo ResolveRepositoryMethod(
            string methodName, IRepositoryMethodParameters parameters)
        {
            RepositoryMethodResolveInfo res = null;
            foreach (var strategy in RepositoryMethodResolveStrategies)
            {
                res = strategy.Resolve(methodName, parameters);
                if (res != null)
                {
                    return res;
                }
            }

            throw new Exception($"'{methodName}' repository method hasn't been found for the specified set of parameters.");
        }

        public DefaultRepositoryMethodExecutor(TRepository repository, IRepositoryAdapter repositoryAdapter)
        {
            _repository = repository;
            _repositoryAdapter = repositoryAdapter;
        }

        public CallMethodResult Execute(string methodName, IRepositoryMethodParameters parameters = null)
        {
            object methodResult;
            var resolveInfo = ResolveRepositoryMethod(methodName, parameters);
            try
            {
                methodResult = resolveInfo.Method.Invoke(resolveInfo.Instance, resolveInfo.Parameters);

                if (methodResult is Task task)
                {
                    methodResult = TaskReflectionHelper.WaitForTask(task);
                }
            }
            catch (TargetInvocationException ex) when (ex.InnerException != null)
            {
                try
                {
                    throw ex.InnerException;
                }
                catch (AggregateException e)
                {
                    throw e.Flatten();
                }
            }

            if (typeof(CallMethodResult).IsAssignableFrom(resolveInfo.Method.ReturnType))
            {
                return methodResult as CallMethodResult;
            }

            return new CallMethodResult
            {
                Result = methodResult,
                ResultType = resolveInfo.Method.ReturnType,
                ResultIsHandled = false
            };
        }
    }
}
