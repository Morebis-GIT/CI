using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Autofac.Multitenant;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using Quartz;
using xggameplan.core.FeatureManagement.Interfaces;

namespace xggameplan.SchedulerTasks
{
    public class MvcTenantScopeJobRunner : IJob
    {
        private readonly IEnumerable<Tenant> _tenants;
        private readonly MultitenantContainer _resolver;

        public MvcTenantScopeJobRunner(MultitenantContainer resolver, IEnumerable<Tenant> tenants)
        {
            _tenants = tenants;
            _resolver = resolver;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            bool isResolved = false;

            foreach (var tenant in _tenants)
            {
                var tenantScope = _resolver.GetTenantScope(tenant.Id);
                using (var scope = tenantScope.BeginLifetimeScope())
                {
                    if (!scope.Resolve<IFeatureManager>().IsJobFeaturesEnabled(context.JobDetail.JobType))
                    {
                        continue;
                    }

                    var job = (IJob)scope.Resolve(context.JobDetail.JobType);

                    isResolved = job != null;

                    if (!isResolved)
                    {
                        continue;
                    }

                    await job.Execute(context).ConfigureAwait(false);
                }
            }

            if (!isResolved)
            {
                throw new JobExecutionException("Job is not registered in IoC Container")
                {
                    UnscheduleFiringTrigger = true
                };
            }
        }
    }
}
