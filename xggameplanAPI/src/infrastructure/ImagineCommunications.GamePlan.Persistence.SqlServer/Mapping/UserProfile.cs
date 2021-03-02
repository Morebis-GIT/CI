using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Shared.System.Users;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile() {
            CreateMap<User, Entities.Master.User>().
                ForMember(x => x.Password, opt =>
                {
                    opt.PreCondition(src => src.Password != null);
                    opt.MapFrom(src => src.Password.HashedValue);
                })
                .ForMember(x => x.UserSettings, opt =>
                 {
                     opt.PreCondition(src => src.UserSettings != null);
                     opt.MapFrom(src =>
                         src.UserSettings.Select(x =>
                         new Entities.Master.UserSetting() { Name = x.Key, Value = x.Value })
                         .ToList());
                 })
               
                .ReverseMap()
                .ForMember(x => x.Password, opt =>
                 {
                     opt.PreCondition(src => src.Password != null);
                     opt.MapFrom(src => User.UserPassword.CreateFromHash(src.Password));
                 })
                .ForMember(x => x.UserSettings, opt =>
                 {
                     opt.PreCondition(src => src.UserSettings != null);
                     opt.MapFrom(src => src.UserSettings.ToDictionary(k => k.Name, v => v.Value));
                 });
        }
    }
}
