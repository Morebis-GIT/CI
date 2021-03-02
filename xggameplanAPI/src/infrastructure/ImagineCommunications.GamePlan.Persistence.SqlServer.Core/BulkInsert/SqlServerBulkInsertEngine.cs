using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using Microsoft.Extensions.Logging;
using xggameplan.common.Extensions;
using xggameplan.common.Helpers;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.BulkInsert
{
    public class SqlServerBulkInsertEngine : ISqlServerBulkInsertEngine
    {
        private readonly Microsoft.EntityFrameworkCore.DbContext _dbContext;
        private readonly SqlServerBulkInsertEngineOptions _bulkInsertEngineOptions;

        private EmptyGuidPkEntityPreProcessor _entityPreProcessor;
        private bool _loggerPrepared;
        private ILogger _logger;

        protected virtual EmptyGuidPkEntityPreProcessor EntityPreProcessor =>
            _entityPreProcessor ?? (_entityPreProcessor = new EmptyGuidPkEntityPreProcessor());

        private IReadOnlyCollection<(TEntity Entity, int Index)> PreparePreserveOrderedEntitiesTuple<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : class
        {
            Logger?.LogInformation($"PreserveOrder starting for '{GetType().Name}'.");
            var newCount = 0;
            var originalOrderedTuple = entities.Select((x, idx) =>
            {
                newCount += ((IIdentityPrimaryKey)x).Id <= 0 ? 1 : 0;
                return (entity: x, index: idx);
            }).ToList();
            var res = originalOrderedTuple
                .Where(x => ((IIdentityPrimaryKey)x.entity).Id > 0)
                .OrderBy(x => ((IIdentityPrimaryKey)x.entity).Id)
                .Union(originalOrderedTuple.Where(x => ((IIdentityPrimaryKey)x.entity).Id <= 0).Select(
                    (x, idx) =>
                    {
                        ((IIdentityPrimaryKey)x.entity).Id = -newCount + idx;
                        return x;
                    })).ToList();
            Logger?.LogInformation($"PreserveOrder completed for '{GetType().Name}'.");
            return res;
        }

        protected ILogger Logger
        {
            get
            {
                if (!_loggerPrepared)
                {
                    _logger = _bulkInsertEngineOptions.LoggerFactory?.CreateLogger(GetType());
                    _loggerPrepared = true;
                }

                return _logger;
            }
        }

        protected BulkInsertOptions CreateDefaultBulkConfig()
        {
            return _bulkInsertEngineOptions.BulkInsertDefaultOptionsFactory();
        }

        protected void PreProcessEntities<TEntity>(IEnumerable<TEntity> entities, BulkInsertOperation operation, BulkInsertOptions options) where TEntity : class
        {
            if (entities == null || !entities.Any())
            {
                return;
            }

            var cnt = 0;

            Logger?.LogInformation($"PreProcessEntities starting for '{GetType().Name}'.");
            foreach (var entity in entities)
            {
                foreach (var preProcessor in _bulkInsertEngineOptions.PreProcessors.Where(p => (p.SupportedOperations & operation) != 0))
                {
                    preProcessor.Process(entity, operation, options);
                }

                cnt++;
            }
            Logger?.LogInformation($"PreProcessEntities completed for '{GetType().Name}' with {cnt} entities prepared.");
        }

        protected virtual void ExecuteBulkInsertAction<TEntity>(IList<TEntity> entities, BulkInsertOperation operation,
            BulkInsertOptions options, Action<IList<TEntity>, BulkInsertOptions> processAction)
            where TEntity : class
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }
            if (processAction == null)
            {
                throw new ArgumentNullException(nameof(processAction));
            }

            if (options == null)
            {
                options = CreateDefaultBulkConfig();
                Logger?.LogWarning("Default bulk insert options has been created: {options}.", options);
            }

            PreProcessEntities(entities, operation, options);
            var needToPreserveOrder = options.PreserveInsertOrder && !options.InsertOrderPrepared &&
                                      typeof(IIdentityPrimaryKey).IsAssignableFrom(typeof(TEntity));
            if (needToPreserveOrder)
            {
                var preservedTuple = PreparePreserveOrderedEntitiesTuple(entities);
                entities.Clear();
                entities.AddRange(preservedTuple.Select(x => x.Entity));
                try
                {
                    ProcessActionExecute();
                }
                finally
                {
                    entities.Clear();
                    entities.AddRange(preservedTuple.OrderBy(x => x.Index).Select(x => x.Entity));
                }
            }
            else
            {
                ProcessActionExecute();
            }

            void ProcessActionExecute()
            {
                try
                {
                    if (Logger == null)
                    {
                        processAction(entities, options);
                    }
                    else
                    {
                        Logger.LogInformation($"BulkInsert starting for '{GetType().Name}'.");
                        var stopwatch = StopwatchHelper.StopwatchAction(() => processAction(entities, options));
                        Logger.LogInformation($"BulkInsert completed ({stopwatch.ElapsedMilliseconds} ms) for '{GetType().Name}'.");
                    }
                }
                catch
                {
                    Logger?.LogInformation($"BulkInsert completed with errors for '{GetType().Name}'.");
                    throw;
                }
            }
        }

        protected virtual async Task ExecuteBulkInsertActionAsync<TEntity>(IList<TEntity> entities, BulkInsertOperation operation,
            BulkInsertOptions options, Func<IList<TEntity>, BulkInsertOptions, Task> processFunc)
            where TEntity : class
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }
            if (processFunc == null)
            {
                throw new ArgumentNullException(nameof(processFunc));
            }
            if (options == null)
            {
                options = CreateDefaultBulkConfig();
                Logger?.LogWarning("Default bulk insert options has been created: {options}.", options);
            }

            PreProcessEntities(entities, operation, options);
            var needToPreserveOrder = options.PreserveInsertOrder && !options.InsertOrderPrepared &&
                                      typeof(IIdentityPrimaryKey).IsAssignableFrom(typeof(TEntity));
            if (needToPreserveOrder)
            {
                var preservedTuple = PreparePreserveOrderedEntitiesTuple(entities);
                entities.Clear();
                entities.AddRange(preservedTuple.Select(x => x.Entity));
                try
                {
                    await ProcessFuncExecute().ConfigureAwait(false);
                }
                finally
                {
                    entities.Clear();
                    entities.AddRange(preservedTuple.OrderBy(x => x.Index).Select(x => x.Entity));
                }
            }
            else
            {
                await ProcessFuncExecute().ConfigureAwait(false);
            }

            async Task ProcessFuncExecute()
            {
                try
                {
                    if (Logger == null)
                    {
                        await processFunc(entities, options).ConfigureAwait(false);
                    }
                    else
                    {
                        Logger.LogInformation($"BulkInsert starting for '{GetType().Name}'.");
                        var stopwatch = Stopwatch.StartNew();
                        await processFunc(entities, options).ConfigureAwait(false);
                        stopwatch.Stop();
                        Logger.LogInformation($"BulkInsert completed ({stopwatch.ElapsedMilliseconds} ms) for '{GetType().Name}'.");
                    }
                }
                catch
                {
                    Logger?.LogInformation($"BulkInsert completed with errors for '{GetType().Name}'.");
                    throw;
                }
            }
        }

        public SqlServerBulkInsertEngine(Microsoft.EntityFrameworkCore.DbContext dbContext, SqlServerBulkInsertEngineOptions bulkInsertEngineOptions)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _bulkInsertEngineOptions = bulkInsertEngineOptions ?? throw new ArgumentNullException(nameof(bulkInsertEngineOptions));
        }

        public void BulkInsert<TEntity>(IList<TEntity> entities, BulkInsertOptions options = null) where TEntity : class
        {
            Logger?.LogInformation($"Executing BulkInsert method of '{GetType().Name}'.");
            ExecuteBulkInsertAction(entities, BulkInsertOperation.BulkInsert, options,
                (ent, opt) => _dbContext.BulkInsert(ent, opt));
            Logger?.LogInformation($"Executed BulkInsert method of '{GetType().Name}'.");
        }

        public async Task BulkInsertAsync<TEntity>(IList<TEntity> entities, BulkInsertOptions options = null, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            Logger?.LogInformation($"Executing BulkInsertAsync method of '{GetType().Name}'.");
            await ExecuteBulkInsertActionAsync(entities, BulkInsertOperation.BulkInsert, options,
                    async (ent, opt) => await _dbContext.BulkInsertAsync(ent, opt, null, cancellationToken).ConfigureAwait(false))
                .ConfigureAwait(false);
            Logger?.LogInformation($"Executed BulkInsertAsync method of '{GetType().Name}'.");
        }

        public void BulkInsertOrUpdate<TEntity>(IList<TEntity> entities, BulkInsertOptions options = null) where TEntity : class
        {
            Logger?.LogInformation($"Executing BulkInsertOrUpdate method of '{GetType().Name}'.");
            ExecuteBulkInsertAction(entities, BulkInsertOperation.BulkInsertOrUpdate, options,
                (ent, opt) => _dbContext.BulkInsertOrUpdate(ent, opt));
            Logger?.LogInformation($"Executed BulkInsertOrUpdate method of '{GetType().Name}'.");
        }

        public async Task BulkInsertOrUpdateAsync<TEntity>(IList<TEntity> entities, BulkInsertOptions options = null, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            Logger?.LogInformation($"Executing BulkInsertOrUpdateAsync method of '{GetType().Name}'.");
            await ExecuteBulkInsertActionAsync(entities, BulkInsertOperation.BulkInsertOrUpdate, options,
                    async (ent, opt) => await _dbContext.BulkInsertOrUpdateAsync(ent, opt, null, cancellationToken)
                        .ConfigureAwait(false))
                .ConfigureAwait(false);
            Logger?.LogInformation($"Executed BulkInsertOrUpdateAsync method of '{GetType().Name}'.");
        }

        public void BulkUpdate<TEntity>(IList<TEntity> entities, BulkInsertOptions options = null) where TEntity : class
        {
            Logger?.LogInformation($"Executing BulkUpdate method of '{GetType().Name}'.");
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }
            if (options == null)
            {
                options = CreateDefaultBulkConfig();
                Logger?.LogWarning("Default bulk insert options has been created: {options}.", options);
            }

            PreProcessEntities(entities, BulkInsertOperation.BulkUpdate, options);
            try
            {
                if (Logger == null)
                {
                    _dbContext.BulkUpdate(entities, options);
                }
                else
                {
                    Logger.LogInformation($"BulkUpdate starting for '{GetType().Name}'.");
                    var stopwatch = StopwatchHelper.StopwatchAction(() => _dbContext.BulkUpdate(entities, options));
                    Logger.LogInformation($"BulkUpdate completed ({stopwatch.ElapsedMilliseconds.ToString()} ms) for '{GetType().Name}'.");
                }
            }
            catch
            {
                Logger?.LogInformation($"BulkUpdate completed with errors for '{GetType().Name}'.");
                throw;
            }

            Logger?.LogInformation($"Executed BulkUpdate method of '{GetType().Name}'.");
        }

        public async Task BulkUpdateAsync<TEntity>(IList<TEntity> entities, BulkInsertOptions options = null, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            Logger?.LogInformation($"Executing BulkUpdateAsync method of '{GetType().Name}'.");

            if (entities is null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            if (options is null)
            {
                options = CreateDefaultBulkConfig();
                Logger?.LogWarning("Default bulk insert options has been created: {options}.", options);
            }

            PreProcessEntities(entities, BulkInsertOperation.BulkUpdate, options);
            try
            {
                if (Logger is null)
                {
                    await _dbContext.BulkUpdateAsync(entities, options).ConfigureAwait(false);
                }
                else
                {
                    Logger.LogInformation($"BulkUpdate starting for '{GetType().Name}'.");
                    var stopwatch = Stopwatch.StartNew();
                    await _dbContext.BulkUpdateAsync(entities, options, null, cancellationToken).ConfigureAwait(false);
                    stopwatch.Stop();
                    Logger.LogInformation($"BulkUpdate completed ({stopwatch.ElapsedMilliseconds.ToString()} ms) for '{GetType().Name}'.");
                }
            }
            catch
            {
                Logger?.LogInformation($"BulkUpdate completed with errors for '{GetType().Name}'.");
                throw;
            }

            Logger?.LogInformation($"Executed BulkUpdateAsync method of '{GetType().Name}'.");
        }

        public void BulkDelete<TEntity>(IList<TEntity> entities) where TEntity : class
        {
            Logger?.LogInformation($"Executing BulkDelete method of '{GetType().Name}'.");
            if (entities is null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            try
            {
                if (Logger is null)
                {
                    _dbContext.BulkDelete(entities);
                }
                else
                {
                    Logger.LogInformation($"BulkDelete starting for '{GetType().Name}'.");
                    var stopwatch = StopwatchHelper.StopwatchAction(() => _dbContext.BulkDelete(entities));
                    Logger.LogInformation($"BulkDelete completed ({stopwatch.ElapsedMilliseconds} ms) for '{GetType().Name}'.");
                }
            }
            catch
            {
                Logger?.LogInformation($"BulkUpdate completed with errors for '{GetType().Name}'.");
                throw;
            }

            Logger?.LogInformation($"Executed BulkUpdate method of '{GetType().Name}'.");
        }
    }
}
