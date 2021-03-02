using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.SalesArea;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.SalesArea
{
    public class SalesAreaUpdatedValidator : AbstractValidator<ISalesAreaUpdated>
    {
        public SalesAreaUpdatedValidator()
        {
            RuleFor(data => data.Name).NotEmpty();
            RuleFor(data => data.ShortName).NotEmpty();
            RuleFor(data => data.CurrencyCode).NotEmpty();
            RuleFor(data => data.BaseDemographic1).NotEmpty();
            RuleFor(data => data.BaseDemographic2).NotEmpty();
            RuleFor(data => data.ChannelGroup).NotNull().Must(item => item.Count != 0);
            RuleFor(data => data.DayDuration).NotNull().Must(item => item.Ticks != 0);
            RuleFor(data => data.Demographics).NotEmpty();
        }
    }
}
