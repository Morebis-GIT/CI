using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.BookingPositionGroup;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.BookingPositionGroup
{
    public class BulkBookingPositionGroupDeletedValidator : AbstractValidator<IBulkBookingPositionGroupDeleted>
    {
        public BulkBookingPositionGroupDeletedValidator() => RuleFor(x => x.Data).NotEmpty();
    }
}
