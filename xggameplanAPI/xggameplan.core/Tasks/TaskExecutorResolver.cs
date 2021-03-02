using System;
using Autofac;

namespace xggameplan.core.Tasks
{
    public class TaskExecutorResolver : ITaskExecutorResolver
    {
        private readonly ILifetimeScope _lifetimeScope;

        public TaskExecutorResolver(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
        }

        public ITaskExecutor Resolve(string taskId)
        {
            return _lifetimeScope.ResolveNamed<ITaskExecutor>(taskId);
        }
    }
}
