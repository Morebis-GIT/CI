using System;
using System.Linq;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Controllers;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.BusClient.Abstraction.Models;
using ImagineCommunications.BusClient.Implementation.BigMessage;
using ImagineCommunications.BusClient.MassTransit;
using ImagineCommunications.Gameplan.Integration.Contracts.Shared;
using ImagineCommunications.Intelligence.Test.Api.Common;
using ImagineCommunications.Intelligence.Test.Api.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.Application;

namespace ImagineCommunications.Intelligence.Test.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var appDataPath = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
            var appConfig = new ConfigurationBuilder()
                .SetBasePath(appDataPath)
                .AddJsonFile("config.json", true, true)
                .Build();

            // Web API routes
            config.MapHttpAttributeRoutes();

            // Redirect root to Swagger UI
            config.Routes.MapHttpRoute(
                name: "Swagger UI",
                routeTemplate: "",
                defaults: null,
                constraints: null,
                handler: new RedirectHandler(message => message.RequestUri.ToString().TrimEnd('/'), "swagger/ui/index"));

            config.Routes.MapHttpRoute(
                name: "LandmarkApi",
                routeTemplate: "{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            var services = new ServiceCollection();

            services.AddSingleton<IConfiguration>(appConfig);
            services.AddSingleton<IConfigurationService, ConfigurationService>();
            services.AddControllersAsServices(typeof(WebApiConfig).Assembly.GetExportedTypes()
                .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition)
                .Where(t => typeof(IHttpController).IsAssignableFrom(t)
                            || t.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)));
            services.AddScoped<IContractValidatorService, ContractValidatorService>();
            services.AddScoped<IServiceBus, ServiceBus>();
            services.AddScoped<ILogFileExportService, LogFileExportService>(_ =>
                new LogFileExportService(appConfig.GetValue<string>("Logging:IntelligenceLogPath")));

            var serviceBusConfigModel = appConfig.GetSection("ServiceBusConfig").Get<ServiceBusConfigModel>();
            ServiceBusConfigurator.Init(services, serviceBusConfigModel);

            var payloadStorageConfig = appConfig.GetSection("PayloadStorage").Get<ObjectStorageConfiguration>();
            if (!string.IsNullOrWhiteSpace(payloadStorageConfig.ProfilesLocation) &&
                payloadStorageConfig.ProfilesLocation.StartsWith("~/"))
            {
                payloadStorageConfig.ProfilesLocation = HostingEnvironment.MapPath(payloadStorageConfig.ProfilesLocation);
            }
            services.AddBigMessages(payloadStorageConfig);

            var resolver = new LandmarkDependencyResolver(services.BuildServiceProvider());
            config.DependencyResolver = resolver;
        }
    }
}
