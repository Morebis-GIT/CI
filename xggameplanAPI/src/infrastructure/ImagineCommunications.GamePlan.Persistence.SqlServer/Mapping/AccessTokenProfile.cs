using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Shared.System.AccessTokens;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class AccessTokenProfile : Profile
    {
        public AccessTokenProfile()
        {
            CreateMap<AccessToken, Entities.Master.AccessToken>()
                .ForMember(m => m.ValidUntilValue,
                    opt => opt.MapFrom(src => src.ValidUntilValue.UtcDateTime))
                .ReverseMap()
                .ForMember(m => m.ValidUntilValue,
                    opt => opt.MapFrom(src => new System.DateTimeOffset(src.ValidUntilValue)));
        }
    }
}
