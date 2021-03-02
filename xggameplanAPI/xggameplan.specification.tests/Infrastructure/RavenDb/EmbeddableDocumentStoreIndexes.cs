using ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes;
using Raven.Client;
using Raven.Client.Indexes;
using xggameplan.RavenDB.Index;

namespace xggameplan.specification.tests.Infrastructure.RavenDb
{
    /// <summary>
    /// Contains indexes that must be used during the unit test session.
    /// </summary>
    public static class EmbeddableDocumentStoreIndexes
    {
        /// <summary>
        /// Attach required indexes before using the document store.
        /// </summary>
        /// <param name="store">The document store the indexes must be attached to.</param>
        public static void AttachIndexesBeforeUse(
            this IDocumentStore store
            )
        {
            // Note: need to use side-by-side execute because multiple tests can try to
            // create a specific index at the same time. This causes the resulting DLL
            // to be locked for writing and the tests start crashing.
            new AnalysisGroups_Default().SideBySideExecute(store);
            new Breaks_ByManyFields().SideBySideExecute(store);
            new Campaigns_ById().SideBySideExecute(store);
            new CampaignSettings_ById().SideBySideExecute(store);
            new Campaigns_BySalesAreaCampaignTargetAndStatusAndTargetRatings_RangeSortByTargetRatings().SideBySideExecute(store);
            new Campaigns_BySearch().SideBySideExecute(store);
            new CampaignWithProduct().SideBySideExecute(store);
            new Clashes_ByUid().SideBySideExecute(store);
            new Clash_BySearch().SideBySideExecute(store);
            new ISRSettings_BySalesArea().SideBySideExecute(store);
            new Passes_Default().SideBySideExecute(store);
            new Passes_IsLibrariedOnly().SideBySideExecute(store);
            new Product_BySearch().SideBySideExecute(store);
            new Products_ByUid().SideBySideExecute(store);
            new Programmes_ByIdAndSalesAreaStartDateTime().SideBySideExecute(store);
            new ProgrammeDictionaries_ByExternalReferenceAndId().SideBySideExecute(store);
            new RatingsPredictionsSchedules_BySalesAreaScheduleDay().SideBySideExecute(store);
            new RavenDocumentsByEntityName().SideBySideExecute(store);
            new Recommendations_Default().SideBySideExecute(store);
            new RecommendationsByScenario().SideBySideExecute(store);
            new Runs_ByExecuteStartedDateTimeSortByExecuteStartedDateTime().SideBySideExecute(store);
            new Runs_BySearch().SideBySideExecute(store);
            new RSSettings_BySalesArea().SideBySideExecute(store);
            new SalesAreas_ByName().SideBySideExecute(store);
            new Scenarios_Default().SideBySideExecute(store);
            new Schedules_ByIdAndBreakIdAndSalesAreaAndDate().SideBySideExecute(store);
            new ScheduleDocumentsCount().SideBySideExecute(store);
            new SmoothFailures_ByRunId().SideBySideExecute(store);
            new Spots_ByManyFields().SideBySideExecute(store);
            new SpotPlacements_Default().SideBySideExecute(store);
            new Universes_Default().SideBySideExecute(store);
        }
    }
}
