using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class ClearanceCodeProfile : Profile
    {
        public ClearanceCodeProfile()
        {
            CreateMap<ClearanceCode, Domain.Shared.ClearanceCodes.ClearanceCode>().ReverseMap();
        }
    }
}
