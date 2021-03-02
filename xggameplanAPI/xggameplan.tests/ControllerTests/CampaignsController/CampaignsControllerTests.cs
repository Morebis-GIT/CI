using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Http.Results;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using ImagineCommunications.GamePlan.CommonTestsHelpers;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.ProgrammeCategory;
using ImagineCommunications.GamePlan.Domain.Recommendations;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.Metadatas;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using Moq;
using NodaTime;
using NUnit.Framework;
using xggameplan.Controllers;
using xggameplan.core.FeatureManagement.Interfaces;
using xggameplan.core.Interfaces;
using xggameplan.core.Services;
using xggameplan.model.External.Campaign;
using xggameplan.Model;
using xggameplan.Profile;
using xggameplan.Reports.Common;
using xggameplan.Reports.ExcelReports.Campaigns;
using xggameplan.Services;

namespace xggameplan.tests.ControllerTests
{
    [TestFixture(Category = "Controllers :: Campaigns")]
    public class CampaignsControllerTests : IDisposable
    {
        private Mock<IDemographicRepository> _mockDemographicRepository;
        private CampaignsController _controller;
        private CampaignsController _controllerToValidatePut;
        private Mock<ICampaignRepository> _mockCampaignRepository;
        private Mock<IRecommendationRepository> _mockRecommendationRepository;
        private Mock<IMetadataRepository> _mockMetadataRepository;
        private Mock<IProgrammeCategoryHierarchyRepository> _mockProgrammeCategoryRepository;
        private Mock<ISalesAreaRepository> _mockSalesAreaRepository;
        private Mock<IProductRepository> _mockProductRepository;
        private Mock<IDataChangeValidator> _mockDataChangeValidator;
        private Mock<ICampaignExcelReportGenerator> _mockCampaignExcelGenerator;
        private Mock<IReportColumnFormatter> _mockReportColumnFormatter;
        private Mock<IFeatureManager> _mockFeatureManager;
        private Mock<ICampaignCleaner> _mockCampaignCleaner;
        private List<CreateCampaign> _campaigns;
        private IMapper _mapper;
        private Fixture _fixture;

        [SetUp]
        public void Init()
        {
            _campaigns = CreateValidCampaignObjectList();

            _mapper = new Mapper(new MapperConfiguration(expression => expression.AddProfile<CampaignProfile>()));

            _mockCampaignRepository = new Mock<ICampaignRepository>();
            _mockDemographicRepository = new Mock<IDemographicRepository>();
            _mockRecommendationRepository = new Mock<IRecommendationRepository>();
            _mockMetadataRepository = new Mock<IMetadataRepository>();
            _mockProgrammeCategoryRepository = new Mock<IProgrammeCategoryHierarchyRepository>();
            _mockSalesAreaRepository = new Mock<ISalesAreaRepository>();
            _mockProductRepository = new Mock<IProductRepository>();
            _mockDataChangeValidator = new Mock<IDataChangeValidator>();
            _mockCampaignExcelGenerator = new Mock<ICampaignExcelReportGenerator>();
            _mockReportColumnFormatter = new Mock<IReportColumnFormatter>();
            _mockFeatureManager = new Mock<IFeatureManager>();
            _mockCampaignCleaner = new Mock<ICampaignCleaner>();

            _ = _mockFeatureManager
                .Setup(p => p.IsEnabled(It.Is<string>(e => e == nameof(ProductFeature.IncludeChannelGroupFileForOptimiser))))
                .Returns(true);

            var flattener = new CampaignFlattener(
                _mockProductRepository.Object,
                _mockDemographicRepository.Object,
                null,
                _mapper);

            _controller = new CampaignsController(
                null,
                null,
                null,
                null,
                _mockDemographicRepository.Object,
                _mockSalesAreaRepository.Object,
                _mockProductRepository.Object,
                _mockCampaignExcelGenerator.Object,
                _mockReportColumnFormatter.Object,
                null,
                null,
                _mockProgrammeCategoryRepository.Object,
                _mockFeatureManager.Object,
                flattener,
                _mockCampaignCleaner.Object,
                null);

            _controllerToValidatePut = new CampaignsController(
                _mockCampaignRepository.Object,
                _mockRecommendationRepository.Object,
                _mockDataChangeValidator.Object,
                _mapper,
                _mockDemographicRepository.Object,
                _mockSalesAreaRepository.Object,
                _mockProductRepository.Object,
                _mockCampaignExcelGenerator.Object,
                _mockReportColumnFormatter.Object,
                null,
                null,
                _mockProgrammeCategoryRepository.Object,
                _mockFeatureManager.Object,
                flattener,
                _mockCampaignCleaner.Object,
                null);

            _fixture = new SafeFixture();
        }

