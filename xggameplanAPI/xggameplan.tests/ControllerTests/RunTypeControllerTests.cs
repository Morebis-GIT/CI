using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using ImagineCommunications.GamePlan.CommonTestsHelpers;
using ImagineCommunications.GamePlan.Domain.AnalysisGroups;
using ImagineCommunications.GamePlan.Domain.AnalysisGroups.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.LandmarkRunQueues;
using ImagineCommunications.GamePlan.Domain.RunTypes;
using ImagineCommunications.GamePlan.Domain.RunTypes.Objects;
using Moq;
using NUnit.Framework;
using xggameplan.Controllers;
using xggameplan.core.FeatureManagement.Interfaces;
using xggameplan.model.External;
using xggameplan.Model;
using xggameplan.Profile;

namespace xggameplan.tests.ControllerTests
{
    [TestFixture(Category = "Controllers :: RunTypes")]
    public class RunTypeControllerTests
    {
        private static Fixture _fixture;
        private IMapper _mapper;
        private List<RunType> _existingRunTypes;
        private RunTypesController _controller { get; set; }
        private CreateRunTypeModel _dummyCreateRunTypeModel { get; set; }
        private Mock<IRunTypeRepository> _mockRunTypeRepository { get; set; }
        private Mock<IFeatureManager> _mockFeatureManager { get; set; }
        private Mock<IAnalysisGroupRepository> _mockAnalysisGroupRepository { get; set; }
        private Mock<ILandmarkRunQueueRepository> _mockLandmarkRunQueueRepository { get; set; }
        private List<AnalysisGroup> _dummyAnalysisGroups { get; set; }

        [SetUp]
        public void Init()
        {
            AssumeDependenciesAreSupplied();
            AssumeDependenciesAreMocked();

            _controller = new RunTypesController(
                _mockRunTypeRepository.Object,
                _mockFeatureManager.Object,
                _mapper,
                _mockAnalysisGroupRepository.Object,
                _mockLandmarkRunQueueRepository.Object);
            _controller.Request = new HttpRequestMessage();
            _controller.Configuration = new HttpConfiguration();
        }

        [Test(Description = "Given create RunType when RunType name is null or empty then correct exception is raised with the correct error message")]
        public async Task GivenCreateRunTypeWhenRunTypeNameIsNullOrEmptyThenCorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            //Arrange
            _dummyCreateRunTypeModel.Name = null;

            //Act
            var response = _controller.Post(_dummyCreateRunTypeModel);
            var result = await response.ExecuteAsync(new CancellationToken());
            var content = await result.Content.ReadAsStringAsync();

            //Assert
            _ = response.Should().BeOfType<ResponseMessageResult>(becauseArgs: null);
            _ = result.StatusCode.Should().Be(HttpStatusCode.BadRequest, becauseArgs: null);
            _ = content.Should().Contain("Name cannot be empty", becauseArgs: null);
        }

        [Test(Description = "Given create RunType when RunType name already exist then correct exception is raised with the correct error message")]
        public async Task GivenCreateRunTypeWhenRunTypeNameIsAlreadyExistThenCorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            //Arrange
            _dummyCreateRunTypeModel.Name = _existingRunTypes[0].Name;

            //Act
            var response = _controller.Post(_dummyCreateRunTypeModel);
            var result = await response.ExecuteAsync(new CancellationToken());
            var content = await result.Content.ReadAsStringAsync();

