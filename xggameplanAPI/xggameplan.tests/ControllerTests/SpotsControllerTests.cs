using System;
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
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using ImagineCommunications.GamePlan.Domain.Spots;
using Moq;
using NUnit.Framework;
using xggameplan.AuditEvents;
using xggameplan.Configuration;
using xggameplan.Controllers;
using xggameplan.core.Configuration;
using xggameplan.core.Services;
using xggameplan.Model;
using xggameplan.Services;

namespace xggameplan.tests.ControllerTests
{
    [TestFixture(Category = "Controllers :: Spots")]
    public class SpotsControllerTests : IDisposable
    {
        private const string InitialBreakRef = "INITIAL_BREAK";
        private const string TargetBreakRef = "TARGET_BREAK_REF";

        private SafeFixture _fixture;
        private Mock<ISpotRepository> _fakeSpotRepository;
        private Mock<ICampaignRepository> _fakeCampaignRepository;
        private Mock<IProductRepository> _fakeProductRepository;
        private Mock<IClashRepository> _fakeClashRepository;
        private Mock<IBreakRepository> _fakeBreakRepository;
        private Mock<IScheduleRepository> _fakeScheduleRepository;
        private Mock<IAuditEventRepository> _fakeAuditEventRepository;
        private Mock<IDataChangeValidator> _fakeDataChangeValidator;
        private IMapper _mapper;

        private SpotsController _controller;

        [SetUp]
        public void Init()
        {
            _fixture = new SafeFixture();
            _fakeSpotRepository = new Mock<ISpotRepository>();
            _fakeCampaignRepository = new Mock<ICampaignRepository>();
            _fakeProductRepository = new Mock<IProductRepository>();
            _fakeClashRepository = new Mock<IClashRepository>();
            _fakeBreakRepository = new Mock<IBreakRepository>();
            _fakeScheduleRepository = new Mock<IScheduleRepository>();
            _fakeAuditEventRepository = new Mock<IAuditEventRepository>();
            _fakeDataChangeValidator = new Mock<IDataChangeValidator>();

            _mapper = AutoMapperInitializer.Initialize(AutoMapperConfig.SetupAutoMapper);

            _controller = new SpotsController(
                _fakeSpotRepository.Object,
                _fakeBreakRepository.Object,
                _fakeScheduleRepository.Object,
                _fakeDataChangeValidator.Object,
                _fakeAuditEventRepository.Object,
                new SpotModelCreator(_fakeCampaignRepository.Object, _fakeProductRepository.Object, _fakeClashRepository.Object, _mapper),
                _mapper
            );

            _controller.Request = new HttpRequestMessage();
            _controller.Request.SetConfiguration(new HttpConfiguration());
        }

        [Test]
        [Description("Given nonexistent date range and sales area when deleting Spots then Not Found must be returned")]
        public void DeleteSpotsByNonExistentDateRangeAndSalesAreasReturnsNotFound()
        {
            Assert.IsInstanceOf<NotFoundResult>(
                _controller.Delete(
                    DateTime.Now,
                    DateTime.Now.AddDays(1),
                    new List<string> { "test" }
                    )
                );
        }

