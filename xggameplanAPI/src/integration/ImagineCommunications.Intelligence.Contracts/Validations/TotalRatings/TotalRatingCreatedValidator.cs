using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.TotalRatings;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.TotalRatings
{
    public class TotalRatingCreatedValidator : AbstractValidator<ITotalRatingCreated>
    {
        public TotalRatingCreatedValidator()
        {
            RuleFor(d => d.SalesArea).NotEmpty();
            RuleFor(d => d.Demograph).NotEmpty();
            RuleFor(d => d.Daypart).NotEmpty();
            RuleFor(d => d.DaypartGroup).NotEmpty();
            RuleFor(d => d.Date).NotEmpty();
            RuleFor(d => d.TotalRatings).NotNull();
        }
    }
}
