using ImagineCommunications.GamePlan.Domain.SmoothFailureMessages;
using xggameplan.Model;

namespace xggameplan.Profile
{
    public class SmoothFailureReasonProfile : AutoMapper.Profile
    {
        public SmoothFailureReasonProfile()
        {            
            CreateMap<SmoothFailureMessage, SmoothFailureMessageModel>();
            CreateMap<SmoothFailureMessageModel, SmoothFailureMessage>();
        }
    }
}
