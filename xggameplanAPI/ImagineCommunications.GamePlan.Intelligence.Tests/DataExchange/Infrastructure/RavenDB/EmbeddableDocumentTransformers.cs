using ImagineCommunications.GamePlan.Persistence.RavenDb.Transformers;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.RavenDB
{
    /// <summary>
    /// Contains transformers that must be used during the unit test session.
    /// </summary>
    public static class EmbeddableDocumentTransformers
    {
        /// <summary>
        /// Attach required transforms before using the document store.
        /// </summary>
        /// <param name="store">The document store the transformers must be attached to.</param>
        public static void AttachTransformersBeforeUse(this IDocumentStore store)
        {
            store.ExecuteTransformer(new ClashTransformer_BySearch());
            store.ExecuteTransformer(new ProductTransformer_BySearch());
            store.ExecuteTransformer(new ProductAdvertiserTransformer_BySearch());
            store.ExecuteTransformer(new ProgrammeTransformer_BySearch());
            store.ExecuteTransformer(new RecommendationSimple_Transformer());
            store.ExecuteTransformer(new RunsWithScenarioId_Transformer());
            store.ExecuteTransformer(new ScenariosWithPassId_Transformer());
            store.ExecuteTransformer(new RatingsPredictionSchedule_TransformerRating());
            store.ExecuteTransformer(new RunsExtendedSearch_Transformer());
            store.ExecuteTransformer(new CampaignReducedModel_Transformer());
        }
    }
}
