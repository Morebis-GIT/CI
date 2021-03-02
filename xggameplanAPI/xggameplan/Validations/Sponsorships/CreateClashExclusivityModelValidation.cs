using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using xggameplan.Model;

namespace xggameplan.Validations
{
    public class CreateClashExclusivityModelValidation : ClashExclusivityModelBaseValidation<CreateClashExclusivityModel>
    {
        public CreateClashExclusivityModelValidation(IClashRepository clashRepository,
                                                     SponsoredItemModelBase sponsoredItemModelBase = null)
                                                   : base(clashRepository, sponsoredItemModelBase)
        {
        }
    }
}
