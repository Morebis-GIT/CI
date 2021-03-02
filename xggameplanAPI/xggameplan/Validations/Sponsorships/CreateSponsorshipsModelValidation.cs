using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Sponsorships;
using xggameplan.Model;

namespace xggameplan.Validations
{
    public class CreateSponsorshipsModelValidation : AbstractValidator<IEnumerable<CreateSponsorshipModel>>
    {
        public CreateSponsorshipsModelValidation(ISponsorshipRepository sponsorshipRepository,
                                                 ISalesAreaRepository salesAreaRepository,
                                                 IProgrammeRepository programmeRepository,
                                                 IProductRepository productRepository,
                                                 IClashRepository clashRepository,
                                                 IValidator<SponsoredDayPartModelBase> sponsoredDayPartModelValidation)
        {

            RuleFor(model => model).NotNull()
                                   .WithMessage("A valid createSponsorshipModels is required");

            When(model => model != null, () =>
            {
                RuleFor(model => model).Must(ContainValidCreateSponsorshipModelItems)
               .WithMessage("createSponsorshipModels is required and should contain a valid CreateSponsorshipModel for each item");

                When(ContainsValidCreateSponsorshipModelItems, () =>
                {
                    RuleFor(model => model)
                                    .Must(ContainUniqueExternalReferenceIds)                                    
                                    .WithMessage("createSponsorshipModels should contain unique ExternalReferenceId for each item")
                                    .DependentRules((model) =>
                                    {
                                        model.RuleFor(createSponsorshipModels => createSponsorshipModels)
                                             .SetCollectionValidator(
                                             new CreateSponsorshipModelValidation(sponsorshipRepository, salesAreaRepository,
                                                                                  programmeRepository, productRepository,
                                                                                  clashRepository, sponsoredDayPartModelValidation));
                                    });
                });
            });
        }

        private bool ContainValidCreateSponsorshipModelItems(IEnumerable<CreateSponsorshipModel> model)
        {
            return model?.Any() == true && !model.Any(a => a == null);
        }

        private bool ContainsValidCreateSponsorshipModelItems(IEnumerable<CreateSponsorshipModel> model)
        {
            return ContainValidCreateSponsorshipModelItems(model);
        }

        private bool ContainUniqueExternalReferenceIds(IEnumerable<CreateSponsorshipModel> model)
        {
            return model.Where(a => a != null && !String.IsNullOrWhiteSpace(a.ExternalReferenceId))
                        .Select(a => a.ExternalReferenceId.Trim()).Distinct().Count() == model.Count();
        }
    }
}
