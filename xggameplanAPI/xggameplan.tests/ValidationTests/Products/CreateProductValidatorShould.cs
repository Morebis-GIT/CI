using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using xggameplan.Model;
using xggameplan.Validations;

namespace xggameplan.tests.ValidationTests
{
    [TestFixture(Category = "Validations :: CreateProduct")]
    public class CreateProductValidatorShould
    {
        private CreateProductValidator _target;
        private Fixture _fixture;
        private CreateProduct _model;

        [OneTimeSetUp]
        public async Task OnInit()
        {
            _fixture = new Fixture();
            AssumeTargetIsInitialized();
        }

        [OneTimeTearDown]
        public async Task OnDestroy() => CleanUp();

        [SetUp]
        public async Task BeforeEach() => AssumeDefaultsAreSetup();

        [Test]
        public async Task PassWhenProvidedValidModel()
        {
            var validationResult = _target.Validate(_model);

            _ = validationResult.IsValid.Should().BeTrue();
            _ = validationResult.Errors.Count.Should().Be(0);
        }

        [Test]
        [TestCase(nameof(CreateProduct.Externalidentifier))]
        [TestCase(nameof(CreateProduct.Name))]
        [TestCase(nameof(CreateProduct.EffectiveStartDate))]
        [TestCase(nameof(CreateProduct.EffectiveEndDate))]
        [TestCase(nameof(CreateProduct.ClashCode))]
        [TestCase(nameof(CreateProduct.AdvertiserIdentifier))]
        [TestCase(nameof(CreateProduct.AdvertiserName))]
        public async Task FailWhenPropertyIsEmpty(string propertyName)
        {
            var property = _model.GetType().GetProperty(propertyName);
            property.SetValue(_model, default);

            var validationResult = _target.Validate(_model);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test]
        public async Task FailWhenDatesAreOverlapping()
        {
            _model.EffectiveStartDate = DateTime.MaxValue;

            var validationResult = _target.Validate(_model);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        protected virtual void AssumeTargetIsInitialized()
        {
            var modelValidator = new CreateProductValidation();
            _target = new CreateProductValidator(modelValidator);
        }

        protected void AssumeDefaultsAreSetup()
        {
            AssumeValidModelIsSupplied();
        }

        protected void AssumeValidModelIsSupplied()
        {
            _model = CreateValidModel();
        }

        protected virtual CreateProduct CreateValidModel()
        {
            var random = new Random();

            return _fixture.Build<CreateProduct>()
                           .With(a => a.Externalidentifier, random.Next(100000, 999999).ToString())
                           .With(a => a.Name, "test name")
                           .With(a => a.EffectiveStartDate, DateTime.Now)
                           .With(a => a.EffectiveEndDate, DateTime.Now.AddDays(1))
                           .With(a => a.ClashCode, "test clash")
                           .With(a => a.AdvertiserIdentifier, random.Next(10000, 99999).ToString())
                           .With(a => a.AdvertiserName, "test name")
                           .Create();
        }

        protected void CleanUp()
        {
            _fixture = null;
            _target = null;
        }
    }
}
