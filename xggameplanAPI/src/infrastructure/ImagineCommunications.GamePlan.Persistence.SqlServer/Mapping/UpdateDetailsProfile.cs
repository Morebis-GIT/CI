using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Maintenance.UpdateDetail;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class UpdateDetailsProfile : Profile
    {
        public UpdateDetailsProfile() => CreateMap<UpdateDetails, Entities.Master.UpdateDetails>().ReverseMap();
    }
}
