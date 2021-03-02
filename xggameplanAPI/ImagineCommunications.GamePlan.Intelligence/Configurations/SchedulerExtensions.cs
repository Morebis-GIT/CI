using System;
using ImagineCommunications.GamePlan.Intelligence.HostServices;
using ImagineCommunications.GamePlan.Intelligence.HostServices.Jobs;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;

namespace ImagineCommunications.GamePlan.Intelligence.Configurations
{
    internal static class SchedulerExtensions
    {
        private const string TRANSACTION_RUNNER_TASK_GROUP = "G1";

        internal static IScheduler AddScheduler(this IServiceCollection services)
        {
            IScheduler schedule = StdSchedulerFactory.GetDefaultScheduler().GetAwaiter().GetResult();

            services.AddSingleton(schedule);

            return schedule;
        }

        internal static void ConfigureJobs(this IScheduler scheduler, IServiceProvider provider)
        {
            scheduler.JobFactory = new JobFactory(provider);

            IJobDetail runner = JobBuilder
                .Create<TransactionRunner>()
                .WithIdentity("transaction-runner", TRANSACTION_RUNNER_TASK_GROUP)
                .Build();

            ITrigger runnerTrigger = TriggerBuilder.Create()
                .WithIdentity("transaction-runner-t", TRANSACTION_RUNNER_TASK_GROUP)
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(60)
                    .RepeatForever())
                    .StartAt(DateTimeOffset.UtcNow.AddSeconds(15))
                .StartNow()
                .Build();

            scheduler.ScheduleJob(runner, runnerTrigger);
        }
    }
}
