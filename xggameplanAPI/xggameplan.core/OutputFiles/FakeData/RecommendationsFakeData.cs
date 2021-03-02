using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Spots;

namespace xggameplan.OutputFiles.FakeData
{
    /// <summary>
    /// Creates recommendation fake data
    /// </summary>
    public class RecommendationsFakeData
    {
        private ISpotRepository _spotRepository;
        private ISalesAreaRepository _salesAreaRepository;
        private IProgrammeRepository _programmeRepository;
        private Random _random;

        public RecommendationsFakeData(ISpotRepository spotRepository, ISalesAreaRepository salesAreaRepository, IProgrammeRepository programmeRepository)
        {
            _spotRepository = spotRepository;
            _salesAreaRepository = salesAreaRepository;
            _programmeRepository = programmeRepository;
        }

        /// <summary>
        /// Creates recommendations data
        /// </summary>
        /// <param name="numberOfSpots">Number of spots to generate data for</param>
        /// <returns></returns>
        public List<Recommendation> Create(int numberOfSpots)
        {
            var recommendations = new List<Recommendation>();
            _random = new Random();
            DateTime processorDateTime = DateTime.UtcNow;

            // Load data
            var spots = _spotRepository.GetAll();   // Doesn't actually return all
            var salesAreas = _salesAreaRepository.GetAll();
            var programmes = _programmeRepository.GetAll();     // Doesn't actually return all
            numberOfSpots = (numberOfSpots > spots.ToList().Count ? spots.ToList().Count : numberOfSpots);     // If insufficient spots then return max number

            if (!spots.Any())
            {
                throw new ArgumentException("Cannot generate fake recommendations data because there are no spots in the database");
            }
            if (!programmes.Any())
            {
                throw new ArgumentException("Cannot generate fake recommendations data because there are no programmes in the database");
            }

            // Add data for each spot
            int programmeIndex = -1;
            for (int spotIndex = 0; spotIndex < numberOfSpots; spotIndex++)
            {
                programmeIndex++;
                programmeIndex = (programmeIndex >= programmes.ToList().Count - 1 ? 0 : programmeIndex);
                var spot = spots.ToList()[spotIndex];
                var salesArea = salesAreas.Where(x => x.Name.Equals(spot.SalesArea)).First();
                var programme = programmes.ToList()[++programmeIndex];

                recommendations.Add(CreateRecommendation(spots.ToList()[spotIndex], salesArea, programme, processorDateTime));                
            }
            return recommendations;
        }
        
        /// <summary>
        /// Creates a recommendation primarily using spot data and remaining fields are random
        /// </summary>
        /// <param name="spot"></param>
        /// <param name="salesArea"></param>
        /// <param name="programme"></param>
        /// <param name="processorDateTime"></param>        
        /// <returns></returns>
        private Recommendation CreateRecommendation(Spot spot, SalesArea salesArea, Programme programme, DateTime processorDateTime)
        {
            // Calculate random percentage for working out random properties based on percentage likelihood
            int percentage = _random.Next(0, 100);            
            
            // Set random properties, define percentage likelihood of picking
            string[] multipartSpots = new string[] { "N", "T", "M" };
            int[] multipartSpotsPercentages = new int[] { 75, 20, 5 };      
            string[] multipartSpotPositions = new string[] { "-1", "1", "2", "3", "97", "98", "99" };
            int[] multipartSpotPositionPercentages = new int[] { 55, 10, 5, 5, 5, 5, 15 };      

            Recommendation recommendation = new Recommendation()
            {
                Action = "A",
                ActualPositionInBreak = "",
                BreakBookingPosition = -1,
                BreakType = "CO",
                ClientPicked = spot.ClientPicked,
                Demographic = spot.Demographic,
                EndDateTime = spot.EndDateTime,
                ExternalBreakNo = spot.ExternalBreakNo,
                ExternalCampaignNumber = spot.ExternalCampaignNumber,
                ExternalSpotRef = spot.ExternalSpotRef,
                Filler = false,
                GroupCode = salesArea.CustomId.ToString(),
                MultipartSpot = Random(multipartSpots, multipartSpotsPercentages, percentage),  // NTM
                MultipartSpotPosition = Random(multipartSpotPositions, multipartSpotPositionPercentages, percentage),
                MultipartSpotRef = "",
                Preemptable = spot.Preemptable,
                Preemptlevel = spot.Preemptlevel,
                Processor = "autobook",
                ProcessorDateTime = processorDateTime,
                Product = spot.Product,
                ExternalProgrammeReference = programme.ExternalReference,
                ProgrammeName = programme.ProgrammeName,
                RequestedPositionInBreak = "",
               SalesArea = salesArea.Name,
                Sponsored = spot.Sponsored,
                SpotEfficiency = Convert.ToDouble(string.Format("{0}.{1}", _random.Next(0, 400), _random.Next(1, 9))),      // 1 decimal place
                SpotLength = spot.SpotLength,
                SpotRating = Convert.ToDecimal("0." +  _random.Next(0, 999999).ToString()),     // 6 decimal places
                StartDateTime = spot.StartDateTime,
            };

            if (percentage <= 5 || recommendation.SpotRating == 0)
            {
                recommendation.SpotRating = 0;
                recommendation.SpotEfficiency = 0;
            }                        
            return recommendation;
        }

        private T Random<T>(List<T> list)
        {
            return list[_random.Next(0, list.Count - 1)];
        }

        private T Random<T>(T[] array)
        {
            return (T)array[_random.Next(0, array.Length - 1)];
        }

        /// <summary>
        /// Calculate random value based on percentages
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="percentages"></param>
        /// <param name="percentage"></param>
        /// <returns></returns>
        private T Random<T>(T[] array, int[] percentages, int percentage)
        {
            int totalPercentage = 0;
            for (int index = 0; index < percentages.Length; index++)
            {
                totalPercentage += percentages[index];
                if (percentage <= totalPercentage)
                {
                    return array[index];
                }
            }
            throw new ArgumentException("Invalid parameters");            
        }
    }
}
