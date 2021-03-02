using System.Collections.Generic;
using Autofac;
using Autofac.Multitenant;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using Quartz;
using Quartz.Spi;

namespace xggameplan.SchedulerTasks
{
    public class MvcJobFactory : IJobFactory
    {
        private readonly IContainer _resolver;
        private readonly MultitenantContainer _tenantResolver;
        private readonly IEnumerable<Tenant> _tenants;

        public MvcJobFactory(IContainer resolver, MultitenantContainer tenantResolver, IEnumerable<Tenant> tenants)
        {
            _resolver = resolver;
            _tenantResolver = tenantResolver;
            _tenants = tenants;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            if (bundle.JobDetail.JobType.GetJobScope() == JobScopeType.Application)
            {
                return new MvcApplicationScopeJobRunner(_resolver);
            }

            return new MvcTenantScopeJobRunner(_tenantResolver, _tenants);
        }

        public void ReturnJob(IJob job) { }
    }
}
