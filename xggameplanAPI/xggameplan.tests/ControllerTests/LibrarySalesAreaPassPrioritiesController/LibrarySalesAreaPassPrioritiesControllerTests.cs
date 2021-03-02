using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentValidation;
using ImagineCommunications.GamePlan.Domain.LibrarySalesAreaPassPriorities;
using ImagineCommunications.GamePlan.Domain.Shared.System.Models;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using Moq;
using NUnit.Framework;
using xggameplan.Controllers;
using xggameplan.core.Configuration;
using xggameplan.Model;
using xggameplan.Validations;

namespace xggameplan.tests.ControllerTests
{
    [TestFixture(Category = "Controllers :: LibrarySalesAreaPassPriorities")]
    public class LibrarySalesAreaPassPrioritiesControllerTests : IDisposable
    {
        private Fixture _fixture;
        private IMapper _mapper;
        private Guid _id;
        private CreateLibrarySalesAreaPassPriorityModel _modelForPost;
        private UpdateLibrarySalesAreaPassPriorityModel _modelForPut;
        private LibrarySalesAreaPassPrioritiesController _target;
        private IEnumerable<LibrarySalesAreaPassPriority> _librarySalesAreaPassPriorities;
        private Mock<ILibrarySalesAreaPassPrioritiesRepository> _librarySalesAreaPassPrioritiesRepository;
        private Mock<ITenantSettingsRepository> _tenantSettingsRepository;
        private Mock<CreateLibrarySalesAreaPassPriorityModelValidator> _validatorForCreate;
        private Mock<UpdateLibrarySalesAreaPassPriorityModelValidator> _validatorForUpdate;

        [OneTimeSetUp]
        public void OnInit()
        {
            _fixture = new Fixture();
            AssumeDependenciesAreSupplied();
            AssumeTargetIsInitialised();
        }

        [Test]
        [Description("GetAllAsync Should Return OkResult With Records When Records Are Found")]
        public async Task GetAllAsyncShouldReturnOkResultWithRecordsWhenRecordsAreFound()
        {
            int noOfRecordsToReturn = 6;
            AssumeLibrarySalesAreaPassPrioritiesRepositoryGetAllAsyncReturnsRecords(noOfRecordsToReturn);

            var result = await AssumeGetAllAsyncIsRequested().ConfigureAwait(false);

            using (new AssertionScope())
            {
                _ = result.Should()
                    .BeAssignableTo<OkNegotiatedContentResult<IEnumerable<LibrarySalesAreaPassPriorityModel>>>
                            ("GetAllAsync did not return OkNegotiatedContentResult");
                _ = (result as OkNegotiatedContentResult<IEnumerable<LibrarySalesAreaPassPriorityModel>>).Content.Count()
                                                    .Should().Be(noOfRecordsToReturn, "GetAllAsync did not return any records");
            }
        }

        [Test]
        [Description("GetAllAsync Should Return OkResult With Records When Records Are Found And The Default Item Should Be Marked As IsDefault")]
        public async Task GetAllAsyncShouldReturnOkResultWithRecordsWhenRecordsAreFoundAndTheDefaultItemShouldBeMarkedAsIsDefault()
        {
            int noOfRecordsToReturn = 6;
            AssumeLibrarySalesAreaPassPrioritiesRepositoryGetAllAsyncReturnsRecords(noOfRecordsToReturn);
            AssumeTenantSettingsRepositoryGetDefaultSalesAreaPassPriorityIdReturns(_librarySalesAreaPassPriorities.FirstOrDefault().Uid);

            var result = await AssumeGetAllAsyncIsRequested().ConfigureAwait(false);

            using (new AssertionScope())
            {
                _ = result.Should().BeAssignableTo<OkNegotiatedContentResult<IEnumerable<LibrarySalesAreaPassPriorityModel>>>
                                ("GetAllAsync did not return OkNegotiatedContentResult");

                _ = (result as OkNegotiatedContentResult<IEnumerable<LibrarySalesAreaPassPriorityModel>>).Content.Count()
                                                    .Should().Be(noOfRecordsToReturn, "GetAllAsync did not return any records");

                _ = (result as OkNegotiatedContentResult<IEnumerable<LibrarySalesAreaPassPriorityModel>>).Content.Any(a => a.IsDefault)
                                                    .Should().BeTrue("GetAllAsync did not mark the Default item as IsDefault");
            }
        }

