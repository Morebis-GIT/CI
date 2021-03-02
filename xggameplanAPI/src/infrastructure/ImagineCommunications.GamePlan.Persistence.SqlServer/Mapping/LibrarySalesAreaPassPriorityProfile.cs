using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class LibrarySalesAreaPassPriorityProfile : Profile
    {
        public LibrarySalesAreaPassPriorityProfile()
        {
            CreateMap<LibrarySalesAreaPassPriority,
                Domain.LibrarySalesAreaPassPriorities.LibrarySalesAreaPassPriority>()
                .ForMember(x => x.DaysOfWeek, opt => opt.MapFrom(e => e.DowPattern))
                .ReverseMap()
                .ForMember(e => e.DowPattern, opt => opt.MapFrom(x => x.DaysOfWeek));
        }
    }
}