        [Test]
        [Description("Given date range and area when deleting Spots then spots must be deleted")]
        public void DeleteSpotsByDateRangeAndSalesAreaDeletesSpots()
        {
            // Arrange
            var fixture = new SafeFixture();

            var breakDateTime = new DateTime(2019, 12, 31, 10, 30, 0);
            var salesAreaName = fixture.Create<string>();

            var fakeBreaks = fixture.CreateMany<Break>(3).ToList();
            var fakeSpots = fixture.CreateMany<Spot>(3).ToList();

            fakeBreaks.ForEach(aBreak => aBreak.ScheduledDate = breakDateTime.AddMinutes(-15));
            fakeBreaks.ForEach(aBreak => aBreak.ScheduledDate = aBreak.ScheduledDate.AddMinutes(15));
            fakeBreaks.ForEach(aBreak => aBreak.SalesArea = salesAreaName);

            fakeSpots.ForEach(spot => spot.ExternalBreakNo = fakeBreaks[0].ExternalBreakRef);
            fakeSpots[2].ExternalBreakNo = fakeBreaks[1].ExternalBreakRef;

            fakeSpots.ForEach(spot => spot.SalesArea = salesAreaName);

            var expectedSpotIds = fakeSpots
                .Where(spot => spot.ExternalBreakNo == fakeBreaks[0].ExternalBreakRef);

            _ = _fakeBreakRepository
                .Setup(r => r.Search(It.IsAny<DateTimeRange>(), It.IsAny<IReadOnlyCollection<string>>()))
                .Returns(fakeBreaks);

            _ = _fakeSpotRepository
                .Setup(r => r.FindByExternalBreakNumbers(It.IsAny<IReadOnlyCollection<string>>()))
                .Returns(expectedSpotIds);

            // Act
            var result = _controller.Delete(
                breakDateTime,
                breakDateTime.AddMinutes(10),
                new List<string> { salesAreaName }
                );

            // Assert
            Assert.IsInstanceOf<OkResult>(result);

            _fakeBreakRepository.Verify(
                m => m.Search(It.IsAny<DateTimeRange>(), It.IsAny<IReadOnlyCollection<string>>()),
                Times.Once);

            _fakeSpotRepository.Verify(
                m => m.FindByExternalBreakNumbers(It.IsAny<IReadOnlyCollection<string>>()),
                Times.Once);

            _fakeSpotRepository.Verify(
                m => m.Delete(It.IsAny<IEnumerable<Guid>>()),
                Times.Once
                );
        }

        [Test]
        [Description("Given missing or default params when deleting Spots then correct validation message must be returned")]
        public void DeleteSpotsByMissingOrDefaultParamsReturnsBadRequest(
            [ValueSource(nameof(_deleteByRangeTestCases))] DeleteByRangeTestData testData)
        {
            var (dateFrom, dateTo) = testData.ScheduledDatesRange;
            var result = _controller.Delete(dateFrom, dateTo, testData.SalesAreaNames) as InvalidModelStateResult;

            Assert.IsTrue(!result.ModelState.IsValid && result.ModelState.ContainsKey(testData.ParamName));
        }

        [Test]
        [Description("Given nonexistent external spot references when deleting Spots then Not Found must be returned")]
        public void DeleteSpotsByNonExistentExternalSpotRefReturnsNotFound()
        {
            Assert.IsInstanceOf<NotFoundResult>(_controller.Delete(new List<string> { "test" }));
        }

        [Test]
        [Description("Given external spot references when deleting Spots then result must be successful")]
        public void DeleteSpotsByExistingExternalSpotRefReturnsOk()
        {
            _ = _fakeSpotRepository
                .Setup(r => r.FindByExternal(It.IsAny<List<string>>()))
                .Returns(new List<Spot> { new Spot() });

            Assert.IsInstanceOf<OkResult>(_controller.Delete(new List<string> { "test" }));
        }

        [Test]
        [Description("Given missing external spot references when deleting Spots then correct validation message must be returned")]
        public void DeleteSpotsByMissingExternalSpotRefReturnsBadRequest()
        {
            var result = _controller.Delete(new List<string>()) as InvalidModelStateResult;

            Assert.Multiple(() =>
            {
                Assert.IsFalse(result.ModelState.IsValid);
                Assert.That(result.ModelState, Contains.Key("externalSpotRef"));
            });
        }

