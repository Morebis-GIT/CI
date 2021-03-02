using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage.Objects;
using xggameplan.model.External;

namespace xggameplan.core.Profile
{
    public class AutoBookTaskReportProfile : AutoMapper.Profile
    {
        public AutoBookTaskReportProfile()
        {
            CreateMap<AutoBookTaskReportModel, AutoBookTaskReport>().ReverseMap();
        }
    }
}
