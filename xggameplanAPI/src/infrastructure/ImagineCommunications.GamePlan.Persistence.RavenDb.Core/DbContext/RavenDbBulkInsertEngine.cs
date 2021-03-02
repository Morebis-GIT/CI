using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Core.Interfaces;
using Raven.Abstractions.Extensions;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Core.DbContext
{
    public class RavenDbBulkInsertEngine : IRavenDbBulkInsertEngine
    {
        private readonly IDocumentSession _documentSession;
        private readonly IAsyncDocumentSession _asyncDocumentSession;

        public RavenDbBulkInsertEngine(IDocumentSession documentSession, IAsyncDocumentSession asyncDocumentSession)
        {
            _documentSession = documentSession ?? throw new ArgumentNullException(nameof(documentSession));
            _asyncDocumentSession =
                asyncDocumentSession ?? throw new ArgumentNullException(nameof(asyncDocumentSession));
        }

        public void BulkInsert<TEntity>(IList<TEntity> entities, BulkInsertOptions options = null) where TEntity : class
        {
            if (options != null)
            {
                options.OverwriteExisting = false;
            }

            using (var bulkInsert = _documentSession.Advanced.DocumentStore.BulkInsert(null, options))
            {
                entities.ForEach(item => bulkInsert.Store(item));
                if (options?.IsWaitForLastTaskToFinish ?? false)
                {
                    bulkInsert.WaitForLastTaskToFinish().Wait();
                }
            }
        }

        public async Task BulkInsertAsync<TEntity>(IList<TEntity> entities, BulkInsertOptions options = null, CancellationToken cancellationToken = default) 
            where TEntity : class
        {
            if (options != null)
            {
                options.OverwriteExisting = false;
            }

            using (var bulkInsert = _asyncDocumentSession.Advanced.DocumentStore.BulkInsert(null, options))
            {
                entities.ForEach(item => bulkInsert.Store(item));
                if (options?.IsWaitForLastTaskToFinish ?? false)
                {
                    await bulkInsert.WaitForLastTaskToFinish().ConfigureAwait(false);
                }
            }
        }

        public void BulkInsertOrUpdate<TEntity>(IList<TEntity> entities, BulkInsertOptions options = null) where TEntity : class
        {
            if (options == null)
            {
                options = new BulkInsertOptions();
            }
            options.OverwriteExisting = true;

            using (var bulkInsert = _documentSession.Advanced.DocumentStore.BulkInsert(null, options))
            {
                entities.ForEach(item => bulkInsert.Store(item));
                if (options.IsWaitForLastTaskToFinish)
                {
                    bulkInsert.WaitForLastTaskToFinish().Wait();
                }
            }
        }

        public async Task BulkInsertOrUpdateAsync<TEntity>(IList<TEntity> entities, BulkInsertOptions options = null, CancellationToken cancellationToken = default) 
            where TEntity : class
        {
            if (options == null)
            {
                options = new BulkInsertOptions();
            }
            options.OverwriteExisting = true;

            using (var bulkInsert = _asyncDocumentSession.Advanced.DocumentStore.BulkInsert(null, options))
            {
                entities.ForEach(item => bulkInsert.Store(item));
                if (options.IsWaitForLastTaskToFinish)
                {
                    await bulkInsert.WaitForLastTaskToFinish().ConfigureAwait(false);
                }
            }
        }
    }
}
