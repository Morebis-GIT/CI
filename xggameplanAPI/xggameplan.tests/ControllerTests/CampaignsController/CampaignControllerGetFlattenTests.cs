using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using ImagineCommunications.GamePlan.CommonTestsHelpers;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using Moq;
using NodaTime;
using NUnit.Framework;
using xggameplan.Controllers;
using xggameplan.core.Configuration;
using xggameplan.core.Interfaces;
using xggameplan.core.Services;
using xggameplan.Profile;

namespace xggameplan.tests.ControllerTests
{
    [TestFixture(Category = "Controllers :: Campaign")]
    public class CampaignControllerGetFlattenTests : IDisposable
    {
        private const string TempDay = "NNNNNNN";
        private const string TimeFormat = "hhmmss";

        private static Fixture _fixture = new SafeFixture();
        private static IMapper _mapper;
        private static readonly List<string> _weekDays = new List<string> { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
        private static Random _random = new Random();

        private CampaignFlattener _campaignFlattener;
        private CampaignsController _controller { get; set; }
        private Mock<ICampaignRepository> _mockCampaignRepository { get; set; }
        private Mock<IProductRepository> _mockProductRepository { get; set; }
        private Mock<IDemographicRepository> _mockDemographicRepository { get; set; }
        private Mock<IClashRepository> _mockClashRepository { get; set; }
        private Mock<ICampaignCleaner> _mockCampaignCleaner { get; set; }
        private IReadOnlyCollection<Campaign> _dummyCampaignList { get; set; }

        [OneTimeSetUp]
        public async Task OnInit()
        {
            AssumeDependenciesAreMocked();
            AssumeDependenciesAreSupplied();

            _mapper = AutoMapperInitializer.Initialize(expression => expression.AddProfile<CampaignProfile>());
            _campaignFlattener = new CampaignFlattener(_mockProductRepository.Object, _mockDemographicRepository.Object,
                _mockClashRepository.Object, _mapper);
            _controller = new CampaignsController(_mockCampaignRepository.Object, null, null, _mapper,
                _mockDemographicRepository.Object, null, _mockProductRepository.Object,
                null, null, null, _mockClashRepository.Object, null, null,
                _campaignFlattener, _mockCampaignCleaner.Object, null);
        }

        [OneTimeTearDown]
        public async Task OnDestroy() => CleanUp();

        [Test]
        public async Task GivenFlattenCampaigns_WhenCampaignsArgNull_ReturnEmptyCampaignFlattenedModelList()
        {
            var result = _campaignFlattener.Flatten(null);
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task GivenFlattenCampaigns_WhenCampaignsArgIsNotNull_ReturnDaysToEndOfCampignAsExpected()
        {
            var result = _campaignFlattener.Flatten(_dummyCampaignList);
            var campaigns = _dummyCampaignList.ToList();
            var expectedDaysToEndOfCampign = (campaigns[0].EndDateTime - DateTime.Now).Days;
            Assert.AreEqual(expectedDaysToEndOfCampign, result[0].DaysToEndOfCampign);
        }

        [Test]
        public async Task GivenFlattenCampaigns_WhenTargetRatingsIsNullOrZero_ReturnCampaignRatingsPercentageAsZero()
        {
            var campaigns = _dummyCampaignList.ToList();
            campaigns[0].TargetRatings = 0;
            _dummyCampaignList = campaigns;
            var result = _campaignFlattener.Flatten(_dummyCampaignList);
            Assert.Zero(result[0].CampaignTargetAchievedPct);
        }

        [Test]
        public async Task GivenFlattenDowPattern_WhenDowPatternArgNull_ReturnAllDaysNo()
        {
            var result = _campaignFlattener.FlattenDowPattern(null);
            Assert.AreEqual(result, string.Empty);
        }

        [Test]
        public async Task GivenFlattenDowPattern_WhenDowPatternArgEmpty_ReturnAllDaysNo()
        {
            var result = _campaignFlattener.FlattenDowPattern(new List<string>());
            Assert.AreEqual(result, TempDay);
        }

        [Test]
        public async Task GivenFlattenDowPattern_WhenDowPatternArgNoIncludeAnyWeekOfDays_ReturnAllDaysNo()
        {
            var dummyDowPatterns = new List<string> { "xyz", "abc", "qwe" };
            var result = _campaignFlattener.FlattenDowPattern(dummyDowPatterns);
            Assert.AreEqual(result, TempDay);
        }

        [Test]
        public async Task GivenFlattenDowPattern_WhenDowPatternArgIncludesAnyWeekOfDays_ReturnSomeDaysYes()
        {
            var dummyDowPatterns = new List<string> { "Mon", "Wed", "Fri" };
            var expectedResult = "NYNYNYN";
            var result = _campaignFlattener.FlattenDowPattern(dummyDowPatterns);
            Assert.AreEqual(result, expectedResult);
        }

        [Test]
        public async Task GivenFlattenTimeSlices_WhenTimesliceModelArgNullOrEmpty_ReturnEmptyString()
        {
            var result = _campaignFlattener.FlattenTimeSlices(null);
            Assert.AreEqual(result, string.Empty);
        }

        [Test]
        public async Task GivenFlattenTimeSlices_WhenTimesliceModelFromTimeFieldNull_ReturnWithoutFromTimeFielString()
        {
            var dummyTimesliceModelList = _fixture.Build<Timeslice>().CreateMany(1).ToList();
            dummyTimesliceModelList[0].DowPattern = new List<string> { "Sun", "Thu", "Sat" };
            dummyTimesliceModelList[0].ToTime = "06:00";
            dummyTimesliceModelList[0].FromTime = null;
            var expectedResult = "(060000 YNNNYNY)";
            var result = _campaignFlattener.FlattenTimeSlices(dummyTimesliceModelList);
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public async Task GivenFlattenTimeSlices_WhenTimesliceModelDowPatternToTimeIsNull_ReturnStringWithoutToTime()
        {
            var dummyTimesliceModelList = _fixture.Build<Timeslice>().CreateMany(1).ToList();
            dummyTimesliceModelList[0].DowPattern = new List<string> { "Wed", "Thu", "Sat" };
            dummyTimesliceModelList[0].ToTime = null;
            dummyTimesliceModelList[0].FromTime = "17:59";
            var expectedResult = "(175900 NNNYYNY)";
            var result = _campaignFlattener.FlattenTimeSlices(dummyTimesliceModelList);
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public async Task GivenFlattenTimeSlices_WhenTimesliceModelIncludeMoreDowPattern_ReturnCommaSeperatedString()
        {
            var dummyTimesliceModelList = _fixture.Build<Timeslice>().CreateMany(2).ToList();
            dummyTimesliceModelList[0].DowPattern = new List<string> { "Sun", "Thu", "Sat" };
            dummyTimesliceModelList[0].ToTime = "06:00";
            dummyTimesliceModelList[0].FromTime = "17:59";
            dummyTimesliceModelList[1].DowPattern = new List<string> { "Wed", "Thu", "Sat" };
            dummyTimesliceModelList[1].ToTime = null;
            dummyTimesliceModelList[1].FromTime = "00:00";
            var expectedResult = "(175900-060000 YNNNYNY; 000000 NNNYYNY)";
            var result = _campaignFlattener.FlattenTimeSlices(dummyTimesliceModelList);
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public async Task GivenFlattenDayPart_WhenDayPartModelNull_ReturnEmtyDayPartFlattenModel()
        {
            var result = _campaignFlattener.FlattenDayPart(null);
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task GivenFlattenDayPart_WhenTimeSlicesNull_ReturnEmtyDaypartTimeAndDays()
        {
            var dummyDayPartModel = _fixture.Build<DayPart>().Create();
            dummyDayPartModel.Timeslices = null;
            var result = _campaignFlattener.FlattenDayPart(dummyDayPartModel);
            Assert.AreEqual(result.DaypartTimeAndDays, string.Empty);
        }

        [Test]
        public async Task GivenFlattenDayPart_WhenDesiredPercentageSplitZero_ReturnDaypartRatingsPercentageZero()
        {
            var dummyDayPartModel = _fixture.Build<DayPart>().Create();
            dummyDayPartModel.DesiredPercentageSplit = 0;
            var result = _campaignFlattener.FlattenDayPart(dummyDayPartModel);
            Assert.AreEqual(result.DaypartTargetAchievedPct, 0);
        }

        [Test]
        public async Task GivenFlattenStrikeWeight_WhenStrikeWeightNull_ReturnEmtyStrikeWeightFlattenModel()
        {
            var result = _campaignFlattener.FlattenStrikeWeight(null);
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task GivenFlattenStrikeWeight_WhenDesiredPercentageSplitZero_ReturnStrikeWeightTargetAchievedPercentageZero()
        {
            var dummyStrikeWeight = _fixture.Build<StrikeWeight>().Create();
            dummyStrikeWeight.DesiredPercentageSplit = 0;
            var result = _campaignFlattener.FlattenStrikeWeight(dummyStrikeWeight);
            Assert.AreEqual(result.StrikeWeightTargetAchievedPct, 0);
        }

        [Test]
        public async Task GivenFlattenDuration_WhenSalesAreaCampaignTargetNull_ReturnEmtyDurationFlattenModel()
        {
            var result = _campaignFlattener.FlattenDuration(null);
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task GivenFlattenDuration_WhenMultipartsIsNull_ReturnDurationRatingsPercentageZero()
        {
            var dummySalesAreaCampaignTarget = _fixture.Build<SalesAreaCampaignTarget>().Create();
            dummySalesAreaCampaignTarget.Multiparts = null;
            var result = _campaignFlattener.FlattenDuration(dummySalesAreaCampaignTarget);
            Assert.AreEqual(result.DurationTargetAchievedPct, 0);
        }

        [Test]
        public async Task GivenFlattenDuration_WhenMultipartsIsNotNull_LengthIsNull_ReturnDurationSecsZero()
        {
            var dummySalesAreaCampaignTarget = _fixture.Build<SalesAreaCampaignTarget>().Create();
            var dummyMultiparts = _fixture.Build<Multipart>().CreateMany(2).ToList();
            dummyMultiparts[0].Lengths = null;
            dummySalesAreaCampaignTarget.Multiparts = dummyMultiparts;
            var result = _campaignFlattener.FlattenDuration(dummySalesAreaCampaignTarget);
            Assert.AreEqual(result.DurationSecs, 0);
        }

        [Test]
        public async Task GivenFlattenDuration_WhenMultipartsIsNotNull_ThenLengthIsNotNullOrEmpty_ReturnDurationSecsZero()
        {
            var dummySalesAreaCampaignTarget = _fixture.Build<SalesAreaCampaignTarget>().Create();
            var dummyMultiparts = _fixture.Build<Multipart>().CreateMany(2).ToList();
            var dummyDuration = new List<MultipartLength>()
            {
                new MultipartLength
                {
                    Length = Duration.Zero
                }
            };
            dummyMultiparts[0].Lengths = dummyDuration;
            dummySalesAreaCampaignTarget.Multiparts = dummyMultiparts;
            var result = _campaignFlattener.FlattenDuration(dummySalesAreaCampaignTarget);
            Assert.AreEqual(result.DurationSecs, 0);
        }

        [Test]
        public async Task GivenFlattenSaleAreaCampaignTarget_WhenSalesAreaCampaignTargetNull_ReturnEmtyDurationFlattenModel()
        {
            var result = _campaignFlattener.FlattenSaleAreaCampaignTarget(null);
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task GivenFlattenSaleAreaCampaignTarget_WhenMultipartsIsNull_ReturnSalesAreaGroupRatingsDifferenceZero()
        {
            var dummySalesAreaCampaignTarget = _fixture.Build<SalesAreaCampaignTarget>().Create();
            dummySalesAreaCampaignTarget.Multiparts = null;
            var result = _campaignFlattener.FlattenSaleAreaCampaignTarget(dummySalesAreaCampaignTarget);
            Assert.AreEqual(result.SalesAreaGroupTargetActualDiff, 0);
        }

        [Test]
        public async Task GivenFlattenSaleAreaCampaignTarget_WhenMultipartsIsNull_ReturnSalesAreaGroupRatingsPercentageZero()
        {
            var dummySalesAreaCampaignTarget = _fixture.Build<SalesAreaCampaignTarget>().Create();
            dummySalesAreaCampaignTarget.Multiparts = null;
            var result = _campaignFlattener.FlattenSaleAreaCampaignTarget(dummySalesAreaCampaignTarget);
            Assert.AreEqual(result.SalesAreaGroupTargetAchievedPct, 0);
        }

        private void AssumeDependenciesAreMocked()
        {
            _mockCampaignRepository = new Mock<ICampaignRepository>();
            _mockProductRepository = new Mock<IProductRepository>();
            _mockDemographicRepository = new Mock<IDemographicRepository>();
            _mockClashRepository = new Mock<IClashRepository>();
            _mockCampaignCleaner = new Mock<ICampaignCleaner>();
        }

        private void AssumeDependenciesAreSupplied()
        {
            _fixture.Customize<Timeslice>(x => x
                .With(p => p.FromTime, _fixture.Create<TimeSpan>().ToString(TimeFormat, CultureInfo.InvariantCulture))
                .With(p => p.ToTime, _fixture.Create<TimeSpan>().ToString(TimeFormat, CultureInfo.InvariantCulture))
                .With(p => p.DowPattern, _weekDays)
            );

            var products = _fixture.Build<Product>().CreateMany(10);
            var demographics = _fixture.Build<Demographic>().CreateMany(10);
            var parentClashes = new List<Clash>();
            var clashes = _fixture.Build<Clash>().CreateMany(10);
            _dummyCampaignList = _fixture.Build<Campaign>().CreateMany(10).ToList();

            foreach (var campaign in _dummyCampaignList)
            {
                var demographic = _fixture.Build<Demographic>()
                    .With(d => d.ExternalRef, campaign.DemoGraphic)
                    .Create();
                _ = demographics.Append(demographic);

                var product = _fixture.Build<Product>()
                    .With(p => p.Externalidentifier, campaign.Product)
                    .Create();
                _ = products.Append(product);
            }

            foreach (var product in products)
            {
                var clash = _fixture.Build<Clash>()
                    .With(c => c.Externalref, product.ClashCode)
                    .Create();
                _ = products.Append(product);
            }

            var productRefs = products.Select(r => r.Externalidentifier).Distinct();
            _ = _mockProductRepository.Setup(m => m.FindByExternal(productRefs.ToList())).Returns(products);

            var demographicRefs = demographics.Select(r => r.ExternalRef).Distinct();
            _ = _mockDemographicRepository.Setup(m => m.GetByExternalRef(demographicRefs.ToList())).Returns(demographics);

            foreach (var clash in clashes)
            {
                if (!clashes.Any(c => c.ParentExternalidentifier == clash.Externalref))
                {
                    var pClash = _fixture.Build<Clash>().With(d => d.Externalref, clash.ParentExternalidentifier).Create();
                    parentClashes.Add(pClash);
                }
            }
            clashes.ToList().AddRange(parentClashes);
            _ = _mockClashRepository.Setup(m => m.GetAll()).Returns(clashes);
        }

        public static IEnumerable<string> GetWeekDaysAsStrList()
        {
            var count = _random.Next(0, 10);
            var randomDays = new HashSet<int>();
            while (randomDays.Count < count)
            {
                int n = _random.Next(0, _weekDays.Count);
                if (randomDays.Add(n))
                {
                    yield return _weekDays[n];
                }
            }
        }

        private void CleanUp()
        {
            if (_controller != null)
            {
                _controller.Dispose();
                _controller = null;
            }
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
                // dispose managed state (managed objects)
                if (_controller != null)
                {
                    _controller.Dispose();
                    _controller = null;
                }
            }

            _disposedValue = true;
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in
            // 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
