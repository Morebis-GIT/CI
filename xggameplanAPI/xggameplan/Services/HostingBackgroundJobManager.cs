using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Hosting;
using Autofac;
using xggameplan.common.BackgroundJobs;

namespace xggameplan.Services
{
    public class HostingBackgroundJobManager : IBackgroundJobManager
    {
        private const string ExecuteMethodName = "Execute";

        private readonly ILifetimeScope _lifetimeScope;

        public HostingBackgroundJobManager(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
        }

        public void StartJob<TBackgroundJob>(params IBackgroundJobParameter[] parameters) where TBackgroundJob : class, IBackgroundJob
        {
            CancellationToken cToken = default;
            if (!_lifetimeScope.IsRegistered<TBackgroundJob>())
            {
                throw new Exception($"'{typeof(TBackgroundJob).Name}' can not be resolved.");
            }

            var executeMethodInfo = typeof(TBackgroundJob)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .SingleOrDefault(mi => mi.Name == ExecuteMethodName && !mi.IsGenericMethod);
            if (executeMethodInfo == null)
            {
                throw new Exception(
                    $"'{ExecuteMethodName}' method should be define for {typeof(TBackgroundJob).Name} background job class");
            }
            var executionParameters = parameters?.ToList() ?? new List<IBackgroundJobParameter>();
            if (executionParameters.All(p => p.Type != typeof(CancellationToken)))
            {
                executionParameters.Add(
                    new BackgroundJobParameter<CancellationToken>(new Lazy<CancellationToken>(() => cToken)));
            }

            var methodParamDefinitions = executeMethodInfo.GetParameters();
            var methodParams = new List<Lazy<object>>();
            foreach (var methodParamDefinition in methodParamDefinitions)
            {
                var p = executionParameters.FirstOrDefault(x =>
                    x.Name == methodParamDefinition.Name && x.Type == methodParamDefinition.ParameterType);
                if (p == null)
                {
                    p = executionParameters.FirstOrDefault(x =>
                        x.Name == methodParamDefinition.Name && methodParamDefinition.ParameterType.IsAssignableFrom(x.Type));
                    if (p == null)
                    {
                        p = executionParameters.FirstOrDefault(x =>
                            string.IsNullOrWhiteSpace(x.Name) && x.Type == methodParamDefinition.ParameterType);
                        if (p == null)
                        {
                            p = executionParameters.FirstOrDefault(x =>
                                string.IsNullOrWhiteSpace(x.Name) && methodParamDefinition.ParameterType.IsAssignableFrom(x.Type));
                        }
                    }
                }

                if (p == null)
                {
                    if (methodParamDefinition.HasDefaultValue)
                    {
                        methodParams.Add(new Lazy<object>(() => methodParamDefinition.DefaultValue));
                        continue;
                    }
                    throw new Exception(
                        $"An appropriate value for '{methodParamDefinition.Name}' parameter of '{methodParamDefinition.ParameterType.Name}' type has not been provided.");
                }

                methodParams.Add(new Lazy<object>(() => p.Value));
            }

            HostingEnvironment.QueueBackgroundWorkItem(async cancellationToken =>
            {
                cToken = cancellationToken;
                using (var scope = _lifetimeScope.BeginLifetimeScope("backgroundJobManager"))
                {
                    var job = scope.Resolve<TBackgroundJob>();
                    var result = executeMethodInfo.Invoke(job, methodParams.Select(p => p.Value).ToArray());
                    await (result as Task ?? Task.CompletedTask).ConfigureAwait(false);
                }
            });
        }
    }
}
