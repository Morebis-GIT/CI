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
    public class CreateSponsorshipModelValidation : SponsorshipModelBaseValidation<CreateSponsorshipModel>
    {
        private readonly ISponsorshipRepository _sponsorshipRepository;

        public CreateSponsorshipModelValidation(ISponsorshipRepository sponsorshipRepository,
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
                              .Must(ContainUniqueExternalReferenceId)
                              .When(model => !String.IsNullOrWhiteSpace(model.ExternalReferenceId))
                              .WithMessage( model => $"ExternalReferenceId: {model.ExternalReferenceId} already exists, A unique ExternalReferenceId is required");

                //RuleFor(model => model.RestrictionLevel)
                //            .Must(ContainValidRestrictionLevel)                       
                //            .WithMessage(model => CreateInvalidRestrictionLevelErrorMessage(model.RestrictionLevel));

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

        //private bool ContainValidRestrictionLevel(SponsorshipRestrictionLevel restrictionLevel)
        //{
        //   return Enum.IsDefined(typeof(SponsorshipRestrictionLevel), restrictionLevel);        
        //}

        //private bool ContainSponsoredItems(IEnumerable<CreateSponsoredItemModel> sponsoredItems)
        //{
        //    return sponsoredItems?.Any() == true && !sponsoredItems.Any(a => a == null);
        //}

        //private bool ContainsSponsoredItems(CreateSponsorshipModel model)
        //{
        //    return ContainSponsoredItems(model.SponsoredItems);
        //}

        private bool ContainUniqueExternalReferenceId(string externalReferenceId)
        {   
            return !_sponsorshipRepository.Exists(externalReferenceId.Trim());
        }

        //private string CreateInvalidRestrictionLevelErrorMessage(SponsorshipRestrictionLevel restrictionLevel)
        //{
        //    return $"RestrictionLevel: {restrictionLevel} is invalid. Allowed values: {String.Join(",", Enum.GetNames(typeof(SponsorshipRestrictionLevel)))}";
        //}
    }
}
