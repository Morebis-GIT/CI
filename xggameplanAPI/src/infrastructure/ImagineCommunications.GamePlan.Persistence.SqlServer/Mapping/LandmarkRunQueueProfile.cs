using AutoMapper;
using ImagineCommunications.GamePlan.Domain.LandmarkRunQueues.Objects;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class LandmarkRunQueueProfile : Profile
    {
        public LandmarkRunQueueProfile()
        {
            CreateMap<LandmarkRunQueue, Entities.Tenant.Runs.LandmarkRunQueue>().ReverseMap();
        }
    }
}
