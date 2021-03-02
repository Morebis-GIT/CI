using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.BookingPositionGroup;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.BookingPositionGroup
{
    public class BulkBookingPositionGroupCreatedValidator : AbstractValidator<IBulkBookingPositionGroupCreated>
    {
        public BulkBookingPositionGroupCreatedValidator() => RuleFor(x => x.Data).NotEmpty();
    }
}
