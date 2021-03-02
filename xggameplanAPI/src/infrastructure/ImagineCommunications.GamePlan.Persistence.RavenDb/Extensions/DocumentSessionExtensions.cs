using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Indexes;
using Raven.Client.Linq;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions
{
    /// <summary>
    /// This class is for a bit of a hack so that it is possible to return the
    /// document Id property using the Select method. If you try to return the
    /// Id property on it's own then the returned value is the data type's default.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class RavenDocumentIdProperty<T>
    {
        public T Id { get; set; }
    }

    public static class DocumentSessionExtensions
    {
        public static TimeSpan DefaultWaitTimeoutForNonStaleIndexes => TimeSpan.FromMinutes(3);
        public static TimeSpan DefaultWaitForRetryOnStaleIndexes => TimeSpan.FromMilliseconds(500);
        public static TimeSpan LongWaitTimeoutForNonStaleIndexes => TimeSpan.FromMinutes(10);

        public static List<T> GetAll<T>(this IDocumentSession session)
        {
            if (session is null)
            {
                throw new ArgumentNullException(nameof(session));
            }

            const int pageSize = 1024;
            int pageNumber = 0;

            RavenQueryStatistics stats;
            var result = session
                .Query<T>()
                .Statistics(out stats)
                .Take(pageSize)
                .ToList();

            pageNumber++;

            while ((pageNumber * pageSize) <= stats.TotalResults)
            {
                var documents = session
                    .Query<T>()
                    .Skip(pageNumber * pageSize)
                    .Take(pageSize)
                    .ToList();

                if (documents.Count > 0)
                {
                    result.AddRange(documents);
                }

                pageNumber++;
            }

            return result;
        }

        public static List<T> GetAll<T>(this IDocumentSession session, Expression<Func<T, bool>> expression)
        {
            // We can't use RavenQueryStatistics here since it always returns
            // total number of documents in database not just the ones that fit
            // our expression
            int size = 1024;
            int page = 0;

            var objects = session
                .Query<T>()
                .Where(expression)
                .Skip(page * size)
                .Take(size)
                .ToList();

            size = objects.Count;
            page++;

            while (size >= 1024)
            {
                var result = session
                    .Query<T>()
                    .Where(expression)
                    .Skip(page * size)
                    .Take(size)
                    .ToList();

                objects.AddRange(result);
                size = result.Count;
                page++;
            }

            return objects;
        }

        public static IReadOnlyCollection<TProjectionModel> GetAllUsingProjection
            <T, TIndexCreator, TProjectionModel>
            (this IDocumentSession session)
            where TIndexCreator : AbstractIndexCreationTask, new()
        {
            return ExecuteSearchQuery(Query);

            IRavenQueryable<TProjectionModel> Query(int start)
            {
                return SearchQuery<T, TIndexCreator>(session, start)
                    .ProjectFromIndexFieldsInto<TProjectionModel>();
            }
        }

        public static IReadOnlyCollection<TProjectionModel> GetAllUsingProjection
            <T, TIndexCreator, TProjectionModel>
            (this IDocumentSession session, Expression<Func<T, bool>> expression)
            where TIndexCreator : AbstractIndexCreationTask, new()
        {
            return ExecuteSearchQuery(Query);

            IRavenQueryable<TProjectionModel> Query(int start)
            {
                return SearchQuery<T, TIndexCreator>(session, start)
                    .Where(expression)
                    .ProjectFromIndexFieldsInto<TProjectionModel>();
            }
        }

        private static IRavenQueryable<T> SearchQuery
            <T, TIndexCreator>
            (IDocumentSession session, int start)
            where TIndexCreator : AbstractIndexCreationTask, new()
        {
            return session
                .Query<T, TIndexCreator>()
                .Skip(start);
        }

        private static List<T> ExecuteSearchQuery<T>(Func<int, IRavenQueryable<T>> queryable)
        {
            int start = 0;
            var objectList = new List<T>();
            List<T> query = null;

            while (true)
            {
                query = queryable(start).ToList();

                start += query.Count;

                if (query.Count > 0)
                {
                    objectList.AddRange(query);
                }
                else
                {
                    break;
                }
            }

            return objectList;
        }

        public static List<T> GetAllWithWait<T>(
            this IDocumentSession session,
            Expression<Func<T, bool>> expression,
            string indexName,
            bool isMapReduce,
            Expression<Func<T, bool>> testExpression)
        {
            // Using WaitForNonStaleResults with a streaming query results in
            // the error below: Since Stream() does not wait for indexing(by
            // design), streaming query with WaitForNonStaleResults is not
            // supported. To overcome it then we issue a dummy non-streaming
            // query before.
            if (testExpression != null)
            {
                var testResults = session
                    .Query<T>(indexName, isMapReduce)
                    .Customize(x => x.WaitForNonStaleResults(LongWaitTimeoutForNonStaleIndexes))
                    .Where(testExpression)
                    .FirstOrDefault();
            }

            var results = new List<T>();
            var query = session
                .Query<T>(indexName, isMapReduce)
                .Where(expression);

            var enumerator = session.Advanced.Stream(query);

            while (enumerator.MoveNext())
            {
                results.Add(enumerator.Current.Document);
            }

            return results;
        }

        /// <summary>
        /// Get all documents of type {T} applying the transformation to the results.
        /// </summary>
        /// <typeparam name="T1">Type to transform</typeparam>
        /// <typeparam name="T2">Transformer</typeparam>
        /// <typeparam name="T3">Result type for transform</typeparam>
        /// <param name="session"></param>
        /// <param name="expression"></param>
        /// <param name="indexName"></param>
        /// <param name="isMapReduce"></param>
        /// <returns></returns>
        public static List<T3> GetAllWithTransform<T1, T2, T3>(
            this IDocumentSession session,
            Expression<Func<T1, bool>> expression,
            string indexName,
            bool isMapReduce
            )
            where T2 : AbstractTransformerCreationTask, new()
        {
            var results = new List<T3>();
            var query = session
                .Query<T1>(indexName, isMapReduce)
                .Where(expression)
                .TransformWith<T2, T3>();

            var enumerator = session.Advanced.Stream(query);

            while (enumerator.MoveNext())
            {
                results.Add(enumerator.Current.Document);
            }

            return results;
        }

        public static List<T> GetAll<T>(
            this IDocumentSession session,
            Expression<Func<T, bool>> expression,
            string indexName,
            bool isMapReduce)
        {
            var results = new List<T>();
            var query = session
                .Query<T>(indexName, isMapReduce)
                .Where(expression);

            using (var enumerator = session.Advanced.Stream(query))
            {
                while (enumerator.MoveNext())
                {
                    results.Add(enumerator.Current.Document);
                }
            }

            return results;
        }

        /// <summary>
        /// Query Index +Search+Orderby+Skip+Top
        /// </summary>
        /// <typeparam name="TIndexResult">Index result Type</typeparam>
        /// <typeparam name="TIndex">Index</typeparam>
        /// <typeparam name="T">Query Result</typeparam>
        /// <param name="session"></param>
        /// <param name="expression"></param>
        /// <param name="totalResult"></param>
        /// <param name="sortBy"></param>
        /// <param name="sortDirection"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public static List<T> GetAll<TIndexResult, TIndex, T>(
            IDocumentSession session,
            Expression<Func<TIndexResult, bool>> expression,
            out int totalResult,
            string sortBy,
            string sortDirection = "asc",
            int? skip = null,
            int? take = null)
            where TIndex : AbstractIndexCreationTask, new()
        {
            var results = new List<T>();
            IQueryable<T> query;

            if (expression != null)
            {
                query = session.Query<TIndexResult, TIndex>()
                   .Where(expression).OrderBy(sortBy, sortDirection).OfType<T>();
            }
            else
            {
                query = session.Query<TIndexResult, TIndex>()
                   .OrderBy(sortBy, sortDirection).OfType<T>();
            }

            var enumerator = session.Advanced.Stream(query, out QueryHeaderInformation streamQueryStats);

            while (enumerator.MoveNext())
            {
                results.Add(enumerator.Current.Document);
            }

            if (skip != null && results.Count > 0)
            {
                results = results.Skip(skip.Value).ToList();
            }

            if (take != null && results.Count > 0)
            {
                results = results.Take(take.Value)?.ToList();
            }

            totalResult = streamQueryStats.TotalResults;

            return results;
        }

        public static List<TTransformResult> GetAll<TIndexResult, TIndex, TTransformer, TTransformResult>(
            IDocumentSession session,
            Expression<Func<TIndexResult, bool>> expression,
            out int totalResult,
            string sortBy,
            string sortDirection = "asc",
            int? skip = null,
            int? take = null)
            where TIndex : AbstractIndexCreationTask, new()
            where TTransformer : AbstractTransformerCreationTask, new()
        {
            var results = new List<TTransformResult>();
            IQueryable<TTransformResult> query;

            if (expression != null)
            {
                query = session.Query<TIndexResult, TIndex>()
                    .Where(expression).TransformWith<TTransformer, TTransformResult>().OrderBy(sortBy, sortDirection);
            }
            else
            {
                query = session.Query<TIndexResult, TIndex>().TransformWith<TTransformer, TTransformResult>()
                    .OrderBy(sortBy, sortDirection).OfType<TTransformResult>();
            }

            var enumerator = session.Advanced.Stream(query);

            while (enumerator.MoveNext())
            {
                results.Add(enumerator.Current.Document);
            }

            totalResult = results.Count;

            if (skip != null && results.Count > 0)
            {
                results = results.Skip(skip.Value).ToList();
            }

            if (take != null && results.Count > 0)
            {
                results = results.Take(take.Value)?.ToList();
            }

            return results;
        }

        private static IRavenQueryable<T> OrderBy<T>(
            this IRavenQueryable<T> query,
            string sortBy,
            string sortDirection = "asc")
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return query;
            }

            var param = Expression.Parameter(typeof(T), "item");
            var sortExpression = Expression.Lambda<Func<T, object>>
                (Expression.Convert(Expression.Property(param, sortBy), typeof(object)), param);

            switch (sortDirection.ToLower())
            {
                case "asc":
                    return query.OrderBy<T, object>(sortExpression);

                default:
                    return query.OrderByDescending<T, object>(sortExpression);
            }
        }

        /// <summary>
        /// Returns number of documents of specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session"></param>
        /// <returns></returns>
        public static int CountAll<T>(this IDocumentSession session) =>
            session
                .Query<T>()
                .Count();

        /// <summary>
        /// Query with Select expression, projects query result to new form
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="session"></param>
        /// <param name="expression"></param>
        /// <param name="select"></param>
        /// <param name="indexName"></param>
        /// <param name="isMapReduce"></param>
        /// <returns></returns>
        public static List<T2> GetAllWithSelect<T1, T2>(
            this IDocumentSession session,
            Expression<Func<T1, bool>> expression,
            Expression<Func<T1, T2>> select,
            string indexName,
            bool isMapReduce)
        {
            var results = new List<T2>();
            var query = session
                .Query<T1>(indexName, isMapReduce)
                .Where(expression)
                .Select(select);

            var enumerator = session.Advanced.Stream(query);

            while (enumerator.MoveNext())
            {
                results.Add(enumerator.Current.Document);
            }

            return results;
        }

        /// <summary>
        /// Waits for index not to be stale. This is necessary because streaming
        /// queries do not allow a call to Customize in order to wait for non
        /// stale results and so a dummy test query has to be executed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session"></param>
        /// <param name="indexName"></param>
        /// <param name="isMapReduce"></param>
        /// <param name="testExpression"></param>
        public static void WaitForIndexes<T>(
            this IDocumentSession session,
            string indexName,
            bool isMapReduce,
            Expression<Func<T, bool>> testExpression
            )
        {
            const int salesResultTimeOutSeconds = 30;

            var salesResultTimeout = TimeSpan.FromSeconds(salesResultTimeOutSeconds);

            lock (session)
            {
                try
                {
                    _ = session
                        .Query<T>(indexName, isMapReduce)
                        .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite(salesResultTimeout))
                        .Any(testExpression);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Timeout expired waiting for an index to update. Index name: {indexName}");
                    Debug.WriteLine(ex.ToString());
                }
            }
        }

        public static void TryDelete(this IDocumentSession session, string indexname, string tag)
        {
            Exception ex = null;
            bool retry = false;
            var startTime = DateTime.UtcNow;

            //attempt to do the delete every n seconds for n minutes
            do
            {
                if (retry)
                {
                    Thread.Sleep(DefaultWaitForRetryOnStaleIndexes);
                }

                ex = DeleteAll.Invoke(session, indexname, tag);
                retry = true;
            } while (ex?.Message.Contains("index is stale") == true && (DateTime.UtcNow - startTime < DefaultWaitTimeoutForNonStaleIndexes));

            if (ex != null)
            {
                throw ex;
            }
        }

        public delegate Exception DeleteA(IDocumentSession session, string indexname, string tag);

        private static readonly DeleteA DeleteAll = DelAll;

        public static Exception DelAll(IDocumentSession session, string indexname, string tag)
        {
            Exception ex = null;
            try
            {
                var RavenJToken_results = session.Advanced.DocumentStore.DatabaseCommands.DeleteByIndex(indexname, new IndexQuery
                {
                    Query = "Tag:[[" + tag + "]]"
                }).WaitForCompletion();
            }
            catch (Exception e)
            {
                return e;
            }

            return ex;
        }
    }

    public static class RavenRepositoryExtensions
    {
        /// <summary>
        /// To avoid 'Exceeded maximum clause count in the query.' exception in
        /// case when elements in clause greater than 1000 we will group them
        /// for several request
        /// </summary>
        /// <param name="elements">search terms to be grouped</param>
        /// <param name="maxClauseCount">
        /// maximum number of elements to be searched by raven
        /// </param>
        /// <returns>Grouped terms to be searched</returns>
        public static IEnumerable<IGrouping<int, T>> GroupElementsForInClause<T>(IEnumerable<T> elements, int maxClauseCount = 1000)
        {
            if (maxClauseCount == 0)
            {
                return Enumerable.Empty<IGrouping<int, T>>();
            }

            maxClauseCount = maxClauseCount < 0 ? 1000 : maxClauseCount;

            return elements.Select((x, i) => new { Item = x, Index = i })
                .GroupBy(x => x.Index / maxClauseCount, x => x.Item);
        }
    }
}
