using BoDi;
using ImagineCommunications.Gameplan.Synchronization;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using Moq;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Dependencies
{
    public class SynchronizationScenarioDependency : IScenarioDependency
    {
        public void Register(IObjectContainer objectContainer)
        {
            objectContainer.RegisterInstanceAs(new SynchronizationServicesConfiguration()
                .Add(SynchronizedServiceType.RunExecution)
                .Add(SynchronizedServiceType.DataSynchronization, 1));
            objectContainer.RegisterInstanceAs(Mock.Of<Gameplan.Synchronization.Interfaces.ISynchronizationObjectRepository>());
            var synchronizationServiceMock = new Mock<Gameplan.Synchronization.Interfaces.ISynchronizationService>();
            var token = SynchronizationToken.Empty;
            _ = synchronizationServiceMock
                .Setup(m => m.TryCapture(It.IsAny<int>(), It.IsAny<string>(), out token))
                .Returns<int, string, SynchronizationToken>((serviceId, ownerId, syncToken) => true);
            objectContainer.RegisterInstanceAs(synchronizationServiceMock.Object);
        }
    }
}
