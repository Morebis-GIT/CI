using AutoFixture;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.LibrarySalesAreaPassPriorities;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using Moq;
using NUnit.Framework;
using xggameplan.core.Configuration;

namespace xggameplan.tests.ControllerTests
{
    public class LibrarySalesAreaPassPrioritiesControllerBaseTests
    {
        [SetUp]
        public void Setup()
        {
            // Arrange
            Mapper = AutoMapperInitializer.Initialize(Configuration.AutoMapperConfig.SetupAutoMapper);

            Fixture = new Fixture();
            FakeLibrarySalesAreaPassPrioritiesRepository = new Mock<ILibrarySalesAreaPassPrioritiesRepository>();
            FakeTenantSettingsRepository = new Mock<ITenantSettingsRepository>();
            FakeSalesAreaPassPriority = Fixture.Create<LibrarySalesAreaPassPriority>();
            FakeTenantSettings = Fixture.Create<TenantSettings>();
        }

        protected Fixture Fixture { get; private set; }
        protected Mock<ILibrarySalesAreaPassPrioritiesRepository> FakeLibrarySalesAreaPassPrioritiesRepository { get; private set; }
        protected Mock<ITenantSettingsRepository> FakeTenantSettingsRepository { get; private set; }
        protected IMapper Mapper { get; private set; }
        protected LibrarySalesAreaPassPriority FakeSalesAreaPassPriority { get; private set; }
        protected TenantSettings FakeTenantSettings { get; private set; }
    }
}
