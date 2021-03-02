using xggameplan.core.FeatureManagement.Interfaces;
using xggameplan.model.External;

namespace xggameplan.Profile
{
    public class FeatureProfile : AutoMapper.Profile
    {
        public FeatureProfile()
        {
            CreateMap<IFeatureFlag, FeatureStateModel>()
                .ForMember(dest => dest.Enabled, opt => opt.MapFrom(src => src.IsEnabled));
        }
    }
}
