using System;
using AutoMapper;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.BusClient.Abstraction.Models;
using ImagineCommunications.BusClient.Implementation.BigMessage;
using ImagineCommunications.BusClient.Implementation.Services;
using ImagineCommunications.Gameplan.Integration.Contracts.Shared;
using ImagineCommunications.Gameplan.Integration.Data.Entities;
using ImagineCommunications.Gameplan.Integration.Data.Installers;
using ImagineCommunications.Gameplan.Synchronization;
using ImagineCommunications.Gameplan.Synchronization.SqlServer;
using ImagineCommunications.Gameplan.Synchronization.SqlServer.Mappings;
using ImagineCommunications.GamePlan.Intelligence.Common;
using ImagineCommunications.GamePlan.Intelligence.Configurations.Dependencies;
using ImagineCommunications.GamePlan.Intelligence.Configurations.Mappings;
using ImagineCommunications.GamePlan.Intelligence.HostServices;
using ImagineCommunications.GamePlan.Intelligence.HostServices.Jobs;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Serilog;
using xggameplan.common.Caching;

namespace ImagineCommunications.GamePlan.Intelligence.Configurations
{
    public static class Bootstrapper
    {
        public static IConfigurationRoot GetConfiguration(string[] args) =>
            new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

        public static IServiceProvider Init(string[] args)
        {
            var config = GetConfiguration(args);

            var services = new ServiceCollection();

            services.AddSingleton<IntelligenceService>();

            services.AddValidators()
                .AddCaching()
                .AddCommonServices()
                .AddLogger(config)
                .AddConfigurations(config)
                .AddAutoMapper(
                    typeof(MappingProfile).Assembly,
                    typeof(AccessTokenProfile).Assembly,
                    typeof(SynchronizationProfile).Assembly,
                    typeof(GroupTransactionInfoProfile).Assembly)
                .AddPersistence(config)
                .AddServiceBus(config)
                .AddBigMessages(config.GetSection("PayloadStorage").Get<ObjectStorageConfiguration>())
                .AddJobs()
                .AddSynchronizationService(c =>
                {
                    c.AddService(SynchronizedServiceType.RunExecution);
                    c.AddService(SynchronizedServiceType.DataSynchronization, maxConcurrencyLevel: 1);
                })
                .UseSqlServerForSynchronizationService(config.GetValue<string>("SqlServerDb:ConnectionString"));

            var scheduler = services.AddScheduler();
            var provider = services.BuildServiceProvider();

            scheduler.ConfigureJobs(provider);

            return provider;
        }

        private static IServiceCollection AddCaching(this IServiceCollection services)
        {
            services.AddSingleton<ICache, InMemoryCache>();

            return services;
        }

        private static IServiceCollection AddCommonServices(this IServiceCollection services)
        {
            services.AddScoped<IMessageTypeService, MessageTypeService>();
            services.AddSingleton<IClock>(SystemClock.Instance);

            return services;
        }

        private static IServiceCollection AddJobs(this IServiceCollection services)
        {
            services.AddTransient<TransactionRunner>();

            return services;
        }

        private static IServiceCollection AddServiceBus(this IServiceCollection services, IConfigurationRoot config)
        {
            var messagingConfig = config.GetSection("ServiceBusConfig").Get<ServiceBusConfigModel>();

            ServiceBusConfigurator.Init(services, messagingConfig);

            return services;
        }

        private static IServiceCollection AddPersistence(this IServiceCollection services, IConfigurationRoot config)
        {
            services.RegisterModule(new HandlersDependencyModule());
            services.RegisterModule(new SqlDatabaseDependencyModule(config.GetValue<string>("SqlServerDb:ConnectionString"), config.GetValue<int>("SqlServerDb:Timeout")));
            services.AddIntegrationPersistance(config.GetValue<string>("SqlServerDb:ConnectionString"));

            return services;
        }

        private static IServiceCollection AddConfigurations(this IServiceCollection services, IConfigurationRoot config)
        {
            services.AddOptions();
            services.AddSingleton<IConfiguration>(config);

            services.AddScoped<ILoggerService, LoggerService>();
            services.AddSingleton<IConfigurationService, ConfigurationService>();

            return services;
        }

        private static IServiceCollection AddValidators(this IServiceCollection services)
        {
            services.AddScoped<IContractValidatorService, ContractValidatorService>();

            return services;
        }

        private static IServiceCollection AddLogger(this IServiceCollection services, IConfigurationRoot config)
        {
            var serilogConfig = config.GetSection("SerilogConfig").Get<LoggerConfigurationModel>();
            services.AddSingleton<ILogger>(s =>
                 new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .WriteTo.File(serilogConfig.Uri, rollingInterval: RollingInterval.Day)
                    .CreateLogger()
        );

            return services;
        }
    }
}
