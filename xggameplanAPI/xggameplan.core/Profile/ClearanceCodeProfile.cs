using ImagineCommunications.GamePlan.Domain.Shared.ClearanceCodes;
using xggameplan.Model;

namespace xggameplan.Profile
{
    internal class ClearanceCodeProfile : AutoMapper.Profile
    {
        public ClearanceCodeProfile()
        {
            CreateMap<CreateClearanceCode, ClearanceCode>();
            CreateMap<ClearanceCode, ClearanceCodeModel>();
        }
    }
}
