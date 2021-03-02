using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using xggameplan.Model;

namespace xggameplan.Profile
{
    internal class ToleranceProfile : AutoMapper.Profile
    {
        public ToleranceProfile()
        {
            CreateMap<Tolerance, ToleranceModel>().ForMember(toleranceModel => toleranceModel.ForceUnderOver, expression => expression.MapFrom(tolerance => tolerance.ForceOverUnder));
            CreateMap<ToleranceModel, Tolerance>().ForMember(toleranceModel => toleranceModel.ForceOverUnder, expression => expression.MapFrom(tolerance => tolerance.ForceUnderOver));
        }
    }
}
