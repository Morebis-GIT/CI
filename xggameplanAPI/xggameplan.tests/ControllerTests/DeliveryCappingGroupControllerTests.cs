using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http.Results;
using AutoFixture;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.DeliveryCappingGroup;
using ImagineCommunications.GamePlan.Domain.Runs;
using Moq;
using NUnit.Framework;
using xggameplan.Controllers;
using xggameplan.core.Configuration;
using xggameplan.Model;
using xggameplan.Validations;
using xggameplan.Validations.DeliveryCappingGroup;

namespace xggameplan.tests.ControllerTests
{
    [TestFixture(Category = "Controllers :: DeliveryCappingGroup")]
    public class DeliveryCappingGroupControllerTests : IDisposable
    {
        private Mock<IDeliveryCappingGroupRepository> _fakeRepository;
        private Mock<IRunRepository> _fakeRunRepository;
        private IMapper _mapper;
        private Fixture _fixture;

        private DeliveryCappingGroupController _controller;
        private IModelDataValidator<DeliveryCappingGroupModel> _validator;

        private DeliveryCappingGroupModel _validModel;
        private IEnumerable<DeliveryCappingGroup> _deliveryCappingGroups;

        [SetUp]
        public void Init()
        {
            _fakeRepository = new Mock<IDeliveryCappingGroupRepository>();
            _fakeRunRepository = new Mock<IRunRepository>();
            _mapper = AutoMapperInitializer.Initialize(Configuration.AutoMapperConfig.SetupAutoMapper);
            _fixture = new Fixture();

            _validModel = CreateValidModel();
            _deliveryCappingGroups = CreateDeliveryCappingGroups(2);

            _validator = new DeliveryCappingGroupValidator(DeliveryCappingGroupValidationAutofacRegistration.GetValidation(_fakeRepository.Object));
            _controller = new DeliveryCappingGroupController(_fakeRepository.Object, _fakeRunRepository.Object, _validator, _mapper);
        }

        [Test]
        [Description("When getting by id delivery capping group then result must be successful")]
        public void GetByIdShouldReturnOk()
        {
            _ = _fakeRepository
                .Setup(r => r.Get(It.IsAny<int>()))
                .Returns(_deliveryCappingGroups.First());

            Assert.IsInstanceOf<OkNegotiatedContentResult<DeliveryCappingGroupModel>>(_controller.Get(1));
        }

        [Test]
        [Description("When getting delivery capping group then result must be successful")]
        public void GetAllShouldReturnOk()
        {
            _ = _fakeRepository
                .Setup(r => r.GetAll())
                .Returns(_deliveryCappingGroups);

            Assert.IsInstanceOf<OkNegotiatedContentResult<IEnumerable<DeliveryCappingGroupModel>>>(_controller.Get());
        }

        [Test]
        [Description("When send valid delivery capping group then result must be successful")]
        public void PostValidModelShouldReturnOk()
        {
            Assert.IsInstanceOf<OkNegotiatedContentResult<DeliveryCappingGroupModel>>(_controller.Post(_validModel));
        }

        [Test]
        [Description("When send invalid group deliviry cappings then result must be 'bad request'")]
        [TestCase(nameof(DeliveryCappingGroupModel.Description), default)]
        [TestCase(nameof(DeliveryCappingGroupModel.Percentage), 0)]
        [TestCase(nameof(DeliveryCappingGroupModel.Percentage), 1000)]
        public void PostInvalidModelShouldReturnOk(string propertyName, object value)
        {
            var model = _validModel;
            var property = model.GetType().GetProperty(propertyName);
            property.SetValue(model, value);

            var result = _controller.Post(model);
            Assert.IsInstanceOf<ResponseMessageResult>(result);

            var statusCode = (result as ResponseMessageResult)?.Response.StatusCode;
            Assert.AreEqual(HttpStatusCode.BadRequest, statusCode);
        }

