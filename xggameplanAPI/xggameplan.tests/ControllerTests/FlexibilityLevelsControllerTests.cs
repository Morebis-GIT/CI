using System;
using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Autopilot.FlexibilityLevels;
using Moq;
using NUnit.Framework;
using xggameplan.Configuration;
using xggameplan.Controllers;
using xggameplan.core.Configuration;
using xggameplan.Model;

namespace xggameplan.tests.ControllerTests
{
    [TestFixture(Category = "Controllers :: FlexibilityLevels")]
    public class FlexibilityLevelsControllerTests : IDisposable
    {
        private Mock<IFlexibilityLevelRepository> _fakeFlexibilityLevelRepository;
        private IMapper _mapper;

        private FlexibilityLevelsController _controller;

        [SetUp]
        public void Init()
        {
            _fakeFlexibilityLevelRepository = new Mock<IFlexibilityLevelRepository>();

            _mapper = AutoMapperInitializer.Initialize(AutoMapperConfig.SetupAutoMapper);

            _controller = new FlexibilityLevelsController(
                _fakeFlexibilityLevelRepository.Object,
                _mapper
            );
        }

        [Test]
        [Description("When getting all flexibility levels then result must be successful")]
        public void GetAllWhenCalledThenShouldReturnOk()
        {
            Assert.IsInstanceOf<List<FlexibilityLevelModel>>(_controller.GetAll());
        }

        [TearDown]
        public void Cleanup()
        {
            _controller = null;
            _mapper = null;
        }

        public void Dispose()
        {
            _controller = null;
            _mapper = null;
        }
    }
}