        [Test]
        public void Validation_When_ExternalIdIsIsNotSet_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].ExternalId = null;
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Property("Message").EqualTo("Value cannot be null.\r\nParameter name: External Id"));
        }

        [Test]
        public void Validation_When_CampaignNameIsNotSet_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].Name = null;
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Property("Message").EqualTo("Value cannot be null.\r\nParameter name: Campaign Name"));
        }

        [Test]
        public void Validation_When_DemoGraphicIsNotSet_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].DemoGraphic = null;
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Property("Message").EqualTo("Value cannot be null.\r\nParameter name: Demographic"));
        }

        [Test]
        public void Validation_When_StartDateTimeIsNotSet_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].StartDateTime = DateTime.MinValue;
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Property("Message").EqualTo("Value cannot be null.\r\nParameter name: Start Date Time"));
        }

        [Test]
        public void Validation_When_EndDateTimeIsNotSet_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].EndDateTime = DateTime.MinValue;
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Property("Message").EqualTo("Value cannot be null.\r\nParameter name: End Date Time"));
        }

        [Test]
        public void Validation_When_ProductIsNotSet_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].Product = null;
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Property("Message").EqualTo("Value cannot be null.\r\nParameter name: Product"));
        }

        [Test]
        public void Validation_When_TargetRatingsIsNotSet_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].TargetRatings = null;
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Property("Message").EqualTo("Value cannot be null.\r\nParameter name: Target Ratings"));
        }

        [Test]
        public void Validation_When_BreakTypesIsNotSet_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            // Arrange
            _campaigns[0].BreakType = null;

            // Act
            var result = _controller.Post(_campaigns);

            // Assert
            _ = result.Should().BeOfType<InvalidModelStateResult>(null);

            var modelState = ((InvalidModelStateResult)result).ModelState;

            _ = modelState
                    .IsValidField(nameof(CreateSpot.BreakType))
                    .Should().BeFalse(null);

            _ = modelState[nameof(CreateSpot.BreakType)].Errors[0].ErrorMessage
                    .Should().StartWith("The BreakType value cannot be null.", null);

            _ = modelState[nameof(CreateSpot.BreakType)].Errors[0].ErrorMessage
                    .Should().EndWith(_campaigns[0].ExternalId, null);
        }

        [Test(Description = "When a BreakType is the correct length but the first two characters contain white space an exception is caught")]
        public void Validation_When_BreakTypesPrefixIsTooShort_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            // Arrange
            const string InvalidBreakType_PrefixTooShort = "X - This is invalid";

            _campaigns[0].BreakType = new List<string> { InvalidBreakType_PrefixTooShort };

            // Act
            var result = _controller.Post(_campaigns);

            // Assert
            _ = result.Should().BeOfType<InvalidModelStateResult>(null);

            var modelState = ((InvalidModelStateResult)result).ModelState;

            _ = modelState
                    .IsValidField(nameof(CreateCampaign.BreakType))
                    .Should().BeFalse(null);

            _ = modelState[nameof(CreateCampaign.BreakType)].Errors[0].ErrorMessage
                    .Should().StartWith("The BreakType prefix value is too short.", null);
        }

        [Test(Description = "When a BreakType is too short a BreakTypeTooShortException is caught")]
        public void Validation_When_BreakTypesIsTooShort_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            // Arrange
            const string InvalidBreakType_TooShort = "X";

            _campaigns[0].BreakType = new List<string> { InvalidBreakType_TooShort };

            // Act
            var result = _controller.Post(_campaigns);

            // Assert
            _ = result.Should().BeOfType<InvalidModelStateResult>(null);

            var modelState = ((InvalidModelStateResult)result).ModelState;

            _ = modelState
                    .IsValidField(nameof(CreateCampaign.BreakType))
                    .Should().BeFalse(null);

            _ = modelState[nameof(CreateCampaign.BreakType)].Errors[0].ErrorMessage
                    .Should().StartWith("The BreakType value is too short.", null);
        }

        [Test]
        public void Validation_When_SalesAreaCampaignTargetIsNotSet_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].SalesAreaCampaignTarget = null;
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Property("Message").EqualTo("Value cannot be null.\r\nParameter name: Sales Area Campaign Targets"));
        }

        [Test]
        public void Validation_When_IncludeRightSizerIsNotSet_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].IncludeRightSizer = null;
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Property("Message").EqualTo("Value cannot be null.\r\nParameter name: Include Right Sizer"));
        }

        [Test]
        public void Validation_When_IncludeRightSizerDoesNotContainTheRightValue_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].IncludeRightSizer = "IncludeRightSizerWithInvalidValue";
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Property("Message").EqualTo("Value cannot be null.\r\nParameter name: request"));
        }

        [Test]
        public void Validation_When_IncludeOptimisationIsNotSetAndCampaignPassPriorityIsNotZero_Then_ExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].IncludeOptimisation = false;
            _campaigns[0].CampaignPassPriority = 1;
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<InvalidDataException>());
        }

        [Test]
        public void Validation_When_IncludeOptimisationIsSetAndCampaignPassPriorityIsNotOneOrTwoOrThree_Then_ExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].IncludeOptimisation = true;
            _campaigns[0].CampaignPassPriority = 4;
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<InvalidDataException>());
        }

        [Test]
        public void Validation_When_CampaignGroupLengthIsMoreThan20Char_Then_ExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].CampaignGroup = new string('A', 21);
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<InvalidDataException>()
                .With.Property("Message").EqualTo("Campaign Group can't be more than 20 characters: AAAAAAAAAAAAAAAAAAAAA"));
        }

        [Test]
        public void Validation_When_CampaignStartDateTimeIsNotLessThanEndDateTime_Then_ExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].EndDateTime = DateTime.Now;
            _campaigns[0].StartDateTime = _campaigns[0].EndDateTime.AddDays(1);
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<InvalidDataException>()
                .With.Property("Message").EqualTo("Campaign start date should be less than or equal to end date"));
        }

        [Test]
        public void Validation_When_SalesAreaCampaignTargetSalesAreaGroupIsNotSet_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].SalesAreaCampaignTarget[0].SalesAreaGroup = null;
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Property("Message").EqualTo("Value cannot be null.\r\nParameter name: SalesArea Group"));
        }

        [Test]
        public void Validation_When_SalesAreaCampaignTargetMultipartsIsNotSet_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].SalesAreaCampaignTarget[0].Multiparts = null;
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Property("Message").EqualTo("Value cannot be null.\r\nParameter name: Multiparts"));
        }

        [Test]
        public void Validation_When_SalesAreaCampaignTargetCampaignTargetsIsNotSet_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].SalesAreaCampaignTarget[0].CampaignTargets = null;
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Property("Message").EqualTo("Value cannot be null.\r\nParameter name: Campaign Targets"));
        }

        [Test]
        public void Validation_When_SalesAreaCampaignTargetSalesAreaGroupGroupNameIsNotSet_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].SalesAreaCampaignTarget[0].SalesAreaGroup.GroupName = null;
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Property("Message").EqualTo("Value cannot be null.\r\nParameter name: SalesArea Group Name"));
        }

        [Test]
        public void Validation_When_SalesAreaCampaignTargetSalesAreaGroupSalesAreasIsNotSet_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].SalesAreaCampaignTarget[0].SalesAreaGroup.SalesAreas = null;
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Property("Message").EqualTo("Value cannot be null.\r\nParameter name: SalesArea name list"));
        }

        [Test]
        public void Validation_When_SalesAreaCampaignTargetMultipartsLengthsIsNotSet_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].SalesAreaCampaignTarget[0].Multiparts[0].Lengths = null;
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Property("Message").EqualTo("Value cannot be null.\r\nParameter name: Multipart Lengths"));
        }

        [Test]
        public void Validation_When_SalesAreaCampaignCampaignTargetsStrikeWeightsStartDateIsNotSet_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].SalesAreaCampaignTarget[0].CampaignTargets[0].StrikeWeights[0].StartDate = new DateTime();
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Property("Message").EqualTo("Value cannot be null.\r\nParameter name: Strike Weight Start Date"));
        }

        [Test]
        public void Validation_When_SalesAreaCampaignCampaignTargetsStrikeWeightsEndDateIsNotSet_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].SalesAreaCampaignTarget[0].CampaignTargets[0].StrikeWeights[0].EndDate = new DateTime();
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Property("Message").EqualTo("Value cannot be null.\r\nParameter name: Strike Weight End Date"));
        }

        [Test]
        public void Validation_When_SalesAreaCampaignCampaignTargetsStrikeWeightsStartDateIsAfterEndDate_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].SalesAreaCampaignTarget[0].CampaignTargets[0].StrikeWeights[0].StartDate = DateTime.Now.AddDays(1);
            _campaigns[0].SalesAreaCampaignTarget[0].CampaignTargets[0].StrikeWeights[0].EndDate = DateTime.Now;
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<InvalidDataException>()
                .With.Property("Message").EqualTo("Strike weight start date should be less than or equal to end date"));
        }

        [Test]
        public void Validation_When_SalesAreaCampaignCampaignTargetsStrikeWeightsLengthsIsNotSet_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].SalesAreaCampaignTarget[0].CampaignTargets[0].StrikeWeights[0].Lengths = null;
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Property("Message").EqualTo("Value cannot be null.\r\nParameter name: Strike Weight Lengths"));
        }

        [Test]
        public void Validation_When_SalesAreaCampaignCampaignTargetsStrikeWeightsLengthsLengthIsNotSet_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].SalesAreaCampaignTarget[0].CampaignTargets[0].StrikeWeights[0].Lengths[0].length = new NodaTime.Duration();
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Property("Message").EqualTo("Value cannot be null.\r\nParameter name: Length"));
        }

        [Test]
        public void Validation_When_SalesAreaCampaignCampaignTargetsStrikeWeightsDayPartsIsNotSet_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].SalesAreaCampaignTarget[0].CampaignTargets[0].StrikeWeights[0].DayParts = null;
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Property("Message").EqualTo("Value cannot be null.\r\nParameter name: Day Parts"));
        }

        [Test]
        public void Validation_When_SalesAreaCampaignCampaignTargetsStrikeWeightsDayPartsTimeslicesIsNotSet_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].SalesAreaCampaignTarget[0].CampaignTargets[0].StrikeWeights[0].DayParts[0].Timeslices = null;
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Property("Message").EqualTo("Value cannot be null.\r\nParameter name: Time Slices"));
        }

        [Test]
        public void Validation_When_SalesAreaCampaignCampaignTargetsStrikeWeightsDayPartsTimeslicesFromTimeIsNotSet_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].SalesAreaCampaignTarget[0].CampaignTargets[0].StrikeWeights[0].DayParts[0].Timeslices[0].FromTime = null;
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Property("Message").EqualTo("Value cannot be null.\r\nParameter name: Time Slice From Time"));
        }

        [Test]
        public void Validation_When_SalesAreaCampaignCampaignTargetsStrikeWeightsDayPartsTimeslicesToTimeIsNotSet_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].SalesAreaCampaignTarget[0].CampaignTargets[0].StrikeWeights[0].DayParts[0].Timeslices[0].ToTime = null;
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Property("Message").EqualTo("Value cannot be null.\r\nParameter name: Time Slice To Time"));
        }

        [Test]
        public void Validation_When_SalesAreaCampaignCampaignTargetsStrikeWeightsDayPartsTimeslicesDowPatternIsNotSet_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].SalesAreaCampaignTarget[0].CampaignTargets[0].StrikeWeights[0].DayParts[0].Timeslices[0].DowPattern = null;
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Property("Message").EqualTo("Value cannot be null.\r\nParameter name: Dow Pattern"));
        }

        [Test]
        public void Validation_When_SalesAreaCampaignCampaignTargetsStrikeWeightsDayPartsTimeslicesFromTimeHasInValidValue_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].SalesAreaCampaignTarget[0].CampaignTargets[0].StrikeWeights[0].DayParts[0].Timeslices[0].FromTime = "invalidRegXPattern";
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<System.Text.RegularExpressions.RegexMatchTimeoutException>()
                .With.Property("Message").EqualTo("Invalid Time Slice From Time"));
        }

        [Test]
        public void Validation_When_SalesAreaCampaignCampaignTargetsStrikeWeightsDayPartsTimeslicesToTimeHasInValidValue_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].SalesAreaCampaignTarget[0].CampaignTargets[0].StrikeWeights[0].DayParts[0].Timeslices[0].ToTime = "invalidRegXPattern";
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<System.Text.RegularExpressions.RegexMatchTimeoutException>()
                .With.Property("Message").EqualTo("Invalid Time Slice To Time"));
        }

        [Test]
        public void Validation_When_SalesAreaCampaignCampaignTargetsStrikeWeightsDayPartsTimeslicesDowPatternHasInValidValue_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].SalesAreaCampaignTarget[0].CampaignTargets[0].StrikeWeights[0].DayParts[0].Timeslices[0].DowPattern[0] = "invalidRegXPattern";
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<System.Text.RegularExpressions.RegexMatchTimeoutException>()
                .With.Property("Message").EqualTo("Invalid Dow Pattern"));
        }

        [Test]
        public void Validation_When_TimeRestrictionsStartDateTimeIsNotSet_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].TimeRestrictions[0].StartDateTime = new DateTime();
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Property("Message").EqualTo("Value cannot be null.\r\nParameter name: Time Restriction Start Date"));
        }

        [Test]
        public void Validation_When_TimeRestrictionsEndDateTimeIsNotSet_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].TimeRestrictions[0].EndDateTime = new DateTime();
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Property("Message").EqualTo("Value cannot be null.\r\nParameter name: Time Restriction End Date"));
        }

        [Test]
        public void Validation_When_TimeRestrictionsDowPatternIsNotSet_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].TimeRestrictions[0].DowPattern = null;
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Property("Message").EqualTo("Value cannot be null.\r\nParameter name: Time Restrictions DOW Pattern"));
        }

        [Test]
        public void Validation_When_TimeRestrictionsIsIncludeOrExcludeIsNotSet_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].TimeRestrictions[0].IsIncludeOrExclude = null;
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Property("Message").EqualTo("Value cannot be null.\r\nParameter name: Time Restrictions Is Include Or Exclude"));
        }

        [Test]
        public void Validation_When_TimeRestrictionsDowPatternHasInValidValue_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].TimeRestrictions[0].DowPattern[0] = "invalidRegXPattern";
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<System.Text.RegularExpressions.RegexMatchTimeoutException>()
                .With.Property("Message").EqualTo("Invalid Dow Pattern"));
        }

        [Test]
        public void Validation_When_TimeRestrictionsIsIncludeOrExcludeHasInValidValue_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].TimeRestrictions[0].IsIncludeOrExclude = "invalidRegXPattern";
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<System.Text.RegularExpressions.RegexMatchTimeoutException>()
                .With.Property("Message").EqualTo("Invalid Is Include Or Exclude(I/E)"));
        }

        [Test]
        public void Validation_When_TimeRestrictionsEndDateTimeIsEqualToStartDate_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            DateTime now = DateTime.Now;
            _campaigns[0].TimeRestrictions[0].StartDateTime = now;
            _campaigns[0].TimeRestrictions[0].EndDateTime = now;
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<InvalidDataException>()
                .With.Property("Message").EqualTo("Time restriction start date should be less than end date"));
        }

        [Test]
        public void Validation_When_TimeRestrictionsEndDateTimeIsBeforeStartDate_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            DateTime now = DateTime.Now;
            _campaigns[0].TimeRestrictions[0].StartDateTime = now.AddDays(1);
            _campaigns[0].TimeRestrictions[0].EndDateTime = now;
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<InvalidDataException>()
                .With.Property("Message").EqualTo("Time restriction start date should be less than end date"));
        }

        [Test]
        public void Validation_When_ProgrammeRestrictionsIsCategoryOrProgrammeIsNotSet_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].ProgrammeRestrictions[0].IsCategoryOrProgramme = null;
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Property("Message").EqualTo("Value cannot be null.\r\nParameter name: Programme Restrictions Category Or Programme"));
        }

        [Test]
        public void Validation_When_ProgrammeRestrictionsIsIncludeOrExcludeIsNotSet_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].ProgrammeRestrictions[0].IsIncludeOrExclude = null;
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Property("Message").EqualTo("Value cannot be null.\r\nParameter name: Programme Restrictions Is Include Or Exclude"));
        }

        [Test]
        public void Validation_When_CampaignProgrammeRestrictionsIsCategoryOrProgrammeHasInValidValue_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].ProgrammeRestrictions[0].IsCategoryOrProgramme = "invalidRegXPattern";
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<System.Text.RegularExpressions.RegexMatchTimeoutException>()
                .With.Property("Message").EqualTo("Invalid Category Or Programme(C/P)"));
        }

        [Test]
        public void Validation_When_CampaignProgrammeRestrictionsIsIncludeOrExcludeHasInValidValue_Then_CorrectExceptionIsRaisedWithTheCorrectErrorMessage()
        {
            _campaigns[0].ProgrammeRestrictions[0].IsIncludeOrExclude = "invalidRegXPattern";
            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<System.Text.RegularExpressions.RegexMatchTimeoutException>()
                .With.Property("Message").EqualTo("Invalid Is Include Or Exclude(I/E)"));
        }

        [Test]
        public void Validation_When_ValidationPassesForAllProperties_Then_ValidateDemographicsIsCalled()
        {
            _ = _mockDemographicRepository
                .Setup(x => x.GetAll())
                .Returns(new List<Demographic>());

            Assert.That(() => _controller.Post(_campaigns), Throws.Exception.TypeOf<InvalidDataException>()
                .With.Property("Message").EqualTo("Invalid Demographic in Campaigns: demographic"));

            _mockDemographicRepository.Verify(x => x.GetAll(), Times.Once());
        }

        private List<CreateCampaign> CreateValidCampaignObjectList()
        {
            var campaign = new CreateCampaign()
            {
                ExternalId = "External Id",
                Name = "Campaign Name",
                DemoGraphic = "demographic",
                StartDateTime = DateTime.Today,
                EndDateTime = DateTime.Today.AddDays(1),
                Product = "product",
                TargetRatings = 1,
                DeliveryType = CampaignDeliveryType.Rating.ToString(),
                BreakType = new List<string>() { "VA-ValidBreakType" },
                SalesAreaCampaignTarget = new List<SalesAreaCampaignTargetViewModel>()
                {
                    new SalesAreaCampaignTargetViewModel()
                    {
                        SalesAreaGroup = new SalesAreaGroup()
                        {
                            GroupName ="GroupName",
                            SalesAreas =new List<string>(){ "SalesAreas" }
                        },
                        Multiparts = new List<MultipartModel>()
                        {
                            new MultipartModel()
                            {
                                Lengths = new List<Duration>
                                {
                                     Duration.FromMinutes(1)
                                },
                                DesiredPercentageSplit = 1
                            }
                        },
                        CampaignTargets = new List<CampaignTarget>(){
                            new CampaignTarget() {
                                StrikeWeights = new List<StrikeWeight>() {
                                    new StrikeWeight() {
                                        StartDate = DateTime.Now,
                                        EndDate = DateTime.Now.AddDays(1),
                                        DesiredPercentageSplit = 1,
                                        Lengths = new List<Length>()
                                        {
                                            new Length()
                                            {
                                                length = Duration.FromMinutes(1),
                                                DesiredPercentageSplit = 1
                                            }
                                        },
                                        DayParts = new List<DayPart>() {
                                            new DayPart() {
                                                DesiredPercentageSplit = 1,
                                                Timeslices = new List<Timeslice>()
                                                {
                                                    new Timeslice()
                                                    {
                                                        FromTime = "01:00",
                                                        ToTime = "02:00" ,
                                                        DowPattern = new List<string>() { "Mon" }
                                                    }
                                                },
                                                Lengths = new List<DayPartLength>
                                                {
                                                   new DayPartLength
                                                   {
                                                       DesiredPercentageSplit = 1
                                                   }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                TimeRestrictions = new List<TimeRestriction>()
                {
                    new TimeRestriction()
                    {
                        StartDateTime = DateTime.Now,
                        EndDateTime = DateTime.Now.AddDays(1),
                        DowPattern =new List<string>(){"MON"},
                        IsIncludeOrExclude = "I"
                    }
                },
                ProgrammeRestrictions = new List<ProgrammeRestriction>()
                {
                    new ProgrammeRestriction()
                    {
                        IsCategoryOrProgramme ="C",
                        IsIncludeOrExclude ="I"
                    }
                },
                CampaignGroup = "CampaignGroup",
                IncludeRightSizer = "Campaign Level",
                IncludeOptimisation = true,
                CampaignPassPriority = 2
            };

            return new List<CreateCampaign>() { campaign };
        }

        [TearDown]
        public void Cleanup()
        {
            _controller = null;
            _controllerToValidatePut = null;
            _mapper = null;
        }

        public void Dispose()
        {
            _controller = null;
            _controllerToValidatePut = null;
            _mapper = null;
            _controllerToValidatePut?.Dispose();
            _controller?.Dispose();
        }
    }
}
