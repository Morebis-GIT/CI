using System.Threading.Tasks;
using Autofac;
using Quartz;

namespace xggameplan.SchedulerTasks
{
    public class MvcApplicationScopeJobRunner : IJob
    {
        private readonly IContainer _resolver;

        public MvcApplicationScopeJobRunner(IContainer resolver)
        {
            _resolver = resolver;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            if (!_resolver.IsRegistered(context.JobDetail.JobType))
            {
                throw new JobExecutionException("Job is not registered in IoC Container")
                {
                    UnscheduleFiringTrigger = true
                };
            }

            using (var scope = _resolver.BeginLifetimeScope())
            {
                var job = scope.Resolve(context.JobDetail.JobType) as IJob;

                await job.Execute(context).ConfigureAwait(false);
            }
        }
    }
}
