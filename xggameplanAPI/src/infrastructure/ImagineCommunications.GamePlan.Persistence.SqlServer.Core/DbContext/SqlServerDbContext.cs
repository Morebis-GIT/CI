using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Generic.DbContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DbContextOptions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Truncate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using xggameplan.common.ActionProcessing;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DbContext
{
    public abstract class SqlServerDbContext : Microsoft.EntityFrameworkCore.DbContext, ISqlServerDbContext
    {
        private static readonly MethodInfo TruncateGenericMethodInfo =
            typeof(IDbContext).GetMethods(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(x =>
                x.Name == nameof(IDbContext.Truncate) && x.IsGenericMethod)?.GetGenericMethodDefinition();

        private static readonly MethodInfo TruncateAsyncGenericMethodInfo =
            typeof(IDbContext).GetMethods(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(x =>
                x.Name == nameof(IDbContext.TruncateAsync) && x.IsGenericMethod)?.GetGenericMethodDefinition();

        private readonly Microsoft.EntityFrameworkCore.DbContextOptions _dbContextOptions;
        private SqlServerSpecificDbAdapter _specific;
        private IActionProcessor _actionProcessor;
        private ISqlServerBulkInsertEngine _bulkInsertEngine;
        private bool _auditEntityHandlerPrepared;
        private IAuditEntityHandler _auditEntityHandler;
        private bool _loggerPrepared;
        private ILogger _logger;

        protected virtual bool UseSqlCommandForTruncate => true;

        protected virtual SqlServerSpecificDbAdapter CreateSpecificDbAdapter()
        {
            return new SqlServerSpecificDbAdapter(this);
        }

        protected IActionProcessor PostProcessor => _actionProcessor ?? (_actionProcessor = CreatePostProcessor());

        protected virtual IActionProcessor CreatePostProcessor()
        {
            return new ActionProcessor();
        }

        protected virtual ISqlServerBulkInsertEngine CreateBulkInsertEngine()
        {
            var extension = _dbContextOptions.FindExtension<BulkInsertEngineOptionsExtension>();
            return extension?.GetBulkInsertEngine(this);
        }

        protected virtual void ProcessAudit()
        {
            Logger?.LogInformation($"Audit processing is started for '{GetType().Name}'.");
            try
            {
                var entities = ((IDbContextDependencies)this).StateManager.Entries
                    .Where(ee => ee.Entity is IAuditEntity &&
                    (ee.EntityState == EntityState.Added || ee.EntityState == EntityState.Modified)).Select(ee => ee.Entity as IAuditEntity).ToArray();
                AuditEntityHandler?.AddAuditInfo(entities);
                Logger?.LogInformation(
                    $"Audit processing is completed for '{GetType().Name}' with {entities.Length} audited entities.");
            }
            catch
            {
                Logger?.LogInformation($"Audit processing is completed for '{GetType().Name}' with errors.");
                throw;
            }
        }

        protected virtual void ProcessPostActions()
        {
            Logger?.LogInformation($"Post action processing is started for '{GetType().Name}'.");
            try
            {
                PostProcessor.Execute();
                Logger?.LogInformation(
                    $"Post action processing is completed for '{GetType().Name}' with {PostProcessor.Count} post actions executed.");
            }
            catch (ActionProcessorAggregateException ex)
            {
                var errorCount = ex.InnerExceptions.Count;
                Logger?.LogInformation(
                    $@"Post action processing is completed with some errors for '{GetType().Name}' with {PostProcessor.Count - errorCount} of {PostProcessor.Count} post actions executed successfully.");
                throw;
            }
            catch
            {
                Logger?.LogInformation(
                    $"Post action processing is completed for '{GetType().Name}' with errors.");
                throw;
            }
        }

        protected IAuditEntityHandler AuditEntityHandler
        {
            get
            {
                if (!_auditEntityHandlerPrepared)
                {
                    var extension = _dbContextOptions.FindExtension<AuditEntityOptionsExtension>();
                    if (extension != null)
                    {
                        _auditEntityHandler = extension.GetAuditEntityHandler(this);
                    }
                    _auditEntityHandlerPrepared = true;
                }

                return _auditEntityHandler;
            }
        }

        protected ILogger Logger
        {
            get
            {
                if (!_loggerPrepared)
                {
                    var extension = _dbContextOptions.FindExtension<CoreOptionsExtension>();
                    if (extension != null)
                    {
                        _logger = extension.LoggerFactory?.CreateLogger(GetType());
                    }
                    _loggerPrepared = true;
                }

                return _logger;
            }
        }

        public SqlServerDbContext(Microsoft.EntityFrameworkCore.DbContextOptions dbContextOptions)
            : base(dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }

        TEntity IDbContext.Add<TEntity>(TEntity entity)
        {
            return Add(entity).Entity;
        }

        object IDbContext.Add(object entity)
        {
            return Add(entity).Entity;
        }

        async Task<TEntity> IDbContext.AddAsync<TEntity>(TEntity entity)
        {
            return (await AddAsync(entity).ConfigureAwait(false)).Entity;
        }

        async Task<object> IDbContext.AddAsync(object entity)
        {
            return (await AddAsync(entity).ConfigureAwait(false)).Entity;
        }

        void IDbContext.AddRange<TEntity>(params TEntity[] entities)
        {
            AddRange(entities);
        }

        void IDbContext.AddRange(params object[] entities)
        {
            ((IDbContext)this).AddRange(entities.AsEnumerable());
        }

        void IDbContext.AddRange(IEnumerable<object> entities)
        {
            AddRange(entities);
        }

        async Task IDbContext.AddRangeAsync<TEntity>(params TEntity[] entities)
        {
            await AddRangeAsync(entities).ConfigureAwait(false);
        }

        Task IDbContext.AddRangeAsync(params object[] entities)
        {
            return ((IDbContext)this).AddRangeAsync(entities.AsEnumerable());
        }

        async Task IDbContext.AddRangeAsync(IEnumerable<object> entities)
        {
            await AddRangeAsync(entities).ConfigureAwait(false);
        }

        void IDbContext.Remove<TEntity>(TEntity entity)
        {
            Remove(entity);
        }

        void IDbContext.Remove(object entity)
        {
            Remove(entity);
        }

        void IDbContext.RemoveRange<TEntity>(params TEntity[] entities)
        {
            RemoveRange(entities.AsEnumerable());
        }

        void IDbContext.RemoveRange(params object[] entities)
        {
            RemoveRange(entities);
        }

        void IDbContext.RemoveRange(IEnumerable<object> entities)
        {
            RemoveRange(entities);
        }

        TEntity IDbContext.Update<TEntity>(TEntity entity)
        {
            return Update(entity).Entity;
        }

        object IDbContext.Update(object entity)
        {
            return Update(entity).Entity;
        }

        void IDbContext.UpdateRange<TEntity>(params TEntity[] entities)
        {
            UpdateRange(entities);
        }

        void IDbContext.UpdateRange(params object[] entities)
        {
            ((IDbContext)this).UpdateRange(entities.AsEnumerable());
        }

        void IDbContext.UpdateRange(IEnumerable<object> entities)
        {
            UpdateRange(entities);
        }

        TEntity IDbContext.Find<TEntity>(params object[] ids)
        {
            return Find<TEntity>(ids);
        }

        object IDbContext.Find(Type entityType, params object[] ids)
        {
            return Find(entityType, ids);
        }

        Task<TEntity> IDbContext.FindAsync<TEntity>(params object[] ids)
        {
            return FindAsync<TEntity>(ids);
        }

        Task<object> IDbContext.FindAsync(Type entityType, params object[] ids)
        {
            return FindAsync(entityType, ids);
        }

        IQueryable<TEntity> IDbContext.Query<TEntity>()
        {
            var queryTypeIsStoredProcedure = !(Model.GetEntityType<TEntity>()
                .FindAnnotation(SqlServerSpecificDbAdapter.StoredProcAnnotationName) is null);

            if (queryTypeIsStoredProcedure)
            {
                throw new InvalidOperationException(
                    $"'{typeof(TEntity).Name}' is a stored procedure result type but is incorrectly being used as a table result type.");
            }

            return Set<TEntity>();
        }

        public void Truncate<TEntity>()
            where TEntity : class
        {
            if (UseSqlCommandForTruncate)
            {
                Specific.TruncateOrDelete<TEntity>(DeleteFromOptions.TruncateDependent |
                                                   DeleteFromOptions.UseTransaction);
            }
            else
            {
                ((IDbContext)this).RemoveRange(Set<TEntity>().IgnoreQueryFilters().ToArray());
            }
        }

        public void Truncate(Type entityType)
        {
            TruncateGenericMethodInfo.MakeGenericMethod(entityType)
                .Invoke(this, Array.Empty<object>());
        }

        public async Task TruncateAsync<TEntity>() where TEntity : class
        {
            if (UseSqlCommandForTruncate)
            {
                await Specific.TruncateOrDeleteAsync<TEntity>(DeleteFromOptions.TruncateDependent |
                                                              DeleteFromOptions.UseTransaction)
                    .ConfigureAwait(false);
            }
            else
            {
                ((IDbContext)this).RemoveRange(Set<TEntity>().IgnoreQueryFilters().ToArray());
            }
        }

        public async Task TruncateAsync(Type entityType)
        {
            await ((Task)TruncateAsyncGenericMethodInfo.MakeGenericMethod(entityType)
                .Invoke(this, Array.Empty<object>()))
                .ConfigureAwait(false);
        }

        void IDbContext.SaveChanges()
        {
            SaveChanges();
        }

        Task IDbContext.SaveChangesAsync(CancellationToken cancellationToken)
        {
            return SaveChangesAsync(cancellationToken);
        }

        public ISqlServerBulkInsertEngine BulkInsertEngine =>
            _bulkInsertEngine ?? (_bulkInsertEngine = CreateBulkInsertEngine()) ??
            throw new Exception("BulkInsertEngine hasn't been specified.");

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            Logger?.LogInformation($"Executing SaveChanges method of '{GetType().Name}'.");
            try
            {
                ProcessAudit();
                try
                {
                    var res = base.SaveChanges(acceptAllChangesOnSuccess);
                    ProcessPostActions();
                    return res;
                }
                finally
                {
                    PostProcessor.Clear();
                }
            }
            finally
            {
                Logger?.LogInformation($"Executed SaveChanges method of '{GetType().Name}'.");
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            Logger?.LogInformation($"Executing SaveChangesAsync method of '{GetType().Name}'.");
            try
            {
                ProcessAudit();
                try
                {
                    var res = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    ProcessPostActions();
                    return res;
                }
                finally
                {
                    PostProcessor.Clear();
                }
            }
            finally
            {
                Logger?.LogInformation($"Executed SaveChangesAsync method of '{GetType().Name}'.");
            }
        }

        public SqlServerSpecificDbAdapter Specific => _specific ?? (_specific = CreateSpecificDbAdapter());

        public IActionCollection PostActions => PostProcessor;
    }
}