        [Test]
        [Description("GetAllAsync Should Return OkResult With An Empty Collection When No Records Are Found")]
        public async Task GetAllAsyncShouldReturnOkResultWithAnEmptyCollectionWhenNoRecordsAreFound()
        {
            AssumeLibrarySalesAreaPassPrioritiesRepositoryGetAllAsyncReturnsNoRecords();

            var result = await AssumeGetAllAsyncIsRequested().ConfigureAwait(false);

            using (new AssertionScope())
            {
                _ = result.Should().BeAssignableTo<OkNegotiatedContentResult<IEnumerable<LibrarySalesAreaPassPriorityModel>>>
                            ("GetAllAsync did not return OkNegotiatedContentResult");
                _ = (result as OkNegotiatedContentResult<IEnumerable<LibrarySalesAreaPassPriorityModel>>).Content.Count().Should().Be(0, "GetAllAsync did not return an empty result");
            }
        }

        [Test]
        [Description("GetDefaultAsync Should Return NotFoundResult When No Default Item Is Setup In TenantSettings")]
        public async Task GetDefaultAsyncShouldReturnNotFoundResultWhenNoDefaultItemIsSetupInTenantSettings()
        {
            AssumeAnEmptyIdIsSupplied();
            AssumeTenantSettingsRepositoryGetDefaultSalesAreaPassPriorityIdReturns(Guid.Empty);

            var result = await AssumeGetDefaultAsyncIsRequested().ConfigureAwait(false);

            _ = result.Should().BeAssignableTo<NotFoundResult>("GetDefaultAsync did not return NotFoundResult");
        }

        [Test]
        [Description("GetDefaultAsync Should Return OkResult When a Default Item Is Setup In TenantSettings")]
        public async Task GetDefaultAsyncShouldReturnOkResultWhenADefaultItemIsSetupInTenantSettings()
        {
            AssumeValidIdIsSupplied();
            AssumeTenantSettingsRepositoryGetDefaultSalesAreaPassPriorityIdReturns(_id);
            AssumeLibrarySalesAreaPassPrioritiesRepositoryGetAsyncReturnsAMatchingRecord();

            var result = await AssumeGetDefaultAsyncIsRequested().ConfigureAwait(false);

            using (new AssertionScope())
            {
                _ = result.Should().BeAssignableTo<OkNegotiatedContentResult<LibrarySalesAreaPassPriorityModel>>
                            ("GetDefaultAsync did not return OkNegotiatedContentResult");
                _ = (result as OkNegotiatedContentResult<LibrarySalesAreaPassPriorityModel>).Content.Should().NotBeNull("GetDefaultAsync returned null");
            }
        }

        [Test]
        [Description("GetAsync Should Return NotFoundResult When An Empty Id Is Supplied")]
        public async Task GetAsyncShouldReturnNotFoundResultWhenAnEmptyIdIsSupplied()
        {
            AssumeAnEmptyIdIsSupplied();

            var result = await AssumeGetAsyncIsRequested().ConfigureAwait(false);

            _ = result.Should().BeAssignableTo<NotFoundResult>("GetAsync did not return NotFoundResult");
        }

        [Test]
        [Description("GetAsync Should Return NotFoundResult When No Record Is Found For The Supplied Id")]
        public async Task GetAsyncShouldReturnNotFoundResultWhenNoRecordIsFoundForTheSuppliedId()
        {
            AssumeValidIdIsSupplied();
            AssumeLibrarySalesAreaPassPrioritiesRepositoryGetAsyncReturnsNoRecord();

            var result = await AssumeGetAsyncIsRequested().ConfigureAwait(false);

            _ = result.Should().BeAssignableTo<NotFoundResult>("GetAsync did not return NotFoundResult");
        }

        [Test]
        [Description("GetAsync Should Return OkResult When A Record Is Found For The Supplied Id")]
        public async Task GetAsyncShouldReturnOkResultWhenARecordIsFoundForTheSuppliedId()
        {
            AssumeValidIdIsSupplied();
            AssumeLibrarySalesAreaPassPrioritiesRepositoryGetAsyncReturnsAMatchingRecord();

            var result = await AssumeGetAsyncIsRequested().ConfigureAwait(false);

            using (new AssertionScope())
            {
                _ = result.Should().BeAssignableTo<OkNegotiatedContentResult<LibrarySalesAreaPassPriorityModel>>
                            ("GetAsync did not return OkNegotiatedContentResult");
                _ = (result as OkNegotiatedContentResult<LibrarySalesAreaPassPriorityModel>).Content.Should().NotBeNull("GetAsync returned null");
            }
        }

