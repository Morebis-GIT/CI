using System;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using xggameplan.Model;

namespace xggameplan.core.Profile
{
    public class PassSalesAreaPriorityProfile : AutoMapper.Profile
    {
        public PassSalesAreaPriorityProfile()
        {
            DateTime? nullableDateTime = null;

            CreateMap<PassSalesAreaPriority, PassSalesAreaPriorityModel>()
                .ForMember(d => d.StartDate, o => o.MapFrom(s => s.StartDate.HasValue ? s.StartDate.Value.Date : nullableDateTime))
                .ForMember(d => d.EndDate, o => o.MapFrom(s => s.EndDate.HasValue ? s.EndDate.Value.Date : nullableDateTime))
                .ReverseMap()            
                .ForMember(d => d.StartDate, o => o.MapFrom(s => s.StartDate.HasValue ? s.StartDate.Value.Date : nullableDateTime))
                .ForMember(d => d.EndDate, o => o.MapFrom(s => s.EndDate.HasValue ? s.EndDate.Value.Date : nullableDateTime));
        }
    }
}