        [Test]
        [Description("Given nonexistent external spot reference when updating Spot then result must be successful")]
        public async Task UpdateSpotByNonExistentExternalReferenceWithValidSpotPayloadReturnsOk()
        {
            // Arrange
            const string initialSpotRef = InitialBreakRef;
            var createSpotPayload = CreateDummyCreateSpotPayload(initialSpotRef);

            _ = _fakeBreakRepository
                .Setup(r => r.FindByExternal(It.Is<string>(v => v == InitialBreakRef)))
                .Returns(new[] { CreateDummyBreak(InitialBreakRef, createSpotPayload) });

            _ = _fakeBreakRepository
                .Setup(r => r.FindByExternal(It.Is<string>(v => v == TargetBreakRef)))
                .Returns(new[] { CreateDummyBreak(TargetBreakRef, createSpotPayload) });

            // Act
            var actionResult = _controller.Put(initialSpotRef, createSpotPayload);

            // Assert
            using var result = await GetActionResult(actionResult).ConfigureAwait(false);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [Test(Description = "BreakType field::Given a spot with a null break type when updating a spot then result must be OK")]
        public async Task UpdateSpotWithNullBreakTypeReturnsOK()
        {
            // Arrange
            const string initialSpotRef = InitialBreakRef;
            var createSpotPayload = CreateDummyCreateSpotPayload(initialSpotRef);

            createSpotPayload.BreakType = null;

            _ = _fakeBreakRepository
                .Setup(r => r.FindByExternal(It.Is<string>(v => v == InitialBreakRef)))
                .Returns(new[] { CreateDummyBreak(InitialBreakRef, createSpotPayload) });

            _ = _fakeBreakRepository
                .Setup(r => r.FindByExternal(It.Is<string>(v => v == TargetBreakRef)))
                .Returns(new[] { CreateDummyBreak(TargetBreakRef, createSpotPayload) });

            // Act
            var actionResult = _controller.Put(initialSpotRef, createSpotPayload);

            // Assert
            using var result = await GetActionResult(actionResult).ConfigureAwait(false);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [Test(Description = "BreakType field::Given a spot with a white space only break type when updating a spot then result must be a bad request")]
        public async Task UpdateSpotWithWhiteSpaceBreakTypeReturnsBadRequest()
        {
            // Arrange
            const string initialSpotRef = InitialBreakRef;

            var createSpotPayload = CreateDummyCreateSpotPayload(initialSpotRef);
            createSpotPayload.BreakType = "    ";

            _ = _fakeBreakRepository
                .Setup(r => r.FindByExternal(It.Is<string>(v => v == InitialBreakRef)))
                .Returns(new[] { CreateDummyBreak(InitialBreakRef, createSpotPayload) });

            _ = _fakeBreakRepository
                .Setup(r => r.FindByExternal(It.Is<string>(v => v == TargetBreakRef)))
                .Returns(new[] { CreateDummyBreak(TargetBreakRef, createSpotPayload) });

            // Act
            var result = _controller.Put(initialSpotRef, createSpotPayload);

            // Assert
            _ = result.Should().BeOfType<InvalidModelStateResult>(null);

            var modelState = ((InvalidModelStateResult)result).ModelState;

            _ = modelState
                    .IsValidField(nameof(CreateSpot.BreakType))
                    .Should().BeFalse(null);

            _ = modelState[nameof(CreateSpot.BreakType)].Errors[0].ErrorMessage
                    .Should().StartWith("The BreakType value is too short.", null);

            _ = modelState[nameof(CreateSpot.BreakType)].Errors[0].ErrorMessage
                    .Should().EndWith(createSpotPayload.ExternalSpotRef, null);
        }

        [Test(Description = "BreakType field::Given a spot with a break type that's too short when updating a spot then result must be a bad request")]
        public async Task UpdateSpotWithBreakTypeTooShortReturnsBadRequest()
        {
            // Arrange
            const string initialSpotRef = InitialBreakRef;

            var createSpotPayload = CreateDummyCreateSpotPayload(initialSpotRef);
            createSpotPayload.BreakType = "X";

            _ = _fakeBreakRepository
                    .Setup(r => r.FindByExternal(It.Is<string>(v => v == InitialBreakRef)))
                    .Returns(new[] { CreateDummyBreak(InitialBreakRef, createSpotPayload) });

            _ = _fakeBreakRepository
                    .Setup(r => r.FindByExternal(It.Is<string>(v => v == TargetBreakRef)))
                    .Returns(new[] { CreateDummyBreak(TargetBreakRef, createSpotPayload) });

            // Act
            var result = _controller.Put(initialSpotRef, createSpotPayload);

            // Assert
            _ = result.Should().BeOfType<InvalidModelStateResult>(null);

            var modelState = ((InvalidModelStateResult)result).ModelState;

            _ = modelState
                    .IsValidField(nameof(CreateSpot.BreakType))
                    .Should().BeFalse(null);

            _ = modelState[nameof(CreateSpot.BreakType)].Errors[0].ErrorMessage
                    .Should().StartWith("The BreakType value is too short.", null);

            _ = modelState[nameof(CreateSpot.BreakType)].Errors[0].ErrorMessage
                    .Should().EndWith(createSpotPayload.ExternalSpotRef, null);
        }

        [Test(Description = "BreakType field::Given a spot with a break type that's the correct length but the first two characters contain a white space when updating a spot then result must be a bad request")]
        public async Task UpdateSpotWithBreakTypePrefixTooShortReturnsBadRequest()
        {
            // Arrange
            const string initialSpotRef = InitialBreakRef;

            var createSpotPayload = CreateDummyCreateSpotPayload(initialSpotRef);
            createSpotPayload.BreakType = "X - My invalid break type";

            _ = _fakeBreakRepository
                    .Setup(r => r.FindByExternal(It.Is<string>(v => v == InitialBreakRef)))
                    .Returns(new[] { CreateDummyBreak(InitialBreakRef, createSpotPayload) });

            _ = _fakeBreakRepository
                    .Setup(r => r.FindByExternal(It.Is<string>(v => v == TargetBreakRef)))
                    .Returns(new[] { CreateDummyBreak(TargetBreakRef, createSpotPayload) });

            // Act
            var result = _controller.Put(initialSpotRef, createSpotPayload);

            // Assert
            _ = result.Should().BeOfType<InvalidModelStateResult>(null);

            var modelState = ((InvalidModelStateResult)result).ModelState;

            _ = modelState
                    .IsValidField(nameof(CreateSpot.BreakType))
                    .Should().BeFalse(null);

            _ = modelState[nameof(CreateSpot.BreakType)].Errors[0].ErrorMessage
                    .Should().StartWith("The BreakType prefix value is too short.", null);

            _ = modelState[nameof(CreateSpot.BreakType)].Errors[0].ErrorMessage
                    .Should().EndWith(createSpotPayload.ExternalSpotRef, null);
        }

        [Test(Description = "BreakType field::Given a spot with a valid break type value when updating a spot then result must be OK")]
        public async Task UpdateSpotWithValidBreakTypeReturnsOK()
        {
            const string initialSpotRef = InitialBreakRef;
            var createSpotPayload = CreateDummyCreateSpotPayload(initialSpotRef);
            createSpotPayload.BreakType = "VA-Valid";

            _ = _fakeBreakRepository
                .Setup(r => r.FindByExternal(It.Is<string>(v => v == InitialBreakRef)))
                .Returns(new[] { CreateDummyBreak(InitialBreakRef, createSpotPayload) });

            _ = _fakeBreakRepository
                .Setup(r => r.FindByExternal(It.Is<string>(v => v == TargetBreakRef)))
                .Returns(new[] { CreateDummyBreak(TargetBreakRef, createSpotPayload) });

            var actionResult = _controller.Put(initialSpotRef, createSpotPayload);

            using var result = await GetActionResult(actionResult).ConfigureAwait(false);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [Test]
        [Description("Given valid Spot payload without break changes when updating Spots then result must be successful")]
        public async Task UpdateSpotByExternalReferenceWithValidSpotPayloadWithoutBreakChangesReturnsOk()
        {
            const string validSpotRef = "VALID_SPOT";
            var createSpotPayload = CreateDummyCreateSpotPayload(validSpotRef);
            var domainSpot = CreateDummyDomainSpotModel(validSpotRef);

            createSpotPayload.Demographic = "TEST_DEMOGRAPHIC";
            domainSpot.Demographic = "INITIAL_DEMOGRAPHIC";
            domainSpot.ExternalBreakNo = createSpotPayload.ExternalBreakNo;

            _ = _fakeSpotRepository
                    .Setup(r => r.FindByExternalSpotRef(It.Is<string>(v => v == validSpotRef)))
                    .Returns(domainSpot);

            _ = _fakeBreakRepository
                    .Setup(r => r.FindByExternal(It.Is<string>(v => v == validSpotRef)))
                    .Returns(new[] { CreateDummyBreak(validSpotRef, createSpotPayload) });

            var actionResult = _controller.Put(validSpotRef, createSpotPayload);

            using var result = await GetActionResult(actionResult).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.IsTrue(
                    result.TryGetContentValue(out SpotModel updatedSpot),
                    $"PUT request response did not contain a valid model of type {nameof(SpotModel)}");

                Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
                Assert.AreEqual(createSpotPayload.Demographic, updatedSpot.Demographic);
            });
        }

        [Test]
        [Description("Given valid Spot payload with valid break change when updating Spots then result must be successful")]
        public async Task UpdateSpotByExternalReferenceWithValidSpotPayloadWithValidBreakChangeReturnsOk()
        {
            const string validSpotRef = "VALID_SPOT";
            var createSpotPayload = CreateDummyCreateSpotPayload(validSpotRef);
            var domainSpot = CreateDummyDomainSpotModel(validSpotRef);

            domainSpot.ExternalBreakNo = InitialBreakRef;
            createSpotPayload.ExternalBreakNo = TargetBreakRef;

            _ = _fakeSpotRepository
                    .Setup(r => r.FindByExternalSpotRef(It.Is<string>(v => v == validSpotRef)))
                    .Returns(domainSpot);

            _ = _fakeBreakRepository
                    .Setup(r => r.FindByExternal(It.Is<string>(v => v == InitialBreakRef)))
                    .Returns(new[] { CreateDummyBreak(InitialBreakRef, createSpotPayload) });

            _ = _fakeBreakRepository
                    .Setup(r => r.FindByExternal(It.Is<string>(v => v == TargetBreakRef)))
                    .Returns(new[] { CreateDummyBreak(TargetBreakRef, createSpotPayload) });

            var actionResult = _controller.Put(validSpotRef, createSpotPayload);

            using var result = await GetActionResult(actionResult).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.IsTrue(
                    result.TryGetContentValue(out SpotModel updatedSpot),
                    $"PUT request response did not contain a valid model of type {nameof(SpotModel)}"
                    );

                Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
                Assert.AreEqual(createSpotPayload.ExternalBreakNo, updatedSpot.ExternalBreakNo);
            });
        }

