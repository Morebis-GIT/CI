using AutoFixture;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using Microsoft.AspNet.SignalR.Hubs;
using Moq;
using NUnit.Framework;
using xggameplan.core.Hubs;
using xggameplan.Hubs;

namespace xggameplan.tests.HubTests
{
    public class ScenarioNotificationTest
    {
        private Fixture _fixture;
        private static Mock<IHubConnectionContext<IClientContract<ScenarioNotificationModel>>> _mockClients { get; set; }
        private static Mock<HubNotification<ScenarioNotificationHub, ScenarioNotificationModel>> _mockScenarioNotificator { get; set; }
        private static ScenarioNotificationModel _dummyScenarioNotificationObject { get; set; }

        [OneTimeSetUp]
        public void OnInit()
        {
            _fixture = new Fixture();
            _mockClients = new Mock<IHubConnectionContext<IClientContract<ScenarioNotificationModel>>>();
            _mockScenarioNotificator = new Mock<HubNotification<ScenarioNotificationHub, ScenarioNotificationModel>> { CallBase = true };
        }

        [OneTimeTearDown]
        public void OnDestroy() => CleanUp();

        [Test]
        public void ScenarioNotificationHubIsMockableViaDynamic()
        {
            var sendCalled = false;
            var allObjectMock = new Mock<IClientContract<ScenarioNotificationModel>>();

            _ = _mockClients.Setup(m => m.All).Returns(allObjectMock.Object);
            _ = _mockScenarioNotificator.Setup(m => m.Clients).Returns(_mockClients.Object);
            _ = allObjectMock.Setup(m => m.Notify(It.IsAny<ScenarioNotificationModel>()))
                .Callback(() => sendCalled = true);

            _dummyScenarioNotificationObject = CreateDummyScenarioNotificationModel();
            _mockScenarioNotificator.Object.Notify(_dummyScenarioNotificationObject);

            Assert.True(sendCalled);
        }

        [Test]
        public void ScenarioNotificationHubIsMockableViaType()
        {
            var all = new Mock<IClientContract<ScenarioNotificationModel>>();
            all.Setup(m => m.Notify(It.IsAny<ScenarioNotificationModel>())).Verifiable();
            _ = _mockClients.Setup(m => m.All).Returns(all.Object);
            _ = _mockScenarioNotificator.Setup(m => m.Clients).Returns(_mockClients.Object);
            _dummyScenarioNotificationObject = CreateDummyScenarioNotificationModel();
            _mockScenarioNotificator.Object.Notify(_dummyScenarioNotificationObject);

            all.VerifyAll();
        }

        [Test]
        public void ScenarioNotificationHubsGroupAreMockable()
        {
            var groups = new Mock<IClientContract<ScenarioNotificationModel>>();
            groups.Setup(m => m.Notify(It.IsAny<ScenarioNotificationModel>())).Verifiable();
            _ = _mockClients.Setup(m => m.Group("message")).Returns(groups.Object);
            _ = _mockScenarioNotificator.Setup(m => m.Clients).Returns(_mockClients.Object);
            _dummyScenarioNotificationObject = CreateDummyScenarioNotificationModel();
            _mockScenarioNotificator.Object.NotifyGroup("message", _dummyScenarioNotificationObject);
            groups.VerifyAll();
        }

        [Test]
        public void ScenarioNotificationHubsClientIsMockable()
        {
            var clients = new Mock<IClientContract<ScenarioNotificationModel>>();
            clients.Setup(m => m.Notify(It.IsAny<ScenarioNotificationModel>())).Verifiable();
            _ = _mockClients.Setup(m => m.Client("random")).Returns(clients.Object);
            _ = _mockScenarioNotificator.Setup(m => m.Clients).Returns(_mockClients.Object);
            _dummyScenarioNotificationObject = CreateDummyScenarioNotificationModel();
            _mockScenarioNotificator.Object.NotifyIndividual("random", _dummyScenarioNotificationObject);
            clients.VerifyAll();
        }

        private ScenarioNotificationModel CreateDummyScenarioNotificationModel()
            => _fixture.Build<ScenarioNotificationModel>().Create();

        protected void CleanUp()
        {
            _fixture = null;
            _mockClients = null;
            _mockScenarioNotificator = null;
        }
    }
}
