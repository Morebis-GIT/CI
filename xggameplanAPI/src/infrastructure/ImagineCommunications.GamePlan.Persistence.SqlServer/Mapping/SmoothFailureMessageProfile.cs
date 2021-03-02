using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.SmoothFailureMessages;
using SmoothFailureMessageEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.SmoothFailureMessage;
using SmoothFailureMessageDescriptionEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.SmoothFailureMessageDescription;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class SmoothFailureMessageProfile : Profile
    {
        public SmoothFailureMessageProfile()
        {
            CreateMap<SmoothFailureMessage, SmoothFailureMessageEntity>()
                .ForMember(d => d.Descriptions,
                    opt => opt.MapFrom(s => s.Description.Select(x => new SmoothFailureMessageDescriptionEntity
                        {LanguageAbbreviation = x.Key, Description = x.Value})));

            CreateMap<SmoothFailureMessageEntity, SmoothFailureMessage>()
                .ForMember(d => d.Description,
                    opt => opt.MapFrom(
                        s => s.Descriptions.ToDictionary(k => k.LanguageAbbreviation, v => v.Description)));
        }
    }
}
