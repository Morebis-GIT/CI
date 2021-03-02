using FluentValidation;
using ImagineCommunications.BusClient.Abstraction.Interfaces;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.GroupTransaction
{
    public class GroupTransactionEventValidator : AbstractValidator<IGroupTransactionEvent>
    {
        public GroupTransactionEventValidator()
        {
            RuleFor(data => data.EventCount).GreaterThan(0)
                .WithMessage("Value of EventCount argument must be more than zero.");
        }
    }
}
