using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Enums;
using xggameplan.Model;

namespace xggameplan.Validations
{
    public class SponsorshipModelBaseValidation<T> : AbstractValidator<T>
    where T : SponsorshipModelBase
    {
        public SponsorshipModelBaseValidation()
        {
            RuleFor(model => model.ExternalReferenceId)
                                  .NotEmpty()
                                  .WithMessage("ExternalReferenceId is required");

            RuleFor(model => model.RestrictionLevel)
                        .Must(ContainValidRestrictionLevel)
                        .WithMessage(model => CreateInvalidRestrictionLevelErrorMessage(model.RestrictionLevel));
        }

        private bool ContainValidRestrictionLevel(SponsorshipRestrictionLevel restrictionLevel)
        {
            return Enum.IsDefined(typeof(SponsorshipRestrictionLevel), restrictionLevel);
        }

        private string CreateInvalidRestrictionLevelErrorMessage(SponsorshipRestrictionLevel restrictionLevel)
        {
            return $"RestrictionLevel: {restrictionLevel} is invalid. Allowed values: {String.Join(",", Enum.GetNames(typeof(SponsorshipRestrictionLevel)))}";
        }

        protected bool ContainSponsoredItems(IEnumerable<CreateSponsoredItemModel> sponsoredItems)
        {
            return sponsoredItems?.Any() == true && !sponsoredItems.Any(a => a == null);
        }

        protected bool ContainsSponsoredItems(CreateSponsorshipModel model)
        {
            return ContainSponsoredItems(model.SponsoredItems);
        }
    }
}
