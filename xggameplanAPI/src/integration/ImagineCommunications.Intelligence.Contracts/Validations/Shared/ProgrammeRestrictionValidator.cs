using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Shared
{
    public class ProgrammeRestrictionValidator : AbstractValidator<ProgrammeRestriction>
    {
        public ProgrammeRestrictionValidator()
        {
            RuleFor(pr => pr.IsCategoryOrProgramme)
                .NotEmpty().WithMessage("Category Or Programme is missing")
                .Matches("^(C|P)$").WithMessage("Invalid Category Or Programme (C/P)");

            RuleFor(pr => pr.IsIncludeOrExclude)
                .NotEmpty().WithMessage("Is Include Or Exclude is missing")
                .Matches("^(I|E)$").WithMessage("Invalid Is Include Or Exclude (I/E)");
        }
    }
}
