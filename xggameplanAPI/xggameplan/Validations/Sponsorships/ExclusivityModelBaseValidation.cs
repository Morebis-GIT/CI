using FluentValidation;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Enums;
using xggameplan.Model;

namespace xggameplan.Validations
{
    public class ExclusivityModelBaseValidation<T> : AbstractValidator<T>
        where T : ExclusivityModelBase
    {
        private readonly SponsoredItemModelBase _sponsoredItemModelBase;

        public ExclusivityModelBaseValidation(SponsoredItemModelBase sponsoredItemModelBase = null)
        {
            _sponsoredItemModelBase = sponsoredItemModelBase;

            When(model => model != null, () =>
            {
                When(RestrictionTypeIsRequired, () =>
                {
                    RuleFor(model => model.RestrictionType)
                        .Must(ContainRestrictionType)
                        .WithMessage($"A valid {nameof(SponsoredItemModelBase.RestrictionType)} is required");
                });

                When(RestrictionTypeIsNotRequired, () =>
                {
                    RuleFor(model => model.RestrictionType)
                        .Must(NotContainAnyRestrictionType)
                        .WithMessage($"{nameof(SponsoredItemModelBase.RestrictionType)} is not required");
                });

                When(RestrictionValueIsRequired, () =>
                {
                    RuleFor(model => model.RestrictionValue)
                        .Must(ContainRestrictionValue)
                        .WithMessage($"A valid {nameof(SponsoredItemModelBase.RestrictionValue)} is required");
                });

                When(RestrictionValueIsRequired, () =>
                {
                    RuleFor(model => model.RestrictionValue)
                        .Must(IfHasValueContainPositiveIntegerRestrictionValue)
                        .When(CalculationTypeIsFlat)
                        .WithMessage($"{nameof(SponsoredItemModelBase.RestrictionValue)} is not positive integer. Acceptable values are any whole number greater than 0");
                });

                When(RestrictionValueIsRequired, () =>
                {
                    RuleFor(model => model.RestrictionValue)
                        .Must(IfHasValueContainPercentageRestrictionValue)
                        .When(CalculationTypeIsPercentage)
                        .WithMessage($"{nameof(SponsoredItemModelBase.RestrictionValue)} is not percentage. Acceptable values are 1 to 100");
                });

                When(RestrictionValueIsNotRequired, () =>
                {
                    RuleFor(model => model.RestrictionValue)
                        .Must(NotContainAnyRestrictionValue)
                        .WithMessage($"{nameof(SponsoredItemModelBase.RestrictionValue)} is not required");
                });
            });
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

        private bool NotContainAnyRestrictionType(SponsorshipRestrictionType? restrictionType)
        {
            return !restrictionType.HasValue;
        }

        private bool NotContainAnyRestrictionValue(int? restrictionValue)
        {
            return !restrictionValue.HasValue;
        }

        private bool ContainRestrictionType(SponsorshipRestrictionType? restrictionType)
        {
            return restrictionType.HasValue;
        }

        private bool RestrictionValueIsRequired(T model)
        {
            return CalculationTypeIsPercentageOrFlat(model) && ApplicabilityIsEachCompetitor(model);
        }

        private bool RestrictionTypeIsRequired(T model)
        {
            return CalculationTypeIsPercentageOrFlat(model) && ApplicabilityIsEachCompetitor(model);
        }

        private bool ApplicabilityIsEachCompetitor(T model)
        {
            return ApplicabilityIsEachCompetitor(_sponsoredItemModelBase);
        }

        private bool CalculationTypeIsPercentageOrFlat(T model)
        {
            return _sponsoredItemModelBase?.CalculationType == SponsorshipCalculationType.Percentage ||
                _sponsoredItemModelBase?.CalculationType == SponsorshipCalculationType.Flat;
        }

        private bool CalculationTypeIsPercentage(T model)
        {
            return _sponsoredItemModelBase?.CalculationType == SponsorshipCalculationType.Percentage;
        }

        private bool CalculationTypeIsFlat(T model)
        {
            return _sponsoredItemModelBase?.CalculationType == SponsorshipCalculationType.Flat;
        }

        private bool RestrictionTypeIsNotRequired(T model)
        {
            return !RestrictionTypeIsRequired(model);
        }

        private bool RestrictionValueIsNotRequired(T model)
        {
            return !RestrictionValueIsRequired(model);
        }
        
        private bool ApplicabilityIsAllCompetitors(SponsoredItemModelBase model)
        {
            return model?.Applicability == SponsorshipApplicability.AllCompetitors;
        }

        private bool ApplicabilityIsEachCompetitor(SponsoredItemModelBase model)
        {
            return model?.Applicability == SponsorshipApplicability.EachCompetitor;
        }
    }
}
