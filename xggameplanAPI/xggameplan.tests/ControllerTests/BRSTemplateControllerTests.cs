using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http.Results;
using AutoFixture;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.BRS;
using Moq;
using NUnit.Framework;
using xggameplan.Configuration;
using xggameplan.Controllers;
using xggameplan.core.Configuration;
using xggameplan.core.Helpers;
using xggameplan.model.External;
using xggameplan.Validations;
using xggameplan.Validations.BRS;

namespace xggameplan.tests.ControllerTests
{
    [TestFixture(Category = "Controllers :: BRSTemplate")]
    public class BRSTemplateControllerTests : IDisposable
    {
        private Mock<IBRSConfigurationTemplateRepository> _fakeRepository;
        private Mock<IKPIPriorityRepository> _kpiFakeRepository;
        private Fixture _fixture;
        private IMapper _mapper;

        private BRSTemplatesController _controller;
        private IModelDataValidator<CreateOrUpdateBRSConfigurationTemplateModel> _validator;
        private IEnumerable<CreateOrUpdateBRSConfigurationTemplateModel> _validModels;

        [SetUp]
        public void Init()
        {
            _fakeRepository = new Mock<IBRSConfigurationTemplateRepository>();
            _kpiFakeRepository = new Mock<IKPIPriorityRepository>();
            _fixture = new Fixture();

            _mapper = AutoMapperInitializer.Initialize(Configuration.AutoMapperConfig.SetupAutoMapper);

            _validModels = CreateValidModels(1);

            _ = _fakeRepository
                .Setup(r => r.Get(It.Is<int>(x => x > 0)))
                .Returns(CreateRepositoryData().First());

            _ = _fakeRepository
                .Setup(r => r.GetAll())
                .Returns(CreateRepositoryData(3));

            _ = _kpiFakeRepository
                .Setup(x => x.GetAll())
                .Returns(GetKPIPriorities());

            _ = AutoMapperInitializer.Initialize(AutoMapperConfig.SetupAutoMapper);

            _validator = new CreateOrUpdateBRSConfigurationTemplateValidator(new CreateOrUpdateBRSConfigurationTemplateValidation(_kpiFakeRepository.Object, _fakeRepository.Object));
            _controller = new BRSTemplatesController(_fakeRepository.Object, _validator, _mapper);
        }

        [Test]
        [Description("When getting by id then result must be successful")]
        public void GetByIdShouldReturnOk()
        {
            Assert.IsInstanceOf<OkNegotiatedContentResult<BRSConfigurationTemplateModel>>(_controller.Get(1));
        }

        [Test]
        [Description("When getting by invalid id then result must be Not Found")]
        public void GetByIdShouldReturnNotFound()
        {
            Assert.IsInstanceOf<NotFoundResult>(_controller.Get(-1));
        }

        [Test]
        [Description("When getting all then result must be successful")]
        public void GetAllShouldReturnOk()
        {
            Assert.IsInstanceOf<OkNegotiatedContentResult<List<BRSConfigurationTemplateModel>>>(_controller.GetAll());
        }

        [Test]
        [Description("When send valid model then result must be successful")]
        public void PostValidModelShouldReturnOk()
        {
            Assert.IsInstanceOf<OkNegotiatedContentResult<BRSConfigurationTemplateModel>>(_controller.Post(_validModels.First()));
        }

        [Test]
        [Description("When send invalid model then result must be Bad Request")]
        public void PostInvalidModelShouldReturnBadRequest()
        {
            var model = _validModels.First();
            model.Name = null;
            var result = _controller.Post(model);
            var statusCode = (result as ResponseMessageResult)?.Response.StatusCode;
            Assert.AreEqual(HttpStatusCode.BadRequest, statusCode);
        }

        [Test]
        [Description("When update valid model then result must be successful")]
        public void PutValidModelShouldReturnOk()
        {
            var entity = CreateRepositoryData().First();
            var item = _validModels.First();
            item.Id = entity.Id;
            _ = _fakeRepository
                .Setup(r => r.Get(It.IsAny<int>()))
                .Returns(entity);
            Assert.IsInstanceOf<OkNegotiatedContentResult<BRSConfigurationTemplateModel>>(_controller.Put(item.Id, item));
        }

        [Test]
        [Description("When update invalid model then result must be Bad Request")]
        public void PutInvalidModelShouldReturnBadRequest()
        {
            var item = _validModels.First();
            Assert.IsInstanceOf<InvalidModelStateResult>(_controller.Put(-1, item));
        }

        [Test]
        [Description("When update not exists entity then result must be Not Found")]
        public void PutNotExistsEntityShouldReturnNotFound()
        {
            var item = _validModels.First();
            _ = _fakeRepository
                .Setup(r => r.Get(It.IsAny<int>()))
                .Returns(null as BRSConfigurationTemplate);
            Assert.IsInstanceOf<NotFoundResult>(_controller.Put(item.Id, item));
        }

        [Test]
        [Description("When delete exists entity by Id then result must be successful (no content)")]
        public void DeleteExistsEntityByIdShouldReturnOk()
        {
            var entity = CreateRepositoryData().First();
            var result = _controller.Delete(entity.Id);
            Assert.IsInstanceOf<StatusCodeResult>(result);
            var statusCode = (result as StatusCodeResult)?.StatusCode;
            Assert.AreEqual(HttpStatusCode.NoContent, statusCode);
        }

        [Test]
        [Description("When delete not exists entity by Id then result must be Not Found")]
        public void DeleteNotExistsEntityByIdShouldReturnNotFound()
        {
            _ = _fakeRepository
                .Setup(r => r.Get(It.IsAny<int>()))
                .Returns(null as BRSConfigurationTemplate);
            Assert.IsInstanceOf<NotFoundResult>(_controller.Delete(1));
        }

        [TearDown]
        public void Cleanup()
        {
            _controller = null;
        }

        private IEnumerable<CreateOrUpdateBRSConfigurationTemplateModel> CreateValidModels(int count)
        {
            var random = new Random();

            return _fixture.Build<CreateOrUpdateBRSConfigurationTemplateModel>()
                .With(a => a.Id, 1)
                .With(a => a.Name, "test name")
                .With(a => a.KPIConfigurations, BRSHelper.KPIs.Select(x => new BRSConfigurationForKPIModel
                {
                    KPIName = x,
                    PriorityId = random.Next(2, 6)
                }).ToList())
                .CreateMany(count);
        }

        private IEnumerable<BRSConfigurationTemplate> CreateRepositoryData(int count = 1)
        {
            var random = new Random();

            return _fixture.Build<BRSConfigurationTemplate>()
                .With(a => a.Id, 1)
                .With(a => a.Name, "test name")
                .With(a => a.IsDefault, false)
                .With(a => a.KPIConfigurations, BRSHelper.KPIs.Select(x => new BRSConfigurationForKPI
                {
                    KPIName = x,
                    PriorityId = random.Next(2, 6)
                }).ToList())
                .CreateMany(count);
        }

        private List<KPIPriority> GetKPIPriorities()
        {
            return new List<KPIPriority>()
            {
                new KPIPriority { Id = 1, WeightingFactor = 0 },
                new KPIPriority { Id = 2, WeightingFactor = 0.3 },
                new KPIPriority { Id = 3, WeightingFactor = 0.7 },
                new KPIPriority { Id = 4, WeightingFactor = 1 },
                new KPIPriority { Id = 5, WeightingFactor = 1.3 },
                new KPIPriority { Id = 6, WeightingFactor = 1.7 },
            };
        }

        public void Dispose()
        {
            _controller = null;
        }
    }
}
