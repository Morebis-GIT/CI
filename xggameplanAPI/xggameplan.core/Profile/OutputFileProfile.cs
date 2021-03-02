using ImagineCommunications.GamePlan.Domain.OutputFiles.Objects;
using xggameplan.Model;

namespace xggameplan.Profile
{
    internal class OutputFileProfile : AutoMapper.Profile
    {
        public OutputFileProfile()
        {
            CreateMap<OutputFile, OutputFileModel>();
            CreateMap<OutputFileColumn, OutputFileColumnModel>();
        }
    }
}
