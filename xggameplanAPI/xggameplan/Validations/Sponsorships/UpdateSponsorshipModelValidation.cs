using System;
using FluentValidation;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Sponsorships;
using xggameplan.Model;

namespace xggameplan.Validations
{
    public class UpdateSponsorshipModelValidation : SponsorshipModelBaseValidation<UpdateSponsorshipModel>
    {
        private readonly ISponsorshipRepository _sponsorshipRepository;

        public UpdateSponsorshipModelValidation(ISponsorshipRepository sponsorshipRepository,
                                                ISalesAreaRepository salesAreaRepository,
                                                IProgrammeRepository programmeRepository,
                                                IProductRepository productRepository,
                                                IClashRepository clashRepository,
                                                IValidator<SponsoredDayPartModelBase> sponsoredDayPartModelValidation)
        {
            _sponsorshipRepository = sponsorshipRepository;

            RuleFor(model => model).NotNull()
                                   .WithMessage("A valid SponsorshipModel is required");

            When(model => model != null, () =>
            {
                RuleFor(model => model.ExternalReferenceId)
                              .Must(ContainAnExistingExternalReferenceId)
                              .When(model => !String.IsNullOrWhiteSpace(model.ExternalReferenceId))
                              .WithMessage(model => $"ExternalReferenceId: {model.ExternalReferenceId} doesn't exists, An existing ExternalReferenceId is required to update the Sponsorship");              

                RuleFor(model => model.SponsoredItems)
                                     .Must(ContainSponsoredItems)
                                     .WithMessage("SponsoredItems is required and should contain a valid SponsoredItem for each item");

                When(ContainsSponsoredItems, () =>
                {
                    RuleFor(model => model.SponsoredItems)
                                          .SetCollectionValidator(model =>
                                          new CreateSponsoredItemModelValidation(salesAreaRepository, programmeRepository,
                                                                                 productRepository, clashRepository,
                                                                                 sponsoredDayPartModelValidation,
                                                                                 model));
                });
            });
        }
       
        private bool ContainAnExistingExternalReferenceId(string externalReferenceId)
        {
            return _sponsorshipRepository.Exists(externalReferenceId.Trim());
        }         
    }
}
