using xggameplan.CSVImporter;
using xggameplan.OutputFiles.Processing;

namespace xggameplan.Profile
{
    internal class ConversionEfficiencyProfile : AutoMapper.Profile
    {
        public ConversionEfficiencyProfile()
        {
            CreateMap<ConversionEfficiencyImport, ConversionEfficiency>()
                .ForMember(dst => dst.DemographicNumber, opt => opt.MapFrom(src => src.demo_no))
                .ForMember(dst => dst.ConversionEfficiencyIndex, opt => opt.MapFrom(src => src.conv_eff_index));
        }
    }
}
