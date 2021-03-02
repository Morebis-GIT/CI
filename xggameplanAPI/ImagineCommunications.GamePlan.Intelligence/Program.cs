using System;
using ImagineCommunications.GamePlan.Intelligence.Configurations;
using ImagineCommunications.GamePlan.Intelligence.HostServices;
using Microsoft.Extensions.Configuration;
using Topshelf;
using Topshelf.Owin;
using Topshelf.Runtime;

namespace ImagineCommunications.GamePlan.Intelligence
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var appConfig = Bootstrapper.GetConfiguration(args);

            var provider = new Lazy<IServiceProvider>(() => Bootstrapper.Init(args));
            ServiceFactory<IntelligenceService> factory = (s) => provider.Value.GetService(typeof(IntelligenceService)) as IntelligenceService;
            
            HostFactory.Run(host =>
            {
                host.Service<IntelligenceService>(service =>
                {
                    service.ConstructUsing(factory);
                    
                    service.WhenStarted(s => s.OnStart());
                    service.WhenStopped(s => s.OnStop());
                    service.WhenShutdown(s => s.ReleaseResources());
                    
                    service.OwinEndpoint(app =>
                    {
                        app.Domain = appConfig.GetValue<string>("HealthCheckApi:Domain");
                        app.Port = appConfig.GetValue<int>("HealthCheckApi:Port");
                    });
                });
            });
        }
    }
}
