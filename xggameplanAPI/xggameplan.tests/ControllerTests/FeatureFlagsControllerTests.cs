using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Results;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using xggameplan.Configuration;
using xggameplan.Controllers;
using xggameplan.core.Configuration;
using xggameplan.core.FeatureManagement.Interfaces;
using xggameplan.model.External;

namespace xggameplan.tests.ControllerTests
{
    [TestFixture(Category = "Controllers :: Features")]
    public class FeatureFlagsControllerTests : IDisposable
    {
        private Mock<IFeatureManager> _fakeFeatureManager;

        private FeatureFlagsController _controller;
        private IMapper _mapper;
        private Fixture _fixture;

        [SetUp]
        public void Init()
        {
            _fakeFeatureManager = new Mock<IFeatureManager>();
            _fixture = new Fixture();
            _mapper = AutoMapperInitializer.Initialize(AutoMapperConfig.SetupAutoMapper);

            _controller = new FeatureFlagsController(_fakeFeatureManager.Object, _mapper);
        }

        [Test]
        [Description("When getting feature state then result must be successful")]
        public void GetAllWhenCalledThenShouldReturnOk()
        {
            _ = _fakeFeatureManager.Setup(r => r.Features).Returns(CreateFeaturesState());

            _ = _controller.Get().GetType().Should().Be<OkNegotiatedContentResult<List<FeatureStateModel>>>();
        }

        [Test]
        [Description("Given not shared features when getting feature state then zero results must be returned")]
        public void GetAllWhenCalledWithNotSharedFeaturesThenResultsShouldBeEmpty()
        {
            _ = _fakeFeatureManager.Setup(r => r.Features).Returns(CreateFeaturesState(10, false));

            var results = _controller.Get() as OkNegotiatedContentResult<List<FeatureStateModel>>;

            _ = results.Should().NotBeNull();
            _ = results.Content.Should().BeEmpty();
        }

        private IReadOnlyCollection<IFeatureFlag> CreateFeaturesState(int count = 10, bool isShared = true)
        {
            _fixture.Register(() => new FakeFeatureFlag
            {
                Name = _fixture.Create<string>(),
                IsEnabled = true,
                IsShared = isShared
            });

            return _fixture.CreateMany<FakeFeatureFlag>().ToArray();
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

        private class FakeFeatureFlag : IFeatureFlag
        {
            public string Name { get; set; }
            public bool IsShared { get; set; }
            public bool IsEnabled { get; set; }
            public IReadOnlyCollection<IFeatureFlag> DependsOn { get; set; }
        }
    }
}
