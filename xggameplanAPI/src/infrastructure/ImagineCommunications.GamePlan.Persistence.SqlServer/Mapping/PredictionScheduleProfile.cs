using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.RatingSchedules;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.PredictionSchedules;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class PredictionScheduleProfile : Profile
    {
        public PredictionScheduleProfile()
        {
            CreateMap<RatingsPredictionSchedule, PredictionSchedule>().ReverseMap();
            CreateMap<Rating, PredictionScheduleRating>().ReverseMap();
        }
    }
}
