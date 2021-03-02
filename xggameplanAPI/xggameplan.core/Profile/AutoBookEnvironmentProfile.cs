using xggameplan.AutoBooks.AWS;

namespace xggameplan.Profile
{
    internal class AutoBookEnvironmentProfile : AutoMapper.Profile
    {
        public AutoBookEnvironmentProfile()
        {
            CreateMap<AWSPAEnvironment, AutoBooks.Environment>();
        }
    }
 
}
