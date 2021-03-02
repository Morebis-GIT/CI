using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Results;
using AutoFixture;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using Moq;
using NUnit.Framework;
using xggameplan.Controllers;
using xggameplan.core.Configuration;
using xggameplan.model.External;
using xggameplan.Model;
using xggameplan.Services;
using xggameplan.Validations;

namespace xggameplan.tests.ControllerTests
{
    [TestFixture(Category = "Controllers :: Products")]
    public class ProductsControllerTests : IDisposable
    {
        private Mock<IProductRepository> _fakeProductRepository;
        private Mock<IDataChangeValidator> _fakeDataChangeValidator;
        private Fixture _fixture;
        private IMapper _mapper;

        private ProductController _controller;
        private CreateProduct _validProduct;
        private IModelDataValidator<CreateProduct> _productValidator;

        [SetUp]
        public void Init()
        {
            _fakeProductRepository = new Mock<IProductRepository>();
            _fakeDataChangeValidator = new Mock<IDataChangeValidator>();

            _fixture = new Fixture();

            _mapper = AutoMapperInitializer.Initialize(Configuration.AutoMapperConfig.SetupAutoMapper);

            _productValidator = new CreateProductValidator(new CreateProductValidation());

            _controller = new ProductController(
                _fakeProductRepository.Object,
                _fakeDataChangeValidator.Object,
                _mapper,
                _productValidator,
                NodaTime.SystemClock.Instance);

            _validProduct = CreateValidModel();
        }

        [Test]
        [Description("Given valid CreateProduct model when Posting then result must be successful")]
        public void UpsertProductByExternalIdValidProductWithNewExtNumberReturnsOk() => Assert.IsInstanceOf<OkNegotiatedContentResult<ProductModel>>(_controller.Put(_validProduct.Externalidentifier, _validProduct));

        [Test]
        [Description("Given valid CreateProduct model when Updating existing then model property must be updated")]
        public void UpsertProductByExternalIdValidProductUpdatesProperty()
        {
            var product = _mapper.Map<Product>(_validProduct);
            _ = _fakeProductRepository.Setup(r => r.FindByExternal(product.Externalidentifier)).Returns(new List<Product> { product });
            _validProduct.AdvertiserName = "new advertiser name";

            var result = _controller.Put(_validProduct.Externalidentifier, _validProduct) as OkNegotiatedContentResult<ProductModel>;

            Assert.AreEqual(_validProduct.AdvertiserName, result.Content.AdvertiserName);
        }

        [Test]
        [Description("Given CreateProduct with not valid externalId when Posting then correct validation message must be returned")]
        public void UpsertProductByExternalIdNotValidProductExtNumberCorrectMessageReturns()
        {
            var result = _controller.Put(_validProduct.Externalidentifier + "value", _validProduct) as InvalidModelStateResult;

            Assert.IsTrue(!result.ModelState.IsValid &&
                          result.ModelState.ContainsKey(nameof(CreateProduct.Externalidentifier)) &&
                          result.ModelState.Values.Any(v => v.Errors.Any(r => r.ErrorMessage == "External Id does not match")));
        }

        [Test]
        [Description("Given not valid CreateProduct when Posting then correct validation message must be returned")]
        [TestCase(nameof(CreateProduct.Name), "Name is missing")]
        [TestCase(nameof(CreateProduct.EffectiveStartDate), "Effective start date is missing")]
        [TestCase(nameof(CreateProduct.EffectiveEndDate), "Effective end date is missing")]
        [TestCase(nameof(CreateProduct.ClashCode), "Clash code is missing")]
        [TestCase(nameof(CreateProduct.AdvertiserIdentifier), "Advertiser identifier is missing")]
        [TestCase(nameof(CreateProduct.AdvertiserName), "Advertiser name is missing")]
        public void UpsertProductByExternalIdProductWithMissingFieldCorrectMessageReturns(string propertyName, string message)
        {
            var property = _validProduct.GetType().GetProperty(propertyName);
            property.SetValue(_validProduct, default);

            var result = _controller.Put(_validProduct.Externalidentifier, _validProduct) as ResponseMessageResult;

            Assert.IsTrue(result.Response.StatusCode == HttpStatusCode.BadRequest);

            var errors = (result.Response.Content as ObjectContent<IEnumerable<ErrorModel>>)?.Value as IEnumerable<ErrorModel>;

            Assert.IsTrue(errors.Any(err => err.ErrorField == propertyName && err.ErrorMessage == message));
        }

        [Test]
        [Description("Given not valid CreateProduct when Posting then correct validation message must be returned")]
        public void UpsertProductByExternalIdProductWithInvalidDatesRangeCorrectMessageReturns()
        {
            _validProduct.EffectiveStartDate = DateTime.MaxValue;
            var result = _controller.Put(_validProduct.Externalidentifier, _validProduct) as ResponseMessageResult;

            Assert.IsTrue(result.Response.StatusCode == HttpStatusCode.BadRequest);

            var errors = (result.Response.Content as ObjectContent<IEnumerable<ErrorModel>>)?.Value as IEnumerable<ErrorModel>;

            Assert.IsTrue(errors.Any(err => err.ErrorMessage == "Effective start/end dates are overlapping"));
        }

        [TearDown]
        public void Cleanup()
        {
            _controller = null;
            _mapper = null;
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

        public void Dispose()
        {
            _controller = null;
            _mapper = null;
        }
    }
}
