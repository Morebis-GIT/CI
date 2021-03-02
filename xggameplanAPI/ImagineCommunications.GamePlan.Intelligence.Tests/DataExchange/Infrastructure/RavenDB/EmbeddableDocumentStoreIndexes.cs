using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes;
using Raven.Client;
using Raven.Client.Indexes;
using xggameplan.RavenDB.Index;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.RavenDB
{
    /// <summary>
    /// Contains indexes that must be used during the unit test session.
    /// </summary>
    public static class EmbeddableDocumentStoreIndexes
    {
        private static readonly List<AbstractIndexCreationTask> _indexesToCreate = new List<AbstractIndexCreationTask> {
            new RavenDocumentsByEntityName(),
            new AnalysisGroups_Default(),
            new Breaks_ByManyFields(),
            new Campaigns_ById(),
            new CampaignSettings_ById(),
            new Campaigns_BySalesAreaCampaignTargetAndStatusAndTargetRatings_RangeSortByTargetRatings(),
            new Campaigns_BySearch(),
            new CampaignWithProduct(),
            new Clashes_ByUid(),
            new Clash_BySearch(),
            new ISRSettings_BySalesArea(),
            new Passes_Default(),
            new Passes_IsLibrariedOnly(),
            new Product_BySearch(),
            new Products_ByUid(),
            new Programmes_ByIdAndSalesAreaStartDateTime(),
            new ProgrammeDictionaries_ByExternalReferenceAndId(),
            new RatingsPredictionsSchedules_BySalesAreaScheduleDay(),
            new Recommendations_Default(),
            new RecommendationsByScenario(),
            new Runs_ByExecuteStartedDateTimeSortByExecuteStartedDateTime(),
            new Runs_BySearch(),
            new RSSettings_BySalesArea(),
            new SalesAreas_ByName(),
            new Scenarios_Default(),
            new Schedules_ByIdAndBreakIdAndSalesAreaAndDate(),
            new ScheduleDocumentsCount(),
            new SmoothFailures_ByRunId(),
            new Spots_ByManyFields(),
            new SpotPlacements_Default(),
            new Universes_Default()
        };

        /// <summary>
        /// Attach required indexes before using the document store.
        /// </summary>
        /// <param name="store">The document store the indexes must be attached to.</param>
        public static void AttachIndexesBeforeUse(
            this IDocumentStore store
            )
        {
            store.ExecuteIndexesAsync(_indexesToCreate)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }
    }
}
