using System;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using FluentAssertions.Execution;
using ImagineCommunications.GamePlan.Domain.DeliveryCappingGroup;
using Moq;
using NUnit.Framework;
using xggameplan.core.Configuration;
using xggameplan.Model;
using xggameplan.Validations.DeliveryCappingGroup;

namespace xggameplan.tests.ValidationTests
{
    [TestFixture(Category = "Validations :: DeliveryCappingGroupValidation")]
    public class DeliveryCappingGroupValidationShould
    {
        private DeliveryCappingGroupValidation _validation;
        private Fixture _fixture;
        private DeliveryCappingGroupModel _model;
        private Mock<IDeliveryCappingGroupRepository> _repositoryMock;
        private IMapper _mapper;

        [OneTimeSetUp]
        public void OnInit()
        {
            _fixture = new Fixture();
            _mapper = AutoMapperInitializer.Initialize(Configuration.AutoMapperConfig.SetupAutoMapper);

            AssumeTargetIsInitialized();
        }

        [OneTimeTearDown]
        public void OnDestroy() => CleanUp();

        [SetUp]
        public void BeforeEach() => AssumeDefaultsAreSetup();

        [Test]
        public void PassWhenProvidedValidModel()
        {
            var validationResult = _validation.Validate(_model);

            _ = validationResult.IsValid.Should().BeTrue();
            _ = validationResult.Errors.Count.Should().Be(0);
        }

        [Test]
        [TestCase(nameof(DeliveryCappingGroupModel.Percentage))]
        [TestCase(nameof(DeliveryCappingGroupModel.Description))]
        public void FailWhenPropertyIsEmpty(string propertyName)
        {
            var property = _model.GetType().GetProperty(propertyName);
            property.SetValue(_model, default);

            var validationResult = _validation.Validate(_model);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test]
        [TestCase(0)]
        [TestCase(1000)]
        public void FailWhenInvalidPercentagesValue(int invalidPercentagesValue)
        {
            _model.Percentage = invalidPercentagesValue;

            var validationResult = _validation.Validate(_model);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test]
        public void PassWhenDuplicatedNameAndSameIds()
        {
            _ = _repositoryMock.Setup(x => x.GetByDescription(It.IsAny<string>()))
                .Returns(_mapper.Map<DeliveryCappingGroup>(_model));

            var validationResult = _validation.Validate(_model);

            _ = validationResult.IsValid.Should().BeTrue();
            _ = validationResult.Errors.Count.Should().Be(0);
        }

        [Test]
        public void FailWhenDuplicatedNameAndDifferentIds()
        {
            _ = _repositoryMock.Setup(x => x.GetByDescription(It.IsAny<string>()))
                .Returns(() =>
                {
                    var value = _mapper.Map<DeliveryCappingGroup>(_model);
                    value.Id = -1;
                    return value;
                });

            var validationResult = _validation.Validate(_model);

            _ = validationResult.IsValid.Should().BeFalse();
            _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
        }

        protected virtual void AssumeTargetIsInitialized()
        {
            _repositoryMock = new Mock<IDeliveryCappingGroupRepository>();
            _validation = DeliveryCappingGroupValidationAutofacRegistration.GetValidation(_repositoryMock.Object);
        }

        protected void AssumeDefaultsAreSetup()
        {
            AssumeValidModelIsSupplied();
        }

        protected void AssumeValidModelIsSupplied()
        {
            _model = CreateValidModel();
        }

        protected virtual DeliveryCappingGroupModel CreateValidModel()
        {
            var random = new Random();

            return _fixture.Build<DeliveryCappingGroupModel>()
                           .With(a => a.Id, 1)
                           .With(a => a.Description, "test name")
                           .With(a => a.Percentage, random.Next(1, 999))
                           .With(a => a.ApplyToPrice, true)
                           .Create();
        }

        protected void CleanUp()
        {
            _fixture = null;
            _validation = null;
        }
    }
}
