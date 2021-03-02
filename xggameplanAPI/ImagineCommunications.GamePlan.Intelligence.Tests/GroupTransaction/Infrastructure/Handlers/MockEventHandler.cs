using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Handlers.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.GroupTransaction.Infrastructure.EventTypes.Interfaces;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.GroupTransaction.Infrastructure.Handlers
{
    public class MockEventHandler<TMockEvent> : BusClient.Abstraction.Classes.EventHandler<TMockEvent>
        where TMockEvent : IMockEventBase, IEvent
    {
        public override void Handle(TMockEvent command)
        {
            if (!command.BusinessValidationPassed)
            {
                throw new DataSyncException(DataSyncErrorCode.BreakNotFound);
            }
        }
    }
}
