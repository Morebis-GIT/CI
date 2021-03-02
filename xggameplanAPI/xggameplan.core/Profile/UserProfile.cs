using ImagineCommunications.GamePlan.Domain.Shared.System.Users;
using xggameplan.Model;

namespace xggameplan.Profile
{
    public class UserProfile : AutoMapper.Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserModel>();
            CreateMap<User, UserReducedModel>();
        }
    }
}
