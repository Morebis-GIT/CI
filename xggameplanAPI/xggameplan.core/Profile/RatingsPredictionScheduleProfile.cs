using ImagineCommunications.GamePlan.Domain.RatingSchedules;
using xggameplan.Model;

namespace xggameplan.Profile
{

    internal class RatingsPredictionScheduleProfile : AutoMapper.Profile
    {
        public RatingsPredictionScheduleProfile()
        {
            CreateMap<CreateRatingsPredictionSchedule, RatingsPredictionSchedule>();
            CreateMap<RatingsPredictionSchedule, RatingsPredictionScheduleModel>();
        }
    }
}