            //Assert
            _ = response.Should().BeOfType<ResponseMessageResult>(becauseArgs: null);
            _ = result.StatusCode.Should().Be(HttpStatusCode.BadRequest, becauseArgs: null);
            _ = content.Should().Contain("Run type name already exists: " + _dummyCreateRunTypeModel.Name, becauseArgs: null);
        }

        [Test(Description = "Given create RunType when RunType analysis groups have duplicate then correct exception is raised with the correct error message")]
        public async Task GivenCreateRunTypeWhenRunTypeAnalysisGroupsHaveDuplicateThenCorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            //Arrange
            _dummyCreateRunTypeModel.RunTypeAnalysisGroups = new List<CreateRunTypeAnalysisGroupModel>();
            var dummyRunTypeAnalysisGroup = _fixture
                .Build<CreateRunTypeAnalysisGroupModel>()
                .Create();
            _dummyCreateRunTypeModel.RunTypeAnalysisGroups.Add(dummyRunTypeAnalysisGroup);
            _dummyCreateRunTypeModel.RunTypeAnalysisGroups.Add(dummyRunTypeAnalysisGroup);

            //Act
            var response = _controller.Post(_dummyCreateRunTypeModel);
            var result = await response.ExecuteAsync(new CancellationToken());
            var content = await result.Content.ReadAsStringAsync();

            //Assert
            _ = response.Should().BeOfType<ResponseMessageResult>(becauseArgs: null);
            _ = result.StatusCode.Should().Be(HttpStatusCode.BadRequest, becauseArgs: null);
            _ = content.Should().Contain($"There are duplicate Analysis Group and KPI combinations: {dummyRunTypeAnalysisGroup.AnalysisGroupName} - {dummyRunTypeAnalysisGroup.KPI}", becauseArgs: null);
        }

        [Test(Description = "Given create RunType when RunType analysis groups have invalid analysis group id then correct exception is raised with the correct error message")]
        public async Task GivenCreateRunTypeWhenRunTypeAnalysisGroupsHaveInvalidAnalysisGroupIdThenCorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            //Arrange
            _ = _mockAnalysisGroupRepository.Setup(m => m.GetAll()).Returns(new List<AnalysisGroup>());

            //Act
            var response = _controller.Post(_dummyCreateRunTypeModel);
            var result = await response.ExecuteAsync(new CancellationToken());
            var content = await result.Content.ReadAsStringAsync();

            //Assert
            _ = response.Should().BeOfType<ResponseMessageResult>(becauseArgs: null);
            _ = result.StatusCode.Should().Be(HttpStatusCode.BadRequest, becauseArgs: null);
            _ = content.Should().Contain($"Analysis group \\\"{_dummyCreateRunTypeModel.RunTypeAnalysisGroups[0].AnalysisGroupName}\\\" no longer exist", becauseArgs: null);
        }

        [Test(Description = "Given create RunType when RunType analysis groups have invalid KPI then correct exception is raised with the correct error message")]
        public async Task GivenCreateRunTypeWhenRunTypeAnalysisGroupsHaveInvalidKPIThenCorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            //Arrange
            _dummyCreateRunTypeModel.RunTypeAnalysisGroups[1].KPI = "notReallyAKPI";

            //Act
            var response = _controller.Post(_dummyCreateRunTypeModel);
            var result = await response.ExecuteAsync(new CancellationToken());
            var content = await result.Content.ReadAsStringAsync();

            //Assert
            _ = response.Should().BeOfType<ResponseMessageResult>(becauseArgs: null);
            _ = result.StatusCode.Should().Be(HttpStatusCode.BadRequest, becauseArgs: null);
            _ = content.Should().Contain($"RunType-AnalysisGroup: {_dummyCreateRunTypeModel.RunTypeAnalysisGroups[1].KPI} KPI is not valid", becauseArgs: null);
        }

        [Test(Description = "Given create RunType when RunType analysis groups have empty KPI then correct exception is raised with the correct error message")]
        public async Task GivenCreateRunTypeWhenRunTypeAnalysisGroupsHaveEmptyKPIThenCorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            //Arrange
            _dummyCreateRunTypeModel.RunTypeAnalysisGroups[1].KPI = null;

            //Act
            var response = _controller.Post(_dummyCreateRunTypeModel);
            var result = await response.ExecuteAsync(new CancellationToken());
            var content = await result.Content.ReadAsStringAsync();

            //Assert
            _ = response.Should().BeOfType<ResponseMessageResult>(becauseArgs: null);
            _ = result.StatusCode.Should().Be(HttpStatusCode.BadRequest, becauseArgs: null);
            _ = content.Should().Contain("RunType-AnalysisGroup: KPI must have a value", becauseArgs: null);
        }

        [Test(Description = "Given create RunType when default KPI has invalid value then correct exception is raised with the correct error message")]
        public async Task GivenCreateRunTypeWhenDefaultKPIHasInvalidValueThenCorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            //Arrange
            _dummyCreateRunTypeModel.DefaultKPI = "notReallyAKPI";

            //Act
            var response = _controller.Post(_dummyCreateRunTypeModel);
            var result = await response.ExecuteAsync(new CancellationToken());
            var content = await result.Content.ReadAsStringAsync();

            //Assert
            _ = response.Should().BeOfType<ResponseMessageResult>(becauseArgs: null);
            _ = result.StatusCode.Should().Be(HttpStatusCode.BadRequest, becauseArgs: null);
            _ = content.Should().Contain($"Default KPI: {_dummyCreateRunTypeModel.DefaultKPI} KPI is not valid", becauseArgs: null);
        }

        [Test(Description = "Given create RunType when CreateRunTypeModel is valid then return Ok")]
        public async Task GivenCreateRunTypeWhenCreateRunTypeModelValidThenReturnOk()
        {
            //Act
            var actualResult = _controller.Post(_dummyCreateRunTypeModel);

            //Assert
            _ = actualResult.Should().BeOfType<OkNegotiatedContentResult<RunTypeModel>>(becauseArgs: null);
        }

        [Test(Description = "When getting all run types including hidden ones the total count should be 3")]
        public void GivenGetAllWhen3RunTypesExistAndOneOfThemIsHiddenThenShouldReturnListWith3RunTypes()
        {
            //Act
            var actionResult = _controller.GetAll();
            var contentResult = actionResult as OkNegotiatedContentResult<List<RunTypeModel>>;

            //Assert
            _ = contentResult.Content.Should().HaveCount(3, becauseArgs: null);
        }

        [Test(Description = "Given update RunType when id and CreateRunTypeModel are valid then return ok")]
        public void GivenUpdateRunTypeWhenIdAndCreateRunTypeModelAreValidThenReturnOk()
        {
            //Act
            var actualResult = _controller.Put(_existingRunTypes[0].Id, _dummyCreateRunTypeModel);

            //Assert
            _ = actualResult.Should().BeOfType<OkNegotiatedContentResult<RunType>>(becauseArgs: null);
        }

        [Test(Description = "Given update RunType when CreateRunTypeModel is null then correct exception is raised with the correct error message")]
        public async Task GivenUpdateRunTypeWhenCreateRunTypeModelIsNullThenCorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            //Act
            var response = _controller.Put(_existingRunTypes[0].Id, null);
            var result = await response.ExecuteAsync(new CancellationToken());

            //Assert
            _ = response.Should().BeOfType<ResponseMessageResult>(becauseArgs: null);
            _ = result.StatusCode.Should().Be(HttpStatusCode.BadRequest, becauseArgs: null);
        }

        [Test(Description = "Given update RunType when CreateRunTypeModel name is null or empty then correct exception is raised with the correct error message")]
        public async Task GivenUpdateRunTypeWhenCreateRunTypeModelNameIsNullOrEmptyThenCorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            //Arrange
            _dummyCreateRunTypeModel.Name = null;

            //Act
            var response = _controller.Put(_existingRunTypes[0].Id, _dummyCreateRunTypeModel);
            var result = await response.ExecuteAsync(new CancellationToken());
            var content = await result.Content.ReadAsStringAsync();

            //Assert
            _ = response.Should().BeOfType<ResponseMessageResult>(becauseArgs: null);
            _ = result.StatusCode.Should().Be(HttpStatusCode.BadRequest, becauseArgs: null);
            _ = content.Should().Contain("Name cannot be empty", becauseArgs: null);
        }

        [Test(Description = "Given update RunType when update target does not exist then correct exception is raised with the correct error message")]
        public async Task GivenUpdateRunTypeWhenUpdateTargetDoesNotExistThenCorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            //Act
            var response = _controller.Put(_existingRunTypes[1].Id, _dummyCreateRunTypeModel);
            var result = await response.ExecuteAsync(new CancellationToken());

            //Assert
            _ = response.Should().BeOfType<ResponseMessageResult>(becauseArgs: null);
            _ = result.StatusCode.Should().Be(HttpStatusCode.NotFound, becauseArgs: null);
        }

        [Test(Description = "Given update RunType when target name to update already exist in another record then correct exception is raised with the correct error message")]
        public async Task GivenUpdateRunTypeWhenIdAndCreateRunTypeModelAreValidButTargetNameToUpdateAlreadyExistThenCorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            //Arrange
            _dummyCreateRunTypeModel.Name = _existingRunTypes[0].Name;
            _ = _mockRunTypeRepository.Setup(m => m.Get(_existingRunTypes[1].Id)).Returns(_existingRunTypes[1]);

            //Act
            var response = _controller.Put(_existingRunTypes[1].Id, _dummyCreateRunTypeModel);
            var result = await response.ExecuteAsync(new CancellationToken());
            var content = await result.Content.ReadAsStringAsync();

            //Assert
            _ = response.Should().BeOfType<ResponseMessageResult>(becauseArgs: null);
            _ = result.StatusCode.Should().Be(HttpStatusCode.BadRequest, becauseArgs: null);
            _ = content.Should().Contain("Run type name already exists: " + _dummyCreateRunTypeModel.Name, becauseArgs: null);
        }

        [Test(Description = "Given update RunType when default KPI has invalid value then correct exception is raised with the correct error message")]
        public async Task GivenUpdateRunTypeWhenDefaultKPIHasInvalidValueThenCorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            //Arrange
            _dummyCreateRunTypeModel.DefaultKPI = "notReallyAKPI";

            //Act
            var response = _controller.Put(_existingRunTypes[0].Id, _dummyCreateRunTypeModel);
            var result = await response.ExecuteAsync(new CancellationToken());
            var content = await result.Content.ReadAsStringAsync();

            //Assert
            _ = response.Should().BeOfType<ResponseMessageResult>(becauseArgs: null);
            _ = result.StatusCode.Should().Be(HttpStatusCode.BadRequest, becauseArgs: null);
            _ = content.Should().Contain($"Default KPI: {_dummyCreateRunTypeModel.DefaultKPI} KPI is not valid", becauseArgs: null);
        }

        [Test(Description = "Given update RunType when RunType analysis groups have invalid KPI then correct exception is raised with the correct error message")]
        public async Task GivenUpdateRunTypeWhenRunTypeAnalysisGroupsHaveInvalidKPIThenCorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            //Arrange
            _dummyCreateRunTypeModel.RunTypeAnalysisGroups[1].KPI = "notReallyAKPI";
            _ = _mockRunTypeRepository.Setup(m => m.Get(_existingRunTypes[1].Id)).Returns(_existingRunTypes[1]);

            //Act
            var response = _controller.Put(_existingRunTypes[1].Id, _dummyCreateRunTypeModel);
            var result = await response.ExecuteAsync(new CancellationToken());
            var content = await result.Content.ReadAsStringAsync();

            //Assert
            _ = response.Should().BeOfType<ResponseMessageResult>(becauseArgs: null);
            _ = result.StatusCode.Should().Be(HttpStatusCode.BadRequest, becauseArgs: null);
            _ = content.Should().Contain($"RunType-AnalysisGroup: {_dummyCreateRunTypeModel.RunTypeAnalysisGroups[1].KPI} KPI is not valid", becauseArgs: null);
        }

        [Test(Description = "Given update RunType when RunType analysis groups have empty KPI then correct exception is raised with the correct error message")]
        public async Task GivenUpdateRunTypeWhenRunTypeAnalysisGroupsHaveEmptyKPIThenCorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            //Arrange
            _dummyCreateRunTypeModel.RunTypeAnalysisGroups[1].KPI = null;
            _ = _mockRunTypeRepository.Setup(m => m.Get(_existingRunTypes[1].Id)).Returns(_existingRunTypes[1]);

            //Act
            var response = _controller.Put(_existingRunTypes[1].Id, _dummyCreateRunTypeModel);
            var result = await response.ExecuteAsync(new CancellationToken());
            var content = await result.Content.ReadAsStringAsync();

            //Assert
            _ = response.Should().BeOfType<ResponseMessageResult>(becauseArgs: null);
            _ = result.StatusCode.Should().Be(HttpStatusCode.BadRequest, becauseArgs: null);
            _ = content.Should().Contain("RunType-AnalysisGroup: KPI must have a value", becauseArgs: null);
        }

        [Test(Description = "Given update RunType when RunType analysis groups have invalid analysis group id then correct exception is raised with the correct error message")]
        public async Task GivenUpdateRunTypeWhenRunTypeAnalysisGroupsHaveInvalidAnalysisGroupIdThenCorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            //Arrange
            _ = _mockAnalysisGroupRepository.Setup(m => m.GetAll()).Returns(new List<AnalysisGroup>());

            //Act
            var response = _controller.Put(_existingRunTypes[0].Id, _dummyCreateRunTypeModel);
            var result = await response.ExecuteAsync(new CancellationToken());
            var content = await result.Content.ReadAsStringAsync();

            //Assert
            _ = response.Should().BeOfType<ResponseMessageResult>(becauseArgs: null);
            _ = result.StatusCode.Should().Be(HttpStatusCode.BadRequest, becauseArgs: null);
            _ = content.Should().Contain($"Analysis group \\\"{_dummyCreateRunTypeModel.RunTypeAnalysisGroups[0].AnalysisGroupName}\\\" no longer exist", becauseArgs: null);
        }

        private void AssumeDependenciesAreMocked()
        {
            _existingRunTypes = _fixture.CreateMany<RunType>(3).ToList();
            _ = _mockRunTypeRepository.Setup(x => x.GetAll()).Returns(_existingRunTypes);
            _ = _mockRunTypeRepository.Setup(m => m.FindByName(_existingRunTypes[0].Name)).Returns(_existingRunTypes[0]);
            _ = _mockRunTypeRepository.Setup(m => m.Get(_existingRunTypes[0].Id)).Returns(_existingRunTypes[0]);
            _ = _mockFeatureManager.Setup(m => m.IsEnabled(nameof(ProductFeature.RunType))).Returns(true);

            _dummyAnalysisGroups = _fixture.Build<AnalysisGroup>().CreateMany(3).ToList();
            var dummyCreateRunTypeAnalysisGroupModels = _fixture.Build<CreateRunTypeAnalysisGroupModel>()
                .With(o => o.KPI, RunTypeAnalysisGroupKPINames.DeliveryPercentage)
                .CreateMany(3).ToList();

            dummyCreateRunTypeAnalysisGroupModels[0].AnalysisGroupId = _dummyAnalysisGroups[0].Id;
            dummyCreateRunTypeAnalysisGroupModels[1].AnalysisGroupId = _dummyAnalysisGroups[1].Id;
            dummyCreateRunTypeAnalysisGroupModels[2].AnalysisGroupId = _dummyAnalysisGroups[2].Id;

            _dummyCreateRunTypeModel = _fixture.Build<CreateRunTypeModel>()
                .With(o => o.DefaultKPI, RunTypeAnalysisGroupKPINames.PoolValue)
                .With(o => o.RunTypeAnalysisGroups, dummyCreateRunTypeAnalysisGroupModels).Create();

            _ = _mockAnalysisGroupRepository.Setup(m => m.GetAll()).Returns(_dummyAnalysisGroups);
        }

        private void AssumeDependenciesAreSupplied()
        {
            _mapper = new Mapper(new MapperConfiguration(expression => expression.AddProfile<RunTypeProfile>()));
            _mockRunTypeRepository = new Mock<IRunTypeRepository>();
            _mockFeatureManager = new Mock<IFeatureManager>();
            _mockAnalysisGroupRepository = new Mock<IAnalysisGroupRepository>();
            _mockLandmarkRunQueueRepository = new Mock<ILandmarkRunQueueRepository>();
            _fixture = new SafeFixture();
        }

        [TearDown]
        public void Cleanup()
        {
            _mapper = null;
            _fixture = null;
        }
    }
}
