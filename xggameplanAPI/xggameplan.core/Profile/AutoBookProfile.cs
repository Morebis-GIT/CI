using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage.Objects;
using xggameplan.Model;

namespace xggameplan.Profile
{
    internal class AutoBookProfile : AutoMapper.Profile
    {
        public AutoBookProfile()
        {
            CreateMap<AutoBook, AutoBookModel>()
                .ForMember(dest => dest.Id, dest => dest.MapFrom(ca => ca.Id.Replace("AutoBooks/", "")));
            CreateMap<AutoBookTask, AutoBookTaskModel>();
        }
    }
 
} 
