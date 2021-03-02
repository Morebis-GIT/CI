using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Enums;
using xggameplan.Model;

namespace xggameplan.Validations
{
    public abstract class SponsoredItemModelBaseValidation<T> : AbstractValidator<T>
        where T : SponsoredItemModelBase
    {
        private readonly IProductRepository _productRepository;
        private IEnumerable<Product> _matchingProducts;

        public SponsoredItemModelBaseValidation(IProductRepository productRepository)
        {
            _productRepository = productRepository;

            RuleFor(model => model).NotNull()
                                   .WithMessage("A valid SponsoredItemModel is required");

            When(model => model != null, () =>
            {
                RuleFor(model => model.Products)
                    .Must(ContainSomeProducts)
                    .WithMessage($"{nameof(SponsoredItemModelBase.Products)} should contain a valid Products for each item");

                RuleFor(model => model.CalculationType)
                    .Must(ContainValidCalculationType)
                    .WithMessage(model => CreateInvalidCalculationTypeErrorMessage(model.CalculationType.Value));

                RuleFor(model => model.RestrictionType)
                    .Must(ContainValidRestrictionType)
                    .When(m => m.RestrictionType.HasValue)
                    .WithMessage(model => CreateInvalidRestrictionTypeErrorMessage(model.RestrictionType));

                When(RestrictionTypeIsRequired, () =>
                {
                    RuleFor(model => model.RestrictionType)
                        .Must(ContainValidRestrictionType)
                        .WithMessage($"{nameof(SponsoredItemModelBase.RestrictionType)} is required");
                });

                When(RestrictionTypeIsNotRequired, () =>
                {
                    RuleFor(model => model.RestrictionType)
                        .Must(NotContainAnyRestrictionType)
                        .WithMessage($"{nameof(SponsoredItemModelBase.RestrictionType)} is not required");
                });

                When(RestrictionValueIsRequired, () =>
                {
                    RuleFor(a => a.RestrictionValue)
                        .Must(ContainRestrictionValue)
                        .WithMessage($"{nameof(SponsoredItemModelBase.RestrictionValue)} is required");
                });

                RuleFor(model => model.RestrictionValue)
                    .Must(IfHasValueContainPositiveIntegerRestrictionValue)
                    .When(CalculationTypeIsFlat)
                    .WithMessage(model => $"{nameof(SponsoredItemModelBase.RestrictionValue)} is not positive integer. Acceptable values are any whole number greater than 0");

                RuleFor(model => model.RestrictionValue)
                    .Must(IfHasValueContainPercentageRestrictionValue)
                    .When(CalculationTypeIsPercentage)
                    .WithMessage(model => $"{nameof(SponsoredItemModelBase.RestrictionValue)} is not percentage. Acceptable values are 1 to 100");

                When(RestrictionValueIsNotRequired, () =>
                {
                    RuleFor(a => a.RestrictionValue)
                        .Must(NotContainAnyRestrictionValue)
                        .WithMessage($"{nameof(SponsoredItemModelBase.RestrictionValue)} is not required");
                });

                RuleFor(model => model.Applicability)
                    .Must(ContainValidApplicabilityValue)
                    .When(ApplicabilityIsRequired)
                    .WithMessage(model => CreateRequiredApplicabilityeErrorMessage(model.Applicability));

                RuleFor(model => model.Applicability)
                    .Must(ContainValidApplicability)
                    .When(m => m.Applicability.HasValue)
                    .WithMessage(model => CreateInvalidApplicabilityeErrorMessage(model.Applicability));

                RuleFor(model => model.Applicability)
                    .Must(NotContainUnsuitableApplicability)
                    .When(ApplicabilityIsNotRequired)
                    .WithMessage($"{nameof(SponsoredItemModelBase.Applicability)} should not be 'Each Competitor' when {nameof(SponsoredItemModelBase.CalculationType)} is 'None' or 'Exclusive'");
            });
        }

        private string CreateInvalidApplicabilityeErrorMessage(SponsorshipApplicability? applicability)
        {
            return $"{nameof(SponsoredItemModelBase.Applicability)}: {applicability} is invalid. Allowed values: {String.Join(",", Enum.GetNames(typeof(SponsorshipApplicability)))}";
        }

        private string CreateRequiredApplicabilityeErrorMessage(SponsorshipApplicability? applicability)
        {
            return $"Applicability is required. Allowed values: {String.Join(",", Enum.GetNames(typeof(SponsorshipApplicability)))}";
        }

        private bool ContainValidApplicability(SponsorshipApplicability? applicability)
        {
            return Enum.IsDefined(typeof(SponsorshipApplicability), applicability);
        }

        private bool ContainValidApplicabilityValue(SponsorshipApplicability? applicability)
        {
            return applicability.HasValue;
        }

        private bool ContainValidCalculationType(SponsorshipCalculationType? calculationType)
        {
            return Enum.IsDefined(typeof(SponsorshipCalculationType), calculationType.Value);
        }

        private bool IfHasValueContainPositiveIntegerRestrictionValue(int? restrictionValue)
        {
            return !restrictionValue.HasValue || (restrictionValue.HasValue && restrictionValue.Value > 0);
        }

        private bool ContainRestrictionValue(int? restrictionValue)
        {
            return restrictionValue.HasValue;
        }

        private bool IfHasValueContainPercentageRestrictionValue(int? restrictionValue)
        {
            return !restrictionValue.HasValue || (restrictionValue.HasValue && restrictionValue.Value > 0 && restrictionValue.Value <= 100);
        }

        private bool ApplicabilityIsNotRequired(T model)
        {
            return !ApplicabilityIsRequired(model);
        }

        private bool ApplicabilityIsRequired(T model)
        {
            return IsCalculationTypeFlatOrPercentage(model);
        }

        private bool NotContainUnsuitableApplicability(SponsorshipApplicability? applicability)
        {
            return !applicability.HasValue || applicability == SponsorshipApplicability.AllCompetitors;
        }

        private bool NotContainAnyRestrictionType(SponsorshipRestrictionType? restrictionType)
        {
            return !restrictionType.HasValue;
        }

        private bool NotContainAnyRestrictionValue(int? restrictionValue)
        {
            return !restrictionValue.HasValue;
        }

        private bool ContainValidRestrictionType(SponsorshipRestrictionType? restrictionType)
        {
            return restrictionType.HasValue && Enum.IsDefined(typeof(SponsorshipRestrictionType), restrictionType);
        }

        private bool ApplicabilityIsAllCompetitors(T model)
        {
            return model.Applicability == SponsorshipApplicability.AllCompetitors;
        }

        private bool ApplicabilityIsEachCompetitor(T model)
        {
            return model.Applicability == SponsorshipApplicability.EachCompetitor;
        }

        private bool RestrictionTypeIsRequired(T model)
        {
            return IsCalculationTypeFlatOrPercentage(model) && ApplicabilityIsAllCompetitors(model);
        }

        private bool RestrictionValueIsRequired(T model)
        {
            return IsCalculationTypeFlatOrPercentage(model) && ApplicabilityIsAllCompetitors(model);
        }

        private bool RestrictionTypeIsNotRequired(T model)
        {
            return !RestrictionTypeIsRequired(model);
        }

        private bool IsCalculationTypeNoneOrExclusive(T model)
        {
            return model.CalculationType == SponsorshipCalculationType.None ||
                   model.CalculationType == SponsorshipCalculationType.Exclusive;
        }

        private bool IsCalculationTypeFlatOrPercentage(T model)
        {
            return model.CalculationType == SponsorshipCalculationType.Flat ||
                   model.CalculationType == SponsorshipCalculationType.Percentage;
        }

        private bool CalculationTypeIsFlat(T model)
        {
            return model.CalculationType == SponsorshipCalculationType.Flat;
        }

        private bool CalculationTypeIsPercentage(T model)
        {
            return model.CalculationType == SponsorshipCalculationType.Percentage;
        }

        private bool RestrictionValueIsNotRequired(T model)
        {
            return !RestrictionValueIsRequired(model);
        }

        private IEnumerable<string> GetNonExistingProducts(IEnumerable<string> productsToValidate, IEnumerable<Product> matchingProducts)
        {
            return (productsToValidate?.Any() == true && matchingProducts?.Any() == true) ?
                   productsToValidate.Except(matchingProducts.Select(a => a.Externalidentifier).Distinct()) :
                   productsToValidate;
        }

        private IEnumerable<Product> GetMatchingProducts(IEnumerable<string> products)
        {
            if (_matchingProducts == null || !_matchingProducts.Any(p => p != null))
            {
                _matchingProducts = _productRepository.FindByExternal(products.ToList());
            }

            return _matchingProducts;
        }

        private bool ContainSomeProducts(IEnumerable<string> products)
        {
            return products?.Any() == true && !products.Any(s => string.IsNullOrWhiteSpace(s));
        }

        private bool ContainsSomeProducts(T model)
        {
            return ContainSomeProducts(model.Products);
        }

        private string CreateNonExistingProductsErrorMessage(IEnumerable<string> productsToValidate)
        {
            var nonExistingProducts = GetNonExistingProducts(productsToValidate, GetMatchingProducts(productsToValidate));

            return (nonExistingProducts?.Any() == true) ?
                   $"Product(s): [ {string.Join(", ", nonExistingProducts)} ] do not exist" :
                   "Product(s): Error";
        }

        private string CreateProductsNotFromSingleAdvertiserErrorMessage(IEnumerable<string> productsToValidate)
        {
            return $"Product(s): [ {string.Join(", ", productsToValidate)} ] must be linked to a single Advertiser.";
        }

        private string CreateInvalidCalculationTypeErrorMessage(SponsorshipCalculationType calculationType)
        {
            return $"CalculationType: {calculationType} is invalid. Allowed values: {String.Join(",", Enum.GetNames(typeof(SponsorshipCalculationType)))}";
        }

        private string CreateInvalidRestrictionTypeErrorMessage(SponsorshipRestrictionType? restrictionType)
        {
            return $"RestrictionType: {restrictionType} is invalid. Allowed values: {String.Join(",", Enum.GetNames(typeof(SponsorshipRestrictionType)))}";
        }
    }
}
