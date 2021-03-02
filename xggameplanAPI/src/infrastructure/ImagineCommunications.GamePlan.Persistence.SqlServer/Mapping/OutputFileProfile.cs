using AutoMapper;
using ImagineCommunications.GamePlan.Domain.OutputFiles.Objects;
using OutputFileEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.OutputFiles.OutputFile;
using OutputFileColumnEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.OutputFiles.OutputFileColumn;


namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class OutputFileProfile : Profile
    {
        public OutputFileProfile()
        {
            CreateMap<OutputFile, OutputFileEntity>().ReverseMap();
            CreateMap<OutputFileColumn, OutputFileColumnEntity>().ReverseMap();
        }
    }
}
