using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Shared
{
    public class MultipartValidator : AbstractValidator<Multipart>
    {
        public MultipartValidator()
        {
            RuleFor(m => m.Lengths).NotEmpty().WithMessage("Multipart Lengths are missing");
            RuleForEach(m => m.Lengths).SetValidator(x => new MultipartLengthValidator());
        }
    }
}
