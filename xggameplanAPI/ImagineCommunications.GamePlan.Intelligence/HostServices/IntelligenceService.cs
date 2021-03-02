using System;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using Quartz;

namespace ImagineCommunications.GamePlan.Intelligence.HostServices
{
    public class IntelligenceService
    {
        private IServiceBus Bus { get; }
        private IScheduler Scheduler { get; }

        public IntelligenceService(IScheduler scheduler, IServiceBus busControl)
        {
            Bus = busControl ?? throw new ArgumentNullException(nameof(busControl));
            Scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
        }

        public void OnStart()
        {
            Bus.Start();
            Scheduler.Start();
        }

        public void OnStop()
        {
            Bus.Stop();
            ReleaseResources();
        }

        public void ReleaseResources()
        {
            if (!Scheduler.IsShutdown)
            {
                Scheduler.Shutdown();
            }
        }
    }
}
