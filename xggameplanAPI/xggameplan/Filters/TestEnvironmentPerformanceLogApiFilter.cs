using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Autofac.Integration.WebApi;
using Microsoft.Extensions.Logging;
using xggameplan.TestEnv;

namespace xggameplan.Filters
{
    public class TestEnvironmentPerformanceLogApiFilter : IAutofacActionFilter
    {
        private readonly ILogger<PerformanceLog> _logger;

        private Stopwatch _stopwatch;

        protected void OnExecutingLog(HttpActionContext context) => _logger?.LogInformation(
                $"--- WebAPI request. Executing '{context.ActionDescriptor.ControllerDescriptor.ControllerType.Name}, {context.ActionDescriptor.ActionName}' method. Url - [{context.Request.Method.Method}]{context.Request.RequestUri}.");

        protected async Task OnExecutedLogAsync(HttpActionExecutedContext context)
        {
            if (_logger != null)
            {
                var ms = _stopwatch?.ElapsedMilliseconds.ToString(CultureInfo.InvariantCulture) ?? "??";
                var withErrors = context.Exception != null ? " with errors" : String.Empty;
                var message =
                    $@"--- WebAPI request. Executed '{context.ActionContext.ActionDescriptor.ControllerDescriptor.ControllerType.Name}, {context.ActionContext.ActionDescriptor.ActionName}' ({ms} ms) method{withErrors}. Url - [{context.Request.Method.Method}]{context.ActionContext.Request.RequestUri}.";
                if (context.Exception != null)
                {
                    message += Environment.NewLine + context.Exception;
                }
                else if (context.Response != null)
                {
                    message += $" ResponseCode - {context.Response.StatusCode}({(int)context.Response.StatusCode}).";
                    if (!context.Response.IsSuccessStatusCode)
                    {
                        var content = context.Response.Content != null
                            ? await context.Response.Content.ReadAsStringAsync().ConfigureAwait(false)
                            : String.Empty;
                        message +=
                            Environment.NewLine +
                            $"        Reason: '{context.Response.ReasonPhrase}'" +
                            Environment.NewLine +
                            $"        Content: '{content}'";
                    }
                }

                _logger.LogInformation(message);
            }
        }

        public TestEnvironmentPerformanceLogApiFilter(ILogger<PerformanceLog> logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            _stopwatch?.Stop();
            await OnExecutedLogAsync(actionExecutedContext).ConfigureAwait(false);
        }

        public Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            OnExecutingLog(actionContext);
            _stopwatch = Stopwatch.StartNew();
            return Task.CompletedTask;
        }
    }
}