        private static async Task<HttpResponseMessage> GetActionResult(IHttpActionResult actionResult)
        {
            var cancellationToken = new CancellationToken();
            return await actionResult.ExecuteAsync(cancellationToken).ConfigureAwait(false);
        }

        private Spot CreateDummyDomainSpotModel(string externalReference)
        {
            return _fixture.Build<Spot>()
                .With(s => s.ExternalSpotRef, externalReference)
                .With(s => s.ExternalBreakNo, externalReference)
                .With(s => s.StartDateTime, DateTime.UtcNow.Date)
                .With(s => s.EndDateTime, DateTime.UtcNow.Date.AddDays(1))
                .Create();
        }

        private CreateSpot CreateDummyCreateSpotPayload(string externalReference)
        {
            return _fixture.Build<CreateSpot>()
                .With(s => s.ExternalSpotRef, externalReference)
                .With(s => s.ExternalBreakNo, externalReference)
                .With(s => s.StartDateTime, DateTime.UtcNow.Date)
                .With(s => s.EndDateTime, DateTime.UtcNow.Date.AddDays(1))
                .Create();
        }

        private Break CreateDummyBreak(string externalReference, CreateSpot spot)
        {
            return _fixture.Build<Break>()
                .With(b => b.ExternalBreakRef, externalReference)
                .With(b => b.SalesArea, spot.SalesArea)
                .With(b => b.ScheduledDate, spot.StartDateTime)
                .Create();
        }

        private static readonly DeleteByRangeTestData[] _deleteByRangeTestCases =
        {
            new DeleteByRangeTestData
            {
                ScheduledDatesRange = new DateTimeRange(default, DateTime.Now),
                SalesAreaNames = new List<string> {"test"},
                ParamName = "dateRangeStart"
            },
            new DeleteByRangeTestData
            {
                ScheduledDatesRange = new DateTimeRange(DateTime.Now, default),
                SalesAreaNames = new List<string> {"test"},
                ParamName = "dateRangeEnd"
            },
            new DeleteByRangeTestData
            {
                ScheduledDatesRange = new DateTimeRange(DateTime.Now, DateTime.Now),
                SalesAreaNames = new List<string>(),
                ParamName = "salesAreaNames"
            }
        };

        public class DeleteByRangeTestData
        {
            public DateTimeRange ScheduledDatesRange { get; set; }
            public List<string> SalesAreaNames { get; set; }
            public string ParamName { get; set; }
        }

        [TearDown]
        public void Cleanup()
        {
            _controller = null;
            _mapper = null;
        }

        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue)
            {
                return;
            }

            if (disposing)
            {
                _controller?.Dispose();

                _controller = null;
                _mapper = null;
            }

            _disposedValue = true;
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
    }
}
