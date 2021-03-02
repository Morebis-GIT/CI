using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Results;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using ImagineCommunications.GamePlan.Domain.AnalysisGroups;
using ImagineCommunications.GamePlan.Domain.AnalysisGroups.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.System.Users;
using Moq;
using NUnit.Framework;
using xggameplan.Configuration;
using xggameplan.Controllers;
using xggameplan.core.Configuration;
using xggameplan.Extensions;
using xggameplan.Model;
using xggameplan.Validations;
using xggameplan.Validations.AnalysisGroups;

namespace xggameplan.tests.ControllerTests
{
    [TestFixture(Category = "Controllers :: Features")]
    public class AnalysisGroupsControllerTests : IDisposable
    {
        private const string TestAdvertiserId = "TestAvertiserId";

        private Mock<IAnalysisGroupRepository> _mockAnalysisGroupRepository;
        private Mock<ICampaignRepository> _mockCampaignRepository;
        private Mock<IClashRepository> _mockClashRepository;
        private Mock<IProductRepository> _mockProductRepository;
        private Mock<IUsersRepository> _mockUsersRepository;
        private IModelDataValidator<CreateAnalysisGroupModel> _validator;
        private IMapper _mapper;
        private Fixture _fixture;

        private AnalysisGroupsController _controller;

        [SetUp]
        public void Init()
        {
            _mockAnalysisGroupRepository = new Mock<IAnalysisGroupRepository>();
            _mockCampaignRepository = new Mock<ICampaignRepository>();
            _mockClashRepository = new Mock<IClashRepository>();
            _mockProductRepository = new Mock<IProductRepository>();
            _mockUsersRepository = new Mock<IUsersRepository>();
            _mapper = AutoMapperInitializer.Initialize(AutoMapperConfig.SetupAutoMapper);
            _fixture = new Fixture();

            _validator = new CreateAnalysisGroupModelValidator(new CreateAnalysisGroupModelValidation(_mockAnalysisGroupRepository.Object,
                _mockCampaignRepository.Object,
                _mockClashRepository.Object, _mockProductRepository.Object, _mapper));

            var request = new HttpRequestMessage();
            request.SetConfiguration(new HttpConfiguration());

            _controller = new AnalysisGroupsController(_mockAnalysisGroupRepository.Object,
                _mockCampaignRepository.Object,
                _mockClashRepository.Object,
                _mockProductRepository.Object,
                _mockUsersRepository.Object,
                _validator,
                null,
                _mapper)
            {
                ControllerContext = new HttpControllerContext { Request = request }
            };

            HttpContext.Current = new HttpContext(
                new HttpRequest("", "http://test.com", ""),
                new HttpResponse(new StringWriter())
            );
            HttpContext.Current.SetCurrentUser(_fixture.Create<User>());

            _ = _mockProductRepository
                .Setup(x => x.GetAdvertisers(It.IsAny<ICollection<string>>()))
                .Returns(new List<ProductAdvertiserModel>{new ProductAdvertiserModel
                {
                    AdvertiserIdentifier = TestAdvertiserId,
                    AdvertiserName = "some advertiser"
                }});
        }

        [Test]
        [Description("When getting analysis groups then result must be successful")]
        public void GetAllWhenCalledThenShouldReturnOk()
        {
            _ = _controller.Get().GetType().Should().Be<OkNegotiatedContentResult<List<AnalysisGroupModel>>>();
        }

        [Test]
        [Description("When getting analysis groups by id then result must be successful")]
        public void GetByIdShouldReturnOk()
        {
            _ = _mockAnalysisGroupRepository
                .Setup(r => r.Get(It.IsAny<int>()))
                .Returns(_fixture.Create<AnalysisGroup>());

            _ = _controller.Get(1).GetType().Should().Be<OkNegotiatedContentResult<AnalysisGroupModel>>();
        }

