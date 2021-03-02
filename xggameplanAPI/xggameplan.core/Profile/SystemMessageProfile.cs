using xggameplan.Model;

namespace xggameplan.Profile
{
    internal class SystemMessageProfile : AutoMapper.Profile
    {
        public SystemMessageProfile()
        {
            CreateMap<SystemMessage, SystemMessageModel>();
        }
    }
}