        [Test]
        [Description("GetAsync Should Return OkResult With Model Marked As Default When Record Found For The Supplied Id Is The Default Item")]
        public async Task GetAsyncShouldReturnOkResultWithModelMarkedAsDefaultWhenRecordFoundForTheSuppliedIdIsTheDefaultItem()
        {
            AssumeValidIdIsSupplied();
            AssumeLibrarySalesAreaPassPrioritiesRepositoryGetAsyncReturnsAMatchingRecord();
            AssumeTenantSettingsRepositoryGetDefaultSalesAreaPassPriorityIdReturns(_id);

            var result = await AssumeGetAsyncIsRequested().ConfigureAwait(false);

            using (new AssertionScope())
            {
                _ = result.Should().BeAssignableTo<OkNegotiatedContentResult<LibrarySalesAreaPassPriorityModel>>
                            ("GetAsync did not return OkNegotiatedContentResult");
                _ = (result as OkNegotiatedContentResult<LibrarySalesAreaPassPriorityModel>)
                .Content.Should().NotBeNull("GetAsync returned null")
                .And.Subject.As<LibrarySalesAreaPassPriorityModel>().IsDefault.Should().BeTrue("GetAsync did not mark the Default item as IsDefault");
            }
        }

        [Test]
        [Description("PostAsync Should Return BadRequestResult When Supplied Model Fails Validation")]
        public async Task PostAsyncShouldReturnBadRequestResultWhenSuppliedModelFailsValidation()
        {
            AssumeModelIsSuppliedForPost();
            AssumeModelValidationForPostFails();

            var result = await AssumePostAsyncIsRequested().ConfigureAwait(false) as ResponseMessageResult;

            _ = result.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest, "PostAsync did not return BadRequest");
        }

        [Test]
        [Description("PostAsync Should Return OkResult When Supplied Model Is Successfully Added")]
        public async Task PostAsyncShouldReturnOkResultWhenSuppliedModelIsSuccessfullyAdded()
        {
            AssumeModelIsSuppliedForPost();
            AssumeModelValidationForPostPasses();

            var result = await AssumePostAsyncIsRequested().ConfigureAwait(false);

            using (new AssertionScope())
            {
                _ = result.Should().BeAssignableTo<CreatedAtRouteNegotiatedContentResult<LibrarySalesAreaPassPriorityModel>>
                           ("PostAsync did not return CreatedAtRouteNegotiatedContentResult");
                _ = (result as CreatedAtRouteNegotiatedContentResult<LibrarySalesAreaPassPriorityModel>).Content.Should().NotBeNull("PostAsync returned null");
            }
        }

        [Test]
        [Description("PutAsync Should Return NotFoundResult When An Empty Id Is Supplied")]
        public async Task PutAsyncShouldReturnNotFoundResultWhenAnEmptyIdIsSupplied()
        {
            AssumeAnEmptyIdIsSupplied();
            AssumeModelIsSuppliedForPut();
            AssumeModelValidationForPutPasses();

            var result = await AssumePutAsyncIsRequested().ConfigureAwait(false);

            _ = result.Should().BeAssignableTo<NotFoundResult>("PutAsync did not return NotFoundResult");
        }

        [Test]
        [Description("PutAsync Should Return NotFoundResult When No Matching Record Is Found For The Supplied Id")]
        public async Task PutAsyncShouldReturnNotFoundResultWhenNoMatchingRecordIsFoundForTheSuppliedId()
        {
            AssumeValidIdIsSupplied();
            AssumeModelIsSuppliedForPut();
            AssumeModelValidationForPutPasses();
            AssumeLibrarySalesAreaPassPrioritiesRepositoryGetAsyncReturnsNoRecord();

            var result = await AssumePutAsyncIsRequested().ConfigureAwait(false);

            _ = result.Should().BeAssignableTo<NotFoundResult>("PutAsync did not return NotFoundResult");
        }

