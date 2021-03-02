using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Shared.System.Preview;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class PreviewFileProfile : Profile
    {
        public PreviewFileProfile() {
            CreateMap<PreviewFile, Entities.Master.PreviewFile>()
                .ForMember(m => m.Location, opt => { opt.MapFrom(src => src.Id); })
                .ForMember(m => m.Id, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(m => m.Id, opt => { opt.MapFrom(src => src.Location); });
        }
    }
}
