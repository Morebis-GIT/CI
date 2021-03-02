using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using xggameplan.Model;

namespace xggameplan.Profile
{
    internal class ScheduleProfile : AutoMapper.Profile
    {
        public ScheduleProfile()
        {
            CreateMap<Schedule, ScheduleModel>();            
        }
    }
}
