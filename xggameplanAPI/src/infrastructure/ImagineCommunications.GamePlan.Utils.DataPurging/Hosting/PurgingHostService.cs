using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Utils.DataPurging.Extensions;
using ImagineCommunications.GamePlan.Utils.DataPurging.Interfaces;
using ImagineCommunications.GamePlan.Utils.DataPurging.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ImagineCommunications.GamePlan.Utils.DataPurging.Hosting
{
    /// <summary>
    /// Exposes the functionality to execute purging tasks.
    /// </summary>
    public class PurgingHostService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly CommandLineOptions _cmdOptions;
        private readonly ILogger<PurgingHostService> _logger;

        public PurgingHostService(
            IServiceProvider serviceProvider,
            IHostApplicationLifetime hostApplicationLifetime,
            CommandLineOptions cmdOptions,
            ILogger<PurgingHostService> logger)
        {
            _serviceProvider = serviceProvider;
            _hostApplicationLifetime = hostApplicationLifetime;
            _cmdOptions = cmdOptions;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await WaitForApplicationStarted(stoppingToken).ConfigureAwait(false);

            if (stoppingToken.IsCancellationRequested)
            {
                return;
            }

            try
            {
                _logger.LogInformation("Purging service started.");

                await ExecutePurging(stoppingToken);

                _logger.LogInformation("Purging service finished.");
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Purging service cancelled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Purging service error.");
            }
            finally
            {
                if (!stoppingToken.IsCancellationRequested)
                {
                    _hostApplicationLifetime.StopApplication();
                }
            }
        }

        /// <summary>
        /// Waits for application start through checking <see cref="IHostApplicationLifetime.ApplicationStarted"/> token.
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected async Task WaitForApplicationStarted(CancellationToken stoppingToken)
        {
            using (var cancellationTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(stoppingToken,
                    _hostApplicationLifetime.ApplicationStarted))
            {
                await Task.Delay(Timeout.Infinite, cancellationTokenSource.Token)
                    .WaitWithCancellationSuppression().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Executes registered purging handlers.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected virtual async Task ExecutePurging(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var providers = scope.ServiceProvider.GetServices<IDataPurgingHandlerProvider>().ToArray();

                if (_cmdOptions.Entities.Any())
                {
                    providers = providers.Where(p =>
                        _cmdOptions.Entities.Any(e =>
                            string.Equals(e, p.EntityName, StringComparison.OrdinalIgnoreCase))).ToArray();
                }

                _logger.LogInformation($"Entities to process: {providers.Length}");

                foreach (var provider in providers.OrderBy(p => p.Priority))
                {
                    try
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        _logger.LogInformation($"Purging of {provider.EntityName} entities started.");

                        using (var handlerScope = scope.ServiceProvider.CreateScope())
                        {
                            var handler = provider.Get(handlerScope.ServiceProvider);
                            await handler.ExecuteAsync(cancellationToken).ConfigureAwait(false);
                        }

                        _logger.LogInformation($"Purging of {provider.EntityName} entities finished.");
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(
                            $"An exception has been thrown during purging of '{provider.EntityName}' entities.", ex);
                    }
                }
            }
        }
    }
}
