using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using xggameplan.Model;

namespace xggameplan.Validations
{
    public class CreateSponsoredItemModelValidation : SponsoredItemModelBaseValidation<CreateSponsoredItemModel>
    {
        public CreateSponsoredItemModelValidation(ISalesAreaRepository salesAreaRepository,
                                                  IProgrammeRepository programmeRepository,
                                                  IProductRepository productRepository,
                                                  IClashRepository clashRepository,
                                                  IValidator<SponsoredDayPartModelBase> sponsoredDayPartModelValidation,
                                                  SponsorshipModelBase sponsorshipModelBase = null)
                                                : base(productRepository)
        {
            RuleFor(model => model).NotNull()
                                   .WithMessage("A valid SponsoredItem is required");
            
            When(model => model != null, () =>
           {
               RuleFor(model => model.SponsorshipItems)
                                     .Must(ContainSponsorshipItems)
                                     .WithMessage("SponsorshipItems is required and should contain a valid SponsorshipItem for each item");

               When(ContainsSponsorshipItems, () =>
               {
                   RuleFor(model => model.SponsorshipItems)
                                         .SetCollectionValidator(model =>
                                         new CreateSponsorshipItemModelValidation(salesAreaRepository, programmeRepository,                                                                                
                                                                                  sponsoredDayPartModelValidation, sponsorshipModelBase));
               });

               RuleFor(model => model).Must(ContainAdvertiserOrClashExclusivities)
                                      .WithName(model => nameof(model.AdvertiserExclusivities))
                                      .WithMessage("SponsoredItem must contain AdvertiserExclusivities And/Or ClashExclusivities");

               When(ContainsAdvertiserExclusivities, () =>
               {
                   RuleFor(model => model.AdvertiserExclusivities)
                                   .SetCollectionValidator(model => new CreateAdvertiserExclusivityModelValidation(productRepository, model));
               });

               When(ContainsClashExclusivities, () =>
               {
                   RuleFor(model => model.ClashExclusivities)
                                   .SetCollectionValidator(model => new CreateClashExclusivityModelValidation(clashRepository, model));
               });

               When(ContainsBothClashAndAdvertiserExclusivities, () =>
               {
                   RuleFor(model => model).Must(ContainSameRestrictionTypeForBothClashAndAdvertiserExclusivities)
                                          .WithName(model => nameof(model.AdvertiserExclusivities))
                                          .WithMessage("RestrictionType must be same for both AdvertiserExclusivities And ClashExclusivities");
               });
           });
        }

        private bool ContainSponsorshipItems(IEnumerable<CreateSponsorshipItemModel> sponsorshipItems)
        {
            return sponsorshipItems?.Any() == true && !sponsorshipItems.Any(a => a == null);
        }

        private bool ContainsSponsorshipItems(CreateSponsoredItemModel model)
        {
            return ContainSponsorshipItems(model.SponsorshipItems);
        }

        private bool ContainSameRestrictionTypeForBothClashAndAdvertiserExclusivities(CreateSponsoredItemModel model)
        {
            return model.AdvertiserExclusivities.Select(a => a.RestrictionType)
                        .SequenceEqual(model.ClashExclusivities.Select(a => a.RestrictionType));
        }

        private bool ContainAdvertiserOrClashExclusivities(CreateSponsoredItemModel model)
        {
            return model.AdvertiserExclusivities?.Any() == true || model.ClashExclusivities?.Any() == true;
        }

        private bool ContainsBothClashAndAdvertiserExclusivities(CreateSponsoredItemModel model)
        {
            return model.AdvertiserExclusivities?.Any() == true && model.ClashExclusivities?.Any() == true &&
                   model.AdvertiserExclusivities.Count() == model.ClashExclusivities.Count();
        }

        private bool ContainsAdvertiserExclusivities(CreateSponsoredItemModel model)
        {
            return model.AdvertiserExclusivities?.Any() == true;
        }

        private bool ContainsClashExclusivities(CreateSponsoredItemModel model)
        {
            return model.ClashExclusivities?.Any() == true;
        }
    }
}
