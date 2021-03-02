using System;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using Moq;
using NodaTime;
using NUnit.Framework;
using xggameplan.AuditEvents;
using xggameplan.core.FeatureManagement.Interfaces;
using xggameplan.core.Services;

namespace xggameplan.core.tests.Services
{
    [TestFixture]
    public class SystemLogicalDateServiceTests
    {
        private Mock<ITenantSettingsRepository> _fakeTenantSettingsRepository;
        private Mock<IFeatureManager> _fakeFeatureManager;
        private Mock<IClock> _fakeClock;
        private Mock<IAuditEventRepository> _fakeAuditEventRepository;
        private DateTime _currentDateTime;

        [SetUp]
        public void Init()
        {
            _fakeTenantSettingsRepository = new Mock<ITenantSettingsRepository>();
            _fakeFeatureManager = new Mock<IFeatureManager>();
            _fakeClock = new Mock<IClock>();
            _fakeAuditEventRepository = new Mock<IAuditEventRepository>();

            _currentDateTime = DateTime.UtcNow;

            var instant = Instant.FromDateTimeUtc(_currentDateTime);

            _fakeClock.Setup(x => x.GetCurrentInstant()).Returns(instant);
        }

        [Test]
        [Description("Get SystemLogicalDate and date is in TenantSettings and Feature is enabled should return date")]
        public void GetSystemLogicalDateAndDateIsInTenantSettingsAndFeatureIsEnabled_ShouldReturnDate()
        {
            //Arrange
            var expected = new DateTime(2019, 11, 24).Add(_currentDateTime.TimeOfDay);

            var tenantSettings = new TenantSettings
            {
                SystemLogicalDate = "11242019"
            };

            var productFeature = nameof(ProductFeature.UseSystemLogicalDate);

            var useSystemLogicalDate = true;

            _fakeTenantSettingsRepository.Setup(x => x.Get()).Returns(tenantSettings);

            _fakeFeatureManager.Setup(x => x.IsEnabled(productFeature)).Returns(useSystemLogicalDate);

            //Act
            var systemLogicalDateService = new SystemLogicalDateService(_fakeTenantSettingsRepository.Object,
                _fakeFeatureManager.Object, _fakeClock.Object, _fakeAuditEventRepository.Object);

            //Assert
            Assert.IsTrue(systemLogicalDateService.GetSystemLogicalDate() == expected);
        }

        [Test]
        [Description("Get SystemLogicalDate and date is not in TenantSettings and Feature is enabled should return current date")]
        public void GetSystemLogicalDateAndDateIsNotInTenantSettingsAndFeatureIsEnabled_ShouldReturnCurrentDate()
        {
            //Arrange
            var expected = _currentDateTime;

            var tenantSettings = new TenantSettings
            {
                SystemLogicalDate = null
            };

            var productFeature = nameof(ProductFeature.UseSystemLogicalDate);

            var useSystemLogicalDate = true;

            _fakeTenantSettingsRepository.Setup(x => x.Get()).Returns(tenantSettings);

            _fakeFeatureManager.Setup(x => x.IsEnabled(productFeature)).Returns(useSystemLogicalDate);

            //Act
            var systemLogicalDateService = new SystemLogicalDateService(_fakeTenantSettingsRepository.Object,
                _fakeFeatureManager.Object, _fakeClock.Object, _fakeAuditEventRepository.Object);

            //Assert
            Assert.IsTrue(systemLogicalDateService.GetSystemLogicalDate() == expected);
        }

        [Test]
        [Description("Get SystemLogicalDate and date is in TenantSettings and Feature is disabled should return current date")]
        public void GetSystemLogicalDateAndDateIsInTenantSettingsAndFeatureIsDisabled_ShouldReturnCurrentDate()
        {
            //Arrange
            var expected = _currentDateTime;

            var tenantSettings = new TenantSettings
            {
                SystemLogicalDate = "11242019"
            };

            var productFeature = nameof(ProductFeature.UseSystemLogicalDate);

            var useSystemLogicalDate = false;

            _fakeTenantSettingsRepository.Setup(x => x.Get()).Returns(tenantSettings);

            _fakeFeatureManager.Setup(x => x.IsEnabled(productFeature)).Returns(useSystemLogicalDate);

            //Act
            var systemLogicalDateService = new SystemLogicalDateService(_fakeTenantSettingsRepository.Object,
                _fakeFeatureManager.Object, _fakeClock.Object, _fakeAuditEventRepository.Object);

            //Assert
            Assert.IsTrue(systemLogicalDateService.GetSystemLogicalDate() == expected);
        }

        [Test]
        [Description("Get SystemLogicalDate and date is in TenantSettings and Feature is enabled and format is not valid should return current date")]
        public void GetSystemLogicalDateAndDateIsInTenantSettingsAndFeatureIsEnabledAndFormatIsNotValid_ShouldReturnCurrentDate()
        {
            //Arrange
            var expected = _currentDateTime;

            var tenantSettings = new TenantSettings
            {
                SystemLogicalDate = "24/11/2019"
            };

            var productFeature = nameof(ProductFeature.UseSystemLogicalDate);

            var useSystemLogicalDate = true;

            _fakeTenantSettingsRepository.Setup(x => x.Get()).Returns(tenantSettings);

            _fakeFeatureManager.Setup(x => x.IsEnabled(productFeature)).Returns(useSystemLogicalDate);

            //Act
            var systemLogicalDateService = new SystemLogicalDateService(_fakeTenantSettingsRepository.Object,
                _fakeFeatureManager.Object, _fakeClock.Object, _fakeAuditEventRepository.Object);

            //Assert
            Assert.IsTrue(systemLogicalDateService.GetSystemLogicalDate() == expected);
        }
    }
}
