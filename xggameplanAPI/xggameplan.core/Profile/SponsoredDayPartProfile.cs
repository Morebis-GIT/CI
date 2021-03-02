using ImagineCommunications.GamePlan.Domain.Sponsorships.Objects;
using xggameplan.core.Extensions;
using xggameplan.model.AutoGen.AgSponsorships;
using xggameplan.Model;

namespace xggameplan.core.Profile
{
    internal class SponsoredDayPartProfile : AutoMapper.Profile
    {
        public SponsoredDayPartProfile()
        {
            CreateMap<SponsoredDayPart, AgSponsorshipDayPart>()
                .ForMember(d => d.StartTime, m => m.MapFrom(src => AgConversions.ToAgTimeAsHHMMSS(src.StartTime)))
                .ForMember(d => d.EndTime, m => m.MapFrom(src => AgConversions.ToAgTimeAsHHMMSS(src.EndTime)))
                .ForMember(d => d.Days, m => m.MapFrom(src => AgConversions.ToAgDaysAsInt(src.DaysOfWeek)));

            CreateMap<CreateSponsoredDayPartModel, SponsoredDayPart>()
                .ForMember(d => d.EndTime, m => m.PreCondition(src => src.EndTime.HasValue));
        }
    }
}
