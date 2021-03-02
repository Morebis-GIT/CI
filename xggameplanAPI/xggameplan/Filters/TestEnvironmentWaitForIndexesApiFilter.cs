using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Autofac.Features.Metadata;
using Autofac.Integration.WebApi;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;
using Microsoft.Extensions.Logging;
using xggameplan.TestEnv;

namespace xggameplan.Filters
{
    public class TestEnvironmentWaitForIndexesApiFilter : IAutofacActionFilter
    {
        private readonly IEnumerable<Meta<IDatabaseIndexAwaiter>> _indexAwaiters;
        private readonly ILogger<PerformanceLog> _logger;

        public TestEnvironmentWaitForIndexesApiFilter(
            IEnumerable<Meta<IDatabaseIndexAwaiter>> indexAwaiters,
            ILogger<PerformanceLog> logger)
        {
            _indexAwaiters = indexAwaiters;
            _logger = logger;
        }

        public async Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            if (actionExecutedContext.ActionContext.ActionDescriptor
                .GetCustomAttributes<TestEnvironmentIgnoreWaitForIndexesAttribute>().Any())
            {
                return;
            }

            if (_logger != null)
            {
                _logger.LogInformation("Executing WaitForIndexes.");
                var stopwatch = Stopwatch.StartNew();
                await Task.WhenAll(
                    _indexAwaiters.Select(a => a.Value.WaitForIndexesAsync()));
                stopwatch.Stop();
                _logger.LogInformation(
                    $"Executed WaitForIndexes completed ({stopwatch.ElapsedMilliseconds} ms) with {_indexAwaiters.Count()} index awaiters.");
            }
            else
            {
                await Task.WhenAll(
                    _indexAwaiters.Select(a => a.Value.WaitForIndexesAsync())).ConfigureAwait(false);
            }
        }

        public Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class TestEnvironmentIgnoreWaitForIndexesAttribute : Attribute
    {
    }
}
