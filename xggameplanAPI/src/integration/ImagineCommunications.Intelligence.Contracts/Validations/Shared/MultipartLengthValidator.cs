using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Shared
{
    public class MultipartLengthValidator : AbstractValidator<MultipartLength>
    {
        public MultipartLengthValidator()
        {
            RuleFor(m => m.BookingPosition).LessThanOrEqualTo(256);
            RuleFor(m => m.Sequencing).LessThanOrEqualTo(256);
        }
    }
}