        [Test]
        [Description("Given nonexistent id when getting analysis group by id then result must be NotFound")]
        public void GetByIdWhenCalledWithNonExistentIdShouldReturnNotFound()
        {
            var result = _controller.Get(1);
            var statusCode = (result as ResponseMessageResult)?.Response.StatusCode;

            _ = result.GetType().Should().Be<ResponseMessageResult>();
            _ = statusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        [Description("Given valid model when creating analysis group then result must be successful")]
        public void PostValidModelShouldReturnOk()
        {
            _ = _controller.Post(CreateValidApiModel()).GetType().Should().Be<OkNegotiatedContentResult<AnalysisGroupModel>>();
        }

        [Test]
        [Description("Given invalid model when creating analysis group then result must be BadRequest")]
        public void PostInvalidModelShouldReturnBadRequest()
        {
            var model = CreateValidApiModel();
            model.Name = null;

            var result = _controller.Post(model);
            var statusCode = (result as ResponseMessageResult)?.Response.StatusCode;

            _ = result.GetType().Should().Be<ResponseMessageResult>();
            _ = statusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        [Description("Given valid model when updating analysis group then result must be successful")]
        public void PutValidModelShouldReturnOk()
        {
            var entity = CreateValidDomainModel();
            var model = CreateValidApiModel();
            model.Id = entity.Id;

            _ = _mockAnalysisGroupRepository
                .Setup(r => r.Get(It.IsAny<int>()))
                .Returns(entity);

            _ = _controller.Put(model.Id, model).GetType().Should().Be<OkNegotiatedContentResult<AnalysisGroupModel>>();
        }

        [Test]
        [Description("Given model with invalid name when updating analysis group then result must be BadRequest")]
        public void PutInvalidNameShouldReturnBadRequest()
        {
            var entity = CreateValidDomainModel();
            var model = CreateValidApiModel();
            model.Id = entity.Id;
            model.Name = null;

            _ = _mockAnalysisGroupRepository
                .Setup(r => r.Get(It.IsAny<int>()))
                .Returns(entity);

            var result = _controller.Put(model.Id, model);
            var statusCode = (result as ResponseMessageResult)?.Response.StatusCode;

            _ = result.GetType().Should().Be<ResponseMessageResult>();
            _ = statusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        [Description("Given model with invalid advertiser filter when updating analysis group then result must be BadRequest")]
        public void PutInvalidAdvertiserShouldReturnBadRequest()
        {
            var entity = CreateValidDomainModel();
            var model = CreateValidApiModel();
            model.Id = entity.Id;
            model.Filter.AdvertiserExternalIds = new HashSet<string> { "nonexistent" };

            _ = _mockAnalysisGroupRepository
                .Setup(r => r.Get(It.IsAny<int>()))
                .Returns(entity);

            var result = _controller.Put(model.Id, model);
            var statusCode = (result as ResponseMessageResult)?.Response.StatusCode;

            _ = result.GetType().Should().Be<ResponseMessageResult>();
            _ = statusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        [Description("Given valid id when deleting analysis group then result must be successful")]
        public void DeleteWhenCalledWithValidIdShouldReturnNoContent()
        {
            var domainModel = CreateValidDomainModel();
            _ = _mockAnalysisGroupRepository
                .Setup(r => r.Get(It.IsAny<int>()))
                .Returns(domainModel);

            var result = _controller.Delete(domainModel.Id);
            var statusCode = (result as StatusCodeResult)?.StatusCode;

            _ = result.GetType().Should().Be<StatusCodeResult>();
            _ = statusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Test]
        [Description("Given negative id when deleting analysis group then bad request status must be returned")]
        public void DeleteWhenCalledWithNegativeIdShouldReturnBadRequest()
        {
            var result = _controller.Delete(-1);
            var statusCode = (result as ResponseMessageResult)?.Response.StatusCode;

            _ = result.GetType().Should().Be<ResponseMessageResult>();
            _ = statusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        [Description("Given nonexistent id when deleting analysis group group then not found status must be returned")]
        public void DeleteWhenCalledWithNonExistentIdShouldReturnNotFound()
        {
            var result = _controller.Delete(1);
            var statusCode = (result as ResponseMessageResult)?.Response.StatusCode;

            _ = result.GetType().Should().Be<ResponseMessageResult>();
            _ = statusCode.Should().Be(HttpStatusCode.NotFound);
        }

        private AnalysisGroup CreateValidDomainModel() =>
            _fixture.Build<AnalysisGroup>()
                .With(x => x.Name, "Test name")
                .With(x => x.Filter, _fixture.Build<AnalysisGroupFilter>().Create())
                .Create();

        private CreateAnalysisGroupModel CreateValidApiModel(string advertiserName = null)
        {
            var filter = _fixture.Build<CreateAnalysisGroupFilterModel>()
                .OmitAutoProperties()
                .With(x => x.AdvertiserExternalIds, new HashSet<string> { advertiserName ?? TestAdvertiserId })
                .Create();

            return _fixture.Build<CreateAnalysisGroupModel>()
                .With(x => x.Name, "Test name")
                .With(x => x.Filter, filter)
                .Create();
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
