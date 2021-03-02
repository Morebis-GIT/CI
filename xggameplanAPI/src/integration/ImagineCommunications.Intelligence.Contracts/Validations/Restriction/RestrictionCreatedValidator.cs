using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Restriction;
using ImagineCommunications.Gameplan.Integration.Contracts.Shared.Enums;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Restriction
{
    public class RestrictionCreatedValidator : AbstractValidator<IRestrictionCreatedOrUpdated>
    {
        public RestrictionCreatedValidator()
        {
            RuleFor(r => r.RestrictionBasis).Must(x => x.HasValue);
            RuleFor(r => r.RestrictionType).Must(r => r.HasValue);

            When(r => r.RestrictionBasis == RestrictionBasis.Product, () =>
            {
                RuleFor(r => r.ProductCode).NotEmpty().NotEqual(default(int));
                RuleFor(r => r.ClashCode).Empty();
                RuleFor(r => r.ClearanceCode).Empty();
            });

            When(r => r.RestrictionBasis == RestrictionBasis.Clash, () =>
            {
                RuleFor(r => r.ClashCode).NotEmpty();
                RuleFor(r => r.ProductCode).Empty();
                RuleFor(r => r.ClockNumber).Equal("0");
                RuleFor(r => r.ClearanceCode).Empty();
            });

            When(r => r.RestrictionBasis == RestrictionBasis.ClearanceCode, () =>
            {
                RuleFor(r => r.ClearanceCode).NotEmpty();
                RuleFor(r => r.ProductCode).Empty();
                RuleFor(r => r.ClockNumber).Equal("0");
                RuleFor(r => r.ClashCode).Empty();
            });

            When(r => r.RestrictionType == RestrictionType.Time, () =>
            {
                RuleFor(r => r.ExternalProgRef).Empty();
                RuleFor(r => r.ProgrammeCategory).Empty();
                RuleFor(r => r.ProgrammeClassification).Empty();
                RuleFor(r => r.IndexType).Empty();
                RuleFor(r => r.IndexThreshold).Empty();
            });

            When(r => r.RestrictionType == RestrictionType.Programme, () =>
            {
                RuleFor(r => r.ExternalProgRef).NotEmpty();
                RuleFor(r => r.ProgrammeCategory).Empty();
                RuleFor(r => r.ProgrammeClassification).Empty();
                RuleFor(r => r.IndexType).Empty();
                RuleFor(r => r.IndexThreshold).Empty();
            });

            When(r => r.RestrictionType == RestrictionType.ProgrammeCategory, () =>
            {
                RuleFor(r => r.ProgrammeCategory).NotEmpty();
                RuleFor(r => r.ExternalProgRef).Empty();
                RuleFor(r => r.IndexType).Empty();
                RuleFor(r => r.IndexThreshold).Empty();
                RuleFor(r => r.ProgrammeClassification).Empty();
            });

            When(r => r.RestrictionType == RestrictionType.Index, () =>
            {
                RuleFor(r => r.IndexThreshold).NotEmpty().NotEqual(default(int));
                RuleFor(r => r.ExternalProgRef).Empty();
                RuleFor(r => r.ProgrammeCategory).Empty();
                RuleFor(r => r.ProgrammeClassification).Empty();
            });

            When(r => r.RestrictionType == RestrictionType.ProgrammeClassification, () =>
            {
                RuleFor(r => r.ProgrammeClassification).NotEmpty();
                RuleFor(r => r.ExternalProgRef).Empty();
                RuleFor(r => r.IndexType).Empty();
                RuleFor(r => r.IndexThreshold).Empty();
                RuleFor(r => r.ProgrammeCategory).Empty();
            });
        }
    }
}
