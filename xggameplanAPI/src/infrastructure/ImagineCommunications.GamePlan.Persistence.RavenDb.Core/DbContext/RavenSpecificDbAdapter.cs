using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Linq;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Core.DbContext
{
    public class RavenSpecificDbAdapter
    {
        private readonly IDocumentSession _documentSession;
        private readonly IAsyncDocumentSession _asyncDocumentSession;
        private string[] _indexNames;

        protected IEnumerable<string> IndexNames =>
            _indexNames ?? (_indexNames =
                _documentSession.Advanced.DocumentStore.DatabaseCommands.GetIndexNames(0, int.MaxValue));

        protected IEnumerable<T> GetAllInternal<T>(int pageSize, Action pageAction = null)
        {
            var page = 0;
            var query = _documentSession.Query<T>().Take(pageSize);
            RavenQueryStatistics stats = null;

            do
            {
                var source = query.Skip(page * pageSize);
                source = page == 0 ? source.Statistics(out stats) : source;
                foreach (var item in source)
                {
                    yield return item;
                }

                pageAction?.Invoke();
                page++;
            } while (page * pageSize <= (stats?.TotalResults ?? 0));
        }

        public BulkInsertOptions BulkInsertOptions { get; } = new BulkInsertOptions();

        public RavenSpecificDbAdapter(IDocumentSession documentSession, IAsyncDocumentSession asyncDocumentSession)
        {
            _documentSession = documentSession ?? throw new ArgumentNullException(nameof(documentSession));
            _asyncDocumentSession =
                asyncDocumentSession ?? throw new ArgumentNullException(nameof(asyncDocumentSession));
        }

        public IRavenQueryable<T> Query<T>() where T : class
        {
            return _documentSession.Query<T>();
        }

        public IDocumentQuery<T> DocumentQuery<T>() where T : class
        {
            return _documentSession.Advanced.DocumentQuery<T>();
        }

        public void DeleteByIndex(string indexName)
        {
            _ = _documentSession.Advanced.DocumentStore.DatabaseCommands.DeleteByIndex(indexName, new IndexQuery());
        }

        public void WaitForIndexesAfterSaveChanges()
        {
            _documentSession.Advanced.WaitForIndexesAfterSaveChanges();
            _asyncDocumentSession.Advanced.WaitForIndexesAfterSaveChanges();
        }

        public void WaitForIndex(string indexName, bool isMapReduce = false)
        {
            var testResults = _documentSession
                .Query<dynamic>(indexName, isMapReduce)
                .Customize(x => x.WaitForNonStaleResults())
                .FirstOrDefault();
        }

        public async Task WaitForIndexAsync(string indexName, bool isMapReduce = false)
        {
            _ = await _asyncDocumentSession
                .Query<dynamic>(indexName, isMapReduce)
                .Customize(x => x.WaitForNonStaleResults())
                .FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public void WaitForIndexes(bool isMapReduce = false)
        {
            foreach (var indexName in IndexNames)
            {
                WaitForIndex(indexName, isMapReduce);
            }
        }

        public async Task WaitForIndexesAsync(bool isMapReduce = false)
        {
            foreach (var indexName in IndexNames)
            {
                await WaitForIndexAsync(indexName, isMapReduce).ConfigureAwait(false);
            }
        }

        public IEnumerable<T> GetAll<T>(int pageSize = 1024)
        {
            return GetAllInternal<T>(pageSize);
        }

        public IEnumerable<T> GetAllWithNoTracking<T>(int pageSize = 1024)
        {
            return GetAllInternal<T>(pageSize, () => _documentSession.Advanced.Clear());
        }
    }
}
