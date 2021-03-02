using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentAssertions;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers.Pivot;
using ImagineCommunications.GamePlan.Process.Smooth.Types;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers
{
    public static class RecommendationExtensions
    {
        public static void ShouldBeUnplaced(this Recommendation recommendation)
        {
            _ = recommendation.ExternalBreakNo.Should()
                    .BeNull(
                        $"spot {recommendation.ExternalSpotRef} should be unplaced",
                        becauseArgs: null);

            _ = recommendation.ActualPositionInBreak.Should()
                    .BeNull(becauseArgs: null);
        }

        public static void ShouldBeUnplaced(
            this IReadOnlyCollection<Recommendation> recommendations)
        {
            foreach (var item in recommendations)
            {
                _ = item.ExternalBreakNo.Should()
                        .BeNull(
                            $"spot {item.ExternalSpotRef} should be unplaced",
                            becauseArgs: null);

                _ = item.ActualPositionInBreak.Should()
                        .BeNull(becauseArgs: null);
            }
        }

        public static Recommendation ShouldBePlaced(this Recommendation recommendation)
        {
            _ = recommendation.ExternalBreakNo.Should()
                    .NotBeNullOrEmpty(becauseArgs: null);

            _ = recommendation.ActualPositionInBreak.Should()
                    .NotBeNullOrEmpty(becauseArgs: null);

            return recommendation;
        }

        public static Recommendation InBreak(
            this Recommendation recommendation,
            BreakExternalReference value
            )
        {
            _ = recommendation.ExternalBreakNo.Should()
                    .Be(value.ToString(), becauseArgs: null);

            return recommendation;
        }

        public static Recommendation ButNotBeInBreak(
            this Recommendation recommendation,
            BreakExternalReference value
            )
        {
            _ = recommendation.ExternalBreakNo.Should()
                    .NotBe(value.ToString(), becauseArgs: null);

            return recommendation;
        }

        public static Recommendation AtActualPositionInBreak(
            this Recommendation recommendation,
            int actualPositionInBreak
            ) => AtActualPositionInBreak(
                    recommendation,
                    actualPositionInBreak.ToString()
                );

        public static Recommendation AtActualPositionInBreak(
            this Recommendation recommendation,
            string actualPositionInBreak
            )
        {
            _ = recommendation.ActualPositionInBreak.Should()
                    .Be(actualPositionInBreak, becauseArgs: null);

            return recommendation;
        }

        public static Recommendation AndShouldBeSponsored(
            this Recommendation recommendation)
        {
            _ = recommendation.Sponsored.Should().BeTrue(becauseArgs: null);

            return recommendation;
        }

        public static Recommendation WhenRequestedPositionInBreakWas(
            this Recommendation recommendation,
            string requestedPositionInBreak
            )
        {
            _ = recommendation.RequestedPositionInBreak.Should()
                    .Be(requestedPositionInBreak, becauseArgs: null);

            return recommendation;
        }

        [SuppressMessage("Performance", "HAA0101:Array allocation for params parameter")]
        public static Recommendation Spot(
            this IReadOnlyCollection<Recommendation> recommendations,
            SpotExternalReference value)
        {
            var results = recommendations.FirstOrDefault(r => r.ExternalSpotRef == value);

            _ = results.Should()
                    .NotBeNull(
                        because: "the spot {0} should be in the test data",
                        becauseArgs: value.ToString());

            return results;
        }

        public static IReadOnlyCollection<Recommendation> Spots(
            this IReadOnlyCollection<Recommendation> recommendations,
            params SpotExternalReference[] values)
        {
            var results = recommendations.Where(r => values.Contains(r.ExternalSpotRef));

            _ = results.Should()
                    .HaveSameCount(
                        values,
                        "one or more spots should be in the recommendations",
                        becauseArgs: null);

            return results.ToList();
        }

        public static PivotRecommendations Pivot(
            this IReadOnlyCollection<Recommendation> recommendations,
            IRepositoryFactory repositoryFactory)
        {
            using var scope = repositoryFactory.BeginRepositoryScope();
            var breakRepository = scope.CreateRepository<IBreakRepository>();
            var programmeRepository = scope.CreateRepository<IProgrammeRepository>();

            IReadOnlyDictionary<string, Break> allBreaks = breakRepository
                .GetAll()
                .ToDictionary(b => b.ExternalBreakRef);

            IReadOnlyDictionary<string, Programme> allProgrammes = programmeRepository
                .GetAll()
                .ToDictionary(p => p.ExternalReference);

            return new PivotRecommendations(
                recommendations.OrderBy(r => r.ExternalBreakNo).ToList(),
                allBreaks,
                allProgrammes);
        }
    }
}
