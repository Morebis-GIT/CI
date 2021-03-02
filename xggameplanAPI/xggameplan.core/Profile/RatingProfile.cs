using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.RatingSchedules;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using xggameplan.CSVImporter;
using xggameplan.Model;
using xggameplan.OutputFiles.Processing;

namespace xggameplan.Profile
{
    internal class RatingProfile : AutoMapper.Profile
    {
        public RatingProfile()
        {
            CreateMap<RatingModel, Rating>().ReverseMap();
            CreateMap<RatingsPredictionScheduleModel, RatingsPredictionSchedule>().ReverseMap();
            CreateMap<CreateRatingsPredictionSchedule, RatingsPredictionSchedule>().ReverseMap();

            CreateMap<Tuple<List<Rating>, SalesArea, IReadOnlyCollection<Demographic>>, AgPredictions>()
                .ConstructUsing(src => LoadDefaultAgPrediction(src));

            CreateMap<BaseRatingsImport, BaseRatings>()
                .ForMember(dst => dst.BaseDemoAvailableRatings, opt => opt.MapFrom(src => src.base_rtgs_post))
                .ForMember(dst => dst.TotalOpenAvailability, opt => opt.MapFrom(src => src.total_open_avail_post));

            CreateMap<ReserveRatingsImport, ReserveRatings>()
                .ForMember(dst => dst.DemographicNumber, opt => opt.MapFrom(src => src.demo_no))
                .ForMember(dst => dst.AvailableRatings, opt => opt.MapFrom(src => src.rtgs_post))
                .ForMember(dst => dst.ReservedRatings, opt => opt.MapFrom(src => src.rtgs_resv));
        }

        private static AgPredictions LoadDefaultAgPrediction(Tuple<List<Rating>, SalesArea, IReadOnlyCollection<Demographic>> src)
        {
            var (ratings, salesArea, demographics) = src;
            var salesAreaCustomId = salesArea.CustomId;
            var salesAreaBaseDemographic1 = salesArea.BaseDemographic1;

            var agPredictions = new AgPredictions();

            if (ratings == null || ratings.Count == 0)
            {
                agPredictions.Add(
                    new AgPrediction
                    {
                        SalesAreaNo = salesAreaCustomId,
                        DemographicNo = DemographicNumber(salesAreaBaseDemographic1),
                        NoOfRtgs = default
                    });
            }
            else
            {
                agPredictions.AddRange(
                    ratings.Select(rating => new AgPrediction
                    {
                        SalesAreaNo = salesAreaCustomId,
                        DemographicNo = DemographicNumber(rating.Demographic),
                        NoOfRtgs = rating.NoOfRatings
                    })
                .ToList());
            }

            return agPredictions;

            int DemographicNumber(string demographicName)
            {
                if (String.IsNullOrWhiteSpace(demographicName))
                {
                    return 0;
                }

                return demographics.FirstOrDefault(d =>
                    d.ExternalRef.Equals(demographicName, StringComparison.InvariantCultureIgnoreCase))?
                .Id ?? 0;
            }
        }
    }
}
