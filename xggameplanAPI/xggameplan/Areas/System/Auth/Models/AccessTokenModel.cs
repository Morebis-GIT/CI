using System;
using ImagineCommunications.GamePlan.Domain.Shared.System.AccessTokens;

namespace xggameplan.Areas.System.Auth.Models
{
    /// <summary>
    /// Representation of the access token.
    /// </summary>
    public class AccessTokenModel
    {
        /// <summary>
        /// The token itself.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Date and time in UTC when this access token will expire and client will need to obtain a new one.
        /// </summary>
        public DateTime ValidUntil { get; set; }
    }

    //internal class AccessTokenModelProfile : Profile
    public class AccessTokenModelProfile : AutoMapper.Profile
    {

        public AccessTokenModelProfile() =>
            CreateMap<AccessToken, AccessTokenModel>()
            .ForMember(dest => dest.ValidUntil, opt =>
            {
                opt.MapFrom(src =>
                    src.ValidUntilValue.UtcDateTime);
            });
    }
}
