using FluentValidation;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using xggameplan.Model;

namespace xggameplan.Validations
{
    public class CreateSponsorshipItemModelValidation : SponsorshipItemModelBaseValidation<CreateSponsorshipItemModel>
    {
        public CreateSponsorshipItemModelValidation(ISalesAreaRepository salesAreaRepository,
                                                    IProgrammeRepository programmeRepository,
                                                    IValidator<SponsoredDayPartModelBase> sponsoredDayPartModelValidation,
                                                    SponsorshipModelBase sponsorshipModelBase = null)
                                                    : base(salesAreaRepository,
                                                           programmeRepository,
                                                           sponsoredDayPartModelValidation,
                                                           sponsorshipModelBase)
        {
        }
    }
}
