using ImagineCommunications.BusClient.Abstraction.Classes;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.BookingPositionGroup;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Truncate;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.BookingPositionGroup
{
    public class BookingPositionGroupTruncatedEventHandler : EventHandler<IBookingPositionGroupTruncated>
    {
        private readonly ISqlServerTenantDbContext _dbContext;

        public BookingPositionGroupTruncatedEventHandler(ISqlServerTenantDbContext dbContext) =>
            _dbContext = dbContext;

        public override void Handle(IBookingPositionGroupTruncated command) =>
            _dbContext.Specific
                .TruncateOrDelete<
                    GamePlan.Persistence.SqlServer.Entities.Tenant.BookingPositionGroups.BookingPositionGroup>(
                    DeleteFromOptions.TruncateDependent);
    }
}
