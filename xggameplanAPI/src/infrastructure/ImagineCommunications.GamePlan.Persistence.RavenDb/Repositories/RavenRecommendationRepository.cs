using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Recommendations;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Transformers;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Linq;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenRecommendationRepository : IRecommendationRepository
    {
        private readonly IDocumentSession _session;

        public RavenRecommendationRepository(IDocumentSession session)
        {
            _session = session;
        }

        [Obsolete("Should be named AddRange()")]
        public void Insert(IEnumerable<Recommendation> recommendations, bool setIdentity = true)
        {
            const int PageSize = 200;

            lock (_session)
            {
                var options = new BulkInsertOptions
                {
                    OverwriteExisting = true,
                    BatchSize = PageSize
                };

                int page = 0;

                IEnumerable<Recommendation> valuesToSave = recommendations.Take(PageSize);

                while (valuesToSave.Any())
                {
                    using (var bulkInsert = _session.Advanced.DocumentStore.BulkInsert(null, options))
                    {
                        try
                        {
                            valuesToSave
                                .ToList()
                                .ForEach(recommendation => bulkInsert.Store(recommendation));
                        }
                        catch (TimeoutException)
                        {
                            _session.SaveChanges();
                        }
                    }

                    page++;
                    valuesToSave = recommendations.Skip(page * PageSize).Take(PageSize);
                }
            }
        }

        public IEnumerable<Recommendation> GetByScenarioId(Guid scenarioId)
        {
            lock (_session)
            {
                var recommendations = _session.GetAll<Recommendation>(recommendation => recommendation.ScenarioId == scenarioId,
                                       indexName: Recommendations_Default.DefaultIndexName, isMapReduce: false).ToList();
                return recommendations;
            }
        }

        public IEnumerable<RecommendationSimple> GetRecommendationSimplesByScenarioIdAndProcessors(Guid scenarioId, IEnumerable<string> processors)
        {
            lock (_session)
            {
                List<RecommendationSimple> results = _session
                    .GetAllWithTransform<Recommendation, RecommendationSimple_Transformer, RecommendationSimple>(
                        recommendation =>
                            recommendation.ScenarioId == scenarioId &&
                            recommendation.Processor.In(processors),
                            indexName: Recommendations_Default.DefaultIndexName, isMapReduce: false);
                return results;
            }
        }

        public IEnumerable<RecommendationSimple> GetRecommendationSimplesByScenarioIdsAndProcessors(List<Guid> scenarioIds, IEnumerable<string> processors)
        {
            lock (_session)
            {
                List<RecommendationSimple> results = _session
                    .GetAllWithTransform<Recommendation, RecommendationSimple_Transformer, RecommendationSimple>(
                        recommendation =>
                            recommendation.ScenarioId.In(scenarioIds) &&
                            recommendation.Processor.In(processors),
                            indexName: Recommendations_Default.DefaultIndexName, isMapReduce: false);
                return results;
            }
        }

        public IEnumerable<Recommendation> GetByScenarioIdAndProcessors(Guid scenarioId, IEnumerable<string> processors)
        {
            lock (_session)
            {
                var recommendations = _session.GetAll<Recommendation>(recommendation => recommendation.ScenarioId == scenarioId && recommendation.Processor.In(processors),
                                                             indexName: Recommendations_Default.DefaultIndexName, isMapReduce: false).ToList();
                return recommendations;
            }
        }

        public IEnumerable<RecommendationsByScenarioReduceResult> GetCampaigns(Guid scenarioId)
        {
            lock (_session)
            {
                var bb = _session.GetAllWithWait<RecommendationsByScenarioReduceResult>(
                   _ => _.ScenarioId == scenarioId, "RecommendationsByScenario", true,
                   s => s.ScenarioId == Guid.Empty);

                return bb;
            }
        }

        public IEnumerable<RecommendationsByScenarioReduceResult> GetMetrics(Guid scenarioId, string campaignId)
        {
            lock (_session)
            {
                return _session
                    .Query<RecommendationsByScenarioReduceResult, RecommendationsByScenario>()
                    .Customize(x => x.WaitForNonStaleResults(DocumentSessionExtensions.LongWaitTimeoutForNonStaleIndexes))
                    .Statistics(out RavenQueryStatistics stats)
                    .Where(x => x.ScenarioId == scenarioId)
                    .Where(x => x.ExternalCampaignNumber == campaignId);
            }
        }

        public void RemoveByScenarioId(Guid scenarioId)
        {
            lock (_session)
            {
                _session.Advanced.DocumentStore.DatabaseCommands.DeleteByIndex(Recommendations_Default.DefaultIndexName,
                  new IndexQuery()
                  {
                      Query = $"ScenarioId:{scenarioId}"
                  }).WaitForCompletion();
                _session.SaveChanges();
            }
        }

        public void RemoveByScenarioIdAndProcessors(Guid scenarioId, IEnumerable<string> processors)
        {
            IEnumerable<string> cleanProcessors = processors
                .Where(p => !String.IsNullOrWhiteSpace(p));

            if (!cleanProcessors.Any())
            {
                return;
            }

            string queryString = $"ScenarioId:{scenarioId} AND @in<Processor>:({String.Join(",", cleanProcessors)})";

            lock (_session)
            {
                _ = _session.Advanced.DocumentStore.DatabaseCommands.DeleteByIndex(
                        Recommendations_Default.DefaultIndexName,
                        new IndexQuery
                        {
                            Query = queryString,
                            DisableCaching = true,
                            ExplainScores = false
                        },
                        new BulkOperationOptions
                        {
                            StaleTimeout = new TimeSpan(hours: 1, minutes: 0, seconds: 0),
                            RetrieveDetails = false,
                            MaxOpsPerSec = 1024 * 1024
                        }
                    ).WaitForCompletion();

                SaveChanges();
            }
        }

        public void SaveChanges()
        {
            lock (_session)
            {
                _session.SaveChanges();
            }
        }
    }
}
