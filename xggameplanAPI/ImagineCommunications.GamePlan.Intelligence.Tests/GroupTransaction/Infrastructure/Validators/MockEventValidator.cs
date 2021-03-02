using FluentValidation;
using ImagineCommunications.GamePlan.Intelligence.Tests.GroupTransaction.Infrastructure.EventTypes.Interfaces;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.GroupTransaction.Infrastructure.Validators
{
    public class MockEventValidator : AbstractValidator<IMockEventBase>
    {
        public MockEventValidator()
        {
            _ = RuleFor(x => x)
                .Must(x => x.IsModelValid);
        }
    }
}