        [Test]
        [Description("When send valid delivery capping group then result must be successful")]
        public void PutValidModelShouldReturnOk()
        {
            var entity = _deliveryCappingGroups.First();
            var model = _mapper.Map<DeliveryCappingGroupModel>(entity);

            _ = _fakeRepository
                .Setup(r => r.Get(It.IsAny<int>()))
                .Returns(entity);

            Assert.IsInstanceOf<OkNegotiatedContentResult<DeliveryCappingGroupModel>>(_controller.Put(model.Id, model));
        }

        [Test]
        [Description("When send valid delivery capping group, but different ids, then result must be 'invalid model state result'")]
        public void PutDifferentIdsShouldReturnBadRequest()
        {
            var entity = _deliveryCappingGroups.First();
            var model = _mapper.Map<DeliveryCappingGroupModel>(entity);

            Assert.IsInstanceOf<InvalidModelStateResult>(_controller.Put(0, model));
        }

        [Test]
        [Description("When send invalid delivery capping group then result must be 'bad request'")]
        [TestCase(nameof(DeliveryCappingGroupModel.Description), default)]
        [TestCase(nameof(DeliveryCappingGroupModel.Percentage), 0)]
        [TestCase(nameof(DeliveryCappingGroupModel.Percentage), 1000)]
        public void PutInvalidModelShouldReturnBadRequest(string propertyName, object value)
        {
            var model = _validModel;
            var property = model.GetType().GetProperty(propertyName);
            property.SetValue(model, value);

            var result = _controller.Put(model.Id, model);
            Assert.IsInstanceOf<ResponseMessageResult>(result);

            var statusCode = (result as ResponseMessageResult)?.Response.StatusCode;
            Assert.AreEqual(HttpStatusCode.BadRequest, statusCode);
        }

        [Test]
        [Description("Given valid id when deleting delivery capping group then result must be successful")]
        public void DeleteWhenCalledWithValidIdShouldReturnNoContent()
        {
            var entity = _deliveryCappingGroups.First();
            _ = _fakeRepository
                .Setup(r => r.Get(It.IsAny<int>()))
                .Returns(entity);

            var result = _controller.Delete(entity.Id);
            Assert.IsInstanceOf<StatusCodeResult>(result);
            var statusCode = (result as StatusCodeResult)?.StatusCode;
            Assert.AreEqual(HttpStatusCode.NoContent, statusCode);
        }

        [Test]
        [Description("Given not valid id when deleting delivery capping group then bad request status must be returned")]
        public void DeleteWhenCalledWithNotValidIdShouldReturnBadRequest()
        {
            var result = _controller.Delete(-1);
            Assert.IsInstanceOf<InvalidModelStateResult>(result);
        }

        [Test]
        [Description("Given nonexistent id when deleting delivery capping group then not found status must be returned")]
        public void DeleteWhenCalledWithNonExistentIdShouldReturnNotFound()
        {
            var result = _controller.Delete(1);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [TearDown]
        public void Cleanup()
        {
            _controller = null;
        }

        private DeliveryCappingGroupModel CreateValidModel()
        {
            var random = new Random();

            return _fixture.Build<DeliveryCappingGroupModel>()
                .With(a => a.Id, 1)
                .With(a => a.Description, "test name")
                .With(a => a.Percentage, random.Next(1, 999))
                .With(a => a.ApplyToPrice, true)
                .Create();
        }

        private IEnumerable<DeliveryCappingGroup> CreateDeliveryCappingGroups(int count)
        {
            var random = new Random();

            return _fixture.Build<DeliveryCappingGroup>()
                .With(a => a.Id, 1)
                .With(a => a.Description, "test name")
                .With(a => a.Percentage, random.Next(1, 999))
                .With(a => a.ApplyToPrice, true)
                .CreateMany(count);
        }

        public void Dispose()
        {
            _controller = null;
            _mapper = null;
        }
    }
}
