using System;
using Quartz;
using Quartz.Spi;

namespace ImagineCommunications.GamePlan.Intelligence.HostServices
{
    internal class JobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public JobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler) =>
            new QuartzJobRunner(_serviceProvider);

        public void ReturnJob(IJob job)
        {
            (job as IDisposable)?.Dispose();
        }
    }
}
