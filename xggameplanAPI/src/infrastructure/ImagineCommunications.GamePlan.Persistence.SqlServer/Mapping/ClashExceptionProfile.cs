using AutoMapper;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ClashExceptions;
using ClashExceptionDomain = ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Objects.ClashException;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class ClashExceptionProfile : Profile
    {
        public ClashExceptionProfile()
        {
            CreateMap<TimeAndDow, ClashExceptionsTimeAndDow>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ClashExceptionId, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<ClashExceptionDomain, ClashException>()
                .ForMember(dest => dest.ClashExceptionsTimeAndDows, opt => opt.MapFrom(s => s.TimeAndDows))
                .ForMember(dest => dest.IncludeOrExclude, opt => opt.MapFrom(s => (Entities.IncludeOrExclude)(int)s.IncludeOrExclude))
                .ForMember(dest => dest.FromType, opt => opt.MapFrom(s => (Entities.ClashExceptionType)(int)s.FromType))
                .ForMember(dest => dest.ToType, opt => opt.MapFrom(s => (Entities.ClashExceptionType)(int)s.ToType))
                .ReverseMap()
                .ForMember(dest => dest.IncludeOrExclude, opt => opt.MapFrom(s => (IncludeOrExclude)(int)s.IncludeOrExclude))
                .ForMember(dest => dest.FromType,
                    opt => opt.MapFrom(s => (ClashExceptionType)s.FromType))
                .ForMember(dest => dest.ToType,
                    opt => opt.MapFrom(s => (ClashExceptionType)s.ToType))
                .ForMember(dest => dest.TimeAndDows, opt => opt.MapFrom(s => s.ClashExceptionsTimeAndDows));
        }
    }
}
