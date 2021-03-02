using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Shared
{
    public class LengthValidator : AbstractValidator<Length>
    {
        public LengthValidator()
        {
            RuleFor(l => l.length).NotEmpty().WithMessage("Length is missing");
        }
    }
}
