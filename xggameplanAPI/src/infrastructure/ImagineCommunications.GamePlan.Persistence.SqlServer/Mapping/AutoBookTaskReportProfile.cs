using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookTaskReports;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    internal class AutoBookTaskReportProfile : Profile
    {
        public AutoBookTaskReportProfile()
        {
            CreateMap<AutoBookTaskReport, Domain.AutoBookApi.Storage.Objects.AutoBookTaskReport>().ReverseMap();
        }
    }
}
