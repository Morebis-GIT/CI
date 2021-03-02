#pragma warning disable CA1707 // Identifiers should not contain underscores

using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoMapper;
using ImagineCommunications.GamePlan.CommonTestsHelpers;
using ImagineCommunications.GamePlan.Domain.Generic.DbSequence;
using ImagineCommunications.GamePlan.Domain.Passes;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using Moq;
using NUnit.Framework;
using xggameplan.Controllers;
using xggameplan.core.Configuration;
using xggameplan.core.Interfaces;
using xggameplan.Model;

namespace xggameplan.tests.ControllerTests
{
    [TestFixture(Category = "Controllers :: Scenarios")]
    public static class ScenarioControllerTests
    {
        [Test]
        [Description("Given a legacy scenario with a null DateCreated when getting all scenarios then the DateCreated is null")]
        public static void
        Given_a_legacy_scenario_with_a_null_DateCreated_when_getting_all_scenarios_then_the_DateCreated_is_null()
        {
            // Arrange
            Fixture fixture = new SafeFixture();

            var fakeRunRepository = new Mock<IRunRepository>();
            var fakeScenarioRepository = new Mock<IScenarioRepository>();
            var fakePassRepository = new Mock<IPassRepository>();
            var fakeTenantSettingsRepository = new Mock<ITenantSettingsRepository>();
            var fakeIdentityGeneratorResolver = new Mock<IIdentityGeneratorResolver>();
            var fakePassInspectorService = new Mock<IPassInspectorService>();

            IMapper mapper = AutoMapperInitializer.Initialize(Configuration.AutoMapperConfig.SetupAutoMapper);

            IEnumerable<Pass> fakePasses = fixture.CreateMany<Pass>(1);
            IEnumerable<PassReference> fakePassReferences = fixture
                .Build<PassReference>()
                .With(p => p.Id, fakePasses.First().Id)
                .CreateMany(1);

            IEnumerable<Scenario> scenarioWithDateModifiedNull = fixture
                .Build<Scenario>()
                .Without(p => p.DateCreated)
                .With(p => p.Passes, fakePassReferences.ToList())
                .CreateMany(1);

            TenantSettings fakeTenantSettings = fixture
                .Build<TenantSettings>()
                .With(p => p.DefaultScenarioId, Guid.NewGuid())
                .Create();

            _ = fakePassRepository
                .Setup(m => m.FindByIds(It.IsAny<IEnumerable<int>>()))
                .Returns(fakePasses);

            _ = fakeScenarioRepository
                .Setup(m => m.GetAll())
                .Returns(scenarioWithDateModifiedNull);

            _ = fakeTenantSettingsRepository
                .Setup(m => m.Get())
                .Returns(fakeTenantSettings);

            using (var scenariosController = new ScenariosController(
                fakeRunRepository.Object,
                fakeScenarioRepository.Object,
                fakePassRepository.Object,
                fakeTenantSettingsRepository.Object,
                fakeIdentityGeneratorResolver.Object,
                mapper,
                null,
                null,
                fakePassInspectorService.Object
                ))
            {
                // Act
                IEnumerable<ScenarioModel> result = scenariosController.GetAll();

                // Assert
                Assert.That(result.First().DateCreated, Is.Null);
            }
        }
    }
}

#pragma warning restore CA1707 // Identifiers should not contain underscores