        [Test]
        [Description("PutAsync Should Return BadRequestResult When Supplied Model Fails Validation")]
        public async Task PutAsyncShouldReturnBadRequestResultWhenSuppliedModelFailsValidation()
        {
            AssumeValidIdIsSupplied();
            AssumeModelIsSuppliedForPut();
            AssumeModelValidationForPutFails();

            var result = await AssumePutAsyncIsRequested().ConfigureAwait(false) as ResponseMessageResult;

            _ = result.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest, "PutAsync did not return BadRequest");
        }

        [Test]
        [Description("PutAsync Should Return OkResult When Supplied Model Is Successfully Updated")]
        public async Task PutAsyncShouldReturnOkResultWhenSuppliedModelIsSuccessfullyUpdated()
        {
            AssumeValidIdIsSupplied();
            AssumeModelIsSuppliedForPut();
            AssumeModelValidationForPutPasses();
            AssumeLibrarySalesAreaPassPrioritiesRepositoryGetAsyncReturnsAMatchingRecord();

            var result = await AssumePutAsyncIsRequested().ConfigureAwait(false);

            using (new AssertionScope())
            {
                _ = result.Should().BeAssignableTo<OkNegotiatedContentResult<LibrarySalesAreaPassPriorityModel>>
                            ("PutAsync did not return OkNegotiatedContentResult");
                _ = (result as OkNegotiatedContentResult<LibrarySalesAreaPassPriorityModel>).Content.Should().NotBeNull("PutAsync returned null");
            }
        }

        private void AssumeTenantSettingsRepositoryGetDefaultSalesAreaPassPriorityIdReturns(Guid defaultSalesAreaPassPriorityId)
        {
            _ = _tenantSettingsRepository.Setup(a => a.GetDefaultSalesAreaPassPriorityId()).Returns(defaultSalesAreaPassPriorityId);
        }

        private void AssumeLibrarySalesAreaPassPrioritiesRepositoryGetAsyncReturnsNoRecord()
        {
            SetupLibrarySalesAreaPassPrioritiesRepositoryGetAsyncToReturn(null);
        }

        private void AssumeLibrarySalesAreaPassPrioritiesRepositoryGetAsyncReturnsAMatchingRecord()
        {
            SetupLibrarySalesAreaPassPrioritiesRepositoryGetAsyncToReturn(CreateValidEntity(_id));
        }

        private void SetupLibrarySalesAreaPassPrioritiesRepositoryGetAsyncToReturn(LibrarySalesAreaPassPriority theEntity)
        {
            _ = _librarySalesAreaPassPrioritiesRepository.Setup(a => a.GetAsync(It.IsAny<Guid>())).ReturnsAsync(theEntity);
        }

        private void AssumeLibrarySalesAreaPassPrioritiesRepositoryGetAllAsyncReturnsRecords(int noOfRecords)
        {
            SetupLibrarySalesAreaPassPrioritiesRepositoryGetAllAsyncTo(noOfRecords);
        }

        private void AssumeLibrarySalesAreaPassPrioritiesRepositoryGetAllAsyncReturnsNoRecords()
        {
            SetupLibrarySalesAreaPassPrioritiesRepositoryGetAllAsyncTo(0);
        }

        private void SetupLibrarySalesAreaPassPrioritiesRepositoryGetAllAsyncTo(int noOfRecords)
        {
            _librarySalesAreaPassPriorities = CreateEntities(noOfRecords > 0 ? noOfRecords : 0);
            _ = _librarySalesAreaPassPrioritiesRepository.Setup(a => a.GetAllAsync()).ReturnsAsync(_librarySalesAreaPassPriorities);
        }

        private async Task<IHttpActionResult> AssumePostAsyncIsRequested()
        {
            return await _target.PostAsync(_modelForPost).ConfigureAwait(false);
        }

        private async Task<IHttpActionResult> AssumePutAsyncIsRequested()
        {
            return await _target.PutAsync(_id, _modelForPut).ConfigureAwait(false);
        }

        private async Task<IHttpActionResult> AssumeGetAsyncIsRequested()
        {
            return await _target.GetAsync(_id).ConfigureAwait(false);
        }

        private async Task<IHttpActionResult> AssumeGetDefaultAsyncIsRequested()
        {
            return await _target.GetDefaultAsync().ConfigureAwait(false);
        }

        private async Task<IHttpActionResult> AssumeGetAllAsyncIsRequested()
        {
            return await _target.GetAllAsync().ConfigureAwait(false);
        }

        private void AssumeModelIsSuppliedForPost()
        {
            _modelForPost = CreateValidModelForPost();
        }

        private void AssumeModelIsSuppliedForPut()
        {
            _modelForPut = CreateValidModelForPut();
        }

        private void AssumeValidIdIsSupplied()
        {
            _id = Guid.NewGuid();
        }

        private void AssumeAnEmptyIdIsSupplied()
        {
            _id = Guid.Empty;
        }

        private void AssumeTargetIsInitialised()
        {
            _target = new LibrarySalesAreaPassPrioritiesController(_librarySalesAreaPassPrioritiesRepository.Object,
                                                                    _tenantSettingsRepository.Object,
                                                                    _mapper,
                                                                    _validatorForCreate.Object,
                                                                    _validatorForUpdate.Object
                                                                    );
        }

        private void AssumeDependenciesAreSupplied()
        {
            _librarySalesAreaPassPrioritiesRepository = new Mock<ILibrarySalesAreaPassPrioritiesRepository>();
            _tenantSettingsRepository = new Mock<ITenantSettingsRepository>();

            var _createValidation = new Mock<IValidator<CreateLibrarySalesAreaPassPriorityModel>>();
            var _updateValidation = new Mock<IValidator<UpdateLibrarySalesAreaPassPriorityModel>>();
            _validatorForCreate = new Mock<CreateLibrarySalesAreaPassPriorityModelValidator>(_createValidation.Object);
            _validatorForUpdate = new Mock<UpdateLibrarySalesAreaPassPriorityModelValidator>(_updateValidation.Object);

            _mapper = AutoMapperInitializer.Initialize(Configuration.AutoMapperConfig.SetupAutoMapper);
        }

        private void AssumeModelValidationForPostFails()
        {
            SetupTheCreateValidatorIsValidToReturn(false);
        }

        private void AssumeModelValidationForPostPasses()
        {
            SetupTheCreateValidatorIsValidToReturn(true);
        }

        private void SetupTheCreateValidatorIsValidToReturn(bool valueToReturn)
        {
            _ = _validatorForCreate.Setup(a => a.IsValid(It.IsAny<CreateLibrarySalesAreaPassPriorityModel>())).Returns(valueToReturn);
        }

        private void AssumeModelValidationForPutFails()
        {
            SetupTheUpdateValidatorIsValidToReturn(false);
        }

        private void AssumeModelValidationForPutPasses()
        {
            SetupTheUpdateValidatorIsValidToReturn(true);
        }

        private void SetupTheUpdateValidatorIsValidToReturn(bool valueToReturn)
        {
            _ = _validatorForUpdate.Setup(a => a.IsValid(It.IsAny<UpdateLibrarySalesAreaPassPriorityModel>())).Returns(valueToReturn);
        }

        private CreateLibrarySalesAreaPassPriorityModel CreateValidModelForPost()
        {
            return _fixture.Build<CreateLibrarySalesAreaPassPriorityModel>()
                           .With(a => a.DaysOfWeek, "1111111")
                           .With(a => a.StartTime, "06:00")
                           .With(a => a.EndTime, "06:30")
                           .With(a => a.SalesAreaPriorities, _fixture.CreateMany<SalesAreaPriorityModel>(6).ToList())
                           .Create();
        }

        private UpdateLibrarySalesAreaPassPriorityModel CreateValidModelForPut()
        {
            return _fixture.Build<UpdateLibrarySalesAreaPassPriorityModel>()
                           .With(a => a.DaysOfWeek, "1111111")
                           .With(a => a.StartTime, "06:00")
                           .With(a => a.EndTime, "06:30")
                           .With(a => a.SalesAreaPriorities, _fixture.CreateMany<SalesAreaPriorityModel>(6).ToList())
                           .Create();
        }

        private LibrarySalesAreaPassPriority CreateValidEntity(Guid withUId)
        {
            return _fixture.Build<LibrarySalesAreaPassPriority>()
                           .With(a => a.Uid, withUId)
                           .With(a => a.DaysOfWeek, "1111111")
                           .With(a => a.StartTime, new TimeSpan(6, 0, 0))
                           .With(a => a.EndTime, new TimeSpan(6, 30, 0))
                           .With(a => a.SalesAreaPriorities, _fixture.CreateMany<SalesAreaPriority>(6).ToList())
                           .Create();
        }

        private IEnumerable<LibrarySalesAreaPassPriority> CreateEntities(int noOfRecords)
        {
            return (noOfRecords > 0) ? _fixture.CreateMany<LibrarySalesAreaPassPriority>(noOfRecords) : new List<LibrarySalesAreaPassPriority>();
        }

        public void Dispose()
        {
            _target?.Dispose();
            _target = null;
            _fixture = null;
            _mapper = null;
            _librarySalesAreaPassPriorities = null;
            _librarySalesAreaPassPrioritiesRepository = null;
            _tenantSettingsRepository = null;
            _validatorForCreate = null;
            _validatorForUpdate = null;
        }
    }
}
