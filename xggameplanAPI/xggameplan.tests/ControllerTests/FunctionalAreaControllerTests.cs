using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Results;
using AutoFixture;
using AutoMapper;
using ImagineCommunications.GamePlan.CommonTestsHelpers;
using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas;
using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas.Objects;
using Moq;
using NUnit.Framework;
using xggameplan.Configuration;
using xggameplan.Controllers;
using xggameplan.core.Configuration;
using xggameplan.Model;

namespace xggameplan.tests.ControllerTests
{
    [TestFixture(Category = "Controllers :: FunctionalArea")]
    public class FunctionalAreaControllerTests : IDisposable
    {
        private static readonly Fixture Fixture = new SafeFixture();
        private static readonly IMapper Mapper = AutoMapperInitializer.Initialize(AutoMapperConfig.SetupAutoMapper);
        private static FunctionalAreasController Controller { get; set; }
        private static Mock<IFunctionalAreaRepository> MockFunctionalAreaRepository { get; set; }
        private static IEnumerable<FunctionalArea> MockFunctionalArea { get; set; }

        [SetUp]
        public void Init()
        {
            MockFunctionalArea = Fixture.Build<FunctionalArea>().CreateMany(5);
            MockFunctionalAreaRepository = new Mock<IFunctionalAreaRepository>();
            _ = MockFunctionalAreaRepository.Setup(m => m.GetAll()).Returns(MockFunctionalArea);
            Controller = new FunctionalAreasController(MockFunctionalAreaRepository.Object, Mapper);
        }

        [Test]
        [Description("Get Functional areas with full functional area list object")]
        public void GivenGetFunctionalAreas_WhenCalled_ThenShouldReturnFullObject()
        {
            //Act
            IEnumerable<FunctionalArea> result = Controller.Get();

            //Assert
            Assert.That(result, Is.Not.Empty);
        }

        [Test]
        [Description("Update functional areas with Functional area object which includes duplicate FaultTypeId then should raise error")]
        public void GivenPutFunctionalAreas_WhenCalledWithDuplicatedFaultTypeId_ThenShouldRaiseError()
        {
            //Arrange
            var request = Fixture.CreateMany<UpdateFunctionalAreaFaultTypeStatusModel>(4).ToList();
            request[1].FaultTypeId = request[0].FaultTypeId;

            //Act
            var response = Controller.Put(request) as BadRequestErrorMessageResult;

            //Assert
            Assert.That(response.Message, Is.EqualTo("FaultTypeId must be unique"));
        }

        [Test]
        [Description("Update functional areas with Functional area object which includes expected parameters then should return ok")]
        public void GivenPutFunctionalAreas_WhenCalledWithFunctioanlAreaIdAndFaultTypeId_ThenShouldReturnOk()
        {
            //Arrange
            var request = Fixture.CreateMany<UpdateFunctionalAreaFaultTypeStatusModel>(2).ToList();
            request[0].FunctionalAreaId = MockFunctionalArea.ToList()[0].Id;
            request[0].FaultTypeId = MockFunctionalArea.ToList()[0].FaultTypes[0].Id;
            request[1].FunctionalAreaId = MockFunctionalArea.ToList()[1].Id;
            request[1].FaultTypeId = MockFunctionalArea.ToList()[1].FaultTypes[0].Id;

            // Act
            var result = Controller.Put(request) as OkResult;

            // Assert
            Assert.IsNotNull(result);
        }

        public void Dispose()
        {
            MockFunctionalArea = null;
            MockFunctionalAreaRepository = null;
            Controller = null;
        }
    }
}
