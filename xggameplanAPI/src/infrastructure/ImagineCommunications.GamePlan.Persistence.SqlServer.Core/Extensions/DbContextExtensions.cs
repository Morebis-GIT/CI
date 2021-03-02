using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.BulkInsert;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Finding;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.PostProcessing;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.PostProcessing.Builders;
using Microsoft.EntityFrameworkCore;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions
{
    public static class DbContextExtensions
    {
        private static void CreatePostAction<TBuilder>(
            ISqlServerDbContext dbContext,
            Action<TBuilder> postAction,
            TBuilder postActionBuilder) where TBuilder : IPostActionBuilder
        {
            postAction(postActionBuilder);
            var action = postActionBuilder.Build();
            if (action != null)
            {
                dbContext.PostActions.Add(action);
            }
        }

        private static void ExecutePostAction<TBuilder>(
            Action<TBuilder> postAction,
            TBuilder postActionBuilder) where TBuilder : IPostActionBuilder
        {
            postAction(postActionBuilder);
            postActionBuilder.Build()?.Execute();
        }

        public static TEntity Add<TEntity>(this ISqlServerDbContext dbContext, TEntity entity,
            Action<EntityActionBuilder<TEntity>> postAction, IMapper mapper) where TEntity : class
        {
            if (postAction == null)
            {
                throw new ArgumentNullException(nameof(postAction));
            }

            var res = (dbContext ?? throw new ArgumentNullException(nameof(dbContext))).Add(
                entity ?? throw new ArgumentNullException(nameof(entity)));
            CreatePostAction(dbContext, postAction, new EntityActionBuilder<TEntity>(res, mapper));

            return res;
        }

        public static async Task<TEntity> AddAsync<TEntity>(this ISqlServerDbContext dbContext, TEntity entity,
            Action<EntityActionBuilder<TEntity>> postAction, IMapper mapper) where TEntity : class
        {
            if (postAction == null)
            {
                throw new ArgumentNullException(nameof(postAction));
            }

            var res = await (dbContext ?? throw new ArgumentNullException(nameof(dbContext))).AddAsync(
                entity ?? throw new ArgumentNullException(nameof(entity))).ConfigureAwait(false);
            CreatePostAction(dbContext, postAction, new EntityActionBuilder<TEntity>(res, mapper));

            return res;
        }

        public static void AddRange<TEntity>(this ISqlServerDbContext dbContext, TEntity[] entities,
            Action<EntityCollectionActionBuilder<TEntity>> postAction, IMapper mapper) where TEntity : class
        {
            if (postAction == null)
            {
                throw new ArgumentNullException(nameof(postAction));
            }

            (dbContext ?? throw new ArgumentNullException(nameof(dbContext))).AddRange(
                entities ?? throw new ArgumentNullException(nameof(entities)));
            CreatePostAction(dbContext, postAction, new EntityCollectionActionBuilder<TEntity>(entities, mapper));
        }

        public static async Task AddRangeAsync<TEntity>(this ISqlServerDbContext dbContext, TEntity[] entities,
            Action<EntityCollectionActionBuilder<TEntity>> postAction, IMapper mapper) where TEntity : class
        {
            if (postAction == null)
            {
                throw new ArgumentNullException(nameof(postAction));
            }

            await (dbContext ?? throw new ArgumentNullException(nameof(dbContext))).AddRangeAsync(
                entities ?? throw new ArgumentNullException(nameof(entities))).ConfigureAwait(false);
            CreatePostAction(dbContext, postAction, new EntityCollectionActionBuilder<TEntity>(entities, mapper));
        }

        public static TEntity Update<TEntity>(this ISqlServerDbContext dbContext, TEntity entity,
            Action<EntityActionBuilder<TEntity>> postAction, IMapper mapper) where TEntity : class
        {
            if (postAction == null)
            {
                throw new ArgumentNullException(nameof(postAction));
            }

            var res = (dbContext ?? throw new ArgumentNullException(nameof(dbContext))).Update(entity);
            CreatePostAction(dbContext, postAction, new EntityActionBuilder<TEntity>(res, mapper));

            return res;
        }

        public static void UpdateRange<TEntity>(this ISqlServerDbContext dbContext, TEntity[] entities,
            Action<EntityCollectionActionBuilder<TEntity>> postAction, IMapper mapper) where TEntity : class
        {
            if (postAction == null)
            {
                throw new ArgumentNullException(nameof(postAction));
            }

            (dbContext ?? throw new ArgumentNullException(nameof(dbContext))).UpdateRange(
                entities ?? throw new ArgumentNullException(nameof(entities)));
            CreatePostAction(dbContext, postAction, new EntityCollectionActionBuilder<TEntity>(entities, mapper));
        }

        public static TEntity Find<TEntity>(this ISqlServerDbContext dbContext, object[] ids, Action<FindBuilder<TEntity>> findAction)
            where TEntity : class
        {
            if (findAction == null)
            {
                throw new ArgumentNullException(nameof(findAction));
            }
            var entity = (dbContext ?? throw new ArgumentNullException(nameof(dbContext))).Find<TEntity>(ids);
            if (entity != null)
            {
                var builder = new FindBuilder<TEntity>(dbContext.Specific.Entry(entity));
                findAction(builder);
            }

            return entity;
        }

        public static async Task<TEntity> FindAsync<TEntity>(this ISqlServerDbContext dbContext, object[] ids, Action<FindBuilder<TEntity>> findAction)
            where TEntity : class
        {
            if (findAction == null)
            {
                throw new ArgumentNullException(nameof(findAction));
            }

            var entity = await (dbContext ?? throw new ArgumentNullException(nameof(dbContext))).FindAsync<TEntity>(ids)
                .ConfigureAwait(false);
            if (entity != null)
            {
                var builder = new FindBuilder<TEntity>(dbContext.Specific.Entry(entity));
                findAction(builder);
            }

            return entity;
        }

        public static void BulkInsert<TEntity>(this ISqlServerBulkInsertEngine bulkInsertEngine, IList<TEntity> entities,
            Action<BulkInsertActionBuilder<TEntity>> postAction, IMapper mapper)
            where TEntity : class
        {
            BulkInsert(bulkInsertEngine, entities, null, postAction, mapper);
        }

        public static void BulkInsert<TEntity>(this ISqlServerBulkInsertEngine bulkInsertEngine, IList<TEntity> entities,
            BulkInsertOptions options, Action<BulkInsertActionBuilder<TEntity>> postAction, IMapper mapper)
            where TEntity : class
        {
            if (postAction == null)
            {
                throw new ArgumentNullException(nameof(postAction));
            }

            (bulkInsertEngine ?? throw new ArgumentNullException(nameof(bulkInsertEngine))).BulkInsert(
                entities ?? throw new ArgumentNullException(nameof(entities)), options);
            ExecutePostAction(postAction, new BulkInsertActionBuilder<TEntity>(entities, mapper));
        }

        public static void BulkInsertOrUpdate<TEntity>(this ISqlServerBulkInsertEngine bulkInsertEngine, IList<TEntity> entities,
            Action<BulkInsertActionBuilder<TEntity>> postAction, IMapper mapper)
            where TEntity : class
        {
            BulkInsertOrUpdate(bulkInsertEngine, entities, null, postAction, mapper);
        }

        public static void BulkInsertOrUpdate<TEntity>(this ISqlServerBulkInsertEngine bulkInsertEngine, IList<TEntity> entities,
            BulkInsertOptions options, Action<BulkInsertActionBuilder<TEntity>> postAction, IMapper mapper)
            where TEntity : class
        {
            if (postAction == null)
            {
                throw new ArgumentNullException(nameof(postAction));
            }

            (bulkInsertEngine ?? throw new ArgumentNullException(nameof(bulkInsertEngine))).BulkInsertOrUpdate(
                entities ?? throw new ArgumentNullException(nameof(entities)), options);
            ExecutePostAction(postAction, new BulkInsertActionBuilder<TEntity>(entities, mapper));
        }

        public static async Task BulkInsertOrUpdateAsync<TEntity>(this ISqlServerBulkInsertEngine bulkInsertEngine, IList<TEntity> entities,
            Action<BulkInsertActionBuilder<TEntity>> postAction, IMapper mapper)
            where TEntity : class
        {
            await BulkInsertOrUpdateAsync(bulkInsertEngine, entities, null, postAction, mapper).ConfigureAwait(false);
        }

        public static async Task BulkInsertOrUpdateAsync<TEntity>(this ISqlServerBulkInsertEngine bulkInsertEngine, IList<TEntity> entities,
            BulkInsertOptions options, Action<BulkInsertActionBuilder<TEntity>> postAction, IMapper mapper)
            where TEntity : class
        {
            if (postAction == null)
            {
                throw new ArgumentNullException(nameof(postAction));
            }

            await (bulkInsertEngine ?? throw new ArgumentNullException(nameof(bulkInsertEngine))).BulkInsertOrUpdateAsync(
                entities ?? throw new ArgumentNullException(nameof(entities)), options).ConfigureAwait(false);
            ExecutePostAction(postAction, new BulkInsertActionBuilder<TEntity>(entities, mapper));
        }

        public static void SaveChangesWithConcurrencyConflictsResolving(this ISqlServerDbContext dbContext)
        {
            if (dbContext is null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            while (true)
            {
                try
                {
                    dbContext.SaveChanges();
                    break;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (ex.Entries.Count == 0)
                    {
                        throw;
                    }

                    foreach (var entry in ex.Entries)
                    {
                        entry.State = EntityState.Detached;
                    }
                }
            }
        }

        public static async Task SaveChangesWithConcurrencyConflictsResolvingAsync(this ISqlServerDbContext dbContext,
            CancellationToken cancellationToken = default)
        {
            if (dbContext is null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            while (true)
            {
                try
                {
                    await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    break;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (ex.Entries.Count == 0)
                    {
                        throw;
                    }

                    foreach (var entry in ex.Entries)
                    {
                        entry.State = EntityState.Detached;
                    }
                }
            }
        }
    }
}
