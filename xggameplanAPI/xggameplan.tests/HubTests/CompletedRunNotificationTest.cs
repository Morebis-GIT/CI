using AutoFixture;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using Microsoft.AspNet.SignalR.Hubs;
using Moq;
using NUnit.Framework;
using xggameplan.core.Hubs;
using xggameplan.Hubs;

namespace xggameplan.tests.HubTests
{
    public class CompletedRunNotificationTest
    {
        private Fixture _fixture;
        private static Mock<IHubConnectionContext<IClientContract<RunNotification>>> _mockClients { get; set; }
        private static Mock<HubNotification<RunNotificationHub, RunNotification>> _mockCompletedRunNotificator { get; set; }
        private static RunNotification _dummyRunNotification { get; set; }

        [OneTimeSetUp]
        public void OnInit()
        {
            _fixture = new Fixture();
            _mockClients = new Mock<IHubConnectionContext<IClientContract<RunNotification>>>();
            _mockCompletedRunNotificator = new Mock<HubNotification<RunNotificationHub, RunNotification>> { CallBase = true };
        }

        [OneTimeTearDown]
        public void OnDestroy() => CleanUp();

        [Test]
        public void RunNotificationHubIsMockableViaDynamic()
        {
            var sendCalled = false;
            var allObjectMock = new Mock<IClientContract<RunNotification>>();
            _ = _mockClients.Setup(m => m.All).Returns(allObjectMock.Object);
            _ = _mockCompletedRunNotificator.Setup(m => m.Clients).Returns(_mockClients.Object);
            _ = allObjectMock.Setup(m => m.Notify(It.IsAny<RunNotification>())).Callback(() => sendCalled = true);
            _dummyRunNotification = CreateDummyRunNotificationModel();
            _mockCompletedRunNotificator.Object.Notify(_dummyRunNotification);
            Assert.True(sendCalled);
        }

        [Test]
        public void RunNotificationHubIsMockableViaType()
        {
            var all = new Mock<IClientContract<RunNotification>>();
            all.Setup(m => m.Notify(It.IsAny<RunNotification>())).Verifiable();
            _ = _mockClients.Setup(m => m.All).Returns(all.Object);
            _ = _mockCompletedRunNotificator.Setup(m => m.Clients).Returns(_mockClients.Object);
            _dummyRunNotification = CreateDummyRunNotificationModel();
            _mockCompletedRunNotificator.Object.Notify(_dummyRunNotification);
            all.VerifyAll();
        }

        [Test]
        public void RunNotificationHubsGroupAreMockable()
        {
            var groups = new Mock<IClientContract<RunNotification>>();
            groups.Setup(m => m.Notify(It.IsAny<RunNotification>())).Verifiable();
            _ = _mockClients.Setup(m => m.Group("message")).Returns(groups.Object);
            _ = _mockCompletedRunNotificator.Setup(m => m.Clients).Returns(_mockClients.Object);
            _dummyRunNotification = CreateDummyRunNotificationModel();
            _mockCompletedRunNotificator.Object.NotifyGroup("message", _dummyRunNotification);
            groups.VerifyAll();
        }

        [Test]
        public void RunNotificationHubsClientIsMockable()
        {
            var clients = new Mock<IClientContract<RunNotification>>();
            clients.Setup(m => m.Notify(It.IsAny<RunNotification>())).Verifiable();
            _ = _mockClients.Setup(m => m.Client("random")).Returns(clients.Object);
            _ = _mockCompletedRunNotificator.Setup(m => m.Clients).Returns(_mockClients.Object);
            _dummyRunNotification = CreateDummyRunNotificationModel();
            _mockCompletedRunNotificator.Object.NotifyIndividual("random", _dummyRunNotification);
            clients.VerifyAll();
        }

        private RunNotification CreateDummyRunNotificationModel()
            => _fixture.Build<RunNotification>().Create();

        protected void CleanUp()
        {
            _fixture = null;
            _mockClients = null;
            _mockCompletedRunNotificator = null;
        }
    }
}
