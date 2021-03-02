using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class TotalRatingProfile : Profile
    {
        public TotalRatingProfile()
        {
            CreateMap<TotalRating, Domain.TotalRatings.TotalRating>().ReverseMap();
        }
    }
}
