using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces;
using ImagineCommunications.GamePlan.Process.Smooth.Models;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    /// <summary>
    /// Factory for providing Recommendation instances for Smooth
    /// </summary>
    internal class SmoothRecommendationsFactory
    {
        private readonly ISmoothConfiguration _smoothConfiguration;

        public SmoothRecommendationsFactory(ISmoothConfiguration smoothConfiguration) => _smoothConfiguration = smoothConfiguration;

        /// <summary>Creates the recommendation for placed spot.</summary>
        /// <param name="spot">The spot.</param>
        /// <param name="passSequence">The pass sequence.</param>
        /// <param name="passIterationSequence">The pass iteration sequence.</param>
        /// <param name="programme">The programme.</param>
        /// <param name="processorDateTime">The processor date time.</param>
        /// <param name="salesAreaName">Name of the sales area.</param>
        /// <param name="recommendationGroupCode">The recommendation group code.
        /// Usually the sales area's custom Id.
        /// </param>
        ///
        /// <returns></returns>
        private static Recommendation CreateRecommendationForPlacedSpot(
            Spot spot,
            int passSequence,
            int passIterationSequence,
            Programme programme,
            DateTime processorDateTime,
            string salesAreaName,
            string recommendationGroupCode)
        {
            var recommendation = new Recommendation()
            {
                Action = "A",
                ActualPositionInBreak = spot.ActualPositioninBreak,
                BreakBookingPosition = 0,
                BreakType = spot.BreakType,
                ClientPicked = spot.ClientPicked,
                Demographic = spot.Demographic,
                EndDateTime = spot.EndDateTime,
                ExternalBreakNo = spot.ExternalBreakNo,
                ExternalCampaignNumber = spot.ExternalCampaignNumber,
                ExternalSpotRef = spot.ExternalSpotRef,
                Filler = false,
                GroupCode = recommendationGroupCode,
                MultipartSpot = spot.MultipartSpot,
                MultipartSpotPosition = spot.MultipartSpotPosition,
                MultipartSpotRef = spot.MultipartSpotRef,
                Preemptable = spot.Preemptable,
                Preemptlevel = spot.Preemptlevel,
                Processor = "smooth",
                ProcessorDateTime = processorDateTime,
                Product = spot.Product,
                ExternalProgrammeReference = programme.ExternalReference,
                ProgrammeName = programme.ProgrammeName,
                RequestedPositionInBreak = spot.RequestedPositioninBreak,
                SalesArea = salesAreaName,
                Sponsored = spot.Sponsored,
                SpotEfficiency = 0,
                SpotLength = spot.SpotLength,
                SpotRating = 0,
                StartDateTime = spot.StartDateTime,
                PassSequence = passSequence,
                PassIterationSequence = passIterationSequence
            };

            return recommendation;
        }

        private Recommendation CreateRecommendationForUnplacedSpot(
            Spot spot,
            SalesArea salesArea,
            DateTime processorDateTime)
        {
            var recommendation = new Recommendation()
            {
                Action = "A",
                ActualPositionInBreak = null,
                BreakBookingPosition = 0,
                BreakType = spot.BreakType,
                ClientPicked = spot.ClientPicked,
                Demographic = spot.Demographic,
                EndDateTime = spot.EndDateTime,
                ExternalBreakNo = null,
                ExternalCampaignNumber = spot.ExternalCampaignNumber,
                ExternalSpotRef = spot.ExternalSpotRef,
                Filler = false,
                GroupCode = salesArea.CustomId.ToString(CultureInfo.InvariantCulture),
                MultipartSpot = spot.MultipartSpot,
                MultipartSpotPosition = spot.MultipartSpotPosition,
                MultipartSpotRef = spot.MultipartSpotRef,
                Preemptable = spot.Preemptable,
                Preemptlevel = spot.Preemptlevel,
                Processor = "smooth",
                ProcessorDateTime = processorDateTime,
                Product = spot.Product,
                ExternalProgrammeReference = null,
                ProgrammeName = null,
                RequestedPositionInBreak = spot.RequestedPositioninBreak,
                SalesArea = salesArea.Name,
                Sponsored = spot.Sponsored,
                SpotEfficiency = 0,
                SpotLength = spot.SpotLength,
                SpotRating = 0,
                StartDateTime = spot.StartDateTime
            };

            return recommendation;
        }

        public IReadOnlyCollection<Recommendation> CreateRecommendationsForUnplacedSpots(
            IReadOnlyCollection<Spot> spots,
            SalesArea salesArea,
            DateTime processorDateTime)
        {
            var recommendations = new List<Recommendation>();

            foreach (var spot in spots.Where(s => !s.IsBooked()))
            {
                if (spot.ExternalCampaignNumber is null)
                {
                    continue;
                }

                bool isExcludedCampaign = _smoothConfiguration.ExternalCampaignRefsToExclude.Contains(spot.ExternalCampaignNumber);

                if (!isExcludedCampaign || isExcludedCampaign && _smoothConfiguration.RecommendationsForExcludedCampaigns)
                {
                    recommendations.Add(
                        CreateRecommendationForUnplacedSpot(spot, salesArea, processorDateTime)
                        );
                }
            }

            return recommendations;
        }

        public IReadOnlyCollection<Recommendation> CreateRecommendationsForPlacedSpots(
            IReadOnlyCollection<SmoothBreak> smoothBreaks,
            Programme prog,
            SalesArea salesArea,
            DateTime processorDateTime
            )
        {
            var recommendations = new List<Recommendation>();

            foreach (SmoothBreak smoothBreak in smoothBreaks)
            {
                foreach (var spot in smoothBreak.SmoothSpots.Where(s => s.IsCurrent))
                {
                    // Only spots placed in this run
                    recommendations.Add(
                        CreateRecommendationForPlacedSpot(
                            spot.Spot,
                            spot.SmoothPassSequence,
                            spot.SmoothPassIterationSequence,
                            prog,
                            processorDateTime,
                            salesArea.Name,
                            salesArea.CustomId.ToString(CultureInfo.InvariantCulture))
                        );
                }
            }

            return recommendations;
        }
    }
}
